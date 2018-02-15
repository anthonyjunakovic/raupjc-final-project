using FinalProject.Database;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

/*
 * NOTE: Email service is disabled, hence all the commented-out code.
 */
namespace FinalProject.Controllers
{
    [Route("")]
    public class DefaultController : Controller
    {
        private IRepository repository;
        private ILogger<DefaultController> logger;
        private IConfiguration configuration;

        public DefaultController(IRepository repository, ILogger<DefaultController> logger, IConfiguration configuration)
        {
            this.repository = repository;
            this.logger = logger;
            this.configuration = configuration;
            // Services.EmailInfo.Configuration = configuration;
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            switch (asp.status)
            {
                case AccountStatus.OK:
                    return RedirectToAction("Explore");
                case AccountStatus.Inactive:
                    return RedirectToAction("Activate");
            }
            return View();
        }

        [Route("[action]")]
        public async Task<IActionResult> SignUp()
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            if ((ModelState.IsValid) && (model.IsModelValid()))
            {
                Account account = new Account(model.Username, model.Email, model.Password, model.FirstName, model.LastName, model.GetGender());
                account.Verified = true; // SINCE THERE IS NO WAY OF CONFIRMING THE EMAIL ADDRESS
                if (await repository.AddAccount(account))
                {
                    logger.LogInformation($"User @{account.Username} (ID: {account.Id}) registered with email {account.Email} (Verification code: {account.VerificationCode})");
                    try
                    {
                        // Services.Email.SendVerification(account.Email, account.VerificationCode.ToString());
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"AUTO-MAIL FAILED: {ex.Message}");
                        return RedirectToAction("Error");
                    }
                    return View("Registered", new RegisteredModel() { FirstName = account.FirstName, Email = account.Email });
                }
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CheckUsername(string Value)
        {
            if (Value != null)
            {
                return Json(new { Available = (await repository.CheckUsername(Value)).ToString().ToLower() });
            }
            return Json(new { Available = "false" });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CheckEmail(string Value)
        {
            if (Value != null)
            {
                return Json(new { Available = (await repository.CheckEmail(Value)).ToString().ToLower() });
            }
            return Json(new { Available = "false" });
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Registered(RegisteredModel model)
        {
            if ((ModelState.IsValid) && model.IsValid())
            {
                return View(model);
            }
            return RedirectToAction("Error");
        }

        [Route("[action]")]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Activate(string Email, string Code, bool? Resent)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status == AccountStatus.OK)
            {
                return RedirectToAction("Index");
            }
            else if ((Email == null) && (Code == null))
            {
                if (asp.status == AccountStatus.Inactive)
                {
                    if (Resent.HasValue)
                    {
                        return View(new ActivateModel("", false, true));
                    }
                    return View();
                }
            }
            else if (await repository.VerifyAccount(Email, Code))
            {
                return RedirectToAction("LogIn", new { Verified = true });
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Activate(ActivateModel model)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.Inactive)
            {
                return RedirectToAction("Index");
            }
            else if (ModelState.IsValid)
            {
                if (await repository.VerifyAccount(asp.account.Email, model.Code))
                {
                    return RedirectToAction("LogIn");
                }
            }
            return View(new ActivateModel("", true));
        }

        [Route("[action]")]
        public async Task<IActionResult> LogIn(bool? Verified)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            if (Verified.HasValue)
            {
                return View(new LogInModel("", "", false, true));
            }
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogIn(LogInModel model)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            else if (ModelState.IsValid)
            {
                AccountResult ar = await repository.LoginAccount(model.Identifier, model.Password);
                if (ar.Ok)
                {
                    repository.SetAccountStatus(Response, ar.CookieId, ar.CookieHash);
                    return RedirectToAction("Index");
                }
                model.ErrorMessage = true;
                return View(model);
            }
            return View(new LogInModel("", "", true));
        }

