using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.jQuery;

namespace ASPdatabaseNET.UI.PagesFramework
{
    public interface IPage
    {
        PageIdentifier PageId { get; set; }
        void Instantiate();
        jQuery Get_jRoot();
        string Get_HeaderColor();

        void Open();
        void Close();
    }
}