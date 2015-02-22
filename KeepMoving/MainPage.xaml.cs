using System;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using KeepMoving.Background;
using KeepMoving.Framework;
using Mindscape.Raygun4Net;

namespace KeepMoving
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var enable = await Sensor.CheckSensorCoreSupport();
                if (enable)
                {
                    BackgroundReadTask.Register();
                }
            }
            catch(Exception ex)
            {
                RaygunClient.Current.Send(ex);
            }
            NotificationsEnabledSwitch.IsOn = Settings.GetTrackingEnabled();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void NotifcationsEnabledCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            //if (!NotifcationsEnabledCheckBox.IsChecked.HasValue)
            //{
            //    //This is only to make intellisense happy
            //    return;
            //}

            //App.NotificationsEnabled = NotifcationsEnabledCheckBox.IsChecked.Value;
        }

        private void NotifcationsEnabledSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            Settings.SetTrackingEnabled(NotificationsEnabledSwitch.IsOn);
        }

        private async void CheckActivityButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Sensor.DoStuff();
        }

        private async void FeedbackButton_OnClick(object sender, RoutedEventArgs e)
        {
            var em = new EmailMessage();
            em.To.Add(new EmailRecipient("jason@ytechie.com", "Jason Young"));
            em.Subject = "Keep Moving Feedback";

            await EmailManager.ShowComposeNewEmailAsync(em);
        }

        private void SettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
    }
}
