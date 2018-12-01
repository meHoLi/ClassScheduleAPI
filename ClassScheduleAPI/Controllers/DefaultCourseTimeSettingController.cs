using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    //DefaultCourseTimeSetting
    public class DefaultCourseTimeSettingController : Controller
    {
        public ActionResult Index(int childrenID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var list = db.DefaultCourseTimeSetting.Where(p => p.ChildrenID == childrenID).ToList();
                msg.Data = list;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Update(int childrenID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                using (var scope = db.Database.BeginTransaction())
                {
                    ResponseMessage msg = new ResponseMessage();
                    try
                    {
                        db.Database.ExecuteSqlCommand("delete DefaultCourseTimeSetting where ChildrenID= " + childrenID);
                        var modelList = Request.Form["modelList"];
                        var list = JsonConvert.DeserializeObject<List<DefaultCourseTimeSetting>>(modelList);
                        var entity = db.DefaultCourseTimeSetting.AddRange(list);
                        db.SaveChanges();
                        msg.Status = true;
                        scope.Commit();
                    }
                    catch (Exception e)
                    {
                        msg.Status = false;
                        scope.Rollback();
                    }
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult Delete(int id)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete DefaultCourseTimeSetting where id= " + id);
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
    }
}