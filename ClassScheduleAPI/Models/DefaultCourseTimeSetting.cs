//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClassScheduleAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DefaultCourseTimeSetting
    {
        public int ID { get; set; }
        public Nullable<int> ChildrenID { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Nullable<int> CourseIndex { get; set; }
        public Nullable<int> PublicCourseInfoID { get; set; }
    }
}
