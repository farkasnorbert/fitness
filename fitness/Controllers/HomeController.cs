using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using fitness.Models;

namespace fitness.Controllers
{
    public class HomeController : Controller
    {
        private IFirebaseClient Client;

        private readonly IFirebaseConfig Config = new FirebaseConfig
        {
            AuthSecret = "IiftR9cls1Smo9ISn97siEHaGM61fRSydYcVQRej",
            BasePath = "https://fitness-eee13.firebaseio.com/"
        };

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        public ActionResult Login(string name, string password)
        {
            if (name == "")
            {
                ViewBag.Message = "No name";
                return View("Index");
            }

            if (password == "")
            {
                ViewBag.Message = "No password";
                return View("Index");
            }

            Client = new FirebaseClient(Config);
            var response = Client.Get("user/" + name);
            var user = response.ResultAs<User>();
            var savedPasswordHash = user.Password;
            var hashBytes = Convert.FromBase64String(savedPasswordHash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(20);
            for (var i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                {
                    ViewBag.Message = "Wrong password";
                    return View("Index");
                }

            var u = new HttpCookie("user");
            u["user"] = name;
            u.Expires.Add(new TimeSpan(1, 0, 0));
            Response.Cookies.Add(u);
            ViewBag.User = user;
            return View("Main");
        }

        public ActionResult Register(string name, string password)
        {
            if (name == "")
            {
                ViewBag.Message = "No name";
                return View("Register");
            }

            if (password == "")
            {
                ViewBag.Message = "No password";
                return View("Register");
            }

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(20);
            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var savedPasswordHash = Convert.ToBase64String(hashBytes);
            var user = new User
            {
                Name = name,
                Password = savedPasswordHash
            };
            Client = new FirebaseClient(Config);
            var response = Client.Set("user/" + name, user);
            return View("Index");
        }

        public ActionResult Registerv()
        {
            return View("Register");
        }

        public ActionResult LoginV()
        {
            return View("Index");
        }

        public ActionResult Code(string code)
        {
            Client = new FirebaseClient(Config);
            var response = Client.Get("subscription/" + code);
            var subscription = response.ResultAs<Subscription>();
            return View();
        }

        public ActionResult NewS(string name, DateTime exp, int numberofentrence)
        {
            ViewBag.Name = name;
            ViewBag.Exp = exp;
            ViewBag.Numberofentrence = numberofentrence;
            if (name == "")
            {
                ViewBag.Message = "No name";
                return View("New");
            }

            var subscription = new Subscription();
            subscription.name = name;

            if (exp != DateTime.Now) subscription.exp = exp;

            if (numberofentrence > 0) subscription.numberofentrence = numberofentrence;
            Client = new FirebaseClient(Config);
            var response = Client.Get("subscription");
            if (response.Body != "null")
            {
                 var s = response.ResultAs<List<Subscription>>();
                ViewBag.S = response.Body; //s.Count.ToString();
                subscription.code = s.Count;
                var response2 = Client.Set("subscription/" + s.Count, subscription);
            }
            else
            {
                ViewBag.S = response.Body;
                var response2 = Client.Set("subscription/" + "0", subscription);
            }

            return View("Main");
        }
    }
}