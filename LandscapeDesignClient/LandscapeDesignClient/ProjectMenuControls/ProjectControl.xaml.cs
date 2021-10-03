using LandscapeDesignClient.ProjectDrawer;
using LandscapeDesignClient.ProjectDrawer.CustomControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LandscapeDesignClient.ProjectMenuControls
{
    public partial class ProjectControl : UserControl
    {
        public event ButtonClicked CreateNewProjectClick;
        public event ButtonClicked OpenProjectClick;
        public event ButtonClicked ProfileClick;
        public event ButtonClicked SignOutClick;
        private UndoRedo _undoRedo;

        public ProjectControl()
        {
            InitializeComponent();

            ListOfRadioButtons = (ProjectDrawerViewModel)DrawRBs.DataContext;
            ListOfRadioButtons.SelectionIndexUpdated += SpDrawRBsSelectionChanged;

            _undoRedo = new UndoRedo(drawingControl.ProjectCanvas);
            _undoRedo.SetStateForUndoRedo();
            drawingControl.ProjectCanvas.UndoRedo = _undoRedo;
            _undoRedo.EnableDisableUndoRedoFeature += EnableDisableUndoRedo;
        }
        private void CommandCreateNewProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //проверка на сохранение
            CreateNewProjectClick?.Invoke();
        }
        private void CommandOpenProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //проверка на сохранение
            OpenProjectClick?.Invoke();
        }
        private void CommandCloseProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //проверка на сохранение
            ProfileClick?.Invoke();
        }
        private void CommandProfile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //проверка на сохранение
            ProfileClick?.Invoke();
        }
        private void CommandSignOutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //проверка на сохранение
            SignOutClick?.Invoke();
        }

        private void Btn_GenerateXAML_Click(object sender, RoutedEventArgs e)
        {
            //XDocument XMLdocument = new XDocument();
            //XElement nodeInXMLDocument = new XElement("File");
            //foreach (FrameworkElement ElementsInDrawingCanvas in DrawingControl.DrawingCanvas.Children)
            //{
            //    if ((ElementsInDrawingCanvas) is CustomLine)
            //        nodeInXMLDocument.Add(FileHandling.CreateXAMLfromLine(ElementsInDrawingCanvas));
            //    else if ((ElementsInDrawingCanvas) is CustomPolygon)
            //        nodeInXMLDocument.Add(FileHandling.CreateXAMLfromPolygon(ElementsInDrawingCanvas));
            //}
            //XMLdocument.Add(nodeInXMLDocument);
            //XAMLViewer dlg = new XAMLViewer(XMLdocument);
            //dlg.ShowDialog();
        }
        
        private void Btn_AddPicture_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            //{
            //    var withBlock = dlg;
            //    if (withBlock.ShowDialog == Forms.DialogResult.OK)
            //        // No filter here. If its not an image youll get an exception here
            //        DrawingControl.AddPicture(withBlock.FileName);
            //}
        }

        private void ProjectControlMouseMove(object sender, MouseEventArgs e)
        {
            tbPositionX.Text = e.GetPosition(drawingControl.DrawingScroll).X.ToString();
            tbPositionY.Text = e.GetPosition(drawingControl.DrawingScroll).Y.ToString();
        }
        private void EnableDisableUndoRedo(object sender, EventArgs e)
        {
            if (_undoRedo.IsUndoPossible)
            {
                btnUndo.IsEnabled = true;
            }
            else
            {
                btnUndo.IsEnabled = false;
            }

            if (_undoRedo.IsRedoPossible)
            {
                btnRedo.IsEnabled = true;
            }
            else
            {
                btnRedo.IsEnabled = false;
            }
        }
        public void SpDrawRBsSelectionChanged(int value)
        {
            switch (value) {
                case 0:
                    drawingControl.ProjectCanvas.CanvasEvent = SelectedDrawingEvent.SelectCursor;
                    break;
                case 1:
                    drawingControl.ProjectCanvas.CanvasEvent = SelectedDrawingEvent.Hand;
                    break;
                case 2:
                    drawingControl.ProjectCanvas.CanvasEvent = SelectedDrawingEvent.ZoomInRect;
                    break;
                case 3:
                    drawingControl.ProjectCanvas.CanvasEvent = SelectedDrawingEvent.DrawClosedPolygon;
                    break;
                case 4:
                    drawingControl.ProjectCanvas.CanvasEvent = SelectedDrawingEvent.AddPoints;
                    break;
            }
            if (value == 1)
                drawingControl.DrawingScroll.Hand = true;
            else
                drawingControl.DrawingScroll.Hand = false;
        }
        private void ProjectControlKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
                _undoRedo.Undo(1);
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.G)
                _undoRedo.Redo(1);
        }
        public ProjectDrawerViewModel ListOfRadioButtons { get; private set; }
    }
}