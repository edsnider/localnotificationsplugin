using System.IO;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.Support.V4.App;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Broadcast receiver
    /// </summary>
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Plugin Broadcast Receiver")]
    public class ScheduledAlarmHandler : BroadcastReceiver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            var extra = intent.GetStringExtra(LocalNotificationsImplementation.LocalNotificationKey);
            var notification = DeserializeNotification(extra);

            var builder = new NotificationCompat.Builder(Application.Context)
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Body)
                .SetSmallIcon(notification.IconId)
                .SetAutoCancel(true);

            var resultIntent = string.IsNullOrEmpty(LocalNotificationsImplementation.LocalNotificationIntentAction) ?
                                     LocalNotificationsImplementation.GetLauncherActivity()
                                     :
                                     new Intent(LocalNotificationsImplementation.LocalNotificationIntentAction).AddFlags(ActivityFlags.NewTask);

            resultIntent.PutExtra(LocalNotificationsImplementation.LocalNotificationIntentKey, notification.Id);

            var resultPendingIntent = PendingIntent.GetActivity(Application.Context, 0, resultIntent, 0);
            builder.SetContentIntent(resultPendingIntent);

            var notificationManager = NotificationManagerCompat.From(Application.Context);

            notificationManager.Notify(notification.Id, builder.Build());
        }

        private LocalNotification DeserializeNotification(string notificationString)
        {
            var xmlSerializer = new XmlSerializer(typeof(LocalNotification));

            using (var stringReader = new StringReader(notificationString))
            {
                return (LocalNotification)xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}