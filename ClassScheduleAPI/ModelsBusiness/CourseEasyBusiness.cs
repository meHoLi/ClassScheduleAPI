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
            afternoon = new List<List<CourseBusiness>>();
            night = new List<List<CourseBusiness>>();
        }
        public List<List<CourseBusiness>> morningList
        {
            get; set;
        }
        public List<List<CourseBusiness>> afternoon
        {
            get; set;
        }
        public List<List<CourseBusiness>> night
        {
            get; set;
        }
    }
}