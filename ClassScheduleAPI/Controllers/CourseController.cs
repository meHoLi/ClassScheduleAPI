using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;

namespace ClassScheduleAPI.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        public ActionResult Index(int childrenID, string startTime, string endTime)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var list = db.Course.Where(p => p.ChildrenID == childrenID
                && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                .ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(Course model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    var entity = db.Course.Add(model);
                    db.SaveChanges();
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg);
            }
        }
        public ActionResult Update(Course model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Course.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg);
            }
        }
        public ActionResult Delete(int id)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete Course where id= " + id);
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg);
            }
        }
    }
}