using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    //ChildrenStandardCourse
    public class ChildrenStandardCourseController : Controller
    {
        public ActionResult Index(int childrenID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var list = db.ChildrenStandardCourse.Where(p => p.ChildrenID == childrenID).OrderBy(p => p.Sort).ToList();
                msg.Data = list;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetChildrenStandardCourseByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.ChildrenStandardCourse.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(ChildrenStandardCourse model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    var entity = db.ChildrenStandardCourse.Add(model);
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

        public ActionResult Update(ChildrenStandardCourse model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.ChildrenStandardCourse.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
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
                    db.Database.ExecuteSqlCommand("delete ChildrenStandardCourse where id= " + id);
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