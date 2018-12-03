using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using ClassScheduleAPI.ModelsBusiness;
using Newtonsoft.Json;
using System.Globalization;

namespace ClassScheduleAPI.Controllers
{
    public class CourseController : Controller
    {


        // GET: Course
        #region 私有课程表的一些操作

        public ActionResult Index(int childrenID, int page = 1, int pageSize = 7)
        {
            LogHelper.Info("CourseController->Index");
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
            LogHelper.Info("CourseController->GetCourseByID");
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.Course.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childrenID"></param>
        /// <param name="publicCourseInfoID">0 代表不是公共课程的数据</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetChildrenCourseByDate(int childrenID, string startTime, string endTime)
        {
            LogHelper.Info("CourseController->GetChildrenCourseByDate");
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

                        model = ObjectHelper.TransReflection<Course, CourseBusiness>(item);
                        //model.Address = item.Address;
                        //model.ChildrenID = item.ChildrenID;
                        //model.CourseName = item.CourseName;
                        //model.EndTime = item.EndTime;
                        //model.ID = item.ID;
                        if (string.IsNullOrWhiteSpace(item.RemindTime) || item.RemindTime == "-9999")
                            model.RemindDes = "未设置";
                        else
                            model.RemindDes = DateTime.Parse(item.StartTime).AddMinutes(int.Parse(item.RemindTime)) < DateTime.Now ? "已提醒" : "未提醒";
                        //model.RemindTime = item.RemindTime;
                        //model.Phone = item.Phone;
                        //model.Remarks = item.Remarks;
                        //model.SchoolName = item.SchoolName;
                        //model.StartTime = item.StartTime;
                        //model.Teacher = item.Teacher;
                        model.DayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Parse(model.StartTime).DayOfWeek);
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
        /// 【本周】页面使用接口，数据经过了展示处理
        /// </summary>
        /// <param name="childrenID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetChildrenCourseByDateFormatOfWeek(int childrenID = 58, string startTime = "2018-10-12", string endTime = "2018-10-13")
        {
            //上午下午晚上的时间划分间隔
            int interval = 8;
            //上午结束时间
            int morningEndHour = 12;
            //下午结束时间
            int afternoonEndHour = 18;
            //晚上结束时间
            int NightEndHour = 4;


            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                msg.Status = true;
                var list = db.Course.Where(p => p.ChildrenID == childrenID
                           && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                           && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                          .OrderBy(p => p.StartTime).ToList();
                List<CourseBusiness> rList = new List<CourseBusiness>();
                var groupList = list.GroupBy(p => p.StartTime.Substring(0, 10)).ToList();
                //【上午】先把数据按照每天分开
                foreach (var group in groupList)
                {
                    int i = 0;
                    foreach (var item in group)
                    {
                        CourseBusiness model = new CourseBusiness();
                        model = ObjectHelper.TransReflection<Course, CourseBusiness>(item);
                        //【上午】：初始化为当天0点。
                        model.ShowDate = item.StartTime.Substring(0, 11) + "00:00";
                        model.ShowDate = DateTime.Parse(model.ShowDate).AddHours(i).ToString(FormatDateTime.LongDateTimeStr);
                        model.DayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Parse(model.StartTime).DayOfWeek);
                        rList.Add(model);
                        i++;
                    }
                }
                //【下午】把下午的数据showDate更正
                foreach (var group in groupList)
                {
                    int i = 0;
                    foreach (var item in group)
                    {
                        DateTime dt = DateTime.Parse(item.StartTime);
                        //时间大于morningEndHour的，从morningEndHour开始排序
                        //int hour = dt.Hour;
                        //if (hour == morningEndHour && dt.Minute > 0) hour = hour + 1;
                        if (dt.Hour >= morningEndHour)
                        {
                            CourseBusiness model = rList.FirstOrDefault(p => p.ID == item.ID);
                            //初始化为当天morningEndHour点。
                            model.ShowDate = item.StartTime.Substring(0, 11) + morningEndHour.ToString() + ":01";
                            model.ShowDate = DateTime.Parse(model.ShowDate).AddHours(i).ToString(FormatDateTime.LongDateTimeStr);
                            i++;
                        }
                    }
                }
                //【晚上】加完时间之后变为第二天凌晨的需要减去1天
                //【下午】把下午的数据showDate更正
                foreach (var group in groupList)
                {
                    int i = 0;
                    foreach (var item in group)
                    {
                        DateTime dt = DateTime.Parse(item.StartTime);
                        int hour = dt.Hour;
                        //if (hour == afternoonEndHour && dt.Minute > 0) hour = hour + 1;
                        if (hour >= afternoonEndHour)
                        {
                            CourseBusiness model = rList.FirstOrDefault(p => p.ID == item.ID);
                            //model.ShowDate = DateTime.Parse(model.ShowDate).AddHours(interval).ToString(FormatDateTime.LongDateTimeStr);
                            //初始化为当天afternoonEndHour点。
                            model.ShowDate = item.StartTime.Substring(0, 11) + afternoonEndHour.ToString() + ":01";
                            model.ShowDate = DateTime.Parse(model.ShowDate).AddHours(i).ToString(FormatDateTime.LongDateTimeStr);
                            //加完时间之后变为第二天凌晨的需要减去1天
                            //DateTime sdt = DateTime.Parse(model.ShowDate);
                            //if (sdt.Day != DateTime.Parse(model.StartTime).Day) model.ShowDate = sdt.AddDays(-1).ToString(FormatDateTime.LongDateTimeStr);
                            i++;

                        }
                    }
                }

                //重新构造数据  每段时间最多给8节课
                List<CourseBusiness> rList2 = new List<CourseBusiness>();
                var gList = rList.GroupBy(p => p.StartTime.Substring(0, 10)).ToList();
                foreach (var group in gList)
                {
                    //上午
                    var mList = group.Where(p => string.Compare(p.StartTime, p.StartTime.Substring(0, 11) + morningEndHour.ToString(),
                          StringComparison.Ordinal) < 0).ToList();
                    mList = mList.Take(8).ToList();
                    int showNum = 1;
                    foreach (var item in mList)
                    {
                        item.ShowNum = showNum;
                        showNum++;
                    }
                    //下午
                    var aList = group.Where(p => string.Compare(p.StartTime, p.StartTime.Substring(0, 11) + morningEndHour.ToString(),
                          StringComparison.Ordinal) >= 0 && string.Compare(p.StartTime, p.StartTime.Substring(0, 11) + afternoonEndHour.ToString(),
                          StringComparison.Ordinal) < 0).ToList();
                    aList = aList.Take(8).ToList();
                    showNum = 9;
                    foreach (var item in aList)
                    {
                        item.ShowNum = showNum;
                        showNum++;
                    }
                    //晚上
                    var nList = group.Where(p => string.Compare(p.StartTime, p.StartTime.Substring(0, 11) + afternoonEndHour.ToString(),
                          StringComparison.Ordinal) >= 0).ToList();
                    nList = nList.Take(8).ToList();
                    showNum = 17;
                    foreach (var item in nList)
                    {
                        item.ShowNum = showNum;
                        showNum++;
                    }
                    rList2.AddRange(mList);
                    rList2.AddRange(aList);
                    rList2.AddRange(nList);

                }
                msg.Data = rList2.OrderBy(p => p.ShowDate).ThenBy(p => p.ShowNum); //rList.OrderBy(p => p.ShowDate).ToList();
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
            LogHelper.Info("CourseController->CourseListRangeAny");
            bool isExisted = courseList.Any(p =>
                          (string.Compare(p.StartTime, model.EndTime, StringComparison.Ordinal) <= 0 && string.Compare(model.EndTime, p.EndTime, StringComparison.Ordinal) <= 0)
                           ||
                           (string.Compare(p.StartTime, model.StartTime, StringComparison.Ordinal) <= 0 && string.Compare(model.StartTime, p.EndTime, StringComparison.Ordinal) <= 0)
                           );
            return isExisted;
        }


        /// <summary>
        /// 添加课程   如果是公共课程表中的模板，频率传入进来的是每日重复
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Add(Course model)
        {
            LogHelper.Info("CourseController->Add");
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
            LogHelper.Info("CourseController->Update");
            string oldStartTime = model.StartTime;
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

                    //以前数据需要留存，今天（包含）之后的数据删除重新添加。
                    //db.Database.ExecuteSqlCommand("delete Course where BatchID= '" + oldBatchID + "'" );
                    db.Database.ExecuteSqlCommand("delete Course where BatchID= '" + oldBatchID + "' and StartTime>='" + oldStartTime + "'");

                    var entity = db.Course.AddRange(newCourseList);
                    db.SaveChanges();
                    msg.Status = true;
                    msg.Data = entity.LastOrDefault();
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
            LogHelper.Info("CourseController->Delete");
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
        /// <param name="childrenID"></param>
        /// <param name="isOverlap">是否覆盖，否给出提示，是删除后新增</param>
        /// <returns></returns>

        public ActionResult ImportCourse(int publicCourseInfoID, int childrenID, bool isOverlap = false)
        {
            LogHelper.Info("CourseController->ImportCourse");
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
                    //ChildrenID=0  代表的是公共课程表的数据
                    var list = db.Course.Where(p => p.PublicCourseInfoID == publicCourseInfoID && p.ChildrenID == 0).ToList();
                    string startTime = list.Min(p => p.StartTime);
                    string endTime = list.Max(p => p.EndTime);
                    //校验导入课程表的时间段内是否有课程冲突
                    var clist = db.Course.Where(p => p.ChildrenID == childrenID
                  && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                  && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                .ToList();

                    foreach (var group in list.GroupBy(p => p.BatchID))
                    {
                        //导入的课程，没有总的guid，根据每日课程来生成guid，否则修改的时候用guid会出问题,若需要总guid 需要扩展字段
                        var batchID = Guid.NewGuid();
                        foreach (var item in group)
                        {
                            //是否覆盖，如果否则return并给出提示。如果是则删除之后所有数据并重新添加
                            if (!isOverlap)
                            {
                                bool isExisted = CourseListRangeAny(clist, item);
                                if (isExisted)
                                {
                                    msg.Status = false;
                                    msg.Result = "800";
                                    return Json(msg, JsonRequestBehavior.AllowGet);
                                }
                            }
                            Course model = ObjectHelper.TransReflection<Course, Course>(item);
                            //model.PublicCourseInfoID = 0;
                            //model.PublicCourseTypeID = 0;
                            model.ChildrenID = childrenID;
                            model.BatchID = batchID;
                            newCourseList.Add(model);
                        }

                    }
                    //db.Database.ExecuteSqlCommand("delete Course where ChildrenID=" + childrenID + " and StartTime>= '"
                    //    + startTime + "' and EndTime<='" + endTime + "'");
                    db.Database.ExecuteSqlCommand("delete Course where ChildrenID=" + childrenID);
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
            LogHelper.Info("CourseController->GetPublicCourseIndex");
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
            LogHelper.Info("CourseController->GetPublicCourseByDate");
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
                        model = ObjectHelper.TransReflection<Course, CourseBusiness>(item);
                        //model.Address = item.Address;
                        //model.PublicCourseTypeID = item.PublicCourseTypeID;
                        //model.CourseName = item.CourseName;
                        //model.EndTime = item.EndTime;
                        //model.ID = item.ID;
                        if (string.IsNullOrWhiteSpace(item.RemindTime) || item.RemindTime == "-9999")
                            model.RemindDes = "未设置";
                        else
                            model.RemindDes = "已设置";
                        //model.RemindDes = DateTime.Parse(item.StartTime).AddMinutes(int.Parse(item.RemindTime)) < DateTime.Now ? "已提醒" : "未提醒";
                        //model.RemindTime = item.RemindTime;
                        //model.Phone = item.Phone;
                        //model.Remarks = item.Remarks;
                        //model.SchoolName = item.SchoolName;
                        //model.StartTime = item.StartTime;
                        //model.Teacher = item.Teacher;
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
        /// 公共课程表 日程的删除  用 guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteByGuid(string batchID)
        {
            LogHelper.Info("CourseController->DeleteByGuid");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete Course where BatchID='" + batchID + "'");
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
        /// 重置
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult Deletes(int publicCourseInfoID, string startTime, string endTime)
        {
            LogHelper.Info("CourseController->Deletes");
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

        #region 便捷添加课（日）程页面 ，展示课（日）程接口


        //public ActionResult AddRows(int childrenID, string startTime, string endTime)
        //{
        //    ResponseMessage msg = new ResponseMessage();
        //    CourseEasyBusiness ceb = FormatCEB(childrenID, startTime, endTime);
        //    msg.Status = true;
        //    msg.Data = ceb;
        //    return Json(msg, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 便捷添加课（日）程页面 ，展示课（日）程接口
        /// 在最后面做的格式化处理
        /// </summary>
        /// <param name="childrenID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult GetChildrenCourseByDateFormatOfEasy(int childrenID, string startTime, string endTime,
            int publicCourseInfoID, EnumUnit.CourseClassEnum courseClassType)
        {
            ResponseMessage msg = new ResponseMessage();
            CourseEasyBusiness ceb = FormatCEB(childrenID, startTime, endTime, publicCourseInfoID, courseClassType);
            msg.Status = true;
            msg.Data = ceb;
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 【本周】Copy
        /// 数据是基于==》【本周】页面使用接口，数据经过了展示处理  接口
        /// 【**重要**】去掉了+++++++++++每段时间最多给8节课+++++++++++ 的限制
        /// 【**重要**】增加了ID  StartTime   EndTime TimeType 属性
        /// 在最后面做的格式化处理
        /// </summary>
        /// <param name="childrenID">私有课程用，公共课程传0，历史遗留问题，以前的一些接口会有childrenID等于0的逻辑</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="publicCourseInfoID">私有课程不用，公共课程用</param>
        /// <param name="type">1是私有课程，2是公共课程</param>
        /// <returns></returns>
        public List<CourseBusiness> GetChildrenCourseByDateFormatOfWeekCopy(int childrenID, string startTime, string endTime,
            int publicCourseInfoID, EnumUnit.CourseClassEnum courseClassType)
        {
            #region  
            //【**重要**】去掉了+++++++++++每段时间最多给8节课+++++++++++ 的限制
            // 【**重要**】增加了ID  StartTime   EndTime TimeType属性

            //上午下午晚上的时间划分间隔
            int interval = 8;
            //上午结束时间
            int morningEndHour = 12;
            //下午结束时间
            int afternoonEndHour = 18;
            //晚上结束时间
            int NightEndHour = 4;


            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                msg.Status = true;
                var list = new List<Course>();
                if (courseClassType == EnumUnit.CourseClassEnum.PrivateCourse)
                {
                    list = db.Course.Where(p => p.ChildrenID == childrenID
                             && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                             && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0)
                          .OrderBy(p => p.StartTime).ToList();
                }
                else
                {
                    list = db.Course.Where(p => p.PublicCourseInfoID == publicCourseInfoID && p.ChildrenID == 0).ToList();
                }
                List<CourseBusiness> rList = new List<CourseBusiness>();
                var groupList = list.GroupBy(p => p.StartTime.Substring(0, 10)).ToList();
                //【上午】先把数据按照每天分开
                foreach (var group in groupList)
                {
                    int i = 0;
                    foreach (var item in group)
                    {
                        CourseBusiness model = new CourseBusiness();
                        model = ObjectHelper.TransReflection<Course, CourseBusiness>(item);
                        //【上午】：初始化为当天0点。
                        model.ShowDate = item.StartTime.Substring(0, 11) + "00:00";
                        model.ShowDate = DateTime.Parse(model.ShowDate).AddHours(i).ToString(FormatDateTime.LongDateTimeStr);
                        model.DayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Parse(model.StartTime).DayOfWeek);
                        //model.ID = item.ID;
                        //model.StartTime = item.StartTime;
                        //model.EndTime = item.EndTime;
                        rList.Add(model);
                        i++;
                    }
                }
                //【下午】把下午的数据showDate更正
                foreach (var group in groupList)
                {
                    int i = 0;
                    foreach (var item in group)
                    {
                        DateTime dt = DateTime.Parse(item.StartTime);
                        //时间大于morningEndHour的，从morningEndHour开始排序
                        //int hour = dt.Hour;
                        //if (hour == morningEndHour && dt.Minute > 0) hour = hour + 1;
                        if (dt.Hour >= morningEndHour)
                        {
                            CourseBusiness model = rList.FirstOrDefault(p => p.ID == item.ID);
                            //初始化为当天morningEndHour点。
                            model.ShowDate = item.StartTime.Substring(0, 11) + morningEndHour.ToString() + ":01";
                            model.ShowDate = DateTime.Parse(model.ShowDate).AddHours(i).ToString(FormatDateTime.LongDateTimeStr);
                            i++;
                        }
                    }
                }
                //【晚上】加完时间之后变为第二天凌晨的需要减去1天
                //【下午】把下午的数据showDate更正
                foreach (var group in groupList)
                {
                    int i = 0;
                    foreach (var item in group)
                    {
                        DateTime dt = DateTime.Parse(item.StartTime);
                        int hour = dt.Hour;
                        //if (hour == afternoonEndHour && dt.Minute > 0) hour = hour + 1;
                        if (hour >= afternoonEndHour)
                        {
                            CourseBusiness model = rList.FirstOrDefault(p => p.ID == item.ID);
                            //model.ShowDate = DateTime.Parse(model.ShowDate).AddHours(interval).ToString(FormatDateTime.LongDateTimeStr);
                            //初始化为当天afternoonEndHour点。
                            model.ShowDate = item.StartTime.Substring(0, 11) + afternoonEndHour.ToString() + ":01";
                            model.ShowDate = DateTime.Parse(model.ShowDate).AddHours(i).ToString(FormatDateTime.LongDateTimeStr);
                            //加完时间之后变为第二天凌晨的需要减去1天
                            //DateTime sdt = DateTime.Parse(model.ShowDate);
                            //if (sdt.Day != DateTime.Parse(model.StartTime).Day) model.ShowDate = sdt.AddDays(-1).ToString(FormatDateTime.LongDateTimeStr);
                            i++;

                        }
                    }
                }

                //重新构造数据  每段时间最多给8节课
                List<CourseBusiness> rList2 = new List<CourseBusiness>();
                var gList = rList.GroupBy(p => p.StartTime.Substring(0, 10)).ToList();
                foreach (var group in gList)
                {
                    //上午
                    var mList = group.Where(p => string.Compare(p.StartTime, p.StartTime.Substring(0, 11) + morningEndHour.ToString(),
                          StringComparison.Ordinal) < 0).ToList();
                    //mList = mList.Take(8).ToList();
                    int showNum = 1;
                    foreach (var item in mList)
                    {
                        item.ShowNum = showNum;
                        showNum++;
                    }
                    //下午
                    var aList = group.Where(p => string.Compare(p.StartTime, p.StartTime.Substring(0, 11) + morningEndHour.ToString(),
                          StringComparison.Ordinal) >= 0 && string.Compare(p.StartTime, p.StartTime.Substring(0, 11) + afternoonEndHour.ToString(),
                          StringComparison.Ordinal) < 0).ToList();
                    //aList = aList.Take(8).ToList();
                    showNum = 9;
                    foreach (var item in aList)
                    {
                        item.ShowNum = showNum;
                        showNum++;
                    }
                    //晚上
                    var nList = group.Where(p => string.Compare(p.StartTime, p.StartTime.Substring(0, 11) + afternoonEndHour.ToString(),
                          StringComparison.Ordinal) >= 0).ToList();
                    //nList = nList.Take(8).ToList();
                    showNum = 17;
                    foreach (var item in nList)
                    {
                        item.ShowNum = showNum;
                        showNum++;
                    }

                    //mList.ForEach(x => x.TimeType = (int)EnumUnit.TimeTypeEnum.Morning);
                    //aList.ForEach(x => x.TimeType = (int)EnumUnit.TimeTypeEnum.Afternoon);
                    //nList.ForEach(x => x.TimeType = (int)EnumUnit.TimeTypeEnum.Night);

                    rList2.AddRange(mList);
                    rList2.AddRange(aList);
                    rList2.AddRange(nList);

                }
                //暂时按照434去划分上午下午晚上
                rList2.ForEach(x =>
                {
                    if (x.CourseIndex <= 4) x.TimeType = (int)EnumUnit.TimeTypeEnum.Morning;
                    if (x.CourseIndex > 4 && x.CourseIndex <= 7) x.TimeType = (int)EnumUnit.TimeTypeEnum.Afternoon;
                    if (x.CourseIndex > 7) x.TimeType = (int)EnumUnit.TimeTypeEnum.Night;
                });
                return rList2.OrderBy(p => p.ShowDate).ThenBy(p => p.ShowNum).ToList();
            }

            #endregion
        }

        private CourseEasyBusiness GetCEB(string startTime, int mNum, int aNum, int nNum, List<CourseBusiness> cbList)
        {
            CourseEasyBusiness ceb = new CourseEasyBusiness();
            DateTime currentDate = DateTime.Parse(startTime);

            //上午
            for (int i = 1; i <= mNum; i++)
            {
                currentDate = DateTime.Parse(startTime);
                List<CourseBusiness> list = new List<CourseBusiness>();
                for (int j = 0; j < 7; j++)
                {
                    string currentDateStr = currentDate.ToString(FormatDateTime.ShortDateTimeStr);
                    var mList = cbList.Where(p => p.StartTime.Substring(0, 10) == currentDateStr
                    && p.TimeType == (int)EnumUnit.TimeTypeEnum.Morning).ToList();
                    if (mList.Any(p => p.CourseIndex == i))
                    {
                        list.Add(mList.FirstOrDefault(p => p.CourseIndex == i));
                    }
                    else//没有数据的用空数据补位
                    {
                        list.Add(new CourseBusiness() { CourseIndex = i, StartTime = currentDateStr, EndTime = currentDateStr });
                    }
                    currentDate = currentDate.AddDays(1);
                }
                ceb.morningList.Add(list);

            }
            //下午
            for (int i = mNum + 1; i <= mNum + aNum; i++)
            {
                currentDate = DateTime.Parse(startTime);
                List<CourseBusiness> list = new List<CourseBusiness>();
                for (int j = 0; j < 7; j++)
                {
                    string currentDateStr = DateTime.Parse(startTime).ToString(FormatDateTime.ShortDateTimeStr);
                    var aList = cbList.Where(p => p.StartTime.Substring(0, 10) == currentDateStr
                    && p.TimeType == (int)EnumUnit.TimeTypeEnum.Afternoon).ToList();
                    if (aList.Any(p => p.CourseIndex == i))
                    {
                        list.Add(aList.FirstOrDefault(p => p.CourseIndex == i));
                    }
                    else//没有数据的用空数据补位
                    {
                        list.Add(new CourseBusiness() { CourseIndex = i, StartTime = currentDateStr, EndTime = currentDateStr });
                    }
                    currentDate = currentDate.AddDays(1);
                }
                ceb.afternoonList.Add(list);

            }
            //晚上
            for (int i = mNum + aNum + 1; i <= mNum + aNum + nNum; i++)
            {
                currentDate = DateTime.Parse(startTime);
                List<CourseBusiness> list = new List<CourseBusiness>();
                for (int j = 0; j < 7; j++)
                {
                    string currentDateStr = DateTime.Parse(startTime).ToString(FormatDateTime.ShortDateTimeStr);
                    var nList = cbList.Where(p => p.StartTime.Substring(0, 10) == currentDateStr
                    && p.TimeType == (int)EnumUnit.TimeTypeEnum.Night).ToList();
                    if (nList.Any(p => p.CourseIndex == i))
                    {
                        list.Add(nList.FirstOrDefault(p => p.CourseIndex == i));
                    }
                    else//没有数据的用空数据补位
                    {
                        list.Add(new CourseBusiness() { CourseIndex = i, StartTime = currentDateStr, EndTime = currentDateStr });
                    }
                    currentDate = currentDate.AddDays(1);
                }
                ceb.nightList.Add(list);

            }
            return ceb;
        }

        private CourseEasyBusiness FormatCEB(int childrenID, string startTime, string endTime, int publicCourseInfoID, EnumUnit.CourseClassEnum courseClassType)
        {
            CourseEasyBusiness ceb = new CourseEasyBusiness();
            List<CourseBusiness> cbList = new List<CourseBusiness>();

            //按照“本周” 格式化好了数据
            cbList = GetChildrenCourseByDateFormatOfWeekCopy(childrenID, startTime, endTime, publicCourseInfoID, courseClassType);
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var dcs = new DefaultCourseSetting();

                //获取默认设置
                if (courseClassType == EnumUnit.CourseClassEnum.PrivateCourse)
                {
                    dcs = db.DefaultCourseSetting.FirstOrDefault(p => p.ChildrenID == childrenID);
                }
                else
                {
                    dcs = db.DefaultCourseSetting.FirstOrDefault(p => p.PublicCourseInfoID == publicCourseInfoID);
                }

                //获取时间设置
                var dctsList = new List<DefaultCourseTimeSetting>();
                if (courseClassType == EnumUnit.CourseClassEnum.PrivateCourse)
                {
                    dctsList = db.DefaultCourseTimeSetting.Where(p => p.ChildrenID == childrenID).ToList();
                }
                else
                {
                    dctsList = db.DefaultCourseTimeSetting.Where(p => p.PublicCourseInfoID == publicCourseInfoID).ToList();
                }

                //没有进行课程默认设置
                if (dcs == null || dcs?.IsOpen == false)
                {
                    int mListMaxCount = 0;
                    int aListMaxCount = 0;
                    int nListMaxCount = 0;
                    DateTime currentDate = DateTime.Parse(startTime);
                    //分别找出7天中，课次最多的
                    for (int j = 0; j < 7; j++)
                    {
                        var mList = cbList.Where(p => p.StartTime.Substring(0, 10) == currentDate.ToString(FormatDateTime.ShortDateTimeStr)
                        && p.TimeType == (int)EnumUnit.TimeTypeEnum.Morning).ToList();
                        if (mList.Count > mListMaxCount) mListMaxCount = mList.Count;

                        var aList = cbList.Where(p => p.StartTime.Substring(0, 10) == currentDate.ToString(FormatDateTime.ShortDateTimeStr)
                        && p.TimeType == (int)EnumUnit.TimeTypeEnum.Afternoon).ToList();
                        if (aList.Count > aListMaxCount) aListMaxCount = aList.Count;

                        var nList = cbList.Where(p => p.StartTime.Substring(0, 10) == currentDate.ToString(FormatDateTime.ShortDateTimeStr)
                        && p.TimeType == (int)EnumUnit.TimeTypeEnum.Night).ToList();
                        if (nList.Count > nListMaxCount) nListMaxCount = nList.Count;

                        currentDate = currentDate.AddDays(1);
                    }
                    int mNum = mListMaxCount >= 4 ? mListMaxCount : 4;
                    int aNum = aListMaxCount >= 3 ? aListMaxCount : 3;
                    int nNum = nListMaxCount >= 4 ? nListMaxCount : 4;
                    ceb = GetCEB(startTime, mNum, aNum, nNum, cbList);

                    //获取时间设置
                    //var dctsList = db.DefaultCourseTimeSetting.Where(p => p.ChildrenID == childrenID).ToList();
                    #region 给空数据 设置时间

                    ceb.morningList = FormatNullData(ceb.morningList, dctsList, dcs, childrenID, publicCourseInfoID, courseClassType);
                    ceb.afternoonList = FormatNullData(ceb.afternoonList, dctsList, dcs, childrenID, publicCourseInfoID, courseClassType);
                    ceb.nightList = FormatNullData(ceb.nightList, dctsList, dcs, childrenID, publicCourseInfoID, courseClassType);

                    #endregion
                }
                //进行了课程默认设置
                else if (dcs.IsOpen == true)
                {
                    DateTime currentDate = DateTime.Parse(startTime);
                    int mNum = (int)dcs.MorningNum;
                    int aNum = (int)dcs.AfternoonNum;
                    int nNum = (int)dcs.NightNum;

                    ceb = GetCEB(startTime, mNum, aNum, nNum, cbList);

                    #region 给空数据 设置时间

                    ceb.morningList = FormatNullData(ceb.morningList, dctsList, dcs, childrenID, publicCourseInfoID, courseClassType);
                    ceb.afternoonList = FormatNullData(ceb.afternoonList, dctsList, dcs, childrenID, publicCourseInfoID, courseClassType);
                    ceb.nightList = FormatNullData(ceb.nightList, dctsList, dcs, childrenID, publicCourseInfoID, courseClassType);

                    #endregion
                }
            }
            return ceb;
        }

        private List<List<CourseBusiness>> FormatNullData(List<List<CourseBusiness>> dataSource, List<DefaultCourseTimeSetting> dctsList, DefaultCourseSetting dcs, int childrenID, int publicCourseInfoID, EnumUnit.CourseClassEnum courseClassType)
        {
            foreach (var lists in dataSource)
            {
                foreach (var item in lists.Where(p => string.IsNullOrEmpty(p.CourseName)))
                {
                    var dcts = dctsList.FirstOrDefault(p => p.CourseIndex == item.CourseIndex);
                    item.StartTime = item.StartTime + " " + dcts?.StartTime;
                    item.EndTime = item.EndTime + " " + dcts?.EndTime;
                    item.CourseType = dcs == null ? ((int)EnumUnit.CourseTypeEnum.Other).ToString() : dcs.CourseType;
                    if (courseClassType == EnumUnit.CourseClassEnum.PrivateCourse)
                    {
                        item.Frequency = dcs == null ? ((int)EnumUnit.FrequencyEnum.TodayOnly).ToString() : dcs.CourseType;
                        item.ChildrenID = childrenID;
                    }
                    else
                    {
                        item.Frequency = ((int)EnumUnit.FrequencyEnum.EveryWeek).ToString();
                        item.PublicCourseInfoID = publicCourseInfoID;
                    }

                }
            }
            return dataSource;
        }


        #endregion

        #region 便捷添加课（日）程页面 ，新增课（日）程接口

        public ActionResult SaveList()
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                try
                {
                    int courseClassType = int.Parse(Request.Form["courseClassType"]);
                    var publicCourseInfoID = Request.Form["publicCourseInfoID"];
                    var childrenID = Request.Form["childrenID"];
                    var modelList = Request.Form["modelList"];
                    var tempList = JsonConvert.DeserializeObject<List<CourseBusiness>>(modelList);
                    var list = tempList.Where(p => !string.IsNullOrWhiteSpace(p.CourseName)).ToList();
                    //新加入的课程数据集合
                    List<Course> newCourseList = new List<Course>();
                    foreach (var model in list)
                    {
                        string oldStartTime = model.StartTime;
                        string oldBatchID = model.BatchID.ToString();
                        model.BatchID = Guid.NewGuid();
                        for (int i = 0; i < ApplicationConstant.forDay;)
                        {
                            if (model.Frequency == ((int)EnumUnit.FrequencyEnum.TodayOnly).ToString())
                            {
                                //bool isExisted = CourseListRangeAny(courseList, model);
                                //if (isExisted)
                                //{
                                //    msg.Status = false;
                                //    //【仅今天】代表数据已经存在
                                //    msg.Result = "800";
                                //    return Json(msg, JsonRequestBehavior.AllowGet);
                                //}
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
                    }

                    //以前数据需要留存，今天（包含）之后的数据删除重新添加。
                    //修改的逻辑的删除
                    //db.Database.ExecuteSqlCommand("delete Course where BatchID= '" + oldBatchID + "' and StartTime>='" + oldStartTime + "'");
                    //本方法的逻辑的删除
                    var weekIndex = (int)DateTime.Now.DayOfWeek;
                    if (weekIndex == 0) weekIndex = 7;
                    var delStartTime = DateTime.Now.AddDays(-weekIndex).ToString(FormatDateTime.ShortDateTimeStr);
                    if (courseClassType == (int)EnumUnit.CourseClassEnum.PrivateCourse)
                    {
                        db.Database.ExecuteSqlCommand("delete Course where childrenID=" + childrenID + " and StartTime>='" + delStartTime + "'");
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("delete Course where PublicCourseInfoID=" + publicCourseInfoID + " and StartTime>='" + delStartTime + "'");
                    }
                    var entity = db.Course.AddRange(newCourseList);
                    db.SaveChanges();
                    msg.Status = true;
                    //msg.Data = entity.LastOrDefault();
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









        #region 分享成图片
        public ActionResult ShareImg()
        {

            return View();
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