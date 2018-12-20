using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
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
                var list = db.IntegralRecord.Where(p => p.ChildrenID == childrenID).OrderByDescending(p => p.ID).ToList();
                msg.Data = list;
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
        public ActionResult Add(IntegralRecord model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    int totalNumber = db.IntegralRecord.Where(p => p.ChildrenID == model.ChildrenID).OrderByDescending(p => p.ID).LastOrDefault()?.TotalNumber ?? 0;
                    model.CreateTime = DateTime.Now.ToString(FormatDateTime.LongDateTimeStr);
                    if (model.CalcType == (int)EnumUnit.IntegralRecordCalcTypeEnum.Plus)
                    {
                        model.TotalNumber += totalNumber;
                    }
                    else if(model.CalcType == (int)EnumUnit.IntegralRecordCalcTypeEnum.Reduce)
                    {
                        if (model.TotalNumber >= totalNumber)
                        {
                            msg.Status = false;
                            msg.Result = "800";
                            msg.Msg = "兑换积分超过剩余积分";
                            return Json(msg, JsonRequestBehavior.AllowGet);
                        }
                        model.TotalNumber -= totalNumber;
                    }
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
    }
}