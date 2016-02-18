using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.jQuery;

namespace ASPdb.FrameworkUI.MRB
{
    public interface IMRBPattern<M, VM> : IMRBPattern2
    {
        M Model { get; set; }
        VM ViewModel { get; set; }
        new jQuery jRoot { get; set; }

        new void BindUI();
        new void BindUI_Single(string modelKey);
        M GetModelWithoutFiringEvents();
        new jQuery jF(string selector);
        new jQuery jF2(string selector);
        new void Instantiate();
        new string GetHtmlRoot();

        new void Open();
        new void Close();

        new bool Get_IsInstantiated();
        new bool Get_IsOpen();
        new jQuery Get_jRoot();
    }
}