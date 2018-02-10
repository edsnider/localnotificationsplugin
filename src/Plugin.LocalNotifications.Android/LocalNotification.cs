using System;
using Android.Graphics;

namespace Plugin.LocalNotifications
{
    public class LocalNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int Id { get; set; }
        public int IconId { get; set; }
        public DateTime NotifyTime { get; set; }
        public Color? BackgroundColor { get; set; }
        public int SmallIconId { get; set; }
        public int LargeIconId { get; set; }
    }
}