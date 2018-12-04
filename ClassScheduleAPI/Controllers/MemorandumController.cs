using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class MemorandumController : Controller
    {
        //Memorandum
        public ActionResult Index(string openID, string startTime, string endTime)
        {
            LogHelper.Info("MemorandumController->Index");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var list = db.Memorandum.Where(p => p.OpenID == openID
                           && string.Compare(p.StartTime, startTime, StringComparison.Ordinal) >= 0
                           && string.Compare(p.EndTime, endTime, StringComparison.Ordinal) <= 0
                           && p.Type == (int)EnumUnit.MemorandumTypeEnum.SingleMemorandum).ToList();
                //, string orderBy
                //if (orderBy == "desc")
                //{
                //    msg.Data = list.OrderByDescending(p => p.StartTime).ToList();
                //}
                //else
                //{
                msg.Data = list.OrderBy(p => p.StartTime).ToList();
                //}
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据外出清单ID获取备忘列表
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult GetListByGroupID(int groupID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var list = db.Memorandum.Where(p => p.GroupID == groupID
                           && p.Type == (int)EnumUnit.MemorandumTypeEnum.GroupMemorandum).OrderBy(p => p.StartTime).ToList();
                msg.Data = list;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMemorandumByID(int id)
        {
            LogHelper.Info("MemorandumController->GetMemorandumByID");
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.Memorandum.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(Memorandum model)
        {
            LogHelper.Info("MemorandumController->Add");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    var entity = db.Memorandum.Add(model);
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

        public ActionResult Update(Memorandum model)
        {
            LogHelper.Info("MemorandumController->Update");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Memorandum.Attach(model);
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
            LogHelper.Info("MemorandumController->Delete");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete Memorandum where id= " + id);
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