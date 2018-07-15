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

        /// <summary>
        /// 和上周一样(可灵活设置时间段，和间隔)
        /// </summary>
        /// <param name="childrenID"></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ActionResult AddCourseList(int childrenID, string startTime, string endTime, int interval = 7)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var list = db.Course.Where(p => p.ChildrenID == childrenID
                && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                .ToList();
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                msg.Data = new { isExistence = false };
                try
                {
                    string st = DateTime.Parse(startTime).AddDays(interval).ToString(FormatDateTime.LongDateTimeStr);
                    string et = DateTime.Parse(endTime).AddDays(interval).ToString(FormatDateTime.LongDateTimeStr);
                    var clist = db.Course.Where(p => p.ChildrenID == childrenID
                  && string.Compare(p.StartTime, st, StringComparison.Ordinal) >= 0
                  && string.Compare(p.EndTime, et, StringComparison.Ordinal) <= 0)
                .ToList();
                    foreach (var item in list)
                    {
                        item.StartTime = DateTime.Parse(item.StartTime).AddDays(interval).ToString(FormatDateTime.LongDateTimeStr);
                        item.EndTime = DateTime.Parse(item.EndTime).AddDays(interval).ToString(FormatDateTime.LongDateTimeStr);
                        bool isExistence = clist.Any(p => p.ChildrenID == childrenID
                   && string.Compare(p.StartTime, item.StartTime, StringComparison.Ordinal) >= 0
                   && string.Compare(p.EndTime, item.EndTime, StringComparison.Ordinal) <= 0);
                        if (!isExistence)
                        {
                            var entity = db.Course.Add(item);
                            db.SaveChanges();
                        }
                        else
                        {
                            //有冲突的数据，标识并提示
                            msg.Msg += item.StartTime + ",";
                            msg.Data = new { isExistence = true };
                        }
                    }
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