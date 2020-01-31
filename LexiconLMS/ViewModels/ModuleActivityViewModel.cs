using LexiconLMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.ViewModels
{
    public class ModuleActivityViewModel
    {
        public IEnumerable<Module> Modules { get; set; }

        public IEnumerable<ModuleActivity> Activities { get; set; }

    }
}
