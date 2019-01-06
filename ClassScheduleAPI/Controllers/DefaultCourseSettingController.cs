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
    //DefaultCourseSetting
    public class DefaultCourseSettingController : Controller
    {
        public ActionResult GetDefaultCourseSettingByChildrenID(int childrenID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var model = db.DefaultCourseSetting.Where(p => p.ChildrenID == childrenID).FirstOrDefault();
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDefaultCourseSettingByPublicCourseInfoID(int publicCourseInfoID)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                msg.Status = true;
                var model = db.DefaultCourseSetting.Where(p => p.PublicCourseInfoID == publicCourseInfoID).FirstOrDefault();
                msg.Data = model;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetModel()
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    //添加默认时间
                    var modelStr = Request.Form["model"];
                    int courseClassType = int.Parse(Request.Form["courseClassType"]);
                    var model = JsonConvert.DeserializeObject<DefaultCourseSetting>(modelStr);
                    //代表添加
                    if (model.ID == 0)
                    {
                        var entity = db.DefaultCourseSetting.Add(model);
                    }
                    else//代表编辑
                    {
                        db.DefaultCourseSetting.Attach(model);
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    }
                    //删除默认时间设置
                    if (courseClassType == (int)EnumUnit.CourseClassEnum.PrivateCourse)
                    {
                        db.Database.ExecuteSqlCommand("delete DefaultCourseTimeSetting where ChildrenID= " + model.ChildrenID);
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("delete DefaultCourseTimeSetting where PublicCourseInfoID= " + model.PublicCourseInfoID);
                    }
                    //添加默认时间
                    var timeModelList = Request.Form["timeModelList"];
                    var list = JsonConvert.DeserializeObject<List<DefaultCourseTimeSetting>>(timeModelList);
                    db.DefaultCourseTimeSetting.AddRange(list);
                    db.SaveChanges();
                    //上面的时间加上之后再修改下面的时间
                    //更改课程时间  不管是默认设置，还是“新增课程（日程）”里面的手动设置，时间设置上都应该是以最后修改的那个时间为准。 
                    string sql = UpdateCourseTime(model.ChildrenID, model.PublicCourseInfoID, courseClassType);
                    db.Database.ExecuteSqlCommand(sql);
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public string UpdateCourseTime(int? childrenID, int? publicCourseInfoID, int courseClassType)
        {
            string whereSql = " ChildrenID = @ChildrenID";
            string setDeclare = "set @ChildrenID = " + childrenID;
            if (courseClassType == (int)EnumUnit.CourseClassEnum.PrivateCourse)
            {
                whereSql = " ChildrenID = @ChildrenID";
                setDeclare = " @ChildrenID = " + childrenID;
            }
            else
            {
                whereSql = " PublicCourseInfoID = @publicCourseInfoID";
                setDeclare = " @PublicCourseInfoID = " + publicCourseInfoID;
            }
            string sql = @"
            --游标
            -- 声明变量
            DECLARE
                @ID AS INT,
                @StartTime AS NVARCHAR(50),
                @EndTime AS NVARCHAR(50),
                @CourseIndex AS INT,
                @ChildrenID AS INT,
                @PublicCourseInfoID AS INT,
                @DCTSStartTime AS NVARCHAR(50),--DefaultCourseTimeSetting中的时间
                @DCTSEndTime AS NVARCHAR(50); --DefaultCourseTimeSetting中的时间
                set " + setDeclare + @"
            -- 声明游标
            DECLARE C_Course CURSOR FAST_FORWARD FOR
                SELECT ID, CourseIndex, StartTime, EndTime
                FROM Course
                where " + whereSql + @" 
                ORDER BY ID;

                        OPEN C_Course;

                        --取第一条记录
            FETCH NEXT FROM C_Course INTO @ID, @CourseIndex, @StartTime, @EndTime;


                        WHILE @@FETCH_STATUS = 0
            BEGIN

                -- 操作
                --读取DefaultCourseTimeSetting
                select @DCTSStartTime = StartTime,@DCTSEndTime = EndTime from DefaultCourseTimeSetting where " + whereSql + @"  and CourseIndex = @CourseIndex
                      --更新Course
                      UPDATE Course SET StartTime = LEFT(@StartTime, 11) + @DCTSStartTime,EndTime = LEFT(@EndTime, 11) + @DCTSEndTime WHERE ID = @ID;

                        --取下一条记录
                FETCH NEXT FROM C_Course INTO @ID, @CourseIndex, @StartTime, @EndTime;
                        END

                        -- 关闭游标
                        CLOSE C_Course;

                        --释放游标
            DEALLOCATE C_Course; ";
            return sql;
        }

        public ActionResult Delete(int id)
        {
            using (ClassScheduleDBEntities db = new ClassScheduleDBEntities())
            {
                ResponseMessage msg = new ResponseMessage();
                try
                {
                    db.Database.ExecuteSqlCommand("delete DefaultCourseSetting where id= " + id);
                    msg.Status = true;
                }
                catch (Exception e)
                {
                    msg.Status = false;
                }
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
    }
}