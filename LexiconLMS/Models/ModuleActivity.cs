using System;
using System.Collections.Generic;

namespace LexiconLMS.Models
{
    public class ModuleActivity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        // -----
        public ICollection<Document> Documents { get; set; }

        // Navigation Property
        public int ModuleId { get; set; }
        public Module Module { get; set; }
    }
}