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
    public class CanvasPoint : FrameworkElement, INotifyPropertyChanged
    {
        private VisualCollection _children;

        public CanvasPoint()
        {
            _children = new VisualCollection(this)
            {
                CreateDrawingVisualCircle()
            };
        }
        public CanvasPoint(Point p_position)
        {
            PositionOnCanvas = p_position;

            _children = new VisualCollection(this)
            {
                CreateDrawingVisualCircle()
            };
        }

        private DrawingVisual CreateDrawingVisualCircle()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 1.0), PositionOnCanvas, 4, 4);
            drawingContext.Close();
            return drawingVisual;
        }
        private Point _positionOnCanvas = new Point();

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

        public Point PositionOnCanvas
        {
            get
            {
                return _positionOnCanvas;
            }
            set
            {
                _positionOnCanvas = value;
                INotifyChange("PositionOnCanvas");
            }
        }
        public void INotifyChange(string info)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(info));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
