using System;

namespace Plugin.LocalNotifications.Abstractions
{
    /// <summary>
    /// Local Notification Interface
    /// </summary>
    public interface ILocalNotifications
    {
        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        void Show(string title, string body, int id = 0);

        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="backgroundColor">Small icon background color (works only in Android)</param>
        /// <param name="smallIcon">Small icon asset name (works only in Android)</param>
        /// <param name="largeIcon">Large icon asset name (works only in Android)</param>
        void Show(string title, string body, int id = 0, string backgroundColor = null, string smallIcon = null, string largeIcon = null);

        /// <summary>
        /// Show a local notification at a specified time
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="notifyTime">Time to show notification</param>
        void Show(string title, string body, int id, DateTime notifyTime);

        /// <summary>
        /// Show a local notification at a specified time
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="notifyTime">Time to show notification</param>
        /// <param name="backgroundColor">Small icon background color (works only in Android)</param>
        /// <param name="smallIcon">Small icon asset name (works only in Android)</param>
        /// <param name="largeIcon">Large icon asset name (works only in Android)</param>
        void Show(string title, string body, int id, DateTime notifyTime, string backgroundColor = null, string smallIcon = null, string largeIcon = null);

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        void Cancel(int id);
    }
}
