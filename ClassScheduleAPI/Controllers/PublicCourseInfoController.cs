using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class PublicCourseInfoController : Controller
    {
        public ActionResult Index(string openID, int publicCourseTypeID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var list = db.PublicCourseInfo.Where(p => p.OpenID == openID && p.PublicCourseTypeID == publicCourseTypeID).ToList();
                msg.Data = list;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPublicCourseInfoByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.PublicCourseInfo.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPublicCourseInfoByLoginNameAndPassword(string loginName, string password)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.PublicCourseInfo.FirstOrDefault(p => p.LoginName == loginName && p.Password == password);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(PublicCourseInfo model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    bool isExisted = db.PublicCourseInfo.Any(p => p.LoginName == model.LoginName);
                    if (isExisted)
                    {
                        msg.Status = false;
                        msg.Result = "800";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    var entity = db.PublicCourseInfo.Add(model);
                    db.SaveChanges();
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                    msg.Result = "500";
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Update(PublicCourseInfo model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    bool isExisted = db.PublicCourseInfo.Any(p => p.ID != model.ID && p.LoginName == model.LoginName);
                    if (isExisted)
                    {
                        msg.Status = false;
                        msg.Result = "700";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    db.PublicCourseInfo.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                    msg.Result = "500";
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
                    db.Database.ExecuteSqlCommand("delete PublicCourseInfo where id= " + id);
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                    msg.Result = "500";
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
    }
}