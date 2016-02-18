using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.UI.PageParts.SendFeedback.Objs
{
    public class FeedbackInfo
    {
        public bool Anonymous = false;
        public string AppVersion;
        public string Name;
        public string Email;
        public bool RequestFollowup = false;
        public string Message;
    }
}