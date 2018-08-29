using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Plugin.LocalNotifications.Abstractions;
using System;
using System.IO;
using System.Xml.Serialization;
using Android.OS;
using Android.App.Job;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Local Notifications implementation for Android
    /// </summary>
    public class LocalNotificationsImplementation : ILocalNotifications
    {
        string _packageName => Application.Context.PackageName;
        NotificationManager _manager => (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

        /// <summary>
        /// Get or Set Resource Icon to display
        /// </summary>
        public static int NotificationIconId { get; set; }

        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        public void Show(string title, string body, int id = 0)
        {
            var builder = new Notification.Builder(Application.Context);
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

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelId = $"{_packageName}.general";
                var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);

                _manager.CreateNotificationChannel(channel);

                builder.SetChannelId(channelId);
            }

            var resultIntent = GetLauncherActivity();
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);
            var resultPendingIntent =
                stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(resultPendingIntent);

            _manager.Notify(id, builder.Build());
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
            

            

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Java.Lang.Class javaClass = Java.Lang.Class.FromType(typeof(ScheduledJobHandler));
                ComponentName component = new ComponentName(Application.Context, javaClass);

                // Bundle up parameters
                var extras = new PersistableBundle();
                extras.PutString(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);
                extras.PutInt(ScheduledJobHandler.LocalNotificationIconId, NotificationIconId);

                var triggerTime = NotifyTimeInMilliseconds(localNotification.NotifyTime) - NotifyTimeInMilliseconds(DateTime.Now);

                JobInfo.Builder builder = new JobInfo.Builder(id, component)
                                                     .SetMinimumLatency(triggerTime)   // Fire at TriggerTime
                                                     .SetOverrideDeadline(triggerTime + 5000) // Or at least 5 Seconds Later
                                                     .SetExtras(extras)
                                                     .SetPersisted(CheckBootPermission()); //Job will be recreated after Reboot if Permissions are granted
                JobInfo jobInfo = builder.Build();

                JobScheduler jobScheduler = GetJobScheduler();

                int result = jobScheduler.Schedule(jobInfo);

                if (result == JobScheduler.ResultSuccess)
                {
                    // The job was scheduled. So nothing more to do
                }
                else
                {
                    // The job wasn´t scheduled. So just use the old implementation?
                    triggerTime = NotifyTimeInMilliseconds(localNotification.NotifyTime);
                    intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

                    var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);

                    var alarmManager = GetAlarmManager();

                    alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
                }
            }
            else
            {
                intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

                var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);
                
                var alarmManager = GetAlarmManager();
                var triggerTime = NotifyTimeInMilliseconds(localNotification.NotifyTime);
                alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
            }
        }

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                JobScheduler jobScheduler = GetJobScheduler();
                jobScheduler.Cancel(id);
            }
            else
            {
                var intent = CreateIntent(id);
                var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, intent, PendingIntentFlags.CancelCurrent);

                var alarmManager = GetAlarmManager();
                alarmManager.Cancel(pendingIntent);

                var notificationManager = NotificationManagerCompat.From(Application.Context);
                notificationManager.Cancel(id);
            } 
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

        private JobScheduler GetJobScheduler()
        {
            var jobScheduler = Application.Context.GetSystemService(Context.JobSchedulerService) as JobScheduler;
            return jobScheduler;
        }

        private bool CheckBootPermission()
        {
            return Application.Context.CheckSelfPermission("android.permission.RECEIVE_BOOT_COMPLETED") == Android.Content.PM.Permission.Granted;
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
