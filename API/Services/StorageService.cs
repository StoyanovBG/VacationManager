using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace API.Services
{
    public static class StorageService
    {
        public static string SaveFile(HttpPostedFileBase attachment)
        {
            string fileName = Path.GetFileNameWithoutExtension(attachment.FileName);
            string fileExtension = Path.GetExtension(attachment.FileName);
            fileName = DateTime.Now.ToString("yyyyMMdd") + "-" + fileName.Trim() + fileExtension;
            string uploadPath = ConfigurationManager.AppSettings["FileStoragePath"].ToString();

            attachment.SaveAs(uploadPath + fileName);

            return uploadPath + fileName;
        }

        public static void DeleteFile(string attachmentPath)
        {
            if (File.Exists(attachmentPath))
            {
                File.Delete(attachmentPath);
            }
        }
    }
}