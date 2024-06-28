using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Lumina.UserControls;
using Lumina.XAMLs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lumina.XAMLs
{
    public partial class Login : Window
    {
        private string _ticket;
        private string _token;

        public Login()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _token = Luminus.Properties.Settings.Default.Token;
            if (!string.IsNullOrEmpty(_token))
            {
                OpenClient(_token);
            }
        }

        private void Log(string message, string token = "")
        {
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"[DEBUG] {message} Token: {token}");
            }
            else
            {
                Console.WriteLine($"[DEBUG] {message}");
            }
        }

        private void OpenClient(string token)
        {
            Client mainForm = new Client(token, this);
            mainForm.Show();
            this.Hide();
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

        private void EmailTextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailTextHint.Visibility = string.IsNullOrEmpty(EmailTextInput.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void PassTextInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PassTextHint.Visibility = string.IsNullOrEmpty(PassTextInput.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void ButtonLogin_MouseEnter(object sender, MouseEventArgs e)
        {
            var brush = (Brush)this.FindResource("InputHighlightBackgroundBrush");
            ButtonLogin.Background = brush;
        }

        private void ButtonLogin_MouseLeave(object sender, MouseEventArgs e)
        {
            var brush = (Brush)this.FindResource("InputBackgroundBrush");
            ButtonLogin.Background = brush;
        }

        private void ExitButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void TopPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void ButtonLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var email = EmailTextInput.Text;
            var password = PassTextInput.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowNotification("Login", "Please enter both email and password.", NotificationType.Warning);
                return;
            }

            await LoginRequest(email, password);
        }

        private async Task LoginRequest(string email, string password)
        {
            using (var httpClient = new HttpClient())
            {
                var loginUrl = "https://discord.com/api/v9/auth/login";
                var payload = new
                {
                    login = email,
                    password = password,
                    undelete = false,
                    login_source = (string)null,
                    gift_code_sku_id = (string)null
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(loginUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    Log($"Login response received: {responseString}");

                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

                    if (jsonResponse.mfa != null && jsonResponse.mfa == true)
                    {
                        _ticket = jsonResponse.ticket.ToString();

                        Storyboard mfaOpenStoryboard = TryFindResource("mfaopen") as Storyboard;
                        mfaOpenStoryboard?.Begin();
                    }
                    else if (jsonResponse.token != null)
                    {
                        string token = jsonResponse.token.ToString();

                        Luminus.Properties.Settings.Default.Token = token;
                        Luminus.Properties.Settings.Default.Save();

                        ShowNotification("Login", "Login was successful! Opening client...", NotificationType.Info);
                        await Task.Delay(500);
                        OpenClient(token);
                    }
                    else
                    {
                        ShowNotification("Login", "Login was unsuccessful.", NotificationType.Error);
                    }
                }
                catch (Exception ex)
                {
                    Log($"Error during login request: {ex.Message}");
                    ShowNotification("Login Error", "An error occurred during login.", NotificationType.Error);
                }
            }
        }

        private void MFATextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            MFATextHint.Visibility = string.IsNullOrEmpty(MFATextInput.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private async void ButtonVerifyMFA_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mfaCode = MFATextInput.Text;

            if (string.IsNullOrEmpty(mfaCode))
            {
                ShowNotification("2FA", "Please enter a code.", NotificationType.Warning);
                return;
            }

            await VerifyMfa(mfaCode);
        }

        private async Task VerifyMfa(string mfaCode)
        {
            using (var httpClient = new HttpClient())
            {
                var mfaUrl = "https://discord.com/api/v9/auth/mfa/totp";
                var payload = new
                {
                    code = mfaCode,
                    ticket = _ticket,
                    login_source = (string)null,
                    gift_code_sku_id = (string)null
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(mfaUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

                    if (jsonResponse.token != null)
                    {
                        string token = jsonResponse.token.ToString();

                        Luminus.Properties.Settings.Default.Token = token;
                        Luminus.Properties.Settings.Default.Save();

                        Log($"Token saved: {token}");

                        ShowNotification("2FA", "Code valid! Opening client...", NotificationType.Info);

                        await Task.Delay(500);

                        OpenClient(token);

                        Storyboard mfaCloseStoryboard = TryFindResource("mfaclose") as Storyboard;
                        mfaCloseStoryboard?.Begin();
                    }
                    else
                    {
                        Log("MFA verification failed");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Log($"HTTP request exception: {ex.Message}");
                    MessageBox.Show($"Error: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    Log($"JSON parsing exception: {ex.Message}");
                    MessageBox.Show($"Error: Failed to parse server response.");
                }
                catch (Exception ex)
                {
                    Log($"Unexpected exception: {ex.Message}");
                    MessageBox.Show($"Error: Unexpected error occurred.");
                }
            }
        }

        private void ButtonVerifyMFA_MouseEnter(object sender, MouseEventArgs e)
        {
            var brush = (Brush)this.FindResource("InputHighlightBackgroundBrush");
            ButtonVerifyMFA.Background = brush;
        }

        private void ButtonVerifyMFA_MouseLeave(object sender, MouseEventArgs e)
        {
            var brush = (Brush)this.FindResource("InputBackgroundBrush");
            ButtonVerifyMFA.Background = brush;
        }
    }
}
