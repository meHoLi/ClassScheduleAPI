using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassScheduleAPI.ModelsBusiness
{
    public class CourseBusiness : Course
    {
        public string DayOfWeek { get; set; }
        public string BackgroundColor { get; set; }

        public bool IsToday { get; set; }
        public string RemindDes { get; set; }

        public string ShowDate { get; set; }

        public int ShowNum { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 课程开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 课程结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 时间段
        /// </summary>
        public int TimeType { get; set; }
    }
}