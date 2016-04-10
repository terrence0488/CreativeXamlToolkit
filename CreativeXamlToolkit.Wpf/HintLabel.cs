using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CreativeXamlToolkit.Wpf
{
    public enum LongTextExpandDirections
    {
        Left,
        Right
    }

    public enum ExpandOnActions
    {
        MouseClick,
        MouseOver
    }

    public class HintLabel : Control
    {
        static HintLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HintLabel), new FrameworkPropertyMetadata(typeof(HintLabel)));
        }

        #region Events

        public static readonly RoutedEvent IsExpandedChangedEvent = EventManager.RegisterRoutedEvent(
            "IsExpandedChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HintLabel));

        /// <summary>
        /// Is called when the IsExpanded property changed.
        /// </summary>
        public event RoutedEventHandler IsExpandedChanged
        {
            add { AddHandler(IsExpandedChangedEvent, value); }
            remove { RemoveHandler(IsExpandedChangedEvent, value); }
        }

        void RaiseIsExpandedChangedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(IsExpandedChangedEvent);
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region DependencyProperties

        internal static readonly DependencyPropertyKey IsExpandedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsExpanded", typeof(bool), typeof(HintLabel),
                new PropertyMetadata(false,
                    new PropertyChangedCallback(OnIsExpandedChanged)));

        public static readonly DependencyProperty IsExpandedProperty = IsExpandedPropertyKey.DependencyProperty;

        /// <summary>
        /// Return True when the HintLabel is expanded, False then it collapsed.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HintLabel hintLabel = (HintLabel)d;
            hintLabel.RaiseIsExpandedChangedEvent();
        }

        /// <summary>
        /// Registers a dependency property as backing store for the ShortText property
        /// </summary>
        public static readonly DependencyProperty ShortTextProperty =
            DependencyProperty.Register("ShortText", typeof(string), typeof(HintLabel),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Short text that display when the label collapsed.
        /// </summary>
        /// <value>The short text string</value>
        public string ShortText
        {
            get { return (string)GetValue(ShortTextProperty); }
            set { SetValue(ShortTextProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property as backing store for the AutoGenerateShortText property
        /// </summary>
        public static readonly DependencyProperty AutoGenerateShortTextProperty =
           DependencyProperty.Register("AutoGenerateShortText", typeof(bool), typeof(HintLabel),
           new FrameworkPropertyMetadata(
               false,
               FrameworkPropertyMetadataOptions.AffectsRender |
               FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// If set to true, short text will be showing short form of Long Text value.
        /// </summary>
        /// <value>The Long text string</value>
        public bool AutoGenerateShortText
        {
            get { return (bool)GetValue(AutoGenerateShortTextProperty); }
            set { SetValue(AutoGenerateShortTextProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property as backing store for the LongText property
        /// </summary>
        public static readonly DependencyProperty LongTextProperty =
            DependencyProperty.Register("LongText", typeof(string), typeof(HintLabel),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Long text that display when the label expand.
        /// </summary>
        /// <value>The Long text string</value>
        public string LongText
        {
            get { return (string)GetValue(LongTextProperty); }
            set { SetValue(LongTextProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property as backing store for the LongTextExpandDirection property
        /// </summary>
        public static readonly DependencyProperty LongTextExpandDirectionProperty =
            DependencyProperty.Register("LongTextExpandDirection", typeof(LongTextExpandDirections), typeof(HintLabel),
            new FrameworkPropertyMetadata(
                LongTextExpandDirections.Right,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// To set the direction when the short text expand to long text.
        /// </summary>
        public LongTextExpandDirections LongTextExpandDirection
        {
            get { return (LongTextExpandDirections)GetValue(LongTextExpandDirectionProperty); }
            set { SetValue(LongTextExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property as backing store for the ExpandOn property
        /// </summary>
        public static readonly DependencyProperty ExpandOnProperty =
            DependencyProperty.Register("ExpandOn", typeof(ExpandOnActions), typeof(HintLabel),
            new FrameworkPropertyMetadata(
                ExpandOnActions.MouseOver,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// To set the action that expand the HintLabel.
        /// </summary>
        public ExpandOnActions ExpandOn
        {
            get { return (ExpandOnActions)GetValue(ExpandOnProperty); }
            set { SetValue(ExpandOnProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property as backing store for the CornerRadius property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(HintLabel),
            new FrameworkPropertyMetadata(
                new CornerRadius(0),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion DependencyProperty Content

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.Template != null)
            {
                if (AutoGenerateShortText)
                {
                    string ShortForm = "";
                    LongText.ToString().Split(' ').ToList().ForEach(i => ShortForm = ShortForm + i[0]);
                    //this.SetValue(ShortTextProperty, ShortForm.ToUpper());
                    this.ShortText = ShortForm.ToUpper();
                }

                Popup popLongText = this.Template.FindName("popLongText", this) as Popup;
                popLongText.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(placePopup);
                popLongText.Opened += (sender, e) => { SetValue(IsExpandedPropertyKey, true); };
                popLongText.Closed += (sender, e) => { SetValue(IsExpandedPropertyKey, false); };

                Border brdShortText = this.Template.FindName("brdShortText", this) as Border;
                Border brdLongText = this.Template.FindName("brdLongText", this) as Border;

                if (this.ExpandOn == ExpandOnActions.MouseOver)
                {
                    brdShortText.MouseEnter += BrdShortText_MouseEnter;
                    brdLongText.MouseMove += BrdLongText_MouseMove;
                }
                else if (this.ExpandOn == ExpandOnActions.MouseClick)
                {
                    brdShortText.MouseLeftButtonUp += BrdShortText_MouseLeftButtonUp;
                }

                if (LongTextExpandDirection == LongTextExpandDirections.Left)
                    brdLongText.RenderTransformOrigin = new Point(1, 0);
            }
        }

        private void BrdShortText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsExpanded)
                ExpandLongText();
            else
                CollapseLongText();
        }

        private void BrdShortText_MouseEnter(object sender, MouseEventArgs e)
        {
            ExpandLongText();
        }

        private void BrdLongText_MouseMove(object sender, MouseEventArgs e)
        {
            Border brdLongText = (Border)sender;

            //When mouse is captured, IsMouseOver property will always be true.
            //Therefore, manual mouse over detection is needed.
            Point pt = e.GetPosition(brdLongText);
            if ((pt.X < 0.0) || (pt.Y < 0.0) || (pt.X >= brdLongText.ActualWidth) || (pt.Y >= brdLongText.ActualHeight))
            {
                CollapseLongText();

                if (brdLongText.IsMouseCaptured)
                    brdLongText.ReleaseMouseCapture();
            }
        }

        public CustomPopupPlacement[] placePopup(Size popupSize, Size targetSize, Point offset)
        {
            CustomPopupPlacement placement1;

            if (this.LongTextExpandDirection == LongTextExpandDirections.Right)
                placement1 = new CustomPopupPlacement(new Point(0, 0), PopupPrimaryAxis.Vertical);
            else
                placement1 = new CustomPopupPlacement(new Point(-1 * (popupSize.Width - targetSize.Width), 0), PopupPrimaryAxis.Vertical);

            CustomPopupPlacement placement2 =
                new CustomPopupPlacement(new Point(0, 0), PopupPrimaryAxis.Horizontal);

            CustomPopupPlacement[] ttplaces = new CustomPopupPlacement[] { placement1, placement2 };
            return ttplaces;
        }

        private void ExpandLongText()
        {
            Popup popLongText = this.Template.FindName("popLongText", this) as Popup;
            popLongText.IsOpen = true;

            Border brdLongText = this.Template.FindName("brdLongText", this) as Border;
            ScaleTransform trans = new ScaleTransform();
            brdLongText.RenderTransform = trans;
            DoubleAnimation anim = new DoubleAnimation(0.5, 1, TimeSpan.FromMilliseconds(150));
            trans.BeginAnimation(ScaleTransform.ScaleXProperty, anim);

            //If Expand action is MouseOver, then we need to capture the mouse to fire Mousemove.
            if (ExpandOn == ExpandOnActions.MouseOver)
                brdLongText.CaptureMouse();
        }

        private void CollapseLongText()
        {
            Popup popLongText = this.Template.FindName("popLongText", this) as Popup;
            popLongText.IsOpen = false;
        }
    }
}