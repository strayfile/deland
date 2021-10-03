using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LandscapeDesignClient.ProjectDrawer.Controls
{
    public class PictureElement : FrameworkElement, INotifyPropertyChanged
    {
        private VisualCollection _children;

        public PictureElement(Point p_position, string Url)
        {
            TopLeft = p_position;
            _children = new VisualCollection(this);
            _children.Add(CreateDrawingVisualImage(Url));
        }
        
        private Point _topLeft = new Point();
        public Point TopLeft
        {
            get
            {
                return _topLeft;
            }
            set
            {
                _topLeft = value;
                INotifyChange("TopLeft");
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _children.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
                throw new ArgumentOutOfRangeException();

            return _children[index];
        }

        public double ImageHeight { get; set; }

        public double ImageWidth { get; set; }

        private DrawingVisual CreateDrawingVisualImage(string img)
        {
            var imgConv = new ImageSourceConverter();
            ImageSource imageSource;
            imageSource = (ImageSource)imgConv.ConvertFromString(img);
            ImageHeight = imageSource.Height;
            ImageWidth = imageSource.Width;

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(imageSource, new Rect(0, 0, ImageWidth, ImageHeight));
            drawingContext.Close();

            return drawingVisual;
        }
        
        public void INotifyChange(string info)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}