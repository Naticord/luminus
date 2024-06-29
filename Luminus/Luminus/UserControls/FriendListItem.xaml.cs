using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Luminus.UserControls
{
    public partial class FriendListItem : UserControl
    {
        public FriendListItem()
        {
            InitializeComponent();
        }

        public void SetFriendDetails(string name, string avatarUrl)
        {
            usernameLabel.Content = name;
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(avatarUrl));
                    usernameAvatar.ImageSource = bitmap;
                }
                catch (UriFormatException ex)
                {
                    Console.WriteLine($"Invalid avatar URL: {avatarUrl}. Exception: {ex.Message}");
                    usernameAvatar.ImageSource = null;
                }
            }
            else
            {
                usernameAvatar.ImageSource = null;
            }
        }
    }
}
