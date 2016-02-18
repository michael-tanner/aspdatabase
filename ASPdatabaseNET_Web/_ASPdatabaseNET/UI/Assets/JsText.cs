using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;

namespace ASPdatabaseNET.UI.Assets
{
    //----------------------------------------------------------------------------------------------------////
    public class JsText
    {
        //----------------------------------------------------------------------------------------------------
        public static string Get()
        {
            string rtn = "";
            string key = "ASPdatabaseNET.UI.Assets.JsText";

            var cache = Memory.AppCache.Get();
            try
            {
                if (cache.AnyData.ContainsKey(key))
                    return (string)cache.AnyData[key];
            }
            catch { }

            try
            {
                string baseFolder = @"~\_ASPdatabaseNET\UI\Assets\";
                var files = new string[8];
                files[0] = @"JS\jquery-1.8.0.min.js";
                files[1] = @"JS\jquery.json-2.3.js";
                files[2] = @"JS\jquery-ui-1.10.4.custom.min.js";
                files[3] = @"JS\ASPdb.js";
                files[4] = @"JS\ASPdatabase.NET.js";
                files[5] = @"JS\Security\aes.js";
                files[6] = @"JS\Security\jsencrypt.min.js";
                files[7] = @"JS\Security\pbkdf2.js";
                foreach (string fileName in files)
                {
                    string fileLocation = HttpContext.Current.Server.MapPath(baseFolder) + fileName;
                    rtn += File.ReadAllText(fileLocation);
                }
            }
            catch { }

            if(rtn == "")
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "ASPdatabaseNET_Web._ASPdatabaseNET.UI.Assets.JsText.js";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    rtn += reader.ReadToEnd();
                }
            }


            rtn = String.Format(@"//--------------------------------------------------------------------------------
//  ASPdatabase.NET {0}
//  Download at www.ASPdatabase.net
//  {1}
//--------------------------------------------------------------------------------
//  Please see https://www.aspdatabase.net/about/ for 3rd party copyright notices.
//--------------------------------------------------------------------------------

{2}
",
            Config.SystemProperties.Version,
            Config.SystemProperties.CopyrightLine,
            rtn);

            cache.AnyData[key] = rtn;
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        private static string MinJs1(string str)
        {
            var rtn = new System.Text.StringBuilder();
            var lines = str.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                rtn.Append(line.Trim() + " ");
            }
            return rtn.ToString();
        }
        //----------------------------------------------------------------------------------------------------
        private static string MinJs2(string str)
        {
            var rtn = new System.Text.StringBuilder();
            var lines = str.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                string line_T = line.Trim();
                if (line_T.Length > 0)
                    rtn.Append(line.Trim() + " \n");
            }
            return rtn.ToString();
        }
    }
}