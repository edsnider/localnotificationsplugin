using System.IO;
using System.Xml.Serialization;

using Android.App;
using Android.App.Job;

namespace Plugin.LocalNotifications
{
    [Service(Name = "plugin.localnotifications.ScheduledJobHandler", Permission = "android.permission.BIND_JOB_SERVICE")]
    public class ScheduledJobHandler : JobService
    {
        /// <summary>
        /// 
        /// </summary>
        public const string LocalNotificationIconId = "LocalNotificationIconId";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobParams"></param>
        /// 
        public override bool OnStartJob(JobParameters jobParams)
        {
            var extra = jobParams.Extras.GetString(ScheduledAlarmHandler.LocalNotificationKey);
            var notificationIconId = jobParams.Extras.GetInt(LocalNotificationIconId);
            var notification = DeserializeNotification(extra);

            LocalNotificationsImplementation.NotificationIconId = notificationIconId;
            CrossLocalNotifications.Current.Show(notification.Title, notification.Body, notification.Id);
            return true;
        }

        public override bool OnStopJob(JobParameters jobParams)
        {
            // Called by Android when it has to terminate a running service.
            return false; // Don't reschedule the job.
        }

        private LocalNotification DeserializeNotification(string notificationString)
        {
            var xmlSerializer = new XmlSerializer(typeof(LocalNotification));
            using (var stringReader = new StringReader(notificationString))
            {
                var notification = (LocalNotification)xmlSerializer.Deserialize(stringReader);
                return notification;
            }
        } 
    }
}
