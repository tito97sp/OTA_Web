using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTAWebApp.Utils
{
    public class CommonTools
    {
        public static string DateFormated(DateTime DateTime)
        {
            if (DateTime.Now.Subtract(DateTime).Days < 1)
            {
                return DateTime.ToString("HH:mm");
            }
            else if (DateTime.Now.Subtract(DateTime).Days >= 1 && DateTime.Now.Subtract(DateTime).Days < 2)
            {
                return DateTime.Now.Subtract(DateTime).Days.ToString() + " day ago";
            }
            else if (DateTime.Now.Subtract(DateTime).Days >= 2 && DateTime.Now.Subtract(DateTime).Days < 7)
            {
                return DateTime.Now.Subtract(DateTime).Days.ToString() + " day ago";
            }
            else
            {
                return DateTime.Date.ToString("dd:MM:yyyy");
            }
        }

    }
}