        [Route("[action]")]
        public async Task<IActionResult> ResendMail()
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.Inactive)
            {
                return RedirectToAction("Index");
            }
            asp.account.VerificationCode = ActivateModel.GenerateCode();
            repository.ForceSave();
            try
            {
                // Services.Email.SendVerification(userAccount.Email, userAccount.VerificationCode.ToString(), "A new verification code was requested.");
            }
            catch (Exception ex)
            {
                logger.LogError($"AUTO-MAIL FAILED: {ex.Message}");
                return RedirectToAction("Error");
            }
            logger.LogInformation($"User @{asp.account.Username} (ID: {asp.account.Id}) requested a new verification code to email {asp.account.Email} (Verification code: {asp.account.VerificationCode})");
            return RedirectToAction("Activate", new { Resent = true });
        }

        [Route("[action]")]
        public async Task<IActionResult> LogOut()
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status == AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            repository.ClearAccountStatus(Response);
            return RedirectToAction("Index");
        }

        [Route("[action]")]
        public IActionResult FAQ()
        {
            return View();
        }

        [Route("[action]")]
        public async Task<IActionResult> Explore()
        {
            return View(await repository.GetRecentPosts());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CheckFacebookAccount(string Value)
        {
            if (Value != null)
            {
                return Json(new { Unregistered = (await repository.CheckFacebookAccount(Value)).ToString().ToLower() });
            }
            return Json(new { Unregistered = "false" });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegisterFacebookAccount(string Username, string AccessToken)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            if ((Username == null) || (AccessToken == null))
            {
                return RedirectToAction("Error");
            }
            Services.FacebookInfo fi = await Services.Facebook.GetFacebookInfo(AccessToken);
            if (fi.Ok)
            {
                Account account = new Account(Username, fi.Email, (Username + AccessToken), fi.FirstName, fi.LastName, fi.Gender);
                account.Verified = true;
                account.UseFacebook = true;
                account.FacebookID = fi.FacebookId;
                if (await repository.AddAccount(account))
                {
                    logger.LogInformation($"User @{account.Username} (ID: {account.Id}) registered with Facebook account (Facebook ID: {account.FacebookID})");
                    AccountResult ar = await repository.LoginAccountFacebook(fi.FacebookId);
                    if (ar.Ok)
                    {
                        repository.SetAccountStatus(Response, ar.CookieId, ar.CookieHash);
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Error");
                }
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogInFacebook(string AccessToken)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            if (AccessToken == null)
            {
                return RedirectToAction("Error");
            }
            Services.FacebookInfo fi = await Services.Facebook.GetFacebookInfo(AccessToken);
            if (fi.Ok)
            {
                AccountResult ar = await repository.LoginAccountFacebook(fi.FacebookId);
                if (ar.Ok)
                {
                    repository.SetAccountStatus(Response, ar.CookieId, ar.CookieHash);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Error");
            }
            return RedirectToAction("Error");
        }

        [Route("[action]")]
        public async Task<IActionResult> Dashboard()
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.OK)
            {
                return RedirectToAction("Index");
            }
            return View(new DashboardModel(asp.account));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangePassword(string Username, string OldPassword, string NewPassword)
        {
            if ((OldPassword == null) || (OldPassword == "") || (NewPassword == null) || (NewPassword == ""))
            {
                return Json(new { Success = "false" });
            }
            if ((OldPassword.Length < 6) || (OldPassword.Length > 64) || (NewPassword.Length < 6) || (NewPassword.Length > 64))
            {
                return Json(new { Success = "false" });
            }
            Account userAccount = await repository.GetAccount(Username, OldPassword);
            if (userAccount == null)
            {
                return Json(new { Success = "false" });
            }
            if (!Account.HashPassword(OldPassword).SequenceEqual(userAccount.PasswordHashed))
            {
                return Json(new { Success = "false" });
            }
            if (OldPassword == NewPassword)
            {
                return Json(new { Success = "true" });
            }
            userAccount.PasswordHashed = Account.HashPassword(NewPassword);
            repository.ForceSave();
            return Json(new { Success = "true" });
        }

        [HttpGet, HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Upload(UploadModel model)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.OK)
            {
                return RedirectToAction("Index");
            }
            if ((ModelState.IsValid) && (model.IsModelValid()))
            {
                Post post = await repository.UploadPost(asp.account, model.Title, model.ImageURL);
                return RedirectToAction("ViewPost", new { id = post.Id });
            }
            return View(model);
        }

        [Route("[action]")]
        public async Task<IActionResult> ViewPost(int id)
        {
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);

            Post post = await repository.GetPost(id);
            PostModel model = new PostModel();
            model.Id = post.Id;
            model.Title = post.Title;
            model.Owned = ((asp.account != null) && (asp.account == post.Owner));
            model.OwnerName = post.Owner.Username;
            model.ImageURL = post.PostURL;
            return View(model);
        }

        [Route("[action]/{id?}")]
        public async Task<IActionResult> User(string id)
        {
            UserModel um = null;
            if (id != null)
            {
                um = await repository.GetUserModel(id);
            }
            if (um == null)
            {
                AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
                if (asp.status != AccountStatus.OK)
                {
                    return RedirectToAction("Error");
                }
                um = await repository.GetUserModel(asp.account.Username);
                if (um == null)
                {
                    return RedirectToAction("Error");
                }
            }
            return View(um);
        }

        [Route("[action]")]
        public IActionResult MyUploads()
        {
            return RedirectToAction("User");
        }
        
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Error");
            }
            AccountStatusPair asp = await repository.GetAccountStatus(Request, Response);
            if (asp.status != AccountStatus.OK)
            {
                return RedirectToAction("Error");
            }
            if (await repository.DeletePost(id.Value, asp.account))
            {
                return RedirectToAction("User");
            }
            return RedirectToAction("Error");
        }
    }
}
