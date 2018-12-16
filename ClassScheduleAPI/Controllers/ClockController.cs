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
        //剩余列表页的数据格式化
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
        public ActionResult Delete(int batchID)
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
    }
}