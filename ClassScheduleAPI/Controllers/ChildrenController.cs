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
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                try
                {
                    var list = db.Children.Where(p => p.OpenID == openID).ToList();
                    msg.Data = list;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetChildrenByID(int id)
        {
            ResponseMessage msg = new ResponseMessage();
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                var model = db.Children.FirstOrDefault(p => p.ID == id);
                msg.Status = true;
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add()
        {

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
        public ActionResult Update(Children model)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    //https://blog.csdn.net/u011872945/article/details/72966946
                    db.Children.Attach(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg);
            }
        }
        public ActionResult Delete(int id)
        {
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
                }
                return Json(msg);
            }
        }
    }
}