using System;
using Plugin.LocalNotifications.Abstractions;
using Tizen.Applications.Notifications;
using System.Timers;

namespace Plugin.LocalNotifications
{
	/// <summary>
	/// Local Notifications implementation for Tizen
	/// </summary>
	public class LocalNotificationsImplementation : ILocalNotifications
	{
		Timer timer;
		int notificationId = 0;
		/// <summary>
		/// Show a local notification
		/// </summary>
		/// <param name="title">Title of the notification</param>
		/// <param name="body">Body or description of the notification</param>
		/// <param name="id">Id of the notification</param>
		public void Show(string title, string body, int id = 0)
		{
			Notification notification = new Notification
			{
				Title = title,
				Content = body,
				Tag = id.ToString(),
			};
			Notification.IndicatorStyle style = new Notification.IndicatorStyle
			{
				SubText = body
			};
			notification.AddStyle(style);
			NotificationManager.Post(notification);
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
			Notification notification = new Notification
			{
				Title = title,
				Content = body,
				Tag = id.ToString(),
			};
			notificationId = id;
			Notification.IndicatorStyle style = new Notification.IndicatorStyle
			{
				SubText = body
			};
			notification.AddStyle(style);
			NotificationManager.Post(notification);
			
			var time = notifyTime - DateTime.Now;
			int duration = (time.Seconds + time.Milliseconds / 1000);

			timer = new Timer();
			timer.Interval = duration * 1000;
			timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
			timer.Start();
		}

		void TimerElapsed(object sender, ElapsedEventArgs e)
		{
			Cancel(notificationId);
			timer.Stop();
		}

		/// <summary>
		/// Cancel a local notification
		/// </summary>
		/// <param name="id">Id of the notification to cancel</param>
		public void Cancel(int id)
		{
			Notification loadNotification = null;
			try
			{
				loadNotification = NotificationManager.Load(id.ToString());
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
			if (loadNotification != null)
				NotificationManager.Delete(loadNotification);
		}
	}
}
