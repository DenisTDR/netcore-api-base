using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Base.Files.Models.Entities;
using API.Base.Files.Models.ViewModels;
using API.Base.Web.Base.Controllers;
using API.Base.Web.Base.Database.DataLayer;
using API.Base.Web.Base.Database.Repository;
using API.Base.Web.Base.Database.Repository.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Base.Files.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class FileUiController : DiController
    {
        public IRepository<FileEntity> Repo;
        private readonly FileUploadManager _fileUploadManager;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.OrderBy(e => e.Created));
        }

        public FileUiController(IDataLayer dataLayer, FileUploadManager fileUploadManager)
        {
            _fileUploadManager = fileUploadManager;
            Repo = dataLayer.Repo<FileEntity>();
        }

#pragma warning disable 1998
        public async Task<IActionResult> Index()
#pragma warning restore 1998
        {
            return RedirectToAction(nameof(List));
        }


        public async Task<IActionResult> List()
        {
            var files = await GetFiles();
            return View(files);
        }

        public async Task<IActionResult> FileSelectModal()
        {
            var files = await GetFiles();
            return PartialView("_FileSelectModal", files);
        }

        public IActionResult UploadFileModal()
        {
            return PartialView("_UploadFileModal");
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

        public virtual async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await Repo.GetOne(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        } // POST: ExampleAdmin/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(string id)
        {
            await Repo.Delete(id);
            return RedirectToAction(nameof(List));
        }

        public virtual async Task<IActionResult> Edit(string id)
        {
            var entity = await Repo.GetOne(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(string id, FileEntity entity)
        {
            if (id != entity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await Repo.GetOne(id);
                if (existing == null)
                {
                    return NotFound();
                }

                var tempFile = JsonConvert.DeserializeObject<FileEntity>(JsonConvert.SerializeObject(existing));
                tempFile.Protected = entity.Protected;
                tempFile.Description = entity.Description;

                try
                {
                    var epb = EntityUpdateHelper<FileEntity>.GetEpbFor(tempFile, existing);
                    var updated = await Repo.Patch(epb);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await Repo.Exists(id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(entity);
        }

        private static readonly string[] ValidImageExtensions = {"jpg", "jpeg", "bmp", "gif", "png", "svg"};

        private async Task<IEnumerable<FileEntity>> GetFiles()
        {
            var files = (await Repo.GetAll()).ToList();
            foreach (var fileEntity in files)
            {
                fileEntity.Link = Mapper.Map<FileViewModel>(fileEntity).Link;
                fileEntity.IsImage = ValidImageExtensions.Contains(fileEntity.Extension?.ToLower());
            }

            return files;
        }
    }
}