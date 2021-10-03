using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LandscapeDesignClient.ProjectDrawer.Controls
{
    //public class CustomPoint : FrameworkElement, INotifyPropertyChanged
    //{
    //    private VisualCollection _children;

    //    private string filename;

    //    public static int _ID = 0;

    //    public CustomPoint(string Text)
    //    {
    //        _children = new VisualCollection(this)
    //        {
    //            CreateDrawingVisualCircle(),
    //            CreateInvisibleDrawingVisualCircle(),
    //            CreateDrawingVisualText()
    //        };
    //        filename = Text;
    //    }

    //    public CustomPoint()
    //    {
    //        _children = new VisualCollection(this)
    //        {
    //            CreateDrawingVisualCircle(),
    //            CreateInvisibleDrawingVisualCircle(),
    //            CreateDrawingVisualText()
    //        };
    //    }

    //    public CustomPoint(string p_filename, Point p_position)
    //    {
    //        filename = p_filename;
    //        filename = "S" + _ID;
    //        _ID += 1;
    //        PositionOnCanvas = p_position;

    //        _children = new VisualCollection(this)
    //        {
    //            CreateDrawingVisualCircle(),
    //            CreateDrawingVisualText(),
    //            CreateInvisibleDrawingVisualCircle()
    //        };
    //        this.ToolTip = CreateToolTip();
    //    }

    //    public bool IsSelected { get; set; }

    //    private Point _positionOnCanvas = new Point();
    //    public Point PositionOnCanvas
    //    {
    //        get
    //        {
    //            return _positionOnCanvas;
    //        }
    //        set
    //        {
    //            _positionOnCanvas = value;
    //            INotifyChange("PositionOnCanvas");
    //        }
    //    }

    //    protected override int VisualChildrenCount
    //    {
    //        get
    //        {
    //            return _children.Count;
    //        }
    //    }
    //    protected override Visual GetVisualChild(int index)
    //    {
    //        if (index < 0 || index >= _children.Count)
    //            throw new ArgumentOutOfRangeException();

    //        return _children[index];
    //    }

    //    private DrawingVisual CreateDrawingVisualCircle()
    //    {
    //        DrawingVisual drawingVisual = new DrawingVisual();
    //        DrawingContext drawingContext = drawingVisual.RenderOpen();
    //        drawingContext.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 1.0), PositionOnCanvas, 4, 4);
    //        drawingContext.Close();
    //        return drawingVisual;
    //    }

    //    private DrawingVisual CreateInvisibleDrawingVisualCircle()
    //    {
    //        DrawingVisual drawingVisual = new DrawingVisual();
    //        DrawingContext drawingContext = drawingVisual.RenderOpen();
    //        drawingContext.DrawEllipse(Brushes.Transparent, new Pen(Brushes.Transparent, 1.0), PositionOnCanvas, 20, 20);
    //        drawingContext.Close();
    //        return drawingVisual;
    //    }

    //    private DrawingVisual CreateDrawingVisualText()
    //    {
    //        DrawingVisual drawingVisual = new DrawingVisual();
    //        DrawingContext drawingContext = drawingVisual.RenderOpen();
    //        drawingContext.DrawText(new FormattedText(filename, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black), new Point(PositionOnCanvas.X + 5, PositionOnCanvas.Y - 15));
    //        drawingContext.Close();
    //        return drawingVisual;
    //    }


    //    public void INotifyChange(string info)
    //    {
    //        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(info));
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    public void ReDraw()
    //    {
    //        _children.Clear();
    //        _children = new VisualCollection(this)
    //        {
    //            CreateDrawingVisualCircle(),
    //            CreateInvisibleDrawingVisualCircle(),
    //            CreateDrawingVisualText()
    //        };
    //        this.ToolTip = CreateToolTip();
    //    }

    //    private ToolTip CreateToolTip()
    //    {
    //        Border objTitleBorder = new Border
    //        {
    //            CornerRadius = new CornerRadius(5),
    //            BorderThickness = new Thickness(2),
    //            Width = 250,
    //            Height = 130,
    //            Background = new SolidColorBrush(Colors.WhiteSmoke),
    //            BorderBrush = System.Windows.Media.Brushes.Blue,
    //            Margin = new Thickness(0, 0, 0, 12),
    //            HorizontalAlignment = HorizontalAlignment.Center
    //        };

    //        StackPanel OuterStackPanel = new StackPanel();
    //        WrapPanel wrap = new WrapPanel();
    //        wrap.Margin = new Thickness(5);
    //        StackPanel LabelStackPanel = new StackPanel();
    //        StackPanel TextBlockStackPanel = new StackPanel();

    //        Label lblDate = new Label
    //        {
    //            Content = "Measurment date: ",
    //            Height = 30,
    //            Width = 120,
    //            HorizontalAlignment = HorizontalAlignment.Left
    //        };

    //        Label lblSPL = new Label
    //        {
    //            Content = "Measured SPL (dBA): ",
    //            Height = 30,
    //            Width = 120,
    //            HorizontalAlignment = HorizontalAlignment.Left
    //        };

    //        Label lblMeasurementNr = new Label
    //        {
    //            Content = "Measurement Nr: ",
    //            Height = 30,
    //            Width = 120,
    //            HorizontalAlignment = HorizontalAlignment.Left
    //        };


    //        Label lblFromInstrument = new Label
    //        {
    //            Content = "From instrument: ",
    //            Height = 30,
    //            Width = 120,
    //            HorizontalAlignment = HorizontalAlignment.Left
    //        };

    //        LabelStackPanel.Children.Add(lblSPL);
    //        LabelStackPanel.Children.Add(lblMeasurementNr);
    //        LabelStackPanel.Children.Add(lblDate);
    //        LabelStackPanel.Children.Add(lblFromInstrument);


    //        TextBox txtDate = new TextBox
    //        {
    //            Text = "20.02.2012",
    //            Height = 30,
    //            Width = 100,
    //            HorizontalAlignment = HorizontalAlignment.Left
    //        };

    //        TextBox txtSPL = new TextBox();
    //        txtSPL.Text = "104,8 dBA";
    //        txtSPL.Height = 30;
    //        txtSPL.Width = 100;
    //        txtSPL.HorizontalAlignment = HorizontalAlignment.Left;

    //        TextBox txtMeasurementNr = new TextBox
    //        {
    //            Text = "005",
    //            Height = 30,
    //            Width = 100,
    //            HorizontalAlignment = HorizontalAlignment.Left
    //        };

    //        TextBox txtFromInstrument = new TextBox
    //        {
    //            Text = "NOR 140 BGN",
    //            Height = 30,
    //            Width = 100,
    //            HorizontalAlignment = HorizontalAlignment.Left
    //        };


    //        TextBlockStackPanel.Children.Add(txtSPL);
    //        TextBlockStackPanel.Children.Add(txtMeasurementNr);
    //        TextBlockStackPanel.Children.Add(txtDate);
    //        TextBlockStackPanel.Children.Add(txtFromInstrument);
    //        wrap.Children.Add(LabelStackPanel);
    //        wrap.Children.Add(TextBlockStackPanel);

    //        objTitleBorder.Child = wrap;

    //        ToolTip tt = new ToolTip
    //        {
    //            Content = objTitleBorder,
    //            SnapsToDevicePixels = true,
    //            BorderBrush = Brushes.Transparent,
    //            Background = Brushes.Transparent
    //        };
    //        ToolTipService.SetHasDropShadow(tt, false);
    //        tt.Margin = new Thickness(15, 17, 15, 17);

    //        return tt;
    //    }
    //}
}