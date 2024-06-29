using Luminus.UserControls;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Luminus.XAMLs
{
    public partial class Client : Window
    {
        private string AccessToken;
        private Login signin;
        private string userPFP;
        private const string DiscordApiBaseUrl = "https://discord.com/api/v9/";
        private List<FriendListItem> allFriends;

        public Client(string token, Login signinArg)
        {
            InitializeComponent();

            signin = signinArg;
            AccessToken = token;
            SetUserInfo();
            PopulateFriendsTab();
        }

        private void PopulateFriendsTab()
        {
            try
            {
                dynamic channels = GetApiResponse("users/@me/channels");
                dynamic relationships = GetApiResponse("users/@me/relationships");
                HashSet<long> blockedUsers = new HashSet<long>();

                foreach (var relationship in relationships)
                {
                    if ((int)relationship.type == 2)
                    {
                        blockedUsers.Add((long)relationship.id);
                    }
                }

                allFriends = new List<FriendListItem>();
                HashSet<long> channelIds = new HashSet<long>();

                foreach (var channel in channels)
                {
                    long channelId = (long)channel.id;
                    if (channelIds.Contains(channelId))
                    {
                        continue;
                    }

                    string channelType = "";
                    string namesOrName = "";
                    string avatarUrl = "";
                    string userId = "";

                    switch ((int)channel.type)
                    {
                        case 1:
                            if (channel.recipients != null && channel.recipients.Count > 0)
                            {
                                List<string> names = new List<string>();
                                foreach (var recipient in channel.recipients)
                                {
                                    string recipientName = (string)recipient.nickname ?? (string)recipient.global_name ?? (string)recipient.username;
                                    names.Add(recipientName);
                                    userId = (string)recipient.id;
                                    avatarUrl = (string)recipient.avatar;
                                }
                                namesOrName = string.Join(", ", names);
                                channelType = "Direct Message";

                                if (blockedUsers.Contains((long)channel.recipients[0].id))
                                {
                                    namesOrName += " - Blocked";
                                }

                                if (!string.IsNullOrEmpty(avatarUrl))
                                {
                                    avatarUrl = $"https://cdn.discordapp.com/avatars/{userId}/{avatarUrl}.png";
                                }
                            }
                            else
                            {
                                namesOrName = "Unknown User";
                                channelType = "Direct Message";
                            }
                            break;

                        case 3:
                            if (channel.name != null)
                            {
                                namesOrName = (string)channel.name;
                                channelType = "Group Message";
                            }
                            else if (channel.recipients != null && channel.recipients.Count > 0)
                            {
                                List<string> names = new List<string>();
                                foreach (var recipient in channel.recipients)
                                {
                                    string recipientName = (string)recipient.nickname ?? (string)recipient.global_name ?? (string)recipient.username;
                                    names.Add(recipientName);
                                    userId = (string)recipient.id;
                                    avatarUrl = (string)recipient.avatar;
                                }
                                namesOrName = string.Join(", ", names);
                                channelType = "Group Message";

                                if (!string.IsNullOrEmpty(avatarUrl))
                                {
                                    avatarUrl = $"https://cdn.discordapp.com/avatars/{userId}/{avatarUrl}.png";
                                }
                            }

                            namesOrName += $" - {channel.recipients.Count} members";
                            break;
                    }

                    FriendListItem friendListItem = new FriendListItem();
                    friendListItem.SetFriendDetails(namesOrName, avatarUrl);
                    allFriends.Add(friendListItem);
                    channelIds.Add(channelId);
                }

                friendsList.Items.Clear();
                foreach (var friend in allFriends)
                {
                    friendsList.Items.Add(friend);
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Failed to retrieve channel list: " + ex.Message);
            }
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
