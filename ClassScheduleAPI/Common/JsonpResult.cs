using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ClassScheduleAPI.Common
{
    public class JsonpResult<T> : ActionResult
    {
        public JsonpResult(T obj, string callback)
        {
            this.Obj = obj;
            this.CallbackName = callback;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var js = new JavaScriptSerializer();
            var jsonp = this.CallbackName.Trim() == "" ? js.Serialize(this.Obj) :
                this.CallbackName + "(" + js.Serialize(this.Obj) + ")";

            string host = HttpContext.Current.Request.UrlReferrer != null ?
                HttpContext.Current.Request.UrlReferrer.Host : "";
            string port = HttpContext.Current.Request.UrlReferrer != null ?
                HttpContext.Current.Request.UrlReferrer.Port.ToString() : "80";

            if (AllowedHost.Find(o => o == host) != null || host == "")
            {
                context.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            }
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(jsonp);
        }

        public T Obj { get; set; }
        public string CallbackName { get; set; }
        protected List<string> AllowedHost = new List<string>() 
        { 
            //"192.168.1.241",
            //"localhost"
        };

    }
}