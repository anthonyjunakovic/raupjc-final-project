using FinalProject.Database;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace FinalProject.Controllers
{
    [Route("")]
    public class DefaultController : Controller
    {
        private IRepository repository;
        private ILogger<DefaultController> logger;

        public DefaultController(IRepository repository, ILogger<DefaultController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("[action]")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SignUp(SignUpModel model)
        {
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
    }
}
