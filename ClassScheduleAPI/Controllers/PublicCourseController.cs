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
    public class PublicPublicCourseController : Controller
    {

        /***********************************************************************************************
        弃用  ，统一使用 Course  表
        ************************************************************************************************/
        // GET: PublicPublicCourse    


        public ActionResult Index(int publicCourseTypeID, string openID, int page = 1, int pageSize = 7)
        {
            ResponseMessage msg = new ResponseMessage();
            List<PublicCourseBusiness> cbList = new List<PublicCourseBusiness>();
            string startTime;
            string endTime;
            DateTime st;
            DateTime et;
            Children cModel = new Children();
            var ct = DateCalc.GetWeekFirstDayMon(DateTime.Now);
            st = ct.AddDays((page - 1) * pageSize);
            et = ct.AddDays(page * pageSize);
            startTime = st.ToString(FormatDateTime.ShortDateTimeStr);
            endTime = et.ToString(FormatDateTime.ShortDateTimeStr);

            List<PublicCourse> cList = new List<PublicCourse>();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                msg.Status = true;
                try
                {
                    cList = db.PublicCourse.Where(p => p.PublicCourseTypeID == publicCourseTypeID
                           && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                           && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                           .ToList();
                    //默认使用第一个孩子的背景色
                    cModel = db.Children.FirstOrDefault(p => p.OpenID == openID);
                    //防止没有孩子的情况出现。
                    if (cModel == null) cModel = new Children();
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
            }


            for (int i = 0; i < pageSize; i++)
            {
                PublicCourseBusiness model = new PublicCourseBusiness();
                model.DayOfWeek = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(st.AddDays(i).DayOfWeek);
                model.StartTime = st.AddDays(i).ToString(FormatDateTime.ShortDateTimeStr);
                model.BackgroundColor = cList.Any(p => p.StartTime.Contains(model.StartTime)) ? cModel.Background : "";
                model.IsToday = model.StartTime == DateTime.Now.ToString(FormatDateTime.ShortDateTimeStr) ? true : false;
                cbList.Add(model);
            }
            msg.Data = cbList;
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPublicCourseByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.PublicCourse.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetChildrenPublicCourseByDate(int publicCourseTypeID, string startTime, string endTime)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                msg.Status = true;
                try
                {
                    var list = db.PublicCourse.Where(p => p.PublicCourseTypeID == publicCourseTypeID
                           && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                           && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                           .ToList();
                    List<PublicCourseBusiness> cbList = new List<PublicCourseBusiness>();
                    foreach (var item in list)
                    {
                        PublicCourseBusiness model = new PublicCourseBusiness();
                        model.Address = item.Address;
                        model.PublicCourseTypeID = item.PublicCourseTypeID;
                        model.CourseName = item.CourseName;
                        model.EndTime = item.EndTime;
                        model.ID = item.ID;
                        if (string.IsNullOrWhiteSpace(item.RemindTime) || item.RemindTime == "-9999")
                            model.RemindDes = "未设置";
                        else
                            model.RemindDes = DateTime.Parse(item.StartTime).AddMinutes(int.Parse(item.RemindTime)) < DateTime.Now ? "已提醒" : "未提醒";
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
                    msg.Result = "500";
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 时间范围内是否已经存在数据
        /// </summary>
        /// <param name="PublicCourseList"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool PublicCourseListRangeAny(List<PublicCourse> PublicCourseList, PublicCourse model)
        {
            bool isExisted = PublicCourseList.Any(p =>
                          (string.Compare(p.StartTime, model.EndTime, StringComparison.Ordinal) <= 0 && string.Compare(model.EndTime, p.EndTime, StringComparison.Ordinal) <= 0)
                           ||
                           (string.Compare(p.StartTime, model.StartTime, StringComparison.Ordinal) <= 0 && string.Compare(model.StartTime, p.EndTime, StringComparison.Ordinal) <= 0)
                           );
            return isExisted;
        }


        public ActionResult Add(PublicCourse model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    var entity = db.PublicCourse.Add(model);
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
        public ActionResult Update(PublicCourse model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.PublicCourse.Attach(model);
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
                    db.Database.ExecuteSqlCommand("delete PublicCourse where id= " + id);
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
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult Deletes(int publicCourseInfoID, string startTime, string endTime)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete PublicCourse where PublicCourseInfoID=" + publicCourseInfoID
                        + " and StartTime>= '" + startTime + "' and EndTime<='" + endTime + "'");
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