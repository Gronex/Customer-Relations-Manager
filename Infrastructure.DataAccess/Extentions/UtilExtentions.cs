using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Extentions
{
    public static class UtilExtentions
    {
        public static T Parse<T>(this string str, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof (T), str, ignoreCase);
        }
    }
}
