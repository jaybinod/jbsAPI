using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jbsAPI.Models;
using jbsAPI.Repository;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Configuration;

namespace jbsAPI.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginUser model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (ClientLoginDataEntities db = new ClientLoginDataEntities())
            {
                var entity = db.Loginusers.Any(e => e.Email == model.Email && e.password==model.Password);
                if (entity == false)
                {
                    ModelState.AddModelError("", "Invalid Login Attempt");
                    return View(model);
                }
                else
                {
                    Session["UserID"] = System.Guid.NewGuid().ToString();
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult LogOff()
        {
            Session["UserID"] = "";
            return RedirectToAction("Login", "Account");
        }
    }
}