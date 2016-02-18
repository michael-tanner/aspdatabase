using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Security;

namespace ASPdb.Security
{
    // http://stackoverflow.com/questions/3681493/leveraging-asp-net-machinekey-for-encrypting-my-own-data

    //----------------------------------------------------------------------------------------------------////
    public class MachineEncryption
    {
        //----------------------------------------------------------------------------------------------------
        public static string Protect(string stringToProtect, string purposeString)
        {
            var unprotectedBytes = Encoding.UTF8.GetBytes(stringToProtect);
            var protectedBytes = MachineKey.Protect(unprotectedBytes, purposeString);
            return Convert.ToBase64String(protectedBytes);
        }
        //----------------------------------------------------------------------------------------------------
        public static string Unprotect(string protectedString, string purposeString)
        {
            var protectedBytes = Convert.FromBase64String(protectedString);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, purposeString);
            return Encoding.UTF8.GetString(unprotectedBytes);
        }

    }
}