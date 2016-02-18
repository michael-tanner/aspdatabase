using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace ASPdb.Framework
{
    //----------------------------------------------------------------------------------------------------////
    public class Debug
    {
        private static string _filePath = null;
        public static string FilePath
        {
            get
            {
                if (_filePath == null)
                    try
                    {
                        string relativePath = String.Format(@"~/App_Data/Debug/Log--{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
                        _filePath = HttpContext.Current.Server.MapPath(relativePath);
                    }
                    catch { _filePath = ""; }
                return (_filePath);
            }
            set
            {
                _filePath = value;
            }
        }

        private static System.Diagnostics.EventLog _eventLog = null;
        public static System.Diagnostics.EventLog EventLog
        {
            get
            {
                if (_eventLog == null)
                {
                    _eventLog = new System.Diagnostics.EventLog();

                    bool sourceExists = false;
                    if (System.Diagnostics.EventLog.SourceExists("VizziniSrc"))
                        sourceExists = true;
                    else
                        System.Diagnostics.EventLog.CreateEventSource("VizziniSrc", "VizziniLog"); // ---- This can't be done from Network Service

                    if (sourceExists)
                    {
                        _eventLog.Source = "VizziniSrc";
                        _eventLog.Log = "VizziniLog";
                    }
                }
                return _eventLog;
            }
            set
            {
                _eventLog = value;
            }
        }
        public static bool OutputToEventLog = false;





        //----------------------------------------------------------------------------------------------------
        public static void WriteLine(string line)
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DebugLog"].ToLower().Trim() != "true")
                    return;


                if (Debug.OutputToEventLog)
                {
                    Debug.EventLog.WriteEntry(DateTime.Now.ToString() + " ..... \n" + line);
                }
                else
                {
                    if (!File.Exists(Debug.FilePath))
                        using (StreamWriter sw = File.CreateText(Debug.FilePath))
                        {
                            sw.Write("");
                        }

                    using (StreamWriter sw = File.AppendText(Debug.FilePath))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + " ... " + line);
                    }
                }
            }
            catch { }
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Includes String.Format() with 1 parameter</summary>
        public static void WriteLine(string line, object formatObj1)
        {
            Debug.WriteLine(String.Format(line, formatObj1));
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Includes String.Format() with 2 parameters</summary>
        public static void WriteLine(string line, object formatObj1, object formatObj2)
        {
            Debug.WriteLine(String.Format(line, formatObj1, formatObj2));
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Includes String.Format() with 3 parameters</summary>
        public static void WriteLine(string line, object formatObj1, object formatObj2, object formatObj3)
        {
            Debug.WriteLine(String.Format(line, formatObj1, formatObj2, formatObj3));
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Includes String.Format() with 4 parameters</summary>
        public static void WriteLine(string line, object formatObj1, object formatObj2, object formatObj3, object formatObj4)
        {
            Debug.WriteLine(String.Format(line, formatObj1, formatObj2, formatObj3, formatObj4));
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Includes String.Format() with 5 parameters</summary>
        public static void WriteLine(string line, object formatObj1, object formatObj2, object formatObj3, object formatObj4, object formatObj5)
        {
            Debug.WriteLine(String.Format(line, formatObj1, formatObj2, formatObj3, formatObj4, formatObj5));
        }


        //----------------------------------------------------------------------------------------------------
        public static void ClearFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Debug.FilePath))
                {
                    sw.WriteLine("");
                }
            }
            catch { }
        }





        //----------------------------------------------------------------------------------------------------
        public static void RecordException_PlusLine(Exception exc, string line)
        {
            Debug.RecordException(exc);
            Debug.WriteLine(line);
        }


        //----------------------------------------------------------------------------------------------------
        public static void RecordException(Exception exc)
        {
            try
            {
                string s = "";

                if (exc.InnerException == null)
                {
                    s = String.Format(@"
------------------------------------------------------------------------------------ Exception -----
{0}
----------
{1}
--------------------------------------------------
", exc.Message, exc.StackTrace);
                }
                else
                {
                    s = String.Format(@"
------------------------------------------------------------------------------------ Exception -----
{0}
----------
{1}
---------- Inner Exception ----------
{2}
----------
{3}
--------------------------------------------------
", exc.Message, exc.StackTrace, exc.InnerException.Message, exc.InnerException.StackTrace);
                }

                Debug.WriteLine(s);


            }
            catch { }
        }



        ////----------------------------------------------------------------------------------------------------
        //public static void EmailDeveloper(string subject, string body)
        //{
        //    string toAddress = "michael@aspdatabase.net";
        //}
        ////----------------------------------------------------------------------------------------------------
        //public static void EmailDeveloper_Exception(Exception exc)
        //{
        //}


    }


}