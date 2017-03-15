using System;
using Plugin.LocalNotifications;
using Xamarin.Forms;

namespace LocalNotificationsSample
{
	public partial class MainPage : ContentPage
	{
		const int _SAMPLE_ID = 1;
		int _secondsToDelivery;

		public MainPage()
		{
			InitializeComponent();
		}

		void ScheduledSwitchToggled(object sender, ToggledEventArgs e)
		{
			ScheduleSecondsPicker.IsVisible = ScheduleSwitch.IsToggled;

			if (!ScheduleSwitch.IsToggled)
			{
				_secondsToDelivery = 0;
			}
		}

		void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			_secondsToDelivery = int.Parse(ScheduleSecondsPicker.Items[ScheduleSecondsPicker.SelectedIndex]);
		}

		void SendButtonClicked(object sender, EventArgs e)
		{
			if (_secondsToDelivery > 0)
			{
				CrossLocalNotifications.Current.Show(TitleEntry.Text, BodyEntry.Text, _SAMPLE_ID, DateTime.Now.AddSeconds(_secondsToDelivery));
			}
			else
			{
				CrossLocalNotifications.Current.Show(TitleEntry.Text, BodyEntry.Text, _SAMPLE_ID);
			}
		}

		void CancelButtonClicked(object sender, EventArgs e)
		{
			CrossLocalNotifications.Current.Cancel(_SAMPLE_ID);
		}
	}
}
