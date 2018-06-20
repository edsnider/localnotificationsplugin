using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Plugin.LocalNotifications.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Local Notifications implementation for Android
    /// </summary>
    public class LocalNotificationsImplementation : ILocalNotifications
    {
        /// <summary>
        /// Get or Set Resource Icon to display
        /// </summary>
        public static int NotificationIconId { get; set; }

        /// <summary>
        /// Get or Set id of the default channel for the app
        /// </summary>
        public static string GeneralChannelId { get; set; }

        /// <summary>
        /// Get or Set the user visible name of this channel.
        /// </summary>
        public static string GeneralChannelName { get; set; } = "General";

        /// <summary>
        /// Get or Set this channel's notification importance
        /// </summary>
        public static NotificationImportance GeneralChannelImportance { get; set; } = NotificationImportance.Default;

        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        public void Show(string title, string body, int id = 0)
        {
            NotificationManager notificationManager = Android.App.Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                if (GeneralChannelId == null)
                {
                    throw new NullReferenceException("[LocalNotificationsImplementation] You need to defined GeneralChannelId for Android (Build.VERSION.SdkInt >= BuildVersionCodes.O). " +
                        "Example: LocalNotificationImplementation.GeneralChannelId = \"com.yourapp.general\"");
                }
                else
                {
                    if (!notificationManager.NotificationChannels.Any(x => x.Id == GeneralChannelId))
                    {

                        NotificationChannel generalChannel = new NotificationChannel(GeneralChannelId, GeneralChannelName, GeneralChannelImportance);
                        generalChannel.EnableVibration(false);
                        generalChannel.LockscreenVisibility = NotificationVisibility.Public;
                        notificationManager.CreateNotificationChannel(generalChannel);
                    }
                    Notification.Builder builder = new Notification.Builder(Application.Context, GeneralChannelId)
                        .SetContentTitle(title)
                        .SetContentText(body)
                        .SetAutoCancel(true)
                        .SetOngoing(false);

                    if (NotificationIconId != 0)
                    {
                        builder.SetSmallIcon(NotificationIconId);
                    }
                    else
                    {
                        builder.SetSmallIcon(Resource.Drawable.plugin_lc_smallicon);
                    }
                    var resultIntent = GetLauncherActivity();
                    resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
                    stackBuilder.AddNextIntent(resultIntent);
                    var resultPendingIntent =
                        stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
                    builder.SetContentIntent(resultPendingIntent);
                    notificationManager.Notify(id, builder.Build());
                }
            }
            else
            {
                NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context);
                builder.SetContentTitle(title);
                builder.SetContentText(body);
                builder.SetAutoCancel(true);

                if (NotificationIconId != 0)
                {
                    builder.SetSmallIcon(NotificationIconId);
                }
                else
                {
                    builder.SetSmallIcon(Resource.Drawable.plugin_lc_smallicon);
                }

                var resultIntent = GetLauncherActivity();
                resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
                stackBuilder.AddNextIntent(resultIntent);
                var resultPendingIntent =
                    stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
                builder.SetContentIntent(resultPendingIntent);

                notificationManager.Notify(id, builder.Build());
            }
        }


        public static Intent GetLauncherActivity()
        {
            var packageName = Application.Context.PackageName;
            return Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
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
            var intent = CreateIntent(id);

            var localNotification = new LocalNotification();
            localNotification.Title = title;
            localNotification.Body = body;
            localNotification.Id = id;
            localNotification.NotifyTime = notifyTime;
            if (NotificationIconId != 0)
            {
                localNotification.IconId = NotificationIconId;
            }
            else
            {
                localNotification.IconId = Resource.Drawable.plugin_lc_smallicon;
            }

            var serializedNotification = SerializeNotification(localNotification);
            intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);
            var triggerTime = NotifyTimeInMilliseconds(localNotification.NotifyTime);
            var alarmManager = GetAlarmManager();

            alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            var intent = CreateIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);

            var alarmManager = GetAlarmManager();
            alarmManager.Cancel(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Cancel(id);
        }

        private Intent CreateIntent(int id)
        {
            return new Intent(Application.Context, typeof(ScheduledAlarmHandler))
                .SetAction("LocalNotifierIntent" + id);
        }


        private AlarmManager GetAlarmManager()
        {
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            return alarmManager;
        }

        private string SerializeNotification(LocalNotification notification)
        {
            var xmlSerializer = new XmlSerializer(notification.GetType());
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, notification);
                return stringWriter.ToString();
            }
        }

        private long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;

            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;
            return utcAlarmTimeInMillis;
        }
    }
}