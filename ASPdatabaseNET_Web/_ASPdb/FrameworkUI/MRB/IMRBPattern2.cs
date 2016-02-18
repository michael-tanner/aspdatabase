using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.jQuery;

namespace ASPdb.FrameworkUI.MRB
{
    public interface IMRBPattern2
    {
        //M Model { get; set; }
        //VM ViewModel { get; set; }
        jQuery jRoot { get; set; }

        void BindUI();
        void BindUI_Single(string modelKey);
        //M GetModelWithoutFiringEvents();
        jQuery jF(string selector);
        jQuery jF2(string selector);
        void Instantiate();
        string GetHtmlRoot();

        void Open();
        void Close();

        bool Get_IsInstantiated();
        bool Get_IsOpen();
        jQuery Get_jRoot();
    }
}