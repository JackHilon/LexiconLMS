using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        public DateTime StartDate { get; set; }

        //Navigation Property
        public ICollection<ApplicationUser> AppUsers { get; set; }
    }
}
