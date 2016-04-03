using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CreativeXamlToolkit.Wpf
{
    public class MarqueeLabel : Control
    {
        static MarqueeLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeLabel), new FrameworkPropertyMetadata(typeof(MarqueeLabel)));
        }

        #region DependencyProperty Content

        /// <summary>
        /// Registers a dependency property as backing store for the Text property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MarqueeLabel),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Text content of the label
        /// </summary>
        /// <value>The text string</value>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.Template != null)
            {

            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            Canvas cnvFrame = this.Template.FindName("cnvFrame", this) as Canvas;
            TextBlock tblContent = this.Template.FindName("tblContent", this) as TextBlock;

            cnvFrame.Height = tblContent.ActualHeight;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = -1 * (tblContent.ActualWidth - cnvFrame.ActualWidth);
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.AutoReverse = true;
            doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(5));
            doubleAnimation.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseInOut };
            tblContent.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
        }
    }
}
