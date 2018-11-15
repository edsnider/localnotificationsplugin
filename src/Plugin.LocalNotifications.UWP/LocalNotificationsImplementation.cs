using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Plugin.LocalNotifications.Abstractions;
using System;
using System.Linq;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Local Notifications implementation for UWP
    /// </summary>
    public class LocalNotificationsImplementation : ILocalNotifications
    {

        public static string ToastTemplate = "<toast>"
                                            + "<visual>"
                                            + "<binding template='ToastText02'>"
                                            + "<text id='1'>{0}</text>"
                                            + "<text id='2'>{1}</text>"
                                            + "</binding>"
                                            + "</visual>"
                                            + "</toast>";

        private readonly ToastNotifier _manager;

        public LocalNotificationsImplementation()
        {
            // Create a toast notifier and show the toast
            _manager = ToastNotificationManager.CreateToastNotifier();
        }

        /// <summary>
		/// Show a local notification
		/// </summary>
		/// <param name="title">Title of the notification</param>
		/// <param name="body">Body or description of the notification</param>
		/// <param name="id">Id of the notification</param>
        public void Show(string title, string body, int id = 0)
        {
            var xmlData = string.Format(ToastTemplate, title, body);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            // Create a toast
            var toast = new ToastNotification(xmlDoc);

            _manager.Show(toast);
        }

        /// <summary>
		/// Show a local notification at a specified time
		/// </summary>
		/// <param name="title">Title of the notification</param>
		/// <param name="body">Body or description of the notification</param>
		/// <param name="id">Id of the notification</param>
		/// <param name="notifyTime">Time to show notification</param>
        public void Show(string title, string body, int id, DateTime notifyTime)
        {
            var xmlData = string.Format(ToastTemplate, title, body);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            var correctedTime = notifyTime <= DateTime.Now
              ? DateTime.Now.AddMilliseconds(100)
              : notifyTime;

            var scheduledTileNotification = new ScheduledToastNotification(xmlDoc, correctedTime)
            {
                Id = id.ToString()
            };

            _manager.AddToSchedule(scheduledTileNotification);
        }

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            var scheduledNotifications = _manager.GetScheduledToastNotifications();
            var notification =
                scheduledNotifications.FirstOrDefault(n => n.Id.Equals(id.ToString(), StringComparison.OrdinalIgnoreCase));

            if (notification != null)
            {
                _manager.RemoveFromSchedule(notification);
            }
        }
    }
}