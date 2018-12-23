using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassScheduleAPI.Common
{
    public class EnumUnit
    {
        //课程（日程）类型
        public enum CourseClassEnum
        {
            /// <summary>
            ///自己的课程表
            /// </summary>
            PrivateCourse = 1,
            //公共课程表
            PublicCourse = 2,
        }
        //课程（日程）频率
        public enum CourseFrequencyEnum
        {
            //仅今天
            TodayOnly = 1,
            //每天这个时段
            EveryDay = 2,
            //每周这个时段
            EveryWeek = 3,
        }
        //课程（日程）类型
        public enum CourseTypeEnum
        {
            //学校课程
            SchoolCurriculum = 1,
            //辅导班
            TutorshipClass = 2,
            //其它
            Other = 3,
        }
        //时间段
        public enum TimeTypeEnum
        {
            //上午
            Morning = 1,
            //下午
            Afternoon = 2,
            //晚上
            Night = 3,
        }

        //备忘类型
        public enum MemorandumTypeEnum
        {
            //本周备忘
            SingleMemorandum = 1,
            //备忘清单
            GroupMemorandum = 2
        }

        //推荐打卡项目类型
        public enum ClockProjectTypeEnum
        {
            //习惯养成
            Habit = 1,
            //学习打卡
            Study = 2,
            //其他打卡
            Other = 3

        }
        //打卡项目频率
        public enum ClockFrequencyEnum
        {
            //固定
            Fixed = 1,
            //按周
            EveryWeek = 2,
            //按月
            EveryMouth = 3
        }

        //操作类型
        public enum ClockExecuteTypeEnum
        {
            //打卡
            Punch = 1,
            //取消打卡
            Cancel = 2
        }

        //积分记录变更方式
        public enum IntegralRecordCalcTypeEnum
        {
            //加
            Plus = 1,
            //减
            Reduce = 2
        }
        
    }
}