using LandscapeDesignClient.ProjectDrawer.Controls;
using LandscapeDesignClient.ProjectDrawer.CustomControls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LandscapeDesignClient.ProjectDrawer
{
    public partial class CanvasControl : UserControl
    {
        private bool _mouseOnResizeContainer;
        private double _scale = 1;

        public CanvasControl()
        {
            InitializeComponent();
            ProjectCanvas.NewRectangleZoom += ChangeRectangleZoom;
            PreviewMouseWheel += CanvasControlPreviewMouseWheel;
        }
        
        private new void SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (_mouseOnResizeContainer)
                ProjectCanvas.ChangeSize((CanvasContainer.Height - 10) / _scale, (CanvasContainer.Width - 10) / _scale);
        }

        public void ResetZoom()
        {
            DrawingScroll.ScrollFromCode();

            _scale = 1;
            ScaleTransform ScalingTheCanvas = new ScaleTransform(_scale, _scale);
            ProjectCanvas.LayoutTransform = ScalingTheCanvas;
            CanvasContainer.Width = ProjectCanvas.Width * _scale + 10;
            CanvasContainer.Height = ProjectCanvas.Height * _scale + 10;
            DrawingScroll.ScrollToHorizontalOffset(0);
            DrawingScroll.ScrollToVerticalOffset(0);
        }

        public void ChangeRectangleZoom(Point TopLeft, Point BottomRight)
        {
            double _width, _height;
            _height = Math.Abs(TopLeft.Y - BottomRight.Y);
            _width = Math.Abs(TopLeft.X - BottomRight.X);

            if (_height <= 10 || _width <= 10)
                return;

            if (DrawingScroll.ViewportWidth / _width > DrawingScroll.ViewportHeight / _height)
                _scale = DrawingScroll.ViewportHeight / _height;
            else
                _scale = DrawingScroll.ViewportWidth / _width;

            DrawingScroll.ScrollFromCode();
            ScaleTransform ScalingTheCanvas = new ScaleTransform(_scale, _scale);
            
            ProjectCanvas.LayoutTransform = ScalingTheCanvas;
            CanvasContainer.Width = ProjectCanvas.Width * _scale + 10;
            CanvasContainer.Height = ProjectCanvas.Height * _scale + 10;
            
            DrawingScroll.ScrollToHorizontalOffset(_scale * TopLeft.X + _width / 2 + 10);
            DrawingScroll.ScrollToVerticalOffset(_scale * TopLeft.Y + _height / 2 + 10);
        }

        public void AddPicture(string pic)
        {
            // Create a new image element from the selected file
            PictureElement img = new PictureElement(new Point(0, 0), pic);

            // Add the image element to the DrawingCanvas
            ProjectCanvas.Children.Add(img);

            // It dosnt measure the child elements befor it is drawn
            ProjectCanvas.Width = img.ImageWidth;
            ProjectCanvas.Height = img.ImageHeight;

            // Adjusting the resizer decorator according to the new Height and Width of the canvas
            CanvasContainer.Width = ProjectCanvas.Width * _scale + 10;
            CanvasContainer.Height = ProjectCanvas.Height * _scale + 10;

            // Setting the picture to be the lowest Z-Index. This to allow other elements above it.
            Canvas.SetZIndex(ProjectCanvas.Children[ProjectCanvas.Children.Count - 1], -1);
        }

        private void CanvasControlPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!_mouseOnResizeContainer && ProjectCanvas.SelectedElement == ProjectCanvas)
            {
                double scalefactor = 0;
                if (e.Delta > 0)
                    scalefactor = 0.1;
                else if (e.Delta < 0)
                    scalefactor = -0.1;
                var sc = _scale + scalefactor;
                if ((e.Delta > 0 && ((ProjectCanvas.ActualWidth * sc + 10) < 2000 || (ProjectCanvas.ActualHeight * sc + 10) < 2000))
                    || e.Delta < 0 && sc > 0.1)
                {
                    //((ProjectCanvas.ActualWidth * sc + 10) > ProjectCanvas.ActualWidth || (ProjectCanvas.ActualHeight * sc + 10) > ProjectCanvas.ActualHeight)
                    _scale = sc;
                    ScaleTransform ScalingTheCanvas = new ScaleTransform(_scale, _scale);
                    ProjectCanvas.LayoutTransform = ScalingTheCanvas;
                    CanvasContainer.Width = ProjectCanvas.ActualWidth * _scale + 10;
                    CanvasContainer.Height = ProjectCanvas.ActualHeight * _scale + 10;
                }
            }
            else if (ProjectCanvas.SelectedElement != ProjectCanvas & (Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl)))
                ProjectCanvas.ChangeSizeOfElement(e);
            e.Handled = true;
        }

        private void ResizeThumb_MouseEnter(object sender, MouseEventArgs e)
        {
            _mouseOnResizeContainer = true;
        }

        private void ResizeThumb_MouseLeave(object sender, MouseEventArgs e)
        {
            _mouseOnResizeContainer = false;
        }

        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ResetZoom();
                return;
            }
            ProjectCanvas.DrawingCanvasKeyDown(sender, e);
        }
    }
}