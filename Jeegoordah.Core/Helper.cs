using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeegoordah.Core
{
    public static class Helper
    {        
        public static string F(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
