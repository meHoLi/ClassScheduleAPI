using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class ClockController : Controller
    {
        //Clock
        public ActionResult Index(int childrenID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var list = db.Clock.Where(p => p.ChildrenID == childrenID).OrderBy(p => p.ID).ToList();
                msg.Data = list;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetClockByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.Clock.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Add(Clock model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    var entity = db.Clock.Add(model);
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

        public ActionResult Update(Clock model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Clock.Attach(model);
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
                    db.Database.ExecuteSqlCommand("delete Clock where id= " + id);
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