using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DATN_Web.BusinessLayer;
using DATN_Web.Filters;
using DATN_Web.Models.ViewModels;

namespace DATN_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthBLL _auth = new AuthBLL();
        private bool IsAdmin()
        {
            if (Session["Role"] == null) return false;
            byte role = (byte)Session["Role"];
            return role == 2;
        }
        // GET: /Users
        [AdminOnly]
        public ActionResult Index()
        {
            var users = _auth.GetAllUsers();
            return View(users);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginVM model = new LoginVM();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = _auth.Login(vm.Username, vm.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Sai tài khoản/mật khẩu hoặc tài khoản bị khóa");
                return View(vm);
            }

            FormsAuthentication.SetAuthCookie(user.Username, false);
            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["Role"] = user.Role;

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Register
        [HttpGet]
        [AdminOnly]
        public ActionResult Register()
        {
            RegisterVM model = new RegisterVM();
            model.Role = 1;
            return View(model);
        }
        // POST: Account/Register
        [AdminOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string error = _auth.Register(
                model.Username,
                model.Password,
                model.FullName,
                model.Email,
                model.Role
            );

            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(model);
            }

            return RedirectToAction("Login");
        }

        // Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}