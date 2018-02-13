using FinalProject.Database;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

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
            Services.EmailInfo.Configuration = configuration;
        }

        [Route("")]
        public IActionResult Index()
        {
            Account userAccount;
            switch (repository.GetAccountStatus(Request, Response, out userAccount))
            {
                case AccountStatus.OK:
                    return RedirectToAction("Explore");
                case AccountStatus.Inactive:
                    return RedirectToAction("Activate");
            }
            return View();
        }

        [Route("[action]")]
        public IActionResult SignUp()
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SignUp(SignUpModel model)
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            if ((ModelState.IsValid) && (model.IsModelValid()))
            {
                Account account = new Account(model.Username, model.Email, model.Password, model.FirstName, model.LastName, model.GetGender());
                if (repository.AddAccount(account))
                {
                    logger.LogInformation($"User @{account.Username} (ID: {account.Id}) registered with email {account.Email} (Verification code: {account.VerificationCode})");
                    try
                    {
                        Services.Email.SendVerification(account.Email, account.VerificationCode.ToString());
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
        public IActionResult CheckUsername(string Value)
        {
            if (Value != null)
            {
                return Json(new { Available = repository.CheckUsername(Value).ToString().ToLower() });
            }
            return Json(new { Available = "false" });
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CheckEmail(string Value)
        {
            if (Value != null)
            {
                return Json(new { Available = repository.CheckEmail(Value).ToString().ToLower() });
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
        public IActionResult Activate(string Email, string Code, bool? Resent)
        {
            Account userAccount;
            AccountStatus accountStatus = repository.GetAccountStatus(Request, Response, out userAccount);
            if (accountStatus == AccountStatus.OK)
            {
                return RedirectToAction("Index");
            }
            else if ((Email == null) && (Code == null))
            {
                if (accountStatus == AccountStatus.Inactive)
                {
                    if (Resent.HasValue)
                    {
                        return View(new ActivateModel("", false, true));
                    }
                    return View();
                }
            }
            else if (repository.VerifyAccount(Email, Code))
            {
                return RedirectToAction("LogIn", new { Verified = true });
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Activate(ActivateModel model)
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.Inactive)
            {
                return RedirectToAction("Index");
            }
            else if (ModelState.IsValid)
            {
                if (repository.VerifyAccount(userAccount.Email, model.Code))
                {
                    return RedirectToAction("LogIn");
                }
            }
            return View(new ActivateModel("", true));
        }

        [Route("[action]")]
        public IActionResult LogIn(bool? Verified)
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.Invalid)
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
        public IActionResult LogIn(LogInModel model)
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            else if (ModelState.IsValid)
            {
                string userId, userHash;
                if (repository.LoginAccount(model.Identifier, model.Password, out userId, out userHash))
                {
                    repository.SetAccountStatus(Response, userId, userHash);
                    return RedirectToAction("Index");
                }
                model.ErrorMessage = true;
                return View(model);
            }
            return View(new LogInModel("", "", true));
        }

        [Route("[action]")]
        public IActionResult ResendMail()
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.Inactive)
            {
                return RedirectToAction("Index");
            }
            userAccount.VerificationCode = ActivateModel.GenerateCode();
            repository.ForceSave();
            try
            {
                Services.Email.SendVerification(userAccount.Email, userAccount.VerificationCode.ToString(), "A new verification code was requested.");
            }
            catch (Exception ex)
            {
                logger.LogError($"AUTO-MAIL FAILED: {ex.Message}");
                return RedirectToAction("Error");
            }
            logger.LogInformation($"User @{userAccount.Username} (ID: {userAccount.Id}) requested a new verification code to email {userAccount.Email} (Verification code: {userAccount.VerificationCode})");
            return RedirectToAction("Activate", new { Resent = true });
        }

        [Route("[action]")]
        public IActionResult LogOut()
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) == AccountStatus.Invalid)
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
        public IActionResult Explore()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CheckFacebookAccount(string Value)
        {
            if (Value != null)
            {
                return Json(new { Unregistered = repository.CheckFacebookAccount(Value).ToString().ToLower() });
            }
            return Json(new { Unregistered = "false" });
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RegisterFacebookAccount(string Username, string AccessToken)
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            if ((Username == null) || (AccessToken == null))
            {
                return RedirectToAction("Error");
            }
            string email, firstName, lastName, facebookId;
            Gender gender;
            if (Services.Facebook.GetFacebookInfo(AccessToken, out email, out firstName, out lastName, out gender, out facebookId))
            {
                Account account = new Account(Username, email, (Username + AccessToken), firstName, lastName, gender);
                account.Verified = true;
                account.UseFacebook = true;
                account.FacebookID = facebookId;
                if (repository.AddAccount(account))
                {
                    logger.LogInformation($"User @{account.Username} (ID: {account.Id}) registered with Facebook account (Facebook ID: {account.FacebookID})");
                    string cookieId, cookieHash;
                    if (repository.LoginAccountFacebook(facebookId, out cookieId, out cookieHash))
                    {
                        repository.SetAccountStatus(Response, cookieId, cookieHash);
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Error");
                }
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult LogInFacebook(string AccessToken)
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.Invalid)
            {
                return RedirectToAction("Index");
            }
            if (AccessToken == null)
            {
                return RedirectToAction("Error");
            }
            string email, firstName, lastName, facebookId;
            Gender gender;
            if (Services.Facebook.GetFacebookInfo(AccessToken, out email, out firstName, out lastName, out gender, out facebookId))
            {
                string cookieId, cookieHash;
                if (repository.LoginAccountFacebook(facebookId, out cookieId, out cookieHash))
                {
                    repository.SetAccountStatus(Response, cookieId, cookieHash);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Error");
            }
            return RedirectToAction("Error");
        }

        [Route("[action]")]
        public IActionResult Dashboard()
        {
            Account userAccount;
            if (repository.GetAccountStatus(Request, Response, out userAccount) != AccountStatus.OK)
            {
                return RedirectToAction("Index");
            }
            return View(new DashboardModel(userAccount.UseFacebook));
        }
    }
}
