using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Common
{
    public class CurrentStartDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);
            if (dateTime >=DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
