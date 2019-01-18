using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class PublicBoxController : Controller
    {
        public ActionResult Index(string openID, int publicBoxType)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var list = db.PublicBox.Where(p => p.OpenID == openID && p.PublicBoxType == publicBoxType).ToList();
                msg.Data = list;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPublicBoxByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.PublicBox.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPublicBoxByLoginNameAndPassword(string loginName, string password)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var publicCourseInfoList = new List<PublicCourseInfo>();
                var model = db.PublicBox.FirstOrDefault(p => p.LoginName == loginName && p.Password == password);
                if (model != null)
                {
                    msg.Status = true;
                    publicCourseInfoList= db.PublicCourseInfo.Where(p => p.PublicBoxID == model.ID).ToList();
                }
                else msg.Status = false;
                msg.Data = new { model, publicCourseInfoList };
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(PublicBox model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    bool isExisted = db.PublicBox.Any(p => p.LoginName == model.LoginName);
                    if (isExisted)
                    {
                        msg.Status = false;
                        msg.Result = "800";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    var entity = db.PublicBox.Add(model);
                    db.SaveChanges();
                    //"家庭日记"默认增加
                    PublicCourseInfoController pci = new PublicCourseInfoController();
                    PublicCourseInfo pciModel = new PublicCourseInfo();
                    pciModel.Name = "家庭日记";
                    pciModel.OpenID = model.OpenID;
                    pciModel.PublicBoxID = entity.ID;
                    pciModel.DefaultType = (int)EnumUnit.PublicCourseInfoDefaultEnum.Diary;
                    pci.Add(pciModel);
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

        public ActionResult Update(PublicBox model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    bool isExisted = db.PublicBox.Any(p => p.ID != model.ID && p.LoginName == model.LoginName);
                    if (isExisted)
                    {
                        msg.Status = false;
                        msg.Result = "700";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                    db.PublicBox.Attach(model);
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
                    db.Database.ExecuteSqlCommand("delete PublicBox where id= " + id);
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