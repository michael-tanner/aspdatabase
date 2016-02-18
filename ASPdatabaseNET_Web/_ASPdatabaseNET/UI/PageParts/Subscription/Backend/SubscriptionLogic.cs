using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.PageParts.Subscription.Objs;

namespace ASPdatabaseNET.UI.PageParts.Subscription.Backend
{
    //----------------------------------------------------------------------------------------------------////
    public class SubscriptionLogic
    {
        //----------------------------------------------------------------------------------------------------
        public static SubscriptionInfo GetInfo()
        {
            return GetInfo("Check");
        }
        //----------------------------------------------------------------------------------------------------
        public static SubscriptionInfo GetInfo(string actionType)
        {
            var rtn = new SubscriptionInfo();

            rtn.SubscriptionKey = Config.ASPdb_Values.GetFirstValue("SubscriptionKey", "");
            if (rtn.SubscriptionKey.Length < 1)
                return rtn;

            rtn.SubscriptionCount = ASPdatabaseNET.Subscription.SubscriptionAppState.GetSubscribersCount(actionType);
            rtn.LastCheck_MinutesLapsed = ASPdatabaseNET.Subscription.SubscriptionAppState.Get_LastCheck_MinutesLapsed();


            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static SubscriptionInfo SetKey(string subscriptionKey)
        {
            Config.ASPdb_Values.Set("SubscriptionKey", subscriptionKey);
            ASPdatabaseNET.Subscription.SubscriptionAppState.ClearAppState();
            var rtn = GetInfo("Add");

            if (!ASPdatabaseNET.Subscription.SubscriptionAppState.ValidateActiveSubscribers())
                throw new Exception("Validation Error");

            Memory.AppCache.Reset();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static SubscriptionInfo RemoveKey()
        {
            var siteId = ASPdatabaseNET.Subscription.Objs.SiteIdObj.GetNew();
            string subscriptionKey = Config.ASPdb_Values.GetFirstValue("SubscriptionKey", "");
            (new AjaxService.ASPdatabaseService()).SubscriptionService__CheckSubscription(siteId, subscriptionKey, "Remove");
            Config.ASPdb_Values.Remove("SubscriptionKey");

            ASPdatabaseNET.Subscription.SubscriptionAppState.ClearAppState();
            var rtn = GetInfo("Remove");

            if (!ASPdatabaseNET.Subscription.SubscriptionAppState.ValidateActiveSubscribers())
                throw new Exception("Validation Error");

            Memory.AppCache.Reset();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static SubscriptionInfo Refresh()
        {
            ASPdatabaseNET.Subscription.SubscriptionAppState.ClearAppState();
            return GetInfo("Check");
        }
    }
}