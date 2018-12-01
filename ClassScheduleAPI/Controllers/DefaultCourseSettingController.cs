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
    //DefaultCourseSetting
    public class DefaultCourseSettingController : Controller
    {
        public ActionResult GetDefaultCourseSettingByChildrenID(int childrenID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var model = db.DefaultCourseSetting.Where(p => p.ChildrenID == childrenID).FirstOrDefault();
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetModel()
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    //添加默认时间
                    var modelStr = Request.Form["model"];
                    var model = JsonConvert.DeserializeObject<DefaultCourseSetting>(modelStr);
                    //var isExist= db.DefaultCourseSetting.FirstOrDefault(p => p.ID == model.ID);
                    //代表添加
                    if (model.ID==0)
                    {
                        var entity = db.DefaultCourseSetting.Add(model);
                    }
                    else//代表编辑
                    {
                        db.DefaultCourseSetting.Attach(model);
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    }
                    //删除默认时间设置
                    db.Database.ExecuteSqlCommand("delete DefaultCourseTimeSetting where ChildrenID= " + model.ChildrenID);
                    //添加默认时间
                    var timeModelList = Request.Form["timeModelList"];
                    var list = JsonConvert.DeserializeObject<List<DefaultCourseTimeSetting>>(timeModelList);
                    db.DefaultCourseTimeSetting.AddRange(list);
                    db.SaveChanges();
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(int id)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete DefaultCourseSetting where id= " + id);
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