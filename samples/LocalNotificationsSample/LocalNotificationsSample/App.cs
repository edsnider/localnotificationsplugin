using System;
using Plugin.LocalNotifications;
using Xamarin.Forms;

namespace LocalNotificationsSample
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            var titleEntry = new Entry
            {
                Text = "Toast Title",
            };
            var bodyEntry = new Entry
            {
                Text = "This is the Toast Body...",
            };
            var sendButton = new Button
            {
                Text = "Send Local Notification",
            };
            sendButton.Clicked += delegate {
                CrossLocalNotifications.Current.Show(titleEntry.Text, bodyEntry.Text);
            };
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(12),
                    Children = {
                        titleEntry,
                        bodyEntry,
                        sendButton
                    }
                }
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
