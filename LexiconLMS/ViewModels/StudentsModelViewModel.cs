using LexiconLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.ViewModels
{
    public class StudentsModelViewModel
    {
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        [DataType(DataType.Date)]
        public DateTime ModuleStartDate { get; set; }
       
        //public ICollection<StudentsActivityViewModel> Activities { get; set; }
        public ICollection<ModuleActivity> Activities { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
