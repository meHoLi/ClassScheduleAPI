using ClassScheduleAPI.Common;
using ClassScheduleAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class ChildrenController : Controller
    {
        // GET: Children

        public ActionResult Index(string openID)
        {
            LogHelper.Info("ChildrenController->Index");


            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                //try
                //{
                var list = db.Children.Where(p => p.OpenID == openID).ToList();
                msg.Data = list;
                //}
                //catch (Exception e)
                //{
                //    msg.Status = false;
                //}
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetChildrenByID(int id)
        {
            LogHelper.Info("ChildrenController->GetChildrenByID");

            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.Children.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddList()
        {
            LogHelper.Info("ChildrenController->AddList");

            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();

                using (var scope = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelList = Request.Form["modelList"];
                        var list = JsonConvert.DeserializeObject<List<Children>>(modelList);
                        foreach (var item in list)
                        {
                            Children model = new Children();
                            var entity = db.Children.Add(item);
                            db.SaveChanges();
                            //初始化课程
                            var defaultCourseList = db.DefaultCourse.Where(p=>p.AppClass== ApplicationConstant.Course).OrderBy(p => p.Sort).ToList();
                            db.ChildrenStandardCourse.AddRange(defaultCourseList.Select(x => new ChildrenStandardCourse()
                            {
                                ChildrenID = entity.ID,
                                CourseName = x.CourseName,
                                Sort = x.Sort
                            }));
                            db.SaveChanges();
                            //初始化打卡
                            var clockProjectList = db.DefaultCourse.Where(p => p.AppClass == ApplicationConstant.Clock).OrderBy(p => p.Sort).ToList();
                            db.ClockProject.AddRange(clockProjectList.Select(x => new ClockProject()
                            {
                                ChildrenID = entity.ID,
                                Name = x.CourseName,
                                Sort = x.Sort,
                                Type = x.Type
                            }));
                            db.SaveChanges();
                            //初始化积分兑换项目
                            var exchangeProjectList = db.DefaultCourse.Where(p => p.AppClass == ApplicationConstant.Exchange).OrderBy(p => p.Sort).ToList();
                            db.ExchangeProject.AddRange(exchangeProjectList.Select(x => new ExchangeProject()
                            {
                                ChildrenID = entity.ID,
                                Name = x.CourseName,
                                Sort = x.Sort,
                                Value=x.Value
                            }));
                            db.SaveChanges();
                        }
                        msg.Status = true;
                        scope.Commit();
                    }
                    catch (Exception e)
                    {
                        msg.Status = false;
                        scope.Rollback();
                    }
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(Children model)
        {
            LogHelper.Info("ChildrenController->Add");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();

                using (var scope = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = db.Children.Add(model);
                        db.SaveChanges();
                        //初始化课程
                        var defaultCourseList = db.DefaultCourse.Where(p => p.AppClass == ApplicationConstant.Course).OrderBy(p => p.Sort).ToList();
                        db.ChildrenStandardCourse.AddRange(defaultCourseList.Select(x => new ChildrenStandardCourse()
                        {
                            ChildrenID = entity.ID,
                            CourseName = x.CourseName,
                            Sort = x.Sort
                        }));
                        db.SaveChanges();
                        //初始化打卡
                        var clockProjectList = db.DefaultCourse.Where(p => p.AppClass == ApplicationConstant.Clock).OrderBy(p => p.Sort).ToList();
                        db.ClockProject.AddRange(clockProjectList.Select(x => new ClockProject()
                        {
                            ChildrenID = entity.ID,
                            Name = x.CourseName,
                            Sort = x.Sort,
                            Type = x.Type
                        }));
                        db.SaveChanges();
                        //初始化积分兑换项目
                        var exchangeProjectList = db.DefaultCourse.Where(p => p.AppClass == ApplicationConstant.Exchange).OrderBy(p => p.Sort).ToList();
                        db.ExchangeProject.AddRange(exchangeProjectList.Select(x => new ExchangeProject()
                        {
                            ChildrenID = entity.ID,
                            Name = x.CourseName,
                            Sort = x.Sort,
                            Value=x.Value
                        }));
                        db.SaveChanges();
                        scope.Commit();
                        msg.Status = true;
                    }
                    catch (Exception e)
                    {
                        msg.Status = false;
                        msg.Result = "500";
                        scope.Rollback();
                    }
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult Update(Children model)
        {
            LogHelper.Info("ChildrenController->Update");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Children.Attach(model);
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
            LogHelper.Info("ChildrenController->Delete");
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete Children where id= " + id);
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