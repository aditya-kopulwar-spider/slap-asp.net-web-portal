using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace SLAP_App.Services
{
    public class FileService
    {
        public async Task<string> UploadFile(HttpPostedFileBase fileToUpload,string formattedFileName)
        {
         var ext=   Path.GetExtension(fileToUpload.FileName);
            string filePath = null;
            if (fileToUpload == null || fileToUpload.ContentLength == 0)
            {
                return null;
            }
            CloudStorageAccount cloudStorageAccount =CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.ConnectionStrings["cloudConnectionString"].ConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("slapstorage");
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                    );
            }
            string fileName =/* Guid.NewGuid()+ */formattedFileName;
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            cloudBlockBlob.Properties.ContentType = fileToUpload.ContentType;
            await cloudBlockBlob.UploadFromStreamAsync(fileToUpload.InputStream);
            filePath = cloudBlockBlob.Uri.ToString();
            return filePath;
        }

        
    }
}