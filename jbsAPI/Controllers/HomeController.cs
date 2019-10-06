using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using jbsAPI.Models;
using jbsAPI.Repository;
using jbsAPI.Filters;

namespace jbsAPI.Controllers
{
    [UserAuthenticationFilter]
    public class HomeController : Controller
    {
        ServiceRepository serviceObj = new ServiceRepository();
        public ActionResult Index()
        {
            try
            {
                HttpResponseMessage response = serviceObj.GetResponse("api/DBApi");
                response.EnsureSuccessStatusCode();
                List<Models.UserModel> users = response.Content.ReadAsAsync<List<Models.UserModel>>().Result;
                ViewBag.Title = "All Users";
                return View(users);
            }
            catch 
            {
                throw;
            }
        }

        public ActionResult createUser()
        {
            return View();
        }


        [HttpPost]
        public ActionResult createUser(UserModel user)
        {
            HttpResponseMessage response = serviceObj.PostResponse("api/dbapi/", user);
            if (response.ReasonPhrase == "Found")
            {
                ViewBag.Title = "Duplication Email ID not allowed";
                return View(user);
            }
            else
            {
                if (response.IsSuccessStatusCode == true)
                    return RedirectToAction("Index");
                else
                    return View(user);
            }
        }

        public ActionResult Edit(string id)
        {
            try
            {
                HttpResponseMessage response = serviceObj.GetResponse("api/DBApi/" + id.ToString());
                response.EnsureSuccessStatusCode();
                Models.UserModel users = response.Content.ReadAsAsync<Models.UserModel>().Result;
                ViewBag.Title = "Edit User Data";
                return View(users);
            }
            catch {
                ViewBag.Title = "User Not Found";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(UserModel user)
        {
            HttpResponseMessage response = serviceObj.PutResponse("api/dbapi/", user);
            if (response.ReasonPhrase == "Found")
            {
                ViewBag.Title = "Email ID already registered with other user";
                return View(user);
            }
            else
            {
                if (response.IsSuccessStatusCode == true)
                    return RedirectToAction("Index");
                else
                    return View(user);
            }

        }

        public ActionResult Delete(string id)
        {
            HttpResponseMessage response = serviceObj.DeleteResponse("api/dbapi/" + id.ToString());
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index");
        }
    }
}
