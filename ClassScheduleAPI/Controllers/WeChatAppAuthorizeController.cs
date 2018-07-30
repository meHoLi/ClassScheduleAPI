using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassScheduleAPI.Common;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using System.Net;
using System.IO;
using System.Web.Security;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace ClassScheduleAPI.Controllers
{
    public class WeChatAppAuthorizeController : Controller
    {
        // GET: WeChatAppAuthorize
        //cqs
        //private const string APPID = "wx3acda84288bf9573";
        //private const string AppSecret = "06a0ccc89e812a7234a851aa9ea06fc4";
        //my
        private const string APPID = "wx694ef3488ec1381b";
        private const string AppSecret = "67ed5d3fa3aa51f7d4ceb063d9caf457";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetToken()
        {
            ResponseMessage msg = new ResponseMessage();
            var result = new WeChatAppDecrypt(APPID, AppSecret).GetToken();
            msg.Status = true;
            msg.Data = result;
            return Json(msg, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetOpenIdAndSessionKeyString(string code)
        {
            LogHelper.Error("GetOpenIdAndSessionKeyString");

            ResponseMessage msg = new ResponseMessage();
            msg.Status = true;
            try
            {
                string temp = new WeChatAppDecrypt(APPID, AppSecret).GetOpenIdAndSessionKeyString(code);
                msg.Data = temp;
            }
            catch (Exception e)
            {
                msg.Status = false;
                LogHelper.Error($"GetOpenIdAndSessionKeyString:{e.Message}");
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        //https://blog.csdn.net/ivanyoung66/article/details/72523231
        //https://bbs.csdn.net/topics/392320383



        //小程序提醒
        //http://www.cnblogs.com/vanteking/p/7606222.html
        //https://www.cnblogs.com/qiujz/articles/5913796.html


        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="accessToken">AccessToken</param>
        /// <param name="data">发送的模板数据</param>
        /// <returns></returns>

        public string SendTemplateMsg(string accessToken, string data)
        {
            LogHelper.Debug("=============================================================");
            LogHelper.Debug("accessToken:" + accessToken);
            LogHelper.Debug("data:" + data);
            //https://blog.csdn.net/qq_31583959/article/details/52087987
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={0}", accessToken);
            HttpWebRequest hwr = WebRequest.Create(url) as HttpWebRequest;
            hwr.Method = "POST";
            hwr.ContentType = "application/x-www-form-urlencoded";
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(data); //通过UTF-8编码
            hwr.ContentLength = payload.Length;
            Stream writer = hwr.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            var result = hwr.GetResponse() as HttpWebResponse; //此句是获得上面URl返回的数据
            string strMsg = WebResponseGet(result);
            LogHelper.Debug("strMsg:" + strMsg);
            return strMsg;
        }

        /// <summary>
        /// 消息推送，异步
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="data"></param>
        /// <param name="StartTime">上课时间</param>
        /// <param name="RemindTime">提前提醒间隔（分钟）</param>
        public void SendMsgAsync(string accessToken, string data, string StartTime, int RemindTime)
        {
            try
            {
                DateTime st = DateTime.Parse(StartTime).AddMinutes(-RemindTime);
                DateTime dn = DateTime.Now;
                if (st > dn)
                {
                    int s = (int)(st - dn).TotalSeconds;
                    //RemindTime = "-9999"时前台做逻辑，不调用此方法
                    ExecSendMsgAsync(accessToken, data, s);
                }
            }
            catch (Exception e)
            {

            }
        }

        public async Task<int> ExecSendMsgAsync(string accessToken, string data, int s)
        {
            try
            {
                var a = await ThreadSleepAsync(s);

                accessToken = new WeChatAppDecrypt(APPID, AppSecret).GetToken();
                //反序列化结果
                WechatToken tokenModel = JsonConvert.DeserializeObject<WechatToken>(accessToken);
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={0}", tokenModel.access_token);
                HttpWebRequest hwr = WebRequest.Create(url) as HttpWebRequest;
                hwr.Method = "POST";
                hwr.ContentType = "application/x-www-form-urlencoded";
                byte[] payload;
                payload = System.Text.Encoding.UTF8.GetBytes(data); //通过UTF-8编码
                hwr.ContentLength = payload.Length;
                Stream writer = hwr.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                var result = hwr.GetResponse() as HttpWebResponse; //此句是获得上面URl返回的数据
                string strMsg = WebResponseGet(result);
                LogHelper.Debug("strMsgAsync:" + strMsg);
            }
            catch (Exception e)
            {
                LogHelper.Debug("strMsgAsyncError:" + e.Message);

            };
            return 1;
        }
        public async Task<int> ThreadSleepAsync(int s)
        {
            Thread.Sleep(1000 * s);
            return 1;
        }
        public string WebResponseGet(HttpWebResponse webResponse)
        {
            LogHelper.Debug("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            StreamReader responseReader = null;
            string responseData = "";
            try
            {
                responseReader = new StreamReader(webResponse.GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception e)
            {
                LogHelper.Debug("throw:" + e.Message);
            }
            finally
            {
                webResponse.GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }
            return responseData;
        }

        /// <summary>
        /// 微信消息配置 
        /// </summary>
        public void ProcessRequest()
        {
            try
            {
                var signature = Request.QueryString["signature"];
                var timestamp = Request.QueryString["timestamp"];
                var nonce = Request.QueryString["nonce"];
                var echostr = Request.QueryString["echostr"];

                LogHelper.Debug("微信消息服务器验证传入数据" + string.Format("signature:{0},timestamp:{1},nonce:{2},echostr:{3}", signature, timestamp, nonce, echostr));

                var token = "aaaaaaa";//自定义字段(自己填写3-32个字符)

                //timestamp和token和nonce 字典排序
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("token", token);
                dic.Add("nonce", nonce);
                dic.Add("timestamp", timestamp);
                var list = dic.OrderBy(s => s.Value);
                var conbineStr = "";
                foreach (var s in list)
                {
                    conbineStr = conbineStr + s.Value;
                }
                string data = conbineStr;
                //sha1加密
                string secret = FormsAuthentication.HashPasswordForStoringInConfigFile(conbineStr, "SHA1").ToLower();
                var success = signature == secret;
                if (success)
                {
                    data = echostr;
                }
                Response.ContentType = "text/plain";
                Response.Write(data);
            }
            catch (Exception e)
            {
                LogHelper.Debug("微信消息服务器验证传入数据异常：" + e.Message);

            }
        }
    }
}