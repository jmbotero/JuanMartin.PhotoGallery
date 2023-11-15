// Decompiled with JetBrains decompiler
// Type: JuanMartin.PhotoGallery.Controllers.LoginController
// Assembly: JuanMartin.PhotoGallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4CB99D68-5881-4C4E-957B-0BF063CB63CD
// Assembly location: C:\GitHub\temp\JuanMartin.PhotoGallery.dll

using JuanMartin.Kernel.Utilities;
using JuanMartin.Models.Gallery;
using JuanMartin.PhotoGallery.Models;
using JuanMartin.PhotoGallery.Services;
using Microsoft.AspNetCore.Mvc;

namespace JuanMartin.PhotoGallery.Controllers
{
    public class LoginController : Controller
    {
        private readonly IPhotoService _photoService;
        private readonly IConfiguration _configuration;

        public LoginController(IPhotoService photoService, IConfiguration configuration)
        {
            _photoService = photoService;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult ForgotPassword() => View();

       [HttpPost]
        public ActionResult ForgotPassword(LoginViewModel model)    
        {
            User user = _photoService.VerifyEmail(model.Email);
            string str;
            if (user.UserId == -1)
            {
                string guid = Guid.NewGuid().ToString();
                HttpUtility.SendVerificationEmail(user.Email, PasswordResetLink(HttpContext, guid), _configuration);
                _photoService.StoreActivationCode(user.UserId, guid);
                str = "Reset password link has been sent to the specified email.";
            }
            else
                str = "No user, associated to " + model.Email + " was found.";

            ViewBag.Message = str;
            return View(model);
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            ViewBag.IsCodeValid = false;
            ResetPasswordViewModel model = new()
            {
                ResetCode = id
            };
            string str;
            if (string.IsNullOrWhiteSpace(id) && !UtilityString.IsGuid(id))
            {
                str = "Request contained an invalid activation code.";
            }
            else
            {
                (int errorCode, User? user) = _photoService.VerifyActivationCode(id);
                if (user == null)
                    str = "Database problem getting verification code.";
                else
                {
                    model.UserId = user.UserId;
                    model.UserName = user.UserName;
                    switch (errorCode)
                    {
                        case -1:
                            str = "Activation code was not matched in database.";
                            break;
                        case 1:
                            ViewBag.IsCodeValid = true;
                            return View(model);
                        default:
                            str = "Activation code expired.";
                            break;
                    }
                }
            }
            model.ResetMessage = str;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            string str = "";
            bool valid = false;
            if (ModelState.IsValid)
            {
                if (model.NewPassword == model.ConfirmPassword)
                {
                    User user = _photoService.UpdateUserPassword(model.UserId, model.UserName, model.NewPassword);
                    if (user == null)
                    {
                        str = "Database error: password was not updated.";
                        valid = false;
                    }
                    else
                    {
                        str = "New password for " + user.UserName + " updated successfully!";
                        valid = true;
                    }
                }
            }
            else
                str = "Password reset failed.";

            ViewBag.IsCodevalid = valid;
            model.ResetMessage = str;
            return View(model);
        }

        [HttpGet]
        public ActionResult OnPasswordUdateSuccess(int id)
        {
            StartNewSession(id);
            return ViewGalleryIndex(id);
        }

        [HttpGet]
        public ActionResult Register()
        {
            TempData["isSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");
            return View();
        }

        [HttpPost]
        public ActionResult Register(LoginViewModel model)
        {
            string str = "";
            User u = _photoService.AddUser(model.UserName, model.Password, model.Email);
            int userId = u.UserId;
            switch (userId)
            {
                case -2:
                    {
                        str = "User already exists, please try a different user name.";
                        break;
                    }
                case -1:
                    {
                        str = "Error occurred in backend while creating user, please contact the site owner.";
                        break;
                    }
            }
            ViewBag.Message = str;
            StartNewSession(userId);
            if (userId > 0)
                return ViewGalleryIndex(userId);
            TempData["isSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");
            return View(model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            string? sessionId = Convert.ToString(SessionExtensions.Get<ISession>(HttpContext.Session, "sessionId"));
            
            int userId = Convert.ToInt32(SessionExtensions.Get<ISession>(HttpContext.Session, "userId"));
            _photoService.EndSession(sessionId);
            HttpContext.Session.Clear();
            SessionExtensions.Set<ISession, bool>(HttpContext.Session, "isSignedIn", false);
            _photoService.AddAuditMessage(userId, $"User logged out, ended session({sessionId}).");
            return ViewGalleryIndex(userId);
        }

        public ActionResult Login()
        {
            ViewBag.GalleryRedirectUrl = ViewGalleryIndexRedirectUrl(-1);
            TempData["isSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ApplicationException("Error ocurred, model state is invalid.");
            }
            else
            {       
                User user = _photoService.GetUser(model.UserName, model.Password);
                if (user != null)
                {
                    int userId = user.UserId;
                    Guid sessionId = StartNewSession(userId);
                    _photoService.AddAuditMessage(userId, $"User logged in, started session ({sessionId}).");
                    ViewBag.GalleryRedirectUrl = ViewGalleryIndexRedirectUrl(userId);
                    return ViewGalleryIndex(userId);
                }
                else
                    ViewBag.Message = "Incorrect user name and/or password specified. Please try again.";

                TempData["sSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");
                }
                return View(model);
            }

        private Guid StartNewSession(int userId)
        {
            string clientRemoteId = HttpUtility.GetClientRemoteId(HttpContext);
            Guid sessionId = _photoService.LoadSession(userId);
            
            SessionExtensions.Set<ISession, int>(HttpContext.Session, "userId", userId);
            _photoService.ConnectUserAndRemoteHost(userId, clientRemoteId);
            SessionExtensions.Set<ISession, Guid>(HttpContext.Session, "sessionId", sessionId);
            SessionExtensions.Set<ISession, int>(HttpContext.Session, "userId", userId);

            return sessionId;
        }

    private static string PasswordResetLink(HttpContext context, string resetCode)
    {
      return $"{context.Request.Scheme}://{context.Request.Host}/Login/ResetPassword/{resetCode}";
    }

        private ActionResult ViewGalleryIndex(int userId)
        {
            string clientRemoteId = HttpUtility.GetClientRemoteId(HttpContext);
            RedirectResponseModel redirectInfo = _photoService.GetRedirectInfo(userId, clientRemoteId);
            TempData["isSignedIn"] = SessionExtensions.Get<ISession>(HttpContext.Session, "isSignedIn");
            if (redirectInfo == null || !(redirectInfo.RemoteHost != ""))
                return RedirectToAction("Index", "Gallery");
            string controller = redirectInfo.Controller;
         
            return RedirectToAction(redirectInfo.Action, controller, new RouteValueDictionary(redirectInfo.RouteData));
        }

        private string ViewGalleryIndexRedirectUrl(int userId)
        {
            string clientRemoteId = HttpUtility.GetClientRemoteId(HttpContext);
            var redirect = _photoService.GetRedirectInfo(userId, clientRemoteId);
            return HttpUtility.GetRedirectUrl(redirect, HttpUtility.GalleryViewTypes.Index);
        }
  }
}
