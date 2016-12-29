using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gozyy.ToolClass.Models;
using Gozyy.Repositories;
namespace Bolgs.Services
{
    public class HomeService
    {

        public static List<Tbbolgs> List() 
        {
            var list = IRepository<Tbbolgs>.List("");
            return list;
        }
    }
}