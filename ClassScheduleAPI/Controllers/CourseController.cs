using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using ClassScheduleAPI.ModelsBusiness;
using Newtonsoft.Json;

namespace ClassScheduleAPI.Controllers
{
    public class CourseController : Controller
    {


        // GET: Course
        #region 私有课程表的一些操作

        public ActionResult Index(int childrenID, int page = 1, int pageSize = 7)
        {
            ResponseMessage msg = new ResponseMessage();
            List<CourseBusiness> cbList = new List<CourseBusiness>();
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

        public ActionResult GetCourseByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.Course.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
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
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 时间范围内是否已经存在数据
        /// </summary>
        /// <param name="courseList"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CourseListRangeAny(List<Course> courseList, Course model)
        {
            bool isExisted = courseList.Any(p =>
                          (string.Compare(p.StartTime, model.EndTime, StringComparison.Ordinal) <= 0 && string.Compare(model.EndTime, p.EndTime, StringComparison.Ordinal) <= 0)
                           ||
                           (string.Compare(p.StartTime, model.StartTime, StringComparison.Ordinal) <= 0 && string.Compare(model.StartTime, p.EndTime, StringComparison.Ordinal) <= 0)
                           );
            return isExisted;
        }



        public ActionResult Add(Course model)
        {
            //新加入的课程数据集合
            List<Course> newCourseList = new List<Course>();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    model.BatchID = Guid.NewGuid();
                    var courseList = db.Course.Where(p => p.ChildrenID == model.ChildrenID && model.ChildrenID != 0).ToList();
                    for (int i = 0; i < ApplicationConstant.forDay;)
                    {
                        if (model.Frequency == ((int)EnumUnit.FrequencyEnum.TodayOnly).ToString())
                        {
                            bool isExisted = CourseListRangeAny(courseList, model);
                            if (isExisted)
                            {
                                msg.Status = false;
                                //【仅今天】代表数据已经存在
                                msg.Result = "800";
                                return Json(msg, JsonRequestBehavior.AllowGet);
                            }
                            newCourseList.Add(model);
                            i = ApplicationConstant.forDay;
                        }
                        else if (model.Frequency == ((int)EnumUnit.FrequencyEnum.EveryDay).ToString())
                        {
                            bool isExisted = CourseListRangeAny(courseList, model);
                            if (isExisted)
                            {
                                msg.Status = false;
                                //【每天这个时段】代表数据已经存在
                                msg.Result = "800";
                                return Json(msg, JsonRequestBehavior.AllowGet);
                            }
                            var t = ObjectHelper.TransReflection<Course, Course>(model);
                            newCourseList.Add(t);
                            int interval = 1;
                            i = i + interval;
                            model.StartTime = (DateTime.Parse(model.StartTime).AddDays(interval)).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                            model.EndTime = (DateTime.Parse(model.EndTime).AddDays(interval)).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                        }
                        else if (model.Frequency == ((int)EnumUnit.FrequencyEnum.EveryWeek).ToString())
                        {
                            bool isExisted = CourseListRangeAny(courseList, model);
                            if (isExisted)
                            {
                                msg.Status = false;
                                //【每周这个时段】代表数据已经存在
                                msg.Result = "800";
                                return Json(msg, JsonRequestBehavior.AllowGet);
                            }
                            var t = ObjectHelper.TransReflection<Course, Course>(model);
                            newCourseList.Add(t);
                            int interval = 7;
                            i = i + interval;
                            model.StartTime = (DateTime.Parse(model.StartTime).AddDays(interval)).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                            model.EndTime = (DateTime.Parse(model.EndTime).AddDays(interval)).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                        }
                    }


                    var entity = db.Course.AddRange(newCourseList);
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
        public ActionResult Update(Course model)
        {
            string oldBatchID = model.BatchID.ToString();
            //新加入的课程数据集合
            List<Course> newCourseList = new List<Course>();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    model.BatchID = Guid.NewGuid();
                    var courseList = db.Course.Where(p => p.ID != model.ID && p.ChildrenID == model.ChildrenID).ToList();
                    for (int i = 0; i < ApplicationConstant.forDay;)
                    {
                        if (model.Frequency == ((int)EnumUnit.FrequencyEnum.TodayOnly).ToString())
                        {
                            bool isExisted = CourseListRangeAny(courseList, model);
                            if (isExisted)
                            {
                                msg.Status = false;
                                //【仅今天】代表数据已经存在
                                msg.Result = "800";
                                return Json(msg, JsonRequestBehavior.AllowGet);
                            }
                            newCourseList.Add(model);
                            i = ApplicationConstant.forDay;
                        }
                        else if (model.Frequency == ((int)EnumUnit.FrequencyEnum.EveryDay).ToString())
                        {
                            //bool isExisted = CourseListRangeAny(courseList, model);
                            //if (isExisted)
                            //{
                            //    msg.Status = false;
                            //    //【每天这个时段】代表数据已经存在
                            //    msg.Result = "800";
                            //    return Json(msg, JsonRequestBehavior.AllowGet);
                            //}
                            var t = ObjectHelper.TransReflection<Course, Course>(model);
                            newCourseList.Add(t);
                            int interval = 1;
                            i = i + interval;
                            model.StartTime = (DateTime.Parse(model.StartTime).AddDays(interval)).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                            model.EndTime = (DateTime.Parse(model.EndTime).AddDays(interval)).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                        }
                        else if (model.Frequency == ((int)EnumUnit.FrequencyEnum.EveryWeek).ToString())
                        {
                            //bool isExisted = CourseListRangeAny(courseList, model);
                            //if (isExisted)
                            //{
                            //    msg.Status = false;
                            //    //【每周这个时段】代表数据已经存在
                            //    msg.Result = "800";
                            //    return Json(msg, JsonRequestBehavior.AllowGet);
                            //}
                            var t = ObjectHelper.TransReflection<Course, Course>(model);
                            newCourseList.Add(t);
                            int interval = 7;
                            i = i + interval;
                            model.StartTime = (DateTime.Parse(model.StartTime).AddDays(interval)).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                            model.EndTime = (DateTime.Parse(model.EndTime).AddDays(interval)).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                        }
                    }

                    ////导入的课程表先删除，然后重新添加
                    //if (model.PublicCourseInfoID != 0)
                    db.Database.ExecuteSqlCommand("delete Course where BatchID= '" + oldBatchID + "'");
                    //else if (model.PublicCourseInfoID == 0)//代表不是导入的
                    //    db.Database.ExecuteSqlCommand("delete Course where ID= " + model.ID);

                    var entity = db.Course.AddRange(newCourseList);
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
                    db.Database.ExecuteSqlCommand("delete Course where id= " + id);
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 导入课程表
        /// </summary>
        /// <param name="publicCourseInfoID"></param>
        /// <returns></returns>

        public ActionResult ImportCourse(int publicCourseInfoID, int childrenID)
        {
            //新加入的课程数据集合
            List<Course> newCourseList = new List<Course>();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                //isExistence 是否有重复数据
                msg.Data = new { isExistence = false };
                try
                {
                    var list = db.Course.Where(p => p.PublicCourseInfoID == publicCourseInfoID).ToList();
                    string startTime = list.Min(p => p.StartTime);
                    string endTime = list.Max(p => p.EndTime);
                    //校验导入课程表的时间段内是否有课程冲突
                    var clist = db.Course.Where(p => p.ChildrenID == childrenID
                  && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                  && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                .ToList();

                    var batchID = Guid.NewGuid();
                    foreach (var item in list)
                    {
                        bool isExisted = CourseListRangeAny(clist, item);
                        if (isExisted)
                        {
                            msg.Status = false;
                            msg.Result = "800";
                            return Json(msg, JsonRequestBehavior.AllowGet);
                        }
                        Course model = ObjectHelper.TransReflection<Course, Course>(item);
                        model.ChildrenID = childrenID;
                        model.BatchID = batchID;
                        newCourseList.Add(model);
                    }
                    var entity = db.Course.AddRange(newCourseList);
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

        #endregion

        #region 公共课程表的一些操作


        public ActionResult GetPublicCourseIndex(int publicCourseInfoID, string openID, int page = 1, int pageSize = 7)
        {
            ResponseMessage msg = new ResponseMessage();
            List<CourseBusiness> cbList = new List<CourseBusiness>();
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

            List<Course> cList = new List<Course>();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                msg.Status = true;
                try
                {
                    cList = db.Course.Where(p => p.PublicCourseInfoID == publicCourseInfoID
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


        public ActionResult GetPublicCourseByDate(int publicCourseInfoID, string startTime, string endTime)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                msg.Status = true;
                try
                {
                    var list = db.Course.Where(p => p.PublicCourseInfoID == publicCourseInfoID && p.ChildrenID == 0
                           && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                           && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                           .ToList();
                    List<CourseBusiness> cbList = new List<CourseBusiness>();
                    foreach (var item in list)
                    {
                        CourseBusiness model = new CourseBusiness();
                        model.Address = item.Address;
                        model.PublicCourseTypeID = item.PublicCourseTypeID;
                        model.CourseName = item.CourseName;
                        model.EndTime = item.EndTime;
                        model.ID = item.ID;
                        if (string.IsNullOrWhiteSpace(item.RemindTime) || item.RemindTime == "-9999")
                            model.RemindDes = "未设置";
                        else
                            model.RemindDes = "已设置";
                        //model.RemindDes = DateTime.Parse(item.StartTime).AddMinutes(int.Parse(item.RemindTime)) < DateTime.Now ? "已提醒" : "未提醒";
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
                    db.Database.ExecuteSqlCommand("delete Course where PublicCourseInfoID=" + publicCourseInfoID
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


        #endregion





















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
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                //isExistence 是否有重复数据
                msg.Data = new { isExistence = false };
                try
                {
                    //上周开始时间
                    string st = DateTime.Parse(startTime).AddDays(-interval).ToString(FormatDateTime.LongDateTimeStr);
                    //上周结束时间
                    string et = DateTime.Parse(endTime).AddDays(-interval).ToString(FormatDateTime.LongDateTimeStr);

                    ////上周课程数据
                    //var list = db.Course.Where(p => p.ChildrenID == childrenID
                    //  && (
                    //      (string.Compare(p.StartTime, et, StringComparison.Ordinal) <= 0 && string.Compare(et, p.EndTime, StringComparison.Ordinal) <= 0)
                    //       ||
                    //       (string.Compare(p.StartTime, st, StringComparison.Ordinal) <= 0 && string.Compare(st, p.EndTime, StringComparison.Ordinal) <= 0)
                    //       ))
                    //.ToList();

                    //上周课程数据
                    var list = db.Course.Where(p => p.ChildrenID == childrenID
                && string.Compare(p.StartTime, st, StringComparison.Ordinal) >= 0
                && string.Compare(p.EndTime, et, StringComparison.Ordinal) <= 0)
                .ToList();

                    //当前周课程数据
                    var clist = db.Course.Where(p => p.ChildrenID == childrenID
                  && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                  && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                .ToList();
                    foreach (var item in list)
                    {
                        item.StartTime = DateTime.Parse(item.StartTime).AddDays(interval).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                        item.EndTime = DateTime.Parse(item.EndTime).AddDays(interval).ToString(FormatDateTime.LongDateTimeNoSecondStr);
                        bool isExistence = clist.Any(p => p.ChildrenID == childrenID
                        && (
                          (string.Compare(p.StartTime, item.EndTime, StringComparison.Ordinal) <= 0 && string.Compare(item.EndTime, p.EndTime, StringComparison.Ordinal) <= 0)
                           ||
                           (string.Compare(p.StartTime, item.StartTime, StringComparison.Ordinal) <= 0 && string.Compare(item.StartTime, p.EndTime, StringComparison.Ordinal) <= 0)
                           ));
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
                    msg.Result = "500";
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 本周新加课程（清空本周课程）
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult Deletes2(int childrenID, string startTime, string endTime)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete Course where ChildrenID=" + childrenID + " and StartTime>= '" + startTime + "' and EndTime<='" + endTime + "'");
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