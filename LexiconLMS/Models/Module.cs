using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models
{
    public class Module
    {
       // private const object DataType;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        
        // Navigation Property
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<ModuleActivity> Activity { get; set; }
    }
}
