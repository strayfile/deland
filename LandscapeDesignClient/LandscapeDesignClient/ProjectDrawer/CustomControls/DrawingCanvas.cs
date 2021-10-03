using LandscapeDesignClient.ProjectDrawer.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LandscapeDesignClient.ProjectDrawer.CustomControls
{
    public enum SelectedDrawingEvent
    {
        SelectCursor,
        Hand,
        ZoomInRect,
        //DrawLine,
        DrawClosedPolygon,
        AddPoints
    }

    public class DrawingCanvas : Canvas, INotifyPropertyChanged
    {
        private FrameworkElement _selectedElement;
        private Rectangle _rectZoom;
        private SelectedDrawingEvent _canvasEvent;
        private Polyline _solidLineForDrawing, _dottedLineForPreviewDrawing;
        private PointCollection _mouseLeftButtonDownOnDrawingCanvas;
        private string _mousePosition = "";
        private Point startPoint;
        private bool isDragging;

        public DrawingCanvas()
        {
            _selectedElement = this;
            _rectZoom = new Rectangle();
            _canvasEvent = SelectedDrawingEvent.SelectCursor;
            _solidLineForDrawing = new Polyline();
            _dottedLineForPreviewDrawing = new Polyline();
            _mouseLeftButtonDownOnDrawingCanvas = new PointCollection();

            //DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawingCanvas), new FrameworkPropertyMetadata(typeof(DrawingCanvas)));

            _solidLineForDrawing.StrokeThickness = 2;
            _solidLineForDrawing.Stroke = Brushes.Black;
            this.Children.Add(_solidLineForDrawing);

            _dottedLineForPreviewDrawing.StrokeThickness = 1;
            _dottedLineForPreviewDrawing.Stroke = Brushes.Black;
            DoubleCollection d = new DoubleCollection
            {
                3.5,
                1.5
            };
            _dottedLineForPreviewDrawing.StrokeDashArray = d;
            this.Children.Add(_dottedLineForPreviewDrawing);

            _rectZoom.Fill = Brushes.Transparent;
            _rectZoom.Stroke = Brushes.Black;
            _rectZoom.StrokeThickness = 2;
            _rectZoom.StrokeDashArray = d;
            _rectZoom.Height = 0;
            _rectZoom.Width = 0;
            this.Children.Add(_rectZoom);
            this.ClipToBounds = true;

            MouseLeave += DrawingCanvasMouseLeave;
            MouseMove += DrawingCanvasMouseMove;
            PreviewMouseDown += DrawingCanvasPreviewMouseDown;
        }
        
        public static DependencyProperty SelectedElementProperty = DependencyProperty.Register("SelectedElement", typeof(FrameworkElement), typeof(DrawingCanvas), new PropertyMetadata());
        public FrameworkElement SelectedElement
        {
            get
            {
                return _selectedElement;
            }
            set
            {
                _selectedElement = value;
                NotifyPropertyChanged("SelectedElement");
            }
        }
        public static readonly DependencyProperty CanvasEventProperty = DependencyProperty.Register("CanvasEvent", typeof(SelectedDrawingEvent), typeof(DrawingCanvas), new PropertyMetadata());
        public SelectedDrawingEvent CanvasEvent
        {
            get
            {
                return _canvasEvent;
            }
            set
            {
                _canvasEvent = value;
                _solidLineForDrawing.Points.Clear();
                _dottedLineForPreviewDrawing.Points.Clear();
                _mouseLeftButtonDownOnDrawingCanvas.Clear();
            }
        }
        public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(string), typeof(DrawingCanvas), new PropertyMetadata());
        public string MousePosition
        {
            get
            {
                return _mousePosition;
            }
            set
            {
                _mousePosition = value;
                NotifyPropertyChanged("MousePosition");
            }
        }
        
        public void ChangeSize(double newHeight, double newWidth)
        {
            this.Height = newHeight;
            this.Width = newWidth;
        }
        public void ChangeSizeOfElement(MouseWheelEventArgs e)
        {
            double scalefactor = 0;
            double scale = 1;
            if (e.Delta > 0)
                scalefactor = 0.1;
            else if (e.Delta < 0)
                scalefactor = -0.1;
            if (scale + scalefactor < 0)
                return;
            scale = scale + scalefactor;
            if ((SelectedElement) is CustomPolygon)
            {
                PointCollection NewPointCollection = new PointCollection();
                Point OldCenterPoint = new Point();
                Point NewCenterPoint = new Point();
                CustomPolygon PointerCustomPolygon = (CustomPolygon)SelectedElement;
                foreach (Point p in PointerCustomPolygon.Points)
                {
                    OldCenterPoint.X += p.X;
                    OldCenterPoint.Y += p.Y;
                }

                OldCenterPoint.X /= (double)PointerCustomPolygon.Points.Count;
                OldCenterPoint.Y /= (double)PointerCustomPolygon.Points.Count;

                NewCenterPoint.X = OldCenterPoint.X * scale;
                NewCenterPoint.Y = OldCenterPoint.Y * scale;

                Point correction = new Point
                {
                    X = OldCenterPoint.X - NewCenterPoint.X,
                    Y = OldCenterPoint.Y - NewCenterPoint.Y
                };

                foreach (Point p in PointerCustomPolygon.Points)
                    NewPointCollection.Add(new Point(p.X * scale + correction.X, p.Y * scale + correction.Y));

                PointerCustomPolygon.Points = NewPointCollection;
                PointerCustomPolygon.ReDraw();
            }
        }

        private void DrawingCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            _dottedLineForPreviewDrawing.Points.Clear();
        }

        private void DrawingCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Point temp_point = new Point(), modifyedpoint = new Point();
            temp_point = Mouse.GetPosition(this);
            modifyedpoint = Mouse.GetPosition(this);
            if (CanvasEvent == SelectedDrawingEvent.AddPoints)
            {
                if ((SelectedElement) is CustomPolygon)
                {
                    CustomPolygon t = (CustomPolygon)SelectedElement;
                    _dottedLineForPreviewDrawing.Points = GeometryFunctions.InsertPoint(t.Points, temp_point, true);
                    _dottedLineForPreviewDrawing.Points.Add(_dottedLineForPreviewDrawing.Points[0]);
                    return;
                }
            }
            else if (CanvasEvent == SelectedDrawingEvent.ZoomInRect)
            {
                if (_mouseLeftButtonDownOnDrawingCanvas.Count == 1)
                {
                    Point TopLeft = new Point(), BottomRight = new Point();
                    TopLeft.X = Math.Min(_mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].X, temp_point.X);
                    TopLeft.Y = Math.Min(_mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].Y, temp_point.Y);
                    BottomRight.X = Math.Max(_mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].X, temp_point.X);
                    BottomRight.Y = Math.Max(_mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].Y, temp_point.Y);
                    _rectZoom.Height = Math.Abs(TopLeft.Y - BottomRight.Y);
                    _rectZoom.Width = Math.Abs(TopLeft.X - BottomRight.X);
                    Canvas.SetLeft(_rectZoom, TopLeft.X);
                    Canvas.SetTop(_rectZoom, TopLeft.Y);
                }
                else
                {
                    _mouseLeftButtonDownOnDrawingCanvas.Clear();
                    _rectZoom.Width = 0;
                    _rectZoom.Height = 0;
                }
            }
            else if (_mouseLeftButtonDownOnDrawingCanvas.Count > 0)
            {
                if (Math.Abs(temp_point.X - _mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].X) < Math.Abs(temp_point.Y - _mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].Y))
                    modifyedpoint.X = _mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].X;
                else
                    modifyedpoint.Y = _mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].Y;

                if (!(Keyboard.IsKeyDown(Key.LeftShift)))
                {
                    _dottedLineForPreviewDrawing.Points = _mouseLeftButtonDownOnDrawingCanvas.Clone();
                    _dottedLineForPreviewDrawing.Points.Add(temp_point);
                }
                else
                {
                    _dottedLineForPreviewDrawing.Points = _mouseLeftButtonDownOnDrawingCanvas.Clone();
                    _dottedLineForPreviewDrawing.Points.Add(modifyedpoint);
                }
            }
        }

        public void DrawingCanvasKeyDown(object sener, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) == true)
                {
                    if ((SelectedElement) is CustomPolygon)
                    {
                        CustomPolygon c = (CustomPolygon)SelectedElement;
                        if (c.HitFromPoint)
                        {
                            c.Points.RemoveAt(c.SelectedPointID);
                            c.ReDraw();
                        }
                    }
                }
                else
                    this.Children.Remove(SelectedElement);
            }
        }

        private void AddPointToCustomDrawingObject(Point CurrentPosition)
        {
            if ((SelectedElement) is CustomPolygon)
            {
                CustomPolygon Pointer = (CustomPolygon)SelectedElement;
                Pointer.Points = GeometryFunctions.InsertPoint(Pointer.Points, CurrentPosition, true);
                Pointer.ReDraw();
                return;
            }
        }

        private void RectangleZoom(Point CurrentPosition, ref MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                _mouseLeftButtonDownOnDrawingCanvas.Clear();
                _rectZoom.Width = 0;
                _rectZoom.Height = 0;
            }
            else if (_mouseLeftButtonDownOnDrawingCanvas.Count >= 1)
            {
                Point TopLeft = new Point(), BottomRight = new Point();
                TopLeft.X = Math.Min(_mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].X, CurrentPosition.X);
                TopLeft.Y = Math.Min(_mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].Y, CurrentPosition.Y);
                BottomRight.X = Math.Max(_mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].X, CurrentPosition.X);
                BottomRight.Y = Math.Max(_mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].Y, CurrentPosition.Y);

                _mouseLeftButtonDownOnDrawingCanvas.Clear();
                _rectZoom.Width = 0;
                _rectZoom.Height = 0;
                e.Handled = true;
                ChangeRectangleZoom(TopLeft, BottomRight);
                return;
            }
            else
            {
                _mouseLeftButtonDownOnDrawingCanvas.Add(CurrentPosition);
                e.Handled = true;
                return;
            }
        }

        private void DrawCustomObjectWithLines(Point Actual_position, Point Modified_position)
        {
            if (_mouseLeftButtonDownOnDrawingCanvas.Count > 0)
            {
                if (Math.Abs(Actual_position.X - _mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].X) < Math.Abs(Actual_position.Y - _mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].Y))
                    Modified_position.X = _mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].X;
                else
                    Modified_position.Y = _mouseLeftButtonDownOnDrawingCanvas[_mouseLeftButtonDownOnDrawingCanvas.Count - 1].Y;

                if (!(Keyboard.IsKeyDown(Key.LeftShift)))
                {
                    _solidLineForDrawing.Points.Add(Actual_position);
                    _mouseLeftButtonDownOnDrawingCanvas.Add(Actual_position);
                }
                else
                {
                    _solidLineForDrawing.Points.Add(Modified_position);
                    _mouseLeftButtonDownOnDrawingCanvas.Add(Modified_position);
                }
            }
            else
            {
                _solidLineForDrawing.Points.Add(Actual_position);
                _mouseLeftButtonDownOnDrawingCanvas.Add(Actual_position);
            }
        }

        private void ClearTempVariables()
        {
            _mouseLeftButtonDownOnDrawingCanvas.Clear();
            _solidLineForDrawing.Points.Clear();
            _dottedLineForPreviewDrawing.Points.Clear();
        }

        protected void DrawingCanvasPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point Actual_position = new Point(), Modified_position = new Point();
            Actual_position = Mouse.GetPosition(this);
            Modified_position = Mouse.GetPosition(this);

            MousePosition = "X: " + System.Convert.ToString(Actual_position.X) + " Y: " + System.Convert.ToString(Actual_position.Y);

            if (CanvasEvent == SelectedDrawingEvent.SelectCursor)
            {
                //if (Mouse.RightButton == MouseButtonState.Pressed)
                //    CreateContextMenu();
            }
            else if (CanvasEvent == SelectedDrawingEvent.AddPoints)
                AddPointToCustomDrawingObject(Actual_position);
            else if (CanvasEvent == SelectedDrawingEvent.ZoomInRect)
                RectangleZoom(Actual_position, ref e);
            else if (Mouse.LeftButton == MouseButtonState.Pressed | Mouse.MiddleButton == MouseButtonState.Pressed)
            {
                DrawCustomObjectWithLines(Actual_position, Modified_position);
            }
            else if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (_mouseLeftButtonDownOnDrawingCanvas.Count > 1)
                {
                    if (CanvasEvent == SelectedDrawingEvent.DrawClosedPolygon)
                    {
                        PointCollection Points = new PointCollection();
                        Points = _mouseLeftButtonDownOnDrawingCanvas.Clone();
                        CustomPolygon NewCustomPolygon = new CustomPolygon(_mouseLeftButtonDownOnDrawingCanvas.Clone());
                        this.Children.Add(NewCustomPolygon);
                        ClearTempVariables();
                    }
                    //else 
                    //if (CanvasEvent == SelectedDrawingEvent.DrawLine)
                    //{
                    //    CustomLine NewCustomLine = new CustomLine(_mouseLeftButtonDownOnDrawingCanvas.Clone());
                    //    this.Children.Add(NewCustomLine);
                    //    ClearTempVariables();
                    //}
                    else
                    {
                        ClearTempVariables();
                        MessageBox.Show("You have not selected type of drawing");
                    }
                    e.Handled = true;
                }
                else
                    ClearTempVariables();
            }
        }
        
        private bool PointInsidePolygon(Polygon shape, Point po)
        {
            Geometry geo1;
            geo1 = shape.RenderedGeometry;
            return geo1.FillContains(po);
        }
        
        protected override Size MeasureOverride(Size constraint)
        {
            double bottomMost = 0.0;
            double rightMost = 0.0;
            foreach (object obj in Children)
            {
                FrameworkElement child = obj as FrameworkElement;

                if (child != null)
                {
                    child.Measure(constraint);

                    bottomMost = Math.Max(bottomMost, GetTop(child) + child.DesiredSize.Height);
                    rightMost = Math.Max(rightMost, GetLeft(child) + child.DesiredSize.Width);
                }
            }

            if (double.IsNaN(bottomMost) || double.IsInfinity(bottomMost))
                bottomMost = 0.0;

            if (double.IsNaN(rightMost) || double.IsInfinity(rightMost))
                rightMost = 0.0;

            return new Size(rightMost, bottomMost);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (CanvasEvent == SelectedDrawingEvent.SelectCursor)
            {
                startPoint = e.GetPosition(this);
                SelectedElement = e.Source as FrameworkElement;
                this.CaptureMouse();
                isDragging = true;
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
                isDragging = false;
                e.Handled = true;
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            MousePosition = "X: -  Y: - ";
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            Point currentPosition = e.GetPosition(this);
            MousePosition = "X: " + System.Convert.ToString(currentPosition.X) + " Y: " + System.Convert.ToString(currentPosition.Y);
            if (this.IsMouseCaptured)
            {
                if (isDragging)
                {
                    if ((SelectedElement) is CustomPolygon)
                    {
                        CustomPolygon PointerCustomPolygon;
                        PointerCustomPolygon = (CustomPolygon)SelectedElement;
                        if (PointerCustomPolygon.Selection == SelectionType.PointSelection & PointerCustomPolygon.HitFromPoint)
                        {
                            PointCollection NewPointCollection = new PointCollection();
                            for (int i = 0; i <= PointerCustomPolygon.Points.Count - 1; i++)
                            {
                                if (PointerCustomPolygon.SelectedPointID != i)
                                    NewPointCollection.Add(PointerCustomPolygon.Points[i]);
                                else
                                    NewPointCollection.Add(PointerCustomPolygon.Points[i] - startPoint + currentPosition);
                            }
                            PointerCustomPolygon.Points = NewPointCollection;
                            PointerCustomPolygon.ReDraw();
                        }
                        else
                        {
                            PointCollection NewPointCollection = new PointCollection();
                            foreach (Point p in PointerCustomPolygon.Points)
                                NewPointCollection.Add(p - startPoint + currentPosition);

                            PointerCustomPolygon.Points = NewPointCollection;
                            PointerCustomPolygon.ReDraw();
                        }
                    }
                    //else if ((SelectedElement) is CustomLine)
                    //{
                    //    CustomLine CustomLinePointer = (CustomLine)SelectedElement;

                    //    if (CustomLinePointer.Selection == SelectionType.PointSelection & CustomLinePointer.HitFromPoint)
                    //    {
                    //        PointCollection po = new PointCollection();
                    //        for (int i = 0; i <= CustomLinePointer.Points.Count - 1; i++)
                    //        {
                    //            if (CustomLinePointer.SelectedPointID != i)
                    //                po.Add(CustomLinePointer.Points[i]);
                    //            else
                    //                po.Add(CustomLinePointer.Points[i] - startPoint + currentPosition);
                    //        }
                    //        CustomLinePointer.Points = po;
                    //        CustomLinePointer.ReDraw();
                    //    }
                    //    else
                    //    {
                    //        PointCollection po = new PointCollection();
                    //        foreach (Point p in CustomLinePointer.Points)
                    //            po.Add(p - startPoint + currentPosition);
                    //        CustomLinePointer.Points = po;
                    //        CustomLinePointer.ReDraw();
                    //    }
                    //}
                    else if ((SelectedElement) is CanvasPoint)
                    {
                        CanvasPoint PointerCustomCanvasPoint = (CanvasPoint)SelectedElement;
                        PointerCustomCanvasPoint.PositionOnCanvas = PointerCustomCanvasPoint.PositionOnCanvas - startPoint + currentPosition;
                    }
                    startPoint = currentPosition;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event NewRectangleZoomEventHandler NewRectangleZoom;

        public delegate void NewRectangleZoomEventHandler(Point TopLeft, Point BottomRight);

        private void ChangeRectangleZoom(Point TopLeft, Point BottomRight)
        {
            NewRectangleZoom?.Invoke(TopLeft, BottomRight);
        }

        public UndoRedo UndoRedo { get; set; }
    }
}