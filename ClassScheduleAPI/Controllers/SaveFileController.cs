using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class SaveFileController : Controller
    {

        /// <summary>
        /// 时间：2017年9月6日02:16:22
        /// 保存图片，并且返回对象
        /// </summary>
        /// <param name="filedata"></param>
        /// <param name="strFillPath">文件保存路径</param>
        /// <param name="isNewFileName">是否自动生成图片名称</param>
        /// <returns></returns>
        public SaveFileInfo SaveFile(HttpPostedFileBase filedata, string strFillPath, bool isNewFileName)
        {
            //文件名称
            string filePathName = filedata.FileName;
            //文件保存路径
            string strSaveFilePath = strFillPath + "/" + filePathName;// Path.Combine(strFillPath, filePathName);
            //返回文件路径
            string strReturnFilePath = strFillPath + "/" + filePathName;// Path.Combine(strFillPath, filePathName);
            if (isNewFileName)
            {
                string ex = Path.GetExtension(filedata.FileName);
                string newFilePathName = DateTime.Now.ToString("yyyyMMddHHmmss") + ex;
                strReturnFilePath = "/" + strReturnFilePath.Replace(HttpRuntime.AppDomainAppPath, "");
                strReturnFilePath = strReturnFilePath.Replace(filePathName, newFilePathName);
                strSaveFilePath = strSaveFilePath.Replace(filePathName, newFilePathName);
            }

            string uploadPath = System.AppDomain.CurrentDomain.BaseDirectory;
            if (!System.IO.Directory.Exists(uploadPath+strFillPath))
            {
                System.IO.Directory.CreateDirectory(uploadPath + strFillPath);
            }
            filedata.SaveAs(uploadPath + "/" + strSaveFilePath);
            //filedata.SaveAs(strSaveFilePath);

            SaveFileData tempSabeImgData = new SaveFileData();
            tempSabeImgData.src = strReturnFilePath;
            tempSabeImgData.title = "";
            SaveFileInfo tempSaveImgInfo = new SaveFileInfo();
            tempSaveImgInfo.code = 0;
            tempSaveImgInfo.msg = "上传成功！";
            tempSaveImgInfo.data = tempSabeImgData;
            return tempSaveImgInfo;
        }
    }

    /// <summary>
    /// 时间：2017年9月6日02:16:51
    /// 保存完文件返回的对象
    /// </summary>
    public class SaveFileInfo
    {
        public Int32 code { get; set; }
        public string msg { get; set; }
        public SaveFileData data { get; set; }
    }

    public class SaveFileData
    {
        public string src { get; set; }
        public string title { get; set; }

    }
}