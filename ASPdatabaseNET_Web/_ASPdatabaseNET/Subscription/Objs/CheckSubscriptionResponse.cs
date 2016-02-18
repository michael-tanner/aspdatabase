using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.Subscription.Objs
{
    public class CheckSubscriptionResponse
    {
        public int SubscriptionCount = 0;
        public string SubscriptionCode;
        public SubscriptionsHistoryItem[] HistoryItems;
        public string HistoryCode;
    }
}