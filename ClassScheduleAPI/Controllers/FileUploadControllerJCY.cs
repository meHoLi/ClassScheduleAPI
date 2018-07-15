using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassScheduleAPI.Controllers
{
    public class FileUploadControllerJCY : Controller
    {
        // GET: FileUpload
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MuiltUpload()
        {
            return View();
        }

        /// <summary>
        /// 文件上传公共类
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult UpLoad(HttpPostedFileBase file)
        {
            //HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            string filePathName = string.Empty;
            var applicationPath = VirtualPathUtility.ToAbsolute("~") == "/" ? "" : VirtualPathUtility.ToAbsolute("~");
            string urlPath = applicationPath + "/Upload";

            string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Upload");

            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            localPath = localPath + "/" + year + "/" + month + "/" + day;
            string returl = "/Upload/" + year + "/" + month + "/" + day + "/";
            if (Request.Files.Count == 0)
            {
                return Json(new { StatusCode = 2, StatusMsg = "上传失败" });
            }

            //判断是否分片
            if (Request.Params.AllKeys.Any(k => k == "chunk"))
            {
                int chunk = Convert.ToInt32(Request.Form["chunk"]);     // 当前分片
                int chunks = Convert.ToInt32(Request.Form["chunks"]);   // 分片总计

                string tempfilename = localPath + "/" + Request.Form["guid"];
                if (!System.IO.Directory.Exists(tempfilename))
                    System.IO.Directory.CreateDirectory(tempfilename);
                file.SaveAs(Path.Combine(tempfilename + "/" + chunk));


                var files = System.IO.Directory.GetFiles(tempfilename);//获得所有分片文件

                if (files.Length == chunks)
                {
                    string ex = Path.GetExtension(file.FileName);
                    filePathName = Guid.NewGuid().ToString("N") + ex;

                    var finalPath = Path.Combine(localPath, filePathName);//
                    var fs = new FileStream(finalPath, FileMode.Create);
                    foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                    {
                        var bytes = System.IO.File.ReadAllBytes(part);
                        fs.Write(bytes, 0, bytes.Length);
                        bytes = null;
                        System.IO.File.Delete(part);//删除分块
                    }
                    fs.Close();
                    System.IO.Directory.Delete(tempfilename);//删除临时文件夹
                    return Json(new { StatusCode = 1, StatusMsg = "上传成功", Model = new { name = file.FileName, path = returl + filePathName } });
                }
                else
                {
                    return Json(new { StatusCode = 2, StatusMsg = "分片上传成功" });
                }
            }
            else
            {
                string ex = Path.GetExtension(file.FileName);
                filePathName = Guid.NewGuid().ToString("N") + ex;
                if (!System.IO.Directory.Exists(localPath))
                {
                    System.IO.Directory.CreateDirectory(localPath);
                }
                try
                {
                    file.SaveAs(Path.Combine(localPath, filePathName));
                    string filepaths = Path.Combine(localPath, filePathName);
                    return Json(new { StatusCode = 1, StatusMsg = "上传成功", Model =new {name=file.FileName,path= returl + filePathName } });
                }
                catch (Exception exp)
                {
                    return Json(new { StatusCode = -1, StatusMsg = "上传失败", Model = new { error = exp.Message } });
                }
            }
        }
    }
}