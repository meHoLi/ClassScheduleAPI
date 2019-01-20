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
    public class IntegralRecordController : Controller
    {
        //IntegralRecord
        public ActionResult Index(int childrenID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var tempList = db.IntegralRecord.Where(p => p.ChildrenID == childrenID).OrderByDescending(p => p.ID).ToList();
                var totalNumber = tempList.FirstOrDefault()?.TotalNumber ?? 0;
                List<IntegralRecordBusiness> list = new List<IntegralRecordBusiness>();
                foreach (var item in tempList)
                {
                    var bitem = ObjectHelper.TransReflection<IntegralRecord, IntegralRecordBusiness>(item);
                    string calcStr = "获得";
                    string prefix = "完成";
                    if (item.CalcType == (int)EnumUnit.IntegralRecordCalcTypeEnum.Reduce)
                    {
                        calcStr = "减少";
                        prefix = "取消";
                    }
                    if (item.CalcType == (int)EnumUnit.IntegralRecordCalcTypeEnum.Consum)
                    {
                        calcStr = "减少";
                        prefix = "兑换";
                    }
                    bitem.ShowName = bitem.CreateTime + prefix + bitem.Name + calcStr + bitem.Number + "个积分";
                    list.Add(bitem);
                }
                msg.Data = new { list, totalNumber };
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetIntegralRecordByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.IntegralRecord.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 消费积分
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult ConsumIntegralReclrd(IntegralRecord model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    var totalNumber = db.IntegralRecord.Where(p => p.ChildrenID == model.ChildrenID).OrderByDescending(p => p.ID).FirstOrDefault()?.TotalNumber ?? 0;
                    model.CreateTime = DateTime.Now.ToString(FormatDateTime.ShortDateTimeStr);
                    if (model.Number > totalNumber)
                    {
                        msg.Status = false;
                        msg.Result = "800";
                        msg.Msg = "兑换积分超过剩余积分";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    model.TotalNumber = totalNumber - model.Number;
                    var entity = db.IntegralRecord.Add(model);
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

        public ActionResult Update(IntegralRecord model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.IntegralRecord.Attach(model);
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
                    db.Database.ExecuteSqlCommand("delete IntegralRecord where id= " + id);
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
        public ActionResult DeleteByClockID(int clockID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete IntegralRecord where ClockID= " + clockID);
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