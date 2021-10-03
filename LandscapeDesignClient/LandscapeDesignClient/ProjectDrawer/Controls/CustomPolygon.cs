using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace LandscapeDesignClient.ProjectDrawer.Controls
{
    public class CustomPolygon : FrameworkElement, INotifyPropertyChanged
    {
        private VisualCollection _children;

        public CustomPolygon(PointCollection pPoints)
        {
            Points = pPoints;
            _children = new VisualCollection(this)
            {
                CreateDrawingVisualPolygon()
            };
            PreviewMouseDown += CustomPolygonPreviewMouseDown;
        }

        public SelectionType Selection = SelectionType.NotSelected;
        
        private PointCollection _positionOnCanvas = new PointCollection();
        public PointCollection Points
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

        public bool HitFromPoint { get; set; }
        public bool Multiselect { get; set; }
        public int SelectedPointID { get; set; }

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
        
        public void ReDraw()
        {
            _children.Clear();
            _children = new VisualCollection(this);

            for (int i = 0; i <= Points.Count - 1; i++)
            {
                if (i < Points.Count - 1)
                    _children.Add(CreateDrawingVisualLine(Points[i], Points[i + 1]));
                else
                    _children.Add(CreateDrawingVisualLine(Points[i], Points[0]));
                if (Selection == SelectionType.PointSelection)
                    _children.Add(new CanvasPoint(Points[i]));
            }
        }

        private DrawingVisual CreateDrawingVisualPolygon()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            PathGeometry pathGeometry = new PathGeometry
            {
                FillRule = FillRule.Nonzero
            };

            PathFigure PolygonPathFigure = new PathFigure();
            for (int i = 0; i <= Points.Count - 1; i++)
            {
                if (i == 0)
                {
                    var n = new LineSegment(Points[i], true);
                    PolygonPathFigure.Segments.Add(n);
                    PolygonPathFigure.StartPoint = Points[i];
                }
                else if (i == Points.Count - 1)
                {
                    var n = new LineSegment(Points[i], true);
                    PolygonPathFigure.Segments.Add(n);
                }
                else
                {
                    var n = new LineSegment(Points[i], true);
                    PolygonPathFigure.Segments.Add(n);
                    PolygonPathFigure.IsClosed = true;
                }
            }

            pathGeometry.Figures.Add(PolygonPathFigure);

            Pen pen;

            if (Selection == SelectionType.LineSelected)
                pen = new Pen(Brushes.Black, 1);
            else
                pen = new Pen(Brushes.Gray, 1);

            pen.EndLineCap = PenLineCap.Round;
            pen.DashCap = PenLineCap.Round;
            pen.LineJoin = PenLineJoin.Round;
            pen.StartLineCap = PenLineCap.Round;
            pen.MiterLimit = 10.0;

            drawingContext.DrawGeometry(Brushes.Black, pen, pathGeometry);
            for (int i = 0; i <= Points.Count - 1; i++)
            {
                if (i != Points.Count - 1)
                    drawingContext.DrawLine(pen, Points[i], Points[i + 1]);
                else
                    drawingContext.DrawLine(pen, Points[i], Points[0]);
            }
            drawingContext.Close();
            return drawingVisual;
        }

        private DrawingVisual CreateDrawingVisualLine(Point p1, Point p2)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Pen pen;
            if (Selection == SelectionType.LineSelected)
                pen = new Pen(Brushes.Black, 1);
            else
                pen = new Pen(Brushes.Gray, 1);

            pen.EndLineCap = PenLineCap.Round;
            pen.DashCap = PenLineCap.Round;
            pen.LineJoin = PenLineJoin.Round;
            pen.StartLineCap = PenLineCap.Round;
            pen.MiterLimit = 10.0;
            drawingContext.DrawLine(pen, p1, p2);
            drawingContext.Close();
            return drawingVisual;
        }


        public void CustomPolygonPreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (!((e.OriginalSource) is CanvasPoint))
            {
                HitFromPoint = false;
                if (Selection == SelectionType.NotSelected)
                {
                    Selection = SelectionType.LineSelected;
                    for (int i = 0; i <= _children.Count - 1; i++)
                    {
                        if ((_children[i]) is DrawingVisual)
                            ((DrawingVisual)_children[i]).Opacity = 0.5;
                        else if ((_children[i]) is CanvasPoint)
                            ((CanvasPoint)_children[i]).Opacity = 0.5;
                    }
                    ReDraw();
                }
                else if (Selection == SelectionType.LineSelected)
                {
                    Selection = SelectionType.PointSelection;
                    ReDraw();
                }
                else
                {
                    Selection = SelectionType.NotSelected;
                    _children.Clear();
                    _children = new VisualCollection(this)
                    {
                        CreateDrawingVisualPolygon()
                    };
                }
            }
            else if ((e.OriginalSource) is CanvasPoint)
            {
                HitFromPoint = true;
                CanvasPoint CurrentSelectedPoint;
                CurrentSelectedPoint = (CanvasPoint)e.OriginalSource;
                CurrentSelectedPoint.Opacity = 0.5;
                for (int i = 0; i <= this._children.Count - 1; i++)
                {
                    if ((this._children[i]) is CanvasPoint)
                    {
                        if (!Multiselect)
                        {
                            CanvasPoint po = (CanvasPoint)this._children[i];
                            if (po == CurrentSelectedPoint)
                                po.Opacity = 1;
                            else
                                po.Opacity = 0.5;
                        }
                    }
                }
                for (int i = 0; i <= Points.Count - 1; i++)
                    if (CurrentSelectedPoint.PositionOnCanvas == Points[i])
                        SelectedPointID = i;
            }
        }

        public void INotifyChange(string info)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
