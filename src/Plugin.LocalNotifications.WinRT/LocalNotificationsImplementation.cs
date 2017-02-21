using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Plugin.LocalNotifications.Abstractions;
using System;
using System.Linq;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Notifier implementation for Windows
    /// </summary>
    public class LocalNotificationsImplementation : ILocalNotifications
    {
        private const string _TOAST_TEXT02_TEMPLATE = "<toast>"
                                                    + "<visual>"
                                                    + "<binding template='ToastText02'>"
                                                    + "<text id='1'>{0}</text>"
                                                    + "<text id='2'>{1}</text>"
                                                    + "</binding>"
                                                    + "</visual>"
                                                    + "</toast>";

        /// <summary>
        /// Show a local toast notification.  Notification will also appear in the Notification Center on Windows Phone 8.1.
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of notifications</param>
        public void Show(string title, string body, int id = 0)
        {
            var xmlData = string.Format(_TOAST_TEXT02_TEMPLATE, title, body);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            // Create a toast
            var toast = new ToastNotification(xmlDoc);
            

            // Create a toast notifier and show the toast
            var manager = ToastNotificationManager.CreateToastNotifier();
           
            manager.Show(toast);
        }

        /// <summary>
        /// Schedule a local toast notification.  Notification will also appear in the Notification Center on Windows Phone 8.1.
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="notifyTime">Time to show notification</param>
        public void Show(string title, string body, int id, DateTime notifyTime)
        {
            var xmlData = string.Format(_TOAST_TEXT02_TEMPLATE, title, body);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            var correctedTime = notifyTime <= DateTime.Now
              ? DateTime.Now.AddMilliseconds(100)
              : notifyTime;

            var scheduledTileNotification = new ScheduledTileNotification(xmlDoc, correctedTime)
            {
                Id = id.ToString()
            };

            TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(scheduledTileNotification);
        }

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the scheduled notification you'd like to cancel</param>
        public void Cancel(int id)
        {
            var scheduledNotifications = TileUpdateManager.CreateTileUpdaterForApplication().GetScheduledTileNotifications();
            var notification =
                scheduledNotifications.FirstOrDefault(n => n.Id.Equals(id.ToString(), StringComparison.OrdinalIgnoreCase));

            if (notification != null)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().RemoveFromSchedule(notification);
            }
        }
    }
}
