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
    
    public partial class IntegralRecord
    {
        public int ID { get; set; }
        public Nullable<int> ChildrenID { get; set; }
        public Nullable<int> CalcType { get; set; }
        public Nullable<int> Number { get; set; }
        public string Name { get; set; }
        public string CreateTime { get; set; }
        public Nullable<int> TotalNumber { get; set; }
    }
}