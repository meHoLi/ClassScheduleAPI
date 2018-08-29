using ClassScheduleAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string url = @"https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=WebHelper%20phantomjs&oq=c%2523%2520%25E5%2590%258E%25E5%258F%25B0%25E6%2588%25AA%25E5%258F%2596%25E9%25A1%25B5%25E9%259D%25A2&rsv_pq=96e5792a00085000&rsv_t=9987Rrvcz1cofOUpQgrDqfvssP6rAjybxcQsTEX%2F5uVjg2tNEdc7fBNtSms&rqlang=cn&rsv_enter=1&inputT=13942&rsv_n=2&rsv_sug3=46&bs=c%23%20%E5%90%8E%E5%8F%B0%E6%88%AA%E5%8F%96%E9%A1%B5%E9%9D%A2";
            var web = new WebHelper(url, @"D:\ImgName2\A.JPG", @"D:\Private\DK\ClassSchedule\ClassScheduleAPI\ClassScheduleAPI\Pic");
            web.GetImg();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}