using System;
using System.Linq;
using Foundation;
using Plugin.LocalNotifications.Abstractions;

namespace Plugin.LocalNotifications
{
	/// <summary>
	/// Local Notifications implementation for macOS
	/// </summary>
	public class LocalNotificationsImplementation : ILocalNotifications
	{
		/// <summary>
		/// Show a local notification
		/// </summary>
		/// <param name="title">Title of the notification</param>
		/// <param name="body">Body or description of the notification</param>
		/// <param name="id">Id of the notification</param>
		public void Show(string title, string body, int id = 0)
		{
			Show(title, body, id, DateTime.Now);
		}

        public void Show(string title, string body, int id = 0, string backgroundColor = null, string smallIcon = null, string largeIcon = null)
        {
            Show(title, body, id, DateTime.Now);
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
            var notification = new NSUserNotification()
            {
                Title = title,
                InformativeText = body,
                Identifier = id.ToString(),
                DeliveryDate = (NSDate)notifyTime
            };

            NSUserNotificationCenter.DefaultUserNotificationCenter.ScheduleNotification(notification);
		}

        public void Show(string title, string body, int id, DateTime notifyTime, string backgroundColor = null, string smallIcon = null, string largeIcon = null)
        {
            Show(title, body, id, notifyTime);
        }

		/// <summary>
		/// Cancel a local notification
		/// </summary>
		/// <param name="id">Id of the notification to cancel</param>
		public void Cancel(int id)
		{
			var scheduled = NSUserNotificationCenter.DefaultUserNotificationCenter.ScheduledNotifications.FirstOrDefault(x => x.Identifier == id.ToString());
			var delivered = NSUserNotificationCenter.DefaultUserNotificationCenter.DeliveredNotifications.FirstOrDefault(x => x.Identifier == id.ToString());

			if (scheduled != null)
			{
				NSUserNotificationCenter.DefaultUserNotificationCenter.RemoveScheduledNotification(scheduled);
			}

			if (delivered != null)
			{
				NSUserNotificationCenter.DefaultUserNotificationCenter.RemoveDeliveredNotification(delivered);
			}
		}
	}
}
