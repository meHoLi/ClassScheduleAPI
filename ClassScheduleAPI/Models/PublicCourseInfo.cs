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
    
    public partial class PublicCourseInfo
    {
        public int ID { get; set; }
        public Nullable<int> PublicCourseTypeID { get; set; }
        public string OpenID { get; set; }
        public string Name { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public Nullable<int> PublicBoxID { get; set; }
        public Nullable<int> DefaultType { get; set; }
    }
}
