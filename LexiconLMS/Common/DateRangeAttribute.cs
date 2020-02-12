using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Common
{
    public class DateRangeAttribute :RangeAttribute
    {
        string year = "2";
        //public DateRangeAttribute(int startDate, int endDate)
        //{


        //    if (endDate <= startDate)
        //    {
        //        messege = "Check the end date";
        //    }
        //}
        public DateRangeAttribute(string minimumValue ,string maxValue)
            : base(typeof(DateTime), minimumValue, maxValue)
        {

        }

    }
}
