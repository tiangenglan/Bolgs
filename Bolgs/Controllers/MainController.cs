using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Timers;
using Gozyy.Framework;
using Gozyy.ToolClass.Models;
using Bolgs.Services;

namespace Bolgs.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/

        public ActionResult Index()
        {
            
            
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username,string password) 
        {

            var user = UserService.Login(username,password);
            if(user!=null)
            {
                return Content("0");
            }
            return Content("1");
        }

    }
}
