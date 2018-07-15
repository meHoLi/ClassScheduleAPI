using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using ClassScheduleAPI.ModelsBusiness;

namespace ClassScheduleAPI.Controllers
{
    public class CourseController : Controller
    {


        /// <summary>
        /// 得到本周第一天(以星期一为第一天)
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public DateTime GetWeekFirstDayMon(DateTime datetime)
        {
            //星期一为第一天
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);

            //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;

            //本周第一天
            string FirstDay = datetime.AddDays(daydiff).ToString(FormatDateTime.ShortDateTimeStr);
            return Convert.ToDateTime(FirstDay);
        }



        // GET: Course
        public ActionResult Index(int childrenID, int page = 1, int pageSize = 7)
        {
            ResponseMessage msg = new ResponseMessage();
            List<CourseBusiness> cbList = new List<CourseBusiness>();
            string startTime;
            string endTime;
            DateTime st;
            DateTime et;
            Children cModel = new Children();
            var ct = GetWeekFirstDayMon(DateTime.Now);
            st = ct.AddDays((page - 1) * pageSize);
            et = ct.AddDays(page * pageSize);
            startTime = st.ToString(FormatDateTime.ShortDateTimeStr);
            endTime = et.ToString(FormatDateTime.ShortDateTimeStr);

            List<Course> cList = new List<Course>();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                msg.Status = true;
                try
                {
                    cList = db.Course.Where(p => p.ChildrenID == childrenID
                           && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                           && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                           .ToList();

                    cModel = db.Children.FirstOrDefault(p => p.ID == childrenID);
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
            }


            for (int i = 0; i < pageSize; i++)
            {
                CourseBusiness model = new CourseBusiness();
                model.DayOfWeek = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(st.AddDays(i).DayOfWeek);
                model.StartTime = st.AddDays(i).ToString(FormatDateTime.ShortDateTimeStr);
                model.BackgroundColor = cList.Any(p => p.StartTime.Contains(model.StartTime)) ? cModel.Background : "";
                model.IsToday = model.StartTime == DateTime.Now.ToString(FormatDateTime.ShortDateTimeStr) ? true : false;
                cbList.Add(model);
            }
            msg.Data = cbList;
            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetChildrenCourseByDate(int childrenID, string startTime, string endTime)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                msg.Status = true;
                try
                {
                    var list = db.Course.Where(p => p.ChildrenID == childrenID
                           && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                           && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                           .ToList();
                    List<CourseBusiness> cbList = new List<CourseBusiness>();
                    foreach (var item in list)
                    {
                        CourseBusiness model = new CourseBusiness();
                        model.Address = item.Address;
                        model.ChildrenID = item.ChildrenID;
                        model.CourseName = item.CourseName;
                        model.EndTime = item.EndTime;
                        model.ID = item.ID;
                        if (string.IsNullOrWhiteSpace(item.RemindTime))
                            model.RemindDes = "未设置";
                        else
                            model.RemindDes = DateTime.Parse(item.RemindTime) > DateTime.Now ? "已提醒" : "未提醒";
                        model.RemindTime = item.RemindTime;
                        model.Phone = item.Phone;
                        model.Remarks = item.Remarks;
                        model.SchoolName = item.SchoolName;
                        model.StartTime = item.StartTime;
                        model.Teacher = item.Teacher;
                        cbList.Add(model);
                    }
                    msg.Data = cbList;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
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