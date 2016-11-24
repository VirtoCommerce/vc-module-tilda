using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.TildaModule.Web.Model
{
    /// <summary>
    /// Notification for sync.
    /// </summary>
	public class SyncNotification : PushNotification
    {
        public SyncNotification(string creator)
            : base(creator)
        {
            NotifyType = "TildaSync";
        }
    }
}