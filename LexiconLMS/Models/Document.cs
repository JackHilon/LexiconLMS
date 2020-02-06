using System;
using System.Collections.Generic;
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

        // --------------------- For ModuleActivity ---------------------
        //ForeignKey
        public int ModuleActivityId { get; set; }

        //Navigation Property
        public ModuleActivity ModuleActivity { get; set; }
        // --------------------------------------------------------------

        // ====================== For ApplicationUser ===================
        //Navigation Property
        public ApplicationUser AppUser { get; set; }
        // ==============================================================


    }
}
