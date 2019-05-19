using fitnes.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fitness.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string name, string password)
        {
            //some operations goes here
            return RedirectToAction("Index","Home"); //return some view to the user
        }

    }
}