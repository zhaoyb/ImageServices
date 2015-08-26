using System;
using System.IO;
using System.Web;
using System.Web.Http;
using FastDFS.Client;

namespace ImageServices.Controllers
{
    public class UploadController : ApiController
    {
        //CROS检测
        public void Options()
        {


        }

        public string Post()
        {
            var request = HttpContext.Current.Request;
            HttpPostedFile file = request.Files[0];

            string filename = "";
            if (ConfigHelper.GetConfigString("SaveMode") == "Local")
            {
                var savePath = "Upload\\" + GetPathRule();
                string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, savePath);
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }

                string ex = Path.GetExtension(file.FileName);
                filename = Guid.NewGuid().ToString("N") + ex;
                file.SaveAs(Path.Combine(localPath, filename));
                filename = request.Url.Scheme + "://" + request.Url.Authority + "/" + savePath.Replace("\\", "/") + "/" + filename;
            }
            else if (ConfigHelper.GetConfigString("SaveMode") == "Distributed")
            {
                byte[] fileData;
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }

                var storageNode = FastDFSClient.GetStorageNode(FastDfsGlobalConfig.Config.GroupName);
                var fileName = FastDFSClient.UploadFile(storageNode, fileData, Path.GetExtension(file.FileName).TrimStart('.'));
                return ConfigHelper.GetConfigString("TrackerHost") + FastDfsGlobalConfig.Config.GroupName + "/" + fileName;
            }
            return filename;

        }

        private string GetPathRule()
        {
            switch (ConfigHelper.GetConfigString("SavePathRule"))
            {
                case "YYYY":
                    return DateTime.Now.ToString("yyyy");
                case "YYYYMM":
                    return DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM");
                case "YYYYMMDD":
                    return DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM");
                default:
                    return DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd");
            }
        }

    }
}
