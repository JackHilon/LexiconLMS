using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LexiconLMS.Common;

namespace LexiconLMS.Models
{
    public class ModuleActivity
    {
        public int Id { get; set; }
       [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [DataType(DataType.Date)]
        //[CurrentStartDate]
        public DateTime StartDate { get; set; }
       
        [DataType(DataType.Date)]
        
        public DateTime EndDate { get; set; }


        // -----
        public ICollection<Document> Documents { get; set; }

        // Navigation Property
        public int ModuleId { get; set; }
        
        public Module Module { get; set; }
    }
}