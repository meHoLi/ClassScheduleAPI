using ClassScheduleAPI.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class FileUploadController : Controller
    {
        /// <summary>
        /// 跨域上传附件
        /// </summary>
        /// <param name="saveImgSrc">图片路径 例如/File/Product/</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ActionResult HandleFileSave(string saveImgSrc="Iimg", string TenantId="WX", string callback = "")
        {
            JsonpResult<object> json = null;
            string fileName = "";
            string tempSaveFile = "";
            Guid fileId = Guid.NewGuid();
            try
            {
                //接受上传文件
                HttpPostedFileBase postFile = Request.Files[0];
                //string name = Request.QueryString["name"];
                if (postFile != null)
                {
                    //获取上传目录 转换为物理路径
                    string strdt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string uploadPath = Server.MapPath("/File/" + TenantId + "/" + saveImgSrc);
                    //判断路径是否存在 如果不存在 创建文件夹
                    //文件名
                    fileName = postFile.FileName;
                    //获取文件大小
                    long contentLength = postFile.ContentLength;

                    //如果不存在path目录
                    if (!Directory.Exists(uploadPath))
                    {
                        //那么就创建它
                        Directory.CreateDirectory(uploadPath);
                    }
                    tempSaveFile = "/File/" + TenantId + "/" + saveImgSrc + "/" + strdt + "_" + fileId + fileName;
                    //保存文件的物理路径
                    string saveFile = uploadPath + "/" + strdt + "_" + fileId + fileName;
                    try
                    {
                        //保存文件
                        postFile.SaveAs(saveFile);
                    }
                    catch
                    {
                    }

                }
                else
                {

                }
            }
            catch (Exception e)
            {
                //"上传文件失败";
            }
            var getval = new
            {
                jsonrpc = "2.0",
                id = fileId,
                filePath = tempSaveFile

            };
            json = new JsonpResult<object>(getval, callback);
            return json;

        }
    }
}
