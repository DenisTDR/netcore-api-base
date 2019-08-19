using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Base.Files.Models.Entities;
using API.Base.Logging.Logger;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Helpers;
using API.Base.Web.Base.Misc;
using Microsoft.AspNetCore.Http;

namespace API.Base.Files
{
    public class FileUploadManager
    {
        protected readonly IDataLayer DataLayer;
        protected readonly ILLogger Logger;

        public FileUploadManager(IDataLayer dataLayer, ILLogger logger)
        {
            DataLayer = dataLayer;
            Logger = logger;
        }

        public async Task<FileEntity> Upload(IFormFile file)
        {
            var subDir = "upload/files";
            var uploadedFilesDirectory =
                Path.Combine(EnvVarManager.GetOrThrow("CONTENT_DIRECTORY"), subDir).Replace("\\", "/");
//            Console.WriteLine("Creating directory...");
            if (!Directory.Exists(uploadedFilesDirectory))
            {
                Logger.LogInfo("Creating files directory: " + uploadedFilesDirectory);
                Directory.CreateDirectory(uploadedFilesDirectory);
            }

            var fileEntity = new FileEntity
            {
                Name = SanitizeFileName(file.FileName) + "_" + Utilis.GenerateRandomHexString(10),
                Extension = Path.GetExtension(file.FileName).Substring(1).ToLower(),
                OriginalName = file.FileName,
                Size = (int) file.Length,
                SubDirectory = subDir
            };


            var filePath = Path.Combine(uploadedFilesDirectory, $"{fileEntity.Name}.{fileEntity.Extension}")
                .Replace("\\", "/");
//            Console.WriteLine("Saving file...");
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            fileEntity.Path = filePath;

            await DataLayer.Repo<FileEntity>().Add(fileEntity);

            return fileEntity;
        }

        private string SanitizeFileName(string fileName)
        {
            var nfn = new StringBuilder();

            fileName = Path.GetFileNameWithoutExtension(fileName);
            
            foreach (var c in fileName)
            {
                if (Path.GetInvalidFileNameChars().Contains(c))
                {
                    nfn.Append("_");
                }
                else
                {
                    nfn.Append(c);
                }
            }

            return nfn.ToString();
        }
    }
}