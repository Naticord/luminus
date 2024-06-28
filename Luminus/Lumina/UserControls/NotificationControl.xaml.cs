using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lumina.UserControls
{
    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }

    public partial class NotificationControl : UserControl
    {
        public event EventHandler Dismissed;

        public NotificationControl(string name, string message, NotificationType type)
        {
            InitializeComponent();
            NameTextBlock.Text = name;
            MessageTextBlock.Text = message;
            SetNotificationType(type);
        }

        private void SetNotificationType(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info:
                    NotiIcon.Data = Geometry.Parse("M22 12C22 17.5228 17.5228 22 12 22C6.47715 22 2 17.5228 2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12ZM12 17.75C12.4142 17.75 12.75 17.4142 12.75 17V11C12.75 10.5858 12.4142 10.25 12 10.25C11.5858 10.25 11.25 10.5858 11.25 11V17C11.25 17.4142 11.5858 17.75 12 17.75ZM12 7C12.5523 7 13 7.44772 13 8C13 8.55228 12.5523 9 12 9C11.4477 9 11 8.55228 11 8C11 7.44772 11.4477 7 12 7Z");
                    break;
                case NotificationType.Warning:
                    NotiIcon.Data = Geometry.Parse("M30.555 25.219l-12.519-21.436c-1.044-1.044-2.738-1.044-3.782 0l-12.52 21.436c-1.044 1.043-1.044 2.736 0 3.781h28.82c1.046-1.045 1.046-2.738 0.001-3.781zM14.992 11.478c0-0.829 0.672-1.5 1.5-1.5s1.5 0.671 1.5 1.5v7c0 0.828-0.672 1.5-1.5 1.5s-1.5-0.672-1.5-1.5v-7zM16.501 24.986c-0.828 0-1.5-0.67-1.5-1.5 0-0.828 0.672-1.5 1.5-1.5s1.5 0.672 1.5 1.5c0 0.83-0.672 1.5-1.5 1.5z");
                    break;
                case NotificationType.Error:
                    NotiIcon.Data = Geometry.Parse("M1.25 8C1.25 4.27208 4.27208 1.25 8 1.25L16 1.25C19.7279 1.25 22.75 4.27208 22.75 8L22.75 16C22.75 19.7279 19.7279 22.75 16 22.75L8 22.75C4.27208 22.75 1.25 19.7279 1.25 16L1.25 8ZM8.46967 8.46966C8.76257 8.17676 9.23744 8.17677 9.53033 8.46966L12 10.9393L14.4697 8.46966C14.7626 8.17676 15.2374 8.17677 15.5303 8.46966C15.8232 8.76256 15.8232 9.23743 15.5303 9.53032L13.0606 12L15.5303 14.4697C15.8232 14.7626 15.8232 15.2374 15.5303 15.5303C15.2374 15.8232 14.7625 15.8232 14.4696 15.5303L12 13.0606L9.53033 15.5303C9.23743 15.8232 8.76256 15.8232 8.46967 15.5303C8.17678 15.2374 8.17678 14.7625 8.46967 14.4696L10.9393 12L8.46967 9.53032C8.17678 9.23742 8.17678 8.76255 8.46967 8.46966Z");
                    break;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonDismiss_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var brush = (Brush)this.FindResource("InputHighlightBackgroundBrush");
            ButtonDismiss.Background = brush;
        }

        private void ButtonDismiss_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var brush = (Brush)this.FindResource("InputBackgroundBrush");
            ButtonDismiss.Background = brush;
        }
    }
}
