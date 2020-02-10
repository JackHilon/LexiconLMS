using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LexiconLMS.Data;
using LexiconLMS.Models;
using LexiconLMS.Models.Models.Document;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;
using System.Collections;

namespace LexiconLMS.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IFileProvider fileProvider;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public DocumentController(IFileProvider fileProvider,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.fileProvider = fileProvider;
            _context = context;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([Bind("CourseId,CourseName,CourseDescription,StartDate,AppUsers")] IFormFile file, int id)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var activityId = id;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    var UploadDateTime = DateTime.Now;

                    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await userManager.FindByIdAsync(userId);

                    var doc = new Document()
                    {
                        AppUser = user,
                        ModuleActivityId = activityId,

                        DocumentName = file.FileName,
                        DocumentDescription = file.FileName,
                        UploadDate = UploadDateTime.Date,

                        Content = memoryStream.ToArray()
                    };

                    _context.Documents.Add(doc);

                    await _context.SaveChangesAsync();
                }

                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
                return RedirectToAction("ModulePartialView", "Modules");
            }
        }



        public IActionResult Files()
        {
            var model = new FilesViewModel();
            foreach (var item in this.fileProvider.GetDirectoryContents(""))
            {
                model.Files.Add(
                    new FileDetails { Name = item.Name, Path = item.PhysicalPath });
            }
            return View(model);
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/UploadFiles", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }


        // ------------------------------ DownLoadAllFiles ----------------------------------------------------

        public async Task<IActionResult> DownLoadAllFiles(int id)
        {
            var listFiles = _context.Documents.Where(d => d.ModuleActivityId == id).ToList();
            return View(listFiles);
        }


        // --------------------------------- Download a file ---------------------------------------------------
        
        public ActionResult DownLoadFile(int id)
        {
            
            var doc = _context.Documents.FirstOrDefault(d => d.DocumentId == id);
            var fileArray = doc.Content;
            var fileName = doc.DocumentName;

            using (var file = new FileStream($"C:\\Users\\Elev\\Desktop\\{fileName}", FileMode.Create, FileAccess.ReadWrite))
            {
                file.Write(fileArray, 0, fileArray.Length);
                //file.Close();
                file.Flush();
            }

            return RedirectToActionPermanent("DownLoadFile", id);
        }



        // ---------------- Private ----------------------------------
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
