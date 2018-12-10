using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    //GrowthDiary
    public class GrowthDiaryController : Controller
    {
        public ActionResult Index(int publicCourseInfoID,string key)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var query = db.GrowthDiary.Where(p => p.PublicCourseInfoID == publicCourseInfoID
                || p.SharePublicCourseInfoID.Contains(publicCourseInfoID.ToString()));
                if (!string.IsNullOrWhiteSpace(key))
                {
                    query = query.Where(p => p.Content.Contains(key));
                }
                msg.Data = query.ToList();
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetGrowthDiaryByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.GrowthDiary.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(GrowthDiary model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    model.CreateTime = DateTime.Now.ToString(FormatDateTime.LongDateTimeStr);
                    model.UpdateTime = model.CreateTime;
                    var entity = db.GrowthDiary.Add(model);
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

        public ActionResult Update(GrowthDiary model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {

                    model.UpdateTime = DateTime.Now.ToString(FormatDateTime.LongDateTimeStr);
                    db.GrowthDiary.Attach(model);
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
                    db.Database.ExecuteSqlCommand("delete GrowthDiary where id= " + id);
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