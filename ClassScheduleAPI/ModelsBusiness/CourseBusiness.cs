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
    }
}