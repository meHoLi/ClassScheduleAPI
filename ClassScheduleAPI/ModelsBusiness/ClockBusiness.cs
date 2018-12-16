using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassScheduleAPI.ModelsBusiness
{
    public class ClockBusiness : Clock
    {
        public string DayOfWeek { get; set; }
    }
}