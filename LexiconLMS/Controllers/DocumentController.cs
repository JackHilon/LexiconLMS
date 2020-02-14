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
        public async Task<IActionResult> UploadFile([Bind("CourseId,CourseName,CourseDescription,StartDate,AppUsers")] IFormFile file, int id, string Related)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");


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
                            
                            DocumentName = file.FileName, // <--
                            DocumentDescription = file.FileName,
                            UploadDate = UploadDateTime.Date,
                            
                            Related = Related,

                            Content = memoryStream.ToArray()
                        };

                    doc = RelatedDocument(doc, id, Related);

                    _context.Documents.Add(doc);

                        await _context.SaveChangesAsync();
                    }

                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }

                if (Related == "Activity")
                {
                    return RedirectToAction("ModulePartialView", "Modules");
                }
                if (Related == "Module")
                {
                    return RedirectToAction("ModulePartialView", "Modules");
                }
                else //--if (Related == "Course")
                {
                    return RedirectToAction("Index", "Courses");
                }

            } // end using
        }

        private Document RelatedDocument(Document doc, int id, string related) // Initialize a document based on related string 
        {
            int activityId; //Or .. make them nullable keys in the related tables ?!
            int moduleId;   //Or .. make them nullable keys in the related tables ?!
            int courseId;

            var Related = related;

            if (Related == "Activity")
            {
                activityId = id;
                moduleId = _context.ModuleActivity.FirstOrDefault(a => a.Id == id).ModuleId;
                courseId = _context.Module.FirstOrDefault(m => m.Id == moduleId).CourseId;
                doc.ModuleActivityId = activityId;
                doc.ModuleId = moduleId;
                doc.CourseId = courseId;
            }

            if (Related == "Module")
            {
                moduleId = id;
                courseId = _context.Module.FirstOrDefault(m => m.Id == moduleId).CourseId;
                doc.ModuleId = moduleId;
                doc.CourseId = courseId;
            }

            if (Related == "Course")
            {
                courseId = id;
                doc.CourseId = courseId;
            }
            return doc; 
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

        public async Task<IActionResult> Download(string? filename)
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

        public async Task<IActionResult> DownLoadAllFiles(int? id)
        {
            var listFiles = _context.Documents.Where(d => d.ModuleActivityId == id).ToList();
            return View(listFiles);
        }


        // --------------------------------- Download a file ---------------------------------------------------
        
        public ActionResult DownLoadFile(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ModulePartialView", "Modules");
            }
            var doc = _context.Documents.FirstOrDefault(d => d.DocumentId == id);
            var fileArray = doc.Content;
            var fileName = doc.DocumentName;
            var related = doc.Related;
            // -- Radika 
            using (var file = new FileStream($"C:\\Users\\Elev\\Desktop\\{fileName}", FileMode.Create, FileAccess.ReadWrite))
            {
                file.Write(fileArray, 0, fileArray.Length);
                //file.Close();
                file.Flush();
                //  ViewBag.Message = "Document Downloaded Sucessfully!!!";
                ViewBag.Message = "Document Downloaded Sucessfully!!!";
            }

            //---
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Courses");
            }
            else
            { 
                    if (related == "Activity")
                    {
                        return RedirectToAction("ModulePartialView", "Modules");
                    }
                    if (related == "Module")
                    {
                        return RedirectToAction("ModulePartialView", "Modules");
                    }
                    else //--if (related == "Course")
                    {
                        return RedirectToAction("Index", "Courses");
                    }
            }
            //---

            // --(second one)-- return RedirectToAction("ModulePartialView", "Modules");

            //return RedirectToActionPermanent("DownLoadAllFiles", doc.ModuleActivityId);
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

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var @module = await _context.Documents.FindAsync(id);
            _context.Documents.Remove(@module);
            await _context.SaveChangesAsync();
            return RedirectToAction("ModulePartialView", "Modules");
            //return RedirectToAction(nameof(ModulePartialView));
        }

    }
}
