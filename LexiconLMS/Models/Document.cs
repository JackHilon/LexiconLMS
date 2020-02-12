using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentDescription { get; set; }
        public DateTime UploadDate { get; set; }

        public string Related { get; set; } // asp-route-RoleName="@Model.RoleName"
                                            // = {Course, Module, Activity}
        public byte[] Content { get; set; }



        // --------------------- For ModuleActivity ---------------------
        //ForeignKey
        public int? ModuleActivityId { get; set; }

        //Navigation Property
        public ModuleActivity? ModuleActivity { get; set; }
        // --------------------------------------------------------------

        // ====================== For ApplicationUser ===================
        //Navigation Property
        public ApplicationUser AppUser { get; set; }
        // ==============================================================


        // --- ### For Module (Documents connected with Module) ### ---
        //ForeignKey
        public int? ModuleId { get; set; }

        //Navigation Property

        public Module? Module { get; set; }
        // --- ### --- ### --- ### --- ### --- ### --- ### --- ### --- ### --- 


        // --- *** === For Module (Documents connected with Module) === *** ---
        //ForeignKey
        public int? CourseId { get; set; }

        //Navigation Property
        public Course? Course { get; set; }
        // --- *** === --- *** === --- *** === --- *** === --- *** === --- 

    }
}
