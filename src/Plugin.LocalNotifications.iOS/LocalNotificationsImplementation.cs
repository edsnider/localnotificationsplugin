using System;
using Plugin.LocalNotifications.Abstractions;
using System.Linq;
using Foundation;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Local Notifications implementation for iOS
    /// </summary>
    public class LocalNotificationsImplementation : ILocalNotifications
    {
        private const string NotificationKey = "LocalNotificationKey";

        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        public void Show(string title, string body, int id = 0)
        {
            Show(title, body, null, id);
        }

        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="customData">Custom data to attach to notification</param>
        /// <param name="id">Id of the notification</param>
        public void Show(string title, string body, string customData, int id = 0)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(.1, false);
                ShowUserNotification(title, body, id, trigger, customData);
            }
            else
            {
                Show(title, body, id, DateTime.Now, customData);
            }
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
            Show(title, body, id, notifyTime, null);
        }

        /// <summary>
        /// Show a local notification at a specified time
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="notifyTime">Time to show notification</param>
        /// <param name="customData">Custom data attached to notification</param>
        public void Show(string title, string body, int id, DateTime notifyTime, string customData)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var trigger = UNCalendarNotificationTrigger.CreateTrigger(GetNSDateComponentsFromDateTime(notifyTime), false);
                ShowUserNotification(title, body, id, trigger, customData);
            }
            else
            {
                NSDictionary userInfo;
                if (!string.IsNullOrWhiteSpace(customData))
                {
                    userInfo = NSDictionary.FromObjectsAndKeys(
                        new NSObject[]
                        {
                            NSObject.FromObject(id),
                            NSObject.FromObject(customData),
                        },
                        new NSObject[]
                        {
                            NSObject.FromObject(NotificationKey),
                            NSObject.FromObject(CrossLocalNotifications.LocalNotificationCustomData),
                        });
                }
                else userInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey));
                var notification = new UILocalNotification
                {
                    FireDate = (NSDate)notifyTime,
                    AlertTitle = title,
                    AlertBody = body,
                    UserInfo = userInfo,
                    SoundName = UILocalNotification.DefaultSoundName
                };

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }
        }

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new string[] { id.ToString() });
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new string[] { id.ToString() });
            }
            else
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

        /// <summary>
        /// Show local notifications using the UNUserNotificationCenter using a notification trigger (iOS 10+ only)
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notificatio</param>
        /// <param name="trigger">Trigger firing notification</param>
        /// <param name="customData">Custom data attached to notification</param>
        private void ShowUserNotification(string title, string body, int id, UNNotificationTrigger trigger, string customData)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                return;

            NSDictionary userInfo;

            if (!string.IsNullOrWhiteSpace(customData))
            {
                userInfo = NSDictionary.FromObjectsAndKeys(
                    new NSObject[]
                    {
                            NSObject.FromObject(id),
                            NSObject.FromObject(customData),
                    },
                    new NSObject[]
                    {
                            NSObject.FromObject(NotificationKey),
                            NSObject.FromObject(CrossLocalNotifications.LocalNotificationCustomData),
                    });
            }
            else userInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(id), NSObject.FromObject(NotificationKey));

            var content = new UNMutableNotificationContent()
            {
                Title = title,
                Body = body,
                UserInfo = userInfo,
                Sound = UNNotificationSound.Default
            };
            
            var request = UNNotificationRequest.FromIdentifier(id.ToString(), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) => { });
        }

        NSDateComponents GetNSDateComponentsFromDateTime(DateTime dateTime)
        {
            return new NSDateComponents
            {
                Month = dateTime.Month,
                Day = dateTime.Day,
                Year = dateTime.Year,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
        }
    }
}
