using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.PageParts.SendFeedback.Objs;

namespace ASPdatabaseNET.UI.PageParts.SendFeedback.Backend
{
    public class FeedbackLogic
    {
        //----------------------------------------------------------------------------------------------------
        public static FeedbackInfo GetInfo()
        {
            var userSessionInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser();
            string name = (userSessionInfo.UserInfo.FirstName + " " + userSessionInfo.UserInfo.LastName).Trim();
            string email = userSessionInfo.UserInfo.Email;

            return new FeedbackInfo()
            {
                Anonymous = false,
                AppVersion = Config.SystemProperties.Version,
                Name = name,
                Email = email,
                RequestFollowup = false,
                Message = ""
            };
        }

        //----------------------------------------------------------------------------------------------------
        public static string Send(FeedbackInfo feedbackInfo)
        {
            var siteId = ASPdatabaseNET.Subscription.Objs.SiteIdObj.GetNew();
            if(feedbackInfo.Anonymous)
            {
                feedbackInfo.AppVersion = "";
                feedbackInfo.Name = "";
                feedbackInfo.Email = "";
                feedbackInfo.RequestFollowup = false;
            }

            return (new ASPdatabaseNET.AjaxService.ASPdatabaseService())
                .SubscriptionService__SendFeedback(siteId, feedbackInfo.Anonymous, feedbackInfo.AppVersion, feedbackInfo.Name, feedbackInfo.Email, feedbackInfo.RequestFollowup, feedbackInfo.Message);
        }
    }
}