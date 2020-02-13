using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.ViewModels
{
    public class StudentsActivityViewModel
    {
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActivityStartDate { get; set; }
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
    }
}
