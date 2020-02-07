using LexiconLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.ViewModels
{
    public class ModuleActivityListViewModel
    {
        //Module
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }

        [DataType(DataType.Date)]
        public DateTime ModuleStartDate { get; set; }

        //Activity
        //public string ActivityName { get; set; }
        //public string ActivityDescription { get; set; }

        //[DataType(DataType.Date)]
        //public DateTime ActivityStartDate { get; set; }

        public ICollection<ModuleActivity> Activities { get; set; }


    }
}
