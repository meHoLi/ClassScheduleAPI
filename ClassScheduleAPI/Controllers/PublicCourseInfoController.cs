﻿using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class PublicCourseInfoController : Controller
    {
        public ActionResult Index(string openID, int publicBoxID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                //var list = db.PublicCourseInfo.Where(p => p.OpenID == openID && p.PublicBoxID == publicBoxID).ToList();
                var list = db.PublicCourseInfo.Where(p => p.PublicBoxID == publicBoxID).ToList();
                var diary = list.Where(p => p.DefaultType == (int)EnumUnit.PublicCourseInfoDefaultEnum.Diary).ToList();
                var familyCurriculum = list.Where(p => p.DefaultType == (int)EnumUnit.PublicCourseInfoDefaultEnum.FamilyCurriculum).ToList();
                msg.Data = new { diary, familyCurriculum };
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPublicCourseInfoByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.PublicCourseInfo.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPublicCourseInfoByLoginNameAndPassword(string loginName, string password)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.PublicCourseInfo.FirstOrDefault(p => p.LoginName == loginName && p.Password == password);
                if (model != null) msg.Status = true;
                else msg.Status = false;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(PublicCourseInfo model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    //bool isExisted = db.PublicCourseInfo.Any(p => p.LoginName == model.LoginName);
                    //if (isExisted)
                    //{
                    //    msg.Status = false;
                    //    msg.Result = "800";
                    //    return Json(msg, JsonRequestBehavior.AllowGet);
                    //}
                    var entity = db.PublicCourseInfo.Add(model);
                    db.SaveChanges();
                    //初始化课程
                    var defaultCourseList = db.DefaultCourse.Where(p => p.AppClass == ApplicationConstant.Course).OrderBy(p => p.Sort).ToList();
                    db.ChildrenStandardCourse.AddRange(defaultCourseList.Select(x => new ChildrenStandardCourse()
                    {
                        PublicCourseInfoID = entity.ID,
                        CourseName = x.CourseName,
                        Sort = x.Sort
                    }));
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

        public ActionResult Update(PublicCourseInfo model)
        {
            LogHelper.Info("PublicCourseInfoController->Update");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    //bool isExisted = db.PublicCourseInfo.Any(p => p.ID != model.ID && p.LoginName == model.LoginName);
                    //if (isExisted)
                    //{
                    //    msg.Status = false;
                    //    msg.Result = "700";
                    //    return Json(msg, JsonRequestBehavior.AllowGet);
                    //}
                    db.PublicCourseInfo.Attach(model);
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
                    db.Database.ExecuteSqlCommand("delete PublicCourseInfo where id= " + id);
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