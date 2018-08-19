using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassScheduleAPI.Common
{
    public class ApplicationConstant
    {
        /// <summary>
        /// 周期。 循环天数。  【每天这个时段 / 每周这个时段】最多循环添加365天数据
        /// </summary>
        public const int forDay = 365;
    }
}