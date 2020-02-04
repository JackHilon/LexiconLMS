using LexiconLMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.ViewModels
{
    public class StudentListAndCourseName
    {
        public string CourseName;
        public IEnumerable<ApplicationUser> Students;
    }
}
