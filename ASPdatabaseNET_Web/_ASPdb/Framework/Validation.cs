using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ASPdb.Framework
{
    public class Validation
    {
        //----------------------------------------------------------------------------------------------------
        public static bool ValidateTextForSql1(string text, bool throwExceptionIfInvalid)
        {
            string pattern = "^[ a-zA-Z0-9_-]*$";
            var reg = new Regex(pattern);
            if (reg.IsMatch(text))
            {
                return true;
            }
            else
            {
                if (throwExceptionIfInvalid)
                    throw new Exception("Invalid SQL Text");
                return false;
            }
        }

        //----------------------------------------------------------------------------------------------------
        public static bool InSqlText_DoesNameNeedSquareBrackets(string text)
        {
            string pattern = "^[a-zA-Z0-9_]*$";
            var reg = new Regex(pattern);
            if (reg.IsMatch(text))
                return false;
            else
                return true;
        }
    }
}