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
        private static Lazy<ILocalNotifications> _notifier = new Lazy<ILocalNotifications>(CreateNotifier, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the current platform specific ILocalNotifications implementation.
        /// </summary>
        public static ILocalNotifications Current
        {
            get
            {
                var val = _notifier.Value;
                if (val == null)
                    throw NotImplementedInReferenceAssembly();
                return val;
            }
        }

        private static ILocalNotifications CreateNotifier()
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
