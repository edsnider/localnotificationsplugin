using System;
using Plugin.LocalNotifications.Abstractions;
using System.Linq;
using Foundation;
using UIKit;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Notifier implementation for iOS (iPad and iPhone)
    /// </summary>
    public class LocalNotificationsImplementation : ILocalNotifications
    {
        private const string NotificationKey = "LocalNotificationKey";

        /// <summary>
        /// Show a local notification in the Notification Center.
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notificsation</param>
        /// <param name="id">Id of notifications</param>
        public void Show(string title, string body, int id = 0)
        {
            var notification = new UILocalNotification
            {
                FireDate = (NSDate)DateTime.Now,
                AlertAction = title,
                AlertBody = body,
                UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey))
            };

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        /// <summary>
        /// Schedule a local notification in the Notification Center.
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="notifyTime">The time you would like to schedule the notification for</param>
        public void Show(string title, string body, int id, DateTime notifyTime)
        {
            var notification = new UILocalNotification
            {
                FireDate = (NSDate)notifyTime,
                AlertAction = title,
                AlertBody = body,
                UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey))
            };

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        /// <summary>
        /// Show a local toast notification.  Notification will also appear in the Notification Center on Windows Phone 8.1.
        /// </summary>
        /// <param name="id">Id of the scheduled notification you'd like to cancel</param>
        public void Cancel(int id)
        {
            var notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
            var notification = notifications.Where(n => n.UserInfo.ContainsKey(NSObject.FromObject(NotificationKey)))
                .FirstOrDefault(n => n.UserInfo[NotificationKey].Equals(NSObject.FromObject(id)));

            if (notification != null)
            {
                UIApplication.SharedApplication.CancelLocalNotification(notification);
            }
        }
    }
}
