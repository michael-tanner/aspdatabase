using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdb.Framework
{
    //----------------------------------------------------------------------------------------------------////
    public class Str
    {
        //----------------------------------------------------------------------------------------------------
        public static string Join(string[] arr, string joinText)
        {
            if(arr == null)
                return "";

            string rtn = "";
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0)
                    rtn += joinText;
                rtn += arr[i];
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static string Join(string[] arr, string startText, string joinText, string endText)
        {
            return startText + Join(arr, joinText) + endText;
        }

    }
}