using LexiconLMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.ViewModels
{
    public class ListStudentAssgnmnt
    {
        public string ActivityName;
        public int ActivityId;
        public ICollection<ApplicationUser> Students;
    }
}
