using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bolgs.Services;

namespace Bolgs.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        /// <summary>
        /// 主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewData["this"] = "欢迎我的博客！";
            var list = HomeService.List();
            ViewData["list"] = list;

            return View();
        }

    }
}
