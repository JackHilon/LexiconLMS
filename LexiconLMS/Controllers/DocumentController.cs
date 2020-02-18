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
using LexiconLMS.ViewModels;
using Microsoft.EntityFrameworkCore;

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
                if (User.IsInRole("Student"))
                {
                    return RedirectToAction("Index", "Courses");
                }
                else
                {
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
                }

            } // end using
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
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Courses");
            }
            else

                return DelDocBackTo(related);
        }

        public async Task<IActionResult> DelDoc(int? id, string Related)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deletedDoc = DelDocRelated(id, Related);

            if (deletedDoc == null)
            {
                return DelDocBackTo(Related);
            }

            return View(deletedDoc);
        }

        [HttpPost, ActionName("DelDoc")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DelDocConfirmed(int id)
        {
            var deletedDoc = await _context.Documents.FindAsync(id);
            var related = deletedDoc.Related;

            _context.Documents.Remove(deletedDoc);
            await _context.SaveChangesAsync();

            return DelDocBackTo(related);
        }

        // ================================ The teacher see the list of assignments of an activity ==================================

        
        public async Task<IActionResult> SeeAssignments(int? id)             //  id of Activity
        {
            if (id == null)
            {
                return RedirectToAction("ModulePartialView", "Modules");
            }

            var activityId = id;
            var activityName = _context.ModuleActivity.FirstOrDefault(a => a.Id == id).Name;
            var moduleId = _context.ModuleActivity.FirstOrDefault(a => a.Id == id).ModuleId;
            var courseId = _context.Module.FirstOrDefault(m => m.Id == moduleId).Id;

            var allStudents = _context.AppUser.Where(user => user.CourseId == courseId).Include(a => a.Documents).ToList();
            
            var model = new ListStudentAssgnmnt()
            {
                ActivityName = activityName,
                ActivityId = (int) activityId,
                Students = allStudents,
            }; 

            return View("SeeAssignments", model);
        }

        // ======================================= Give a Grade to a student for an activity ===========================================
        [HttpGet]
        public async Task<ActionResult> GiveGrade(string Grade, string? id, int? ActivityId)    // user id
        {
            var currentActivityId = ActivityId;

            if (id == null || ActivityId == null)
            {
                return NotFound();
            }

            _context.Documents
                .Include(u => u.AppUser)
                .Include(a => a.ModuleActivity)
                .FirstOrDefault(a => a.AppUser.Id == id && a.ModuleActivityId == ActivityId).Grade = Grade;

            await _context.SaveChangesAsync();

            // ---------------------------------------------
            var moduleId = _context.ModuleActivity.FirstOrDefault(a => a.Id == currentActivityId).ModuleId;
            var courseId = _context.Module.FirstOrDefault(m => m.Id == moduleId).Id;

            var model = new ListStudentAssgnmnt()
            {
                ActivityName = _context.ModuleActivity.FirstOrDefault(a => a.Id == currentActivityId).Name,
                ActivityId = (int) currentActivityId,
                Students = _context.AppUser.Where(user => user.CourseId == courseId).Include(a => a.Documents).ToList()
            };
            // ---------------------------------------------

            return View("SeeAssignments", model);
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

        private Document DelDocRelated(int? id, string related)
        {
            var deletedDoc = new Document();

            switch (related)
            {
                case "Course":
                    deletedDoc = _context.Documents.FirstOrDefault(c => c.Related == related && c.CourseId == id);
                    break;

                case "Module":
                    deletedDoc = _context.Documents.FirstOrDefault(c => c.Related == related && c.ModuleId == id);
                    break;

                default: // -- case (Related == "Activity")
                    deletedDoc = _context.Documents.FirstOrDefault(c => c.Related == related && c.ModuleActivityId == id);
                    break;
            }
            return deletedDoc;
        }

        private ActionResult DelDocBackTo(string related)
        {
            switch (related)
            {
                case "Course":
                    return RedirectToAction("Index", "Courses");

                case "Module":
                    return RedirectToAction("ModulePartialView", "Modules");

                default: // -- case (related == "Activity")
                    return RedirectToAction("ModulePartialView", "Modules");
            }
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

                doc.ModuleActivity = _context.ModuleActivity.Find(activityId);
                doc.Module = _context.Module.Find(moduleId);
                doc.Course = _context.Courses.Find(courseId);
            }

            if (Related == "Module")
            {
                moduleId = id;
                courseId = _context.Module.FirstOrDefault(m => m.Id == moduleId).CourseId;
                doc.ModuleId = moduleId;
                doc.CourseId = courseId;

                doc.Module = _context.Module.Find(moduleId);
                doc.Course = _context.Courses.Find(courseId);
            }

            if (Related == "Course")
            {
                courseId = id;
                doc.CourseId = courseId;

                doc.Course = _context.Courses.Find(courseId);
            }
            return doc;
        }



        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]                             // -- ?????????????????????????????????????????
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var @module = await _context.Documents.FindAsync(id);
            _context.Documents.Remove(@module);
            await _context.SaveChangesAsync();
            return RedirectToAction("ModulePartialView", "Modules");
            //return RedirectToAction(nameof(ModulePartialView));
        }
                                                                      // -- ??????????????????????????????????????????
    }
}
