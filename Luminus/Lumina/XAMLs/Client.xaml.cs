using Lumina.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lumina.XAMLs
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        private string AccessToken;
        private Login signin;
        private string userPFP;
        private const string DiscordApiBaseUrl = "https://discord.com/api/v9/";


        public Client(string token, Login signinArg)
        {
            InitializeComponent();

            signin = signinArg;
            AccessToken = token;
            SetUserInfo();
        }

        private dynamic GetApiResponse(string endpoint)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.Authorization] = AccessToken;
                string jsonResponse = webClient.DownloadString(DiscordApiBaseUrl + endpoint);
                return Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
            }
        }

        public void ShowNotification(string name, string message, NotificationType type)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = new NotificationControl(name, message, type);
                notification.Dismissed += Notification_Dismissed;

                NotificationHost.Items.Add(notification);

                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
                timer.Tick += (sender, args) =>
                {
                    NotificationHost.Items.Remove(notification);
                    timer.Stop();
                };
                timer.Start();
            });
        }

        public void Notification_Dismissed(object sender, EventArgs e)
        {
            if (sender is NotificationControl notification)
            {
                NotificationHost.Items.Remove(notification);
            }
        }

        private void SetUserInfo()
        {
            try
            {
                dynamic userProfile = GetApiResponse("users/@me");
                string displayname = userProfile.global_name;
                if (userProfile.global_name != null) { displayname = userProfile.global_name; } else { displayname = userProfile.username; }
                string bio = userProfile.bio;
                usernameLabel.Text = displayname;
                //descriptionLabel.Content = bio;

                string userPFP = $"https://cdn.discordapp.com/avatars/{userProfile.id}/{userProfile.avatar}.png";

                BitmapImage bitmap = new BitmapImage(new Uri(userPFP));
                profilepicture.Source = bitmap;
            }
            catch (WebException ex)
            {

                ShowNotification("Profile", "Failed to retrieve user profile. " + ex.ToString(), NotificationType.Error);
            }
        }

        private void TopPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MaximizeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (base.WindowState == WindowState.Maximized)
            {
                base.WindowState = WindowState.Normal;
                base.BorderThickness = new Thickness(0);
            }
            else if (WindowState == WindowState.Normal)
            {
                base.BorderThickness = new Thickness(5);
                base.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
