using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.Subscription.Objs
{
    public class SubscriptionsHistoryItem
    {
        public int Index;
        public DateTime? StartTime;
        public DateTime? EndTime;
        public int SubscriptionCount;
    }
}