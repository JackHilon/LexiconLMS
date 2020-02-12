using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Common
{
    public class ValidationEndDateAttribute :ValidationAttribute
    {
        public bool checkDate(DateTime sDate,DateTime eDate)
        {
            if (eDate >= sDate)
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
