using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Timers;


namespace Bolgs.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/

        public ActionResult Index()
        {
            Timer time = new Timer();
            time.Enabled = true;
            
            return View();
        }

        public ActionResult Login() 
        {
            
            return View();
        }

    }
}
