using System;
using System.Collections.Generic;
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

        public ActionResult New()
        {
            return View();
        }


        public ActionResult Code(string code)
        {
            Client = new FirebaseClient(Config);
            var response = Client.Get("subscription/" + code);
            if (response.Body != "null")
            {
                var subscription = response.ResultAs<Subscription>();
                ViewBag.Subscription = subscription;
                return View();
            }
            ViewBag.Message = "Wrong code";
                return View("Index");
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
            else
            {
                subscription.exp = DateTime.MinValue;
            }

            if (numberofentrence > 0) subscription.numberofentrence = numberofentrence;
            else
            {
                subscription.numberofentrence = -1;
            }
            Client = new FirebaseClient(Config);
            var response = Client.Get("subscription");
            if (response.Body != "null")
            {
                 var s = response.ResultAs<List<Subscription>>();
                 subscription.code = s.Count;
                var response2 = Client.Set("subscription/" + s.Count, subscription);
            }
            else
            {
                var response2 = Client.Set("subscription/" + "0", subscription);
            }

            return View("Index");
        }

        public ActionResult Update(string name,DateTime exp,int maxnumberofentrence,int code)
        {
            Client = new FirebaseClient(Config);
            Subscription subscription = new Subscription();
            subscription.name = name;
            subscription.exp = exp;
            subscription.maxnumberofentrence = maxnumberofentrence;
            subscription.code = code;
            subscription.numberofentrence = 0;
            var response = Client.Update("subscription/"+code,subscription);
            return View("Index");
        }

        public ActionResult Enter(int code)
        {
            Client = new FirebaseClient(Config);
            var response = Client.Get("subscription/" + code);
            var subscription = response.ResultAs<Subscription>();
            subscription.numberofentrence++;
            var response2 = Client.Update("subscription/"+code,subscription);
            return View("Index");
        }
    }
}