using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LandscapeDesignClient.ProjectDrawer.CustomControls
{
    public class DrawingScrollViewer : ScrollViewer
    {
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private bool _scrollFromRectangleZoom = false;

        public static readonly DependencyProperty HandProperty = DependencyProperty.Register("Hand", typeof(Boolean), typeof(DrawingScrollViewer), new PropertyMetadata());

        private bool _hand = false;
        public bool Hand
        {
            get
            {
                return _hand;
            }
            set
            {
                _hand = value;
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (Hand)
            {
                if (IsMouseOver)
                {
                    scrollStartPoint = e.GetPosition(this);
                    scrollStartOffset.X = HorizontalOffset;
                    scrollStartOffset.Y = VerticalOffset;
                    this.Cursor = (ExtentWidth > ViewportWidth) || (ExtentHeight > ViewportHeight) ? Cursors.ScrollAll : Cursors.Arrow;

                    this.CaptureMouse();
                }
                base.OnPreviewMouseDown(e);
                e.Handled = true;
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (Hand)
            {
                if (this.IsMouseCaptured)
                {
                    Point point = e.GetPosition(this);
                    Point delta = new Point((point.X > this.scrollStartPoint.X) ? -(point.X - this.scrollStartPoint.X) : (this.scrollStartPoint.X - point.X), (point.Y > this.scrollStartPoint.Y) ? -(point.Y - this.scrollStartPoint.Y) : (this.scrollStartPoint.Y - point.Y));
                    ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                    ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
                }

                base.OnPreviewMouseMove(e);
                e.Handled = true;
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (Hand)
            {
                if (this.IsMouseCaptured)
                {
                    this.Cursor = Cursors.Arrow;
                    this.ReleaseMouseCapture();
                }
                base.OnPreviewMouseUp(e);
                e.Handled = true;
            }
        }

        public void ScrollFromCode()
        {
            _scrollFromRectangleZoom = true;
        }
        
        protected override void OnScrollChanged(System.Windows.Controls.ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
            if (!_scrollFromRectangleZoom)
            {
                if (e.Source == this)
                {
                    if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
                    {
                        Point mousePosition;
                        _scrollFromRectangleZoom = false;
                        mousePosition = Mouse.GetPosition(this);

                        double offsetx = e.HorizontalOffset + mousePosition.X;
                        double offsety = e.VerticalOffset + mousePosition.Y;

                        double oldExtentWidth = e.ExtentWidth - e.ExtentWidthChange;
                        double oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;

                        double relx = offsetx / oldExtentWidth;
                        double rely = offsety / oldExtentHeight;

                        offsetx = Math.Max(relx * e.ExtentWidth - mousePosition.X, 0);
                        offsety = Math.Max(rely * e.ExtentHeight - mousePosition.Y, 0);

                        this.ScrollToHorizontalOffset(offsetx);
                        this.ScrollToVerticalOffset(offsety);
                    }
                }
            }
            else
                _scrollFromRectangleZoom = false;
        }
    }
}