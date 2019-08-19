using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Base.Files.Models.Entities;
using API.Base.Files.Models.ViewModels;
using API.Base.Web.Base.Controllers.Api;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Base.Files.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class FileController : ApiController
    {
        private readonly FileUploadManager _fileUploadManager;

        public FileController(FileUploadManager fileUploadManager)
        {
            _fileUploadManager = fileUploadManager;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var list = new List<FileEntity>();
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest("no file uploaded. Upload a file with name 'files'.");
                }

                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;
                    var uploadedFile = await _fileUploadManager.Upload(file);
                    list.Add(uploadedFile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var vmList = Mapper.Map<List<FileViewModel>>(list);
            return Ok(vmList);
        }
    }
}