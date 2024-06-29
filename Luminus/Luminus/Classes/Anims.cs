using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Luminus.Classes
{
    class Anims
    {
        public static void AnimateFontSize(UIElement target, double toSize, Duration duration)
        {
            // Create the animation for the font size
            DoubleAnimation fontSizeAnimation = new DoubleAnimation
            {
                To = toSize,
                Duration = duration
            };

            // Apply the animation to the FontSize property
            target.BeginAnimation(Control.FontSizeProperty, fontSizeAnimation);
        }

        public static void AnimateOpacity(UIElement target, double toSize, Duration duration)
        {
            // Create the animation for the font size
            DoubleAnimation fontSizeAnimation = new DoubleAnimation
            {
                To = toSize,
                Duration = duration
            };

            // Apply the animation to the FontSize property
            target.BeginAnimation(Control.OpacityProperty, fontSizeAnimation);
        }
    }
}
