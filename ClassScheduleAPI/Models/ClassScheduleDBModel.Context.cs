﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ClassScheduleDBEntities : DbContext
    {
        public ClassScheduleDBEntities()
            : base("name=ClassScheduleDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Children> Children { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Memorandum> Memorandum { get; set; }
        public virtual DbSet<PublicCourse> PublicCourse { get; set; }
        public virtual DbSet<PublicCourseInfo> PublicCourseInfo { get; set; }
        public virtual DbSet<PublicCourseType> PublicCourseType { get; set; }
    }
}
