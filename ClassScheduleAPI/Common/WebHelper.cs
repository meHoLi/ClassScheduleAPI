using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ClassScheduleAPI.Common
{
    public class WebHelper
    {
        private string link;
        private string path;
        private string serverPath;
        public WebHelper(string link, string path, string serverpath)
        {
            this.link = link;
            this.path = path;
            this.serverPath = serverpath;
        }
        /// <summary>
        /// 启用新线程
        /// </summary>
        public void GetImg()
        {
            var m_thread = new Thread(GetHtmlImage);
            m_thread.SetApartmentState(ApartmentState.STA);
            m_thread.Start();
            m_thread.Join();
        }
        /// <summary>
        /// 调用截图方法，然后进行等待，一直到图片生成为止
        /// </summary>
        private void GetHtmlImage()
        {

            GetImg1(link, path, serverPath);
            while (true)
            {
                //if (File.Exists(serverPath + "\\" + path))
                if (File.Exists(path))
                {
                    break;
                }
                DateTime current = DateTime.Now;
                while (current.AddMilliseconds(2000) > DateTime.Now)
                {
                    Application.DoEvents();//转让控制权
                }
            }
        }
        /// <summary>
        /// 进行截图
        /// </summary>
        /// <param name="links">截图网页链接</param>
        /// <param name="path">截图生成的文件存放的路径及其文件名</param>
        /// <param name="serverPath">phantomjs.exe与screenshot.js所在目录的路径</param>
        /// <returns></returns>
        public bool GetImg1(string links, string path, string serverPath)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = serverPath + @"\phantomjs.exe";
                // p.StartInfo.FileName = @"E:\SoftWare\phantomjs-2.1.1-windows\bin\phantomjs.exe";
                p.StartInfo.WorkingDirectory = serverPath + @"\";
                p.StartInfo.Arguments = string.Format(serverPath + @"\screenshot.js " + links + " " + path);

                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                p.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}


