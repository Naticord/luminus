using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Luminus.UserControls
{
    /// <summary>
    /// Interaction logic for ChangeTabRadioControl.xaml
    /// </summary>
    public partial class ChangeTabRadioControl : UserControl
    {

        public ChangeTabRadioControl()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(ChangeTabRadioControl), new PropertyMetadata(false, OnIsCheckedChanged));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(ChangeTabRadioControl), new PropertyMetadata("FUCK"));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        private void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsChecked = true;
        }

        public static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ChangeTabRadioControl;
            control.UpdateInactive();
            if (control.IsChecked)
            {
                var parent = VisualTreeHelper.GetParent(control) as Panel;
                if (parent != null)
                {
                    foreach (var child in parent.Children)
                    {
                        if (child is ChangeTabRadioControl radioButton && radioButton != control)
                        {
                            radioButton.IsChecked = false;
                            control.UpdateActive();
                        }
                    }
                }
            }
        }

        public void UpdateActive()
        {
            var brush = (Brush)this.FindResource("ForegroundLuminaBrush");
            label.Foreground = brush;

            DoubleAnimation fontSizeAnimation = new DoubleAnimation();
            fontSizeAnimation.To = 14;
            fontSizeAnimation.Duration = TimeSpan.FromMilliseconds(400);
            fontSizeAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            label.BeginAnimation(TextBlock.FontSizeProperty, fontSizeAnimation);

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 1;
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(400);
            opacityAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            ind.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }

        public void UpdateInactive()
        {
            var brush = (Brush)this.FindResource("ForegroundBrush");
            label.Foreground = brush;

            DoubleAnimation fontSizeAnimation = new DoubleAnimation();
            fontSizeAnimation.To = 11;
            fontSizeAnimation.Duration = TimeSpan.FromMilliseconds(400);
            fontSizeAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            label.BeginAnimation(TextBlock.FontSizeProperty, fontSizeAnimation);

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0;
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(400);
            opacityAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            ind.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateInactive();

            if (IsChecked)
            {
                UpdateActive();
            }
        }
    }
}
