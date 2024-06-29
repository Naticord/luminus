using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            UpdateInactive();
        }

        public static readonly DependencyProperty IsCheckedProperty =
    DependencyProperty.Register("IsChecked", typeof(bool), typeof(ChangeTabRadioControl), new PropertyMetadata(false, OnIsCheckedChanged));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ChangeTabRadioControl), new PropertyMetadata("FUCK"));

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

            label.FontSize = 14;
            ind.Opacity = 100;
        }

        public void UpdateInactive()
        {
            var brush = (Brush)this.FindResource("ForegroundBrush");
            label.Foreground = brush;

            label.FontSize = 11;
            ind.Opacity = 0;
        }
    }
}
