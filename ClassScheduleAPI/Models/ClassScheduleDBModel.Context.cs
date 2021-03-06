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
        public virtual DbSet<ChildrenStandardCourse> ChildrenStandardCourse { get; set; }
        public virtual DbSet<Clock> Clock { get; set; }
        public virtual DbSet<ClockProject> ClockProject { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<DefaultCourse> DefaultCourse { get; set; }
        public virtual DbSet<DefaultCourseSetting> DefaultCourseSetting { get; set; }
        public virtual DbSet<DefaultCourseTimeSetting> DefaultCourseTimeSetting { get; set; }
        public virtual DbSet<GrowthDiary> GrowthDiary { get; set; }
        public virtual DbSet<IntegralRecord> IntegralRecord { get; set; }
        public virtual DbSet<Memorandum> Memorandum { get; set; }
        public virtual DbSet<MemorandumGroup> MemorandumGroup { get; set; }
        public virtual DbSet<PageCommon> PageCommon { get; set; }
        public virtual DbSet<PublicBox> PublicBox { get; set; }
        public virtual DbSet<PublicCourseInfo> PublicCourseInfo { get; set; }
        public virtual DbSet<PublicCourseType> PublicCourseType { get; set; }
        public virtual DbSet<ExchangeProject> ExchangeProject { get; set; }
    }
}
