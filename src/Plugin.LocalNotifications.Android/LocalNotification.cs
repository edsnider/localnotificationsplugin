using System;

namespace Plugin.LocalNotifications
{
    /// <summary>
    /// Define the local notification parameters.
    /// </summary>
    public class LocalNotification
    {
        /// <summary>
        /// Gets or sets the notification title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the notification body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the notification identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the notification icon identifier.
        /// </summary>
        public int IconId { get; set; }

        /// <summary>
        /// Gets or sets the notification firing time.
        /// </summary>
        public DateTime NotifyTime { get; set; }
        
        /// <summary>
        /// Gets or sets the custom data attached to notification.
        /// </summary>
        public string CustomData { get; set; }
    }
}