using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassScheduleAPI.Common;
namespace ClassScheduleAPI.Controllers
{
    public class WeChatAppAuthorizeController : Controller
    {
        // GET: WeChatAppAuthorize
        public ActionResult Index()
        {
            return View();
        }


        private ActionResult GetOpenIdAndSessionKeyString(string code)
        {
            ResponseMessage msg = new ResponseMessage();
            msg.Status = true;
            try
            {

            }
            catch (Exception e)
            {
                msg.Status = false;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        //https://blog.csdn.net/ivanyoung66/article/details/72523231
        //https://bbs.csdn.net/topics/392320383



        //小程序提醒
        //http://www.cnblogs.com/vanteking/p/7606222.html
        //https://www.cnblogs.com/qiujz/articles/5913796.html
    }
}