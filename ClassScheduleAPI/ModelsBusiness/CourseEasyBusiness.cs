using ClassScheduleAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassScheduleAPI.ModelsBusiness
{
    public class CourseEasyBusiness
    {
        public CourseEasyBusiness()
        {

            morningList = new List<List<CourseBusiness>>();
            afternoonList = new List<List<CourseBusiness>>();
            nightList = new List<List<CourseBusiness>>();
        }
        public List<List<CourseBusiness>> morningList
        {
            get; set;
        }
        public List<List<CourseBusiness>> afternoonList
        {
            get; set;
        }
        public List<List<CourseBusiness>> nightList
        {
            get; set;
        }
    }
}