using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduleAPI.Common
{
    //返回消息实体
    public class ResponseMessage
    {
        public ResponseMessage() {
            Status = false;
        }
        /// <summary>
        /// 返回状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 返回结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 自定义提示信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }

    }
}
