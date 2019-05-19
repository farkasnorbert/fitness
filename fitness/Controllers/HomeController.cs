using fitness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace fitness.Controllers
{
    public class HomeController : Controller
    {
        IFirebaseConfig Config = new FirebaseConfig
        {
            AuthSecret = "IiftR9cls1Smo9ISn97siEHaGM61fRSydYcVQRej",
            BasePath = "https://fitness-eee13.firebaseio.com/"
        };
        
        IFirebaseClient Client;
        public ActionResult Index()
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
            FirebaseResponse response = Client.Get("user/"+name);
            User user = response.ResultAs<User>();
            string savedPasswordHash = user.Password;
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    ViewBag.Message = "Wrong password";
                    return View("Index");
                }
            }

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
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            var user = new User
            {
                Name = name,
                Password = savedPasswordHash
            };
            Client = new FirebaseClient(Config);
            SetResponse response = Client.Set("user/" + name, user);
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
    }
}