using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using ClassScheduleAPI.ModelsBusiness;
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

        #region 打卡任务

        /// <summary>
        /// 当前打卡任务
        /// </summary>
        /// <param name="childrenID"></param>
        /// <returns></returns>
        public ActionResult Index(int childrenID)
        {
            var msg = GetClockAndTomatoList(childrenID, false);
            return Json(msg, JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// 打卡
        /// </summary>
        /// <returns></returns>
        public ActionResult ExecuteClock(int id, EnumUnit.ClockExecuteTypeEnum executeType)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                try
                {
                    var model = db.Clock.FirstOrDefault(p => p.ID == id);
                    //打卡
                    if (executeType == EnumUnit.ClockExecuteTypeEnum.Punch)
                    {
                        //已经完成打卡
                        if (model.ExecutedNum >= model.ExecuteNum)
                        {
                            msg.Status = false;
                            msg.Result = "800";
                            return Json(msg, JsonRequestBehavior.AllowGet);
                        }
                        model.ExecutedNum++;
                    }//取消打卡
                    else if (executeType == EnumUnit.ClockExecuteTypeEnum.Cancel)
                    {
                        //已取消完所有打卡
                        if (model.ExecutedNum == 0)
                        {
                            msg.Status = false;
                            msg.Result = "801";
                            return Json(msg, JsonRequestBehavior.AllowGet);
                        }
                        //减少积分
                        if (model.ExecutedNum == model.ExecuteNum && model.RewardPoints > 0)
                        {
                            AddIntegralRecord(model, EnumUnit.IntegralRecordCalcTypeEnum.Reduce);
                        }
                        model.ExecutedNum--;
                        model.IsComplated = false;

                    }
                    //校验是否完成本周期打卡任务
                    if (model.ExecutedNum == model.ExecuteNum)
                    {
                        model.IsComplated = true;
                        //增加积分
                        if (model.RewardPoints > 0)
                        {
                            AddIntegralRecord(model, EnumUnit.IntegralRecordCalcTypeEnum.Plus);
                        }
                    }
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    msg.Status = true;
                    msg.Data = model;
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
        /// 添加积分变动
        /// </summary>
        /// <param name="model"></param>
        /// <param name="calcType"></param>
        private void AddIntegralRecord(Clock model, EnumUnit.IntegralRecordCalcTypeEnum calcType)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                try
                {
                    var totalNumber = db.IntegralRecord.Where(p => p.ChildrenID == model.ChildrenID).OrderByDescending(p => p.ID).FirstOrDefault()?.TotalNumber ?? 0;
                    model.IsComplated = true;
                    IntegralRecord irModel = new IntegralRecord();
                    irModel.ClockID = model.ID;
                    irModel.CalcType = (int)calcType;
                    irModel.ChildrenID = model.ChildrenID;
                    irModel.CreateTime = DateTime.Now.ToString(FormatDateTime.ShortDateTimeStr);
                    irModel.Number = model.RewardPoints;
                    irModel.Name = model.Name;
                    if (calcType == EnumUnit.IntegralRecordCalcTypeEnum.Plus)
                    {
                        irModel.TotalNumber = totalNumber + irModel.Number;
                    }
                    else
                    {
                        irModel.TotalNumber = totalNumber - irModel.Number;
                        //减到0就不再减了
                        if (irModel.TotalNumber < 0) irModel.TotalNumber = 0;
                    }
                    db.IntegralRecord.Add(irModel);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                }
            }
        }

        public ActionResult Add(Clock model)
        {
            var msg = ExecAddData(model);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Clock model)
        {
            ResponseMessage msg = new ResponseMessage();
            string oldStartTime = DateTime.Now.ToString(FormatDateTime.ShortDateTimeStr);
            string oldBatchID = model.BatchID.ToString();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                try
                {
                    msg = ExecAddData(model);
                    //以前数据需要留存，今天（包含）之后的数据删除重新添加。
                    db.Database.ExecuteSqlCommand("delete Clock where BatchID= '" + oldBatchID + "' and ClockDate>='" + oldStartTime + "'");
                    //删除相关打卡积分
                    new IntegralRecordController().DeleteByClockID(model.ID);
                }
                catch (Exception e)
                {
                    msg.Status = false;
                    msg.Result = "500";
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ResponseMessage ExecAddData(Clock model)
        {
            ResponseMessage msg = new ResponseMessage();
            //新加入的打卡数据集合
            List<Clock> newClockList = new List<Clock>();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                try
                {
                    DateTime keepst = DateTime.Parse(model.KeepStartTime);
                    DateTime keepet = DateTime.Parse(model.KeepEndTime);
                    //默认StartTime=KeepStartTime  EndTime=KeepEndTime
                    model.ClockDate = keepst.ToString(FormatDateTime.ShortDateTimeStr);
                    model.BatchID = Guid.NewGuid();
                    for (DateTime i = keepst; i <= keepet;)
                    {
                        if (model.Frequency == ((int)EnumUnit.ClockFrequencyEnum.Fixed).ToString())
                        {
                            int dayOfWeek = (int)i.DayOfWeek;
                            if (dayOfWeek == 0) dayOfWeek = 7;
                            //DayOfWeek 格式，比如选中了周一、周三、周日则：1,3,7
                            if (model.DayOfWeek.Contains(dayOfWeek.ToString()))
                            {
                                var t = ObjectHelper.TransReflection<Clock, Clock>(model);
                                newClockList.Add(t);
                            }
                            int interval = 1;
                            i = i.AddDays(interval);
                            //日
                            model.ClockDate = (DateTime.Parse(model.ClockDate).AddDays(interval)).ToString(FormatDateTime.ShortDateTimeStr);
                        }
                        else if (model.Frequency == ((int)EnumUnit.ClockFrequencyEnum.EveryWeek).ToString())
                        {
                            var t = ObjectHelper.TransReflection<Clock, Clock>(model);
                            newClockList.Add(t);
                            int interval = 7;
                            //日
                            i = i.AddDays(interval);
                            model.ClockDate = (DateTime.Parse(model.ClockDate).AddDays(interval)).ToString(FormatDateTime.ShortDateTimeStr);
                        }
                        else if (model.Frequency == ((int)EnumUnit.ClockFrequencyEnum.EveryMouth).ToString())
                        {
                            var t = ObjectHelper.TransReflection<Clock, Clock>(model);
                            newClockList.Add(t);
                            int interval = 1;
                            //月
                            i = i.AddMonths(interval);
                            model.ClockDate = (DateTime.Parse(model.ClockDate).AddMonths(interval)).ToString(FormatDateTime.ShortDateTimeStr);
                        }
                    }
                    var entity = db.Clock.AddRange(newClockList);
                    db.SaveChanges();
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                    msg.Result = "500";
                }
            }
            return msg;
        }

        /// <summary>
        /// 结束习惯
        /// </summary>
        /// <param name="batchID"></param>
        /// <returns></returns>
        public ActionResult Delete(string batchID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    string startTime = DateTime.Now.ToString(FormatDateTime.ShortDateTimeStr);
                    db.Database.ExecuteSqlCommand("delete Clock where BatchID= '" + batchID + "' and ClockDate>='" + startTime + "'");
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

        #region 番茄习惯

        /// <summary>
        /// 获取番茄习惯
        /// </summary>
        /// <param name="childrenID"></param>
        /// <returns></returns>
        public ActionResult GetTomatoHabit(int childrenID)
        {
            var msg = GetClockAndTomatoList(childrenID, true);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 单独添加番茄钟
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddTomato(Clock model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    model.Frequency = ((int)EnumUnit.ClockFrequencyEnum.TodayAdd).ToString();
                    model.ClockDate = DateTime.Now.ToString(FormatDateTime.ShortDateTimeStr);
                    model.KeepStartTime = model.ClockDate;
                    model.KeepEndTime = model.ClockDate;
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
        /// <summary>
        /// 获取打卡列表和番茄钟列表的公共方法
        /// </summary>
        /// <param name="childrenID"></param>
        /// <param name="isTomato"></param>
        /// <returns></returns>
        private ResponseMessage GetClockAndTomatoList(int childrenID, bool isTomato)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                DateTime now = DateTime.Now;
                var today = now.ToString(FormatDateTime.ShortDateTimeStr);
                //今日是周几
                int weekIndex = (int)now.DayOfWeek;
                if (weekIndex == 0) weekIndex = 7;
                //本周开始时间
                string weekStartStr = now.AddDays(-weekIndex).ToString(FormatDateTime.ShortDateTimeStr);
                //本周结束时间
                string weekEndStr = now.AddDays(-weekIndex).AddDays(7).ToString(FormatDateTime.ShortDateTimeStr) + ApplicationConstant.EndTimeSuffix;
                //本月开始时间
                string mouthStartStr = new DateTime(now.Year, now.Month, 1).ToString(FormatDateTime.ShortDateTimeStr);
                //本月结束时间
                string mouthEndStr = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1)
                    .ToString(FormatDateTime.ShortDateTimeStr) + ApplicationConstant.EndTimeSuffix;

                var query = db.Clock.Where(p => p.ChildrenID == childrenID);

                if (isTomato)
                {
                    query = query.Where(p => p.IsLimit == true);
                }
                var list = query.OrderBy(p => p.ID).ToList();
                msg.Data = new
                {
                    //今日打卡任务
                    todayClockList = list.Where(p => p.Frequency == ((int)EnumUnit.ClockFrequencyEnum.Fixed).ToString()
                    && p.ClockDate == today).ToList(),
                    //本周打卡任务
                    weekList = list.Where(p => p.Frequency == ((int)EnumUnit.ClockFrequencyEnum.EveryWeek).ToString()
                    && string.Compare(p.ClockDate, weekStartStr, StringComparison.Ordinal) >= 0
                    && string.Compare(p.ClockDate, weekEndStr, StringComparison.Ordinal) <= 0).ToList(),
                    //本月打卡任务
                    mouthClockList = list.Where(p => p.Frequency == ((int)EnumUnit.ClockFrequencyEnum.EveryMouth).ToString()
                    && string.Compare(p.ClockDate, mouthStartStr, StringComparison.Ordinal) >= 0
                    && string.Compare(p.ClockDate, mouthEndStr, StringComparison.Ordinal) <= 0).ToList(),
                    //历史打卡任务
                    historyClockList = list.Where(p => string.Compare(p.ClockDate, today, StringComparison.Ordinal) <= 0).ToList(),
                    //今日自定义打卡任务
                    todayAddList = list.Where(p => p.Frequency == ((int)EnumUnit.ClockFrequencyEnum.TodayAdd).ToString()
                    && p.ClockDate == today).ToList(),
                };
                return msg;
            }
        }

        #endregion
    }

}