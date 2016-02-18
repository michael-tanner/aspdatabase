using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdb.Framework
{
    public class Int2
    {
        public static int Parse(string str)
        {
            return Parse(str, -1);
        }
        public static int Parse(string str, int defaultValue)
        {
            int rtn = defaultValue;
            Int32.TryParse(str, out rtn);
            return rtn;
        }
    }
}