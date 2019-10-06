using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestAPI.Models;
using System.Net.Http;
using System.Net;
using System.Configuration;
using System.Text;

namespace RestAPI.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            IEnumerable<Models.UserViewModel> users = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:57930/api/");
                //HTTP GET
                var responseTask = client.GetAsync("dbapi");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Models.UserViewModel>>();
                    readTask.Wait();

                    users = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    users = Enumerable.Empty<Models.UserViewModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(users);
        }
        public ActionResult createUser()
        {
            return View();
        }


        [HttpPost]
        public ActionResult createUser(UserViewModel user)
        {
            //user.Id = System.Guid.NewGuid().ToString();
            //ServiceRepository serviceObj = new ServiceRepository();
            //HttpResponseMessage response = serviceObj.PostResponse("api/DBAPI/", user);
            //response.EnsureSuccessStatusCode();
            //return RedirectToAction("Index");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:57930/api/");

                //HTTP POST
                user.Id = System.Guid.NewGuid().ToString();
                var postTask = client.PostAsJsonAsync<UserViewModel>("dbapi", user);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(user);
        }

        public ActionResult Edit(string id)
        {
            UserViewModel user = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:57930/api/");
                //HTTP GET
                var responseTask = client.GetAsync("dbapi?id=" + id.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<UserViewModel>();
                    readTask.Wait();

                    user = readTask.Result;
                }
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel user)
        {
            using (var client = new HttpClient())
            {

                //HTTP POST
                client.BaseAddress = new Uri("http://localhost:57930/api/");

                //HTTP POST

                var putTask = client.PutAsJsonAsync<UserViewModel>("dbapi", user);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }
            return View(user);
        }
    }
}
