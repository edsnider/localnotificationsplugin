using System;
using System.Threading;
using Plugin.LocalNotifications.Abstractions;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Access Cross Local Notifictions
    /// </summary>
    public static class CrossLocalNotifications
    {
        private static Lazy<ILocalNotifications> _impl = new Lazy<ILocalNotifications>(CreateLocalNotificationsImplementation, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Get the key to retrieve the custom data attached to notifications.
        /// </summary>
        public const string LocalNotificationCustomData = "LocalNotificationCustomData";
        
        /// <summary>
        /// Gets the current platform specific ILocalNotifications implementation.
        /// </summary>
        public static ILocalNotifications Current
        {
            get
            {
                var val = _impl.Value;
                if (val == null)
                    throw NotImplementedInReferenceAssembly();
                return val;
            }
        }

        private static ILocalNotifications CreateLocalNotificationsImplementation()
        {
#if PCL
            return null;
#else
            return new LocalNotificationsImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException();
        }
    }
}
