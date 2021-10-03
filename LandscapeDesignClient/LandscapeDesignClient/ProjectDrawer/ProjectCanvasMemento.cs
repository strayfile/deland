using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Xml;

namespace LandscapeDesignClient.ProjectDrawer
{
    public class ProjectEntityState
    {
        public ProjectEntityState(double x, double y, int layer, double rotate, int textureId, UIElement element)
        {
            X = x;
            Y = y;
            Layer = layer;
            RotateDegree = rotate;
            TextureId = textureId;
            Element = element;
        }
        public double X { get; private set; }
        public double Y { get; private set; }
        public int Layer { get; private set; }
        public double RotateDegree { get; private set; }
        public int TextureId { get; set; }
        public UIElement Element { get; set; }
    }

    public class Memento
    {
        public List<ProjectEntityState> ContainerState { get; private set; }
        public Memento(List<ProjectEntityState> containerState)
        {
            ContainerState = containerState;
        }
    }

    public class MementoOriginator
    {
        private Canvas _canvas;

        public MementoOriginator(Canvas canvas)
        {
            _canvas = canvas;
        }

        public Memento Memento
        {
            get
            {
                List<ProjectEntityState> containerState = new List<ProjectEntityState>();
                foreach (ProjectEntityState item in _canvas.Children)
                {
                    ProjectEntityState newItem = DeepClone(item);
                    containerState.Add(newItem);
                }
                return new Memento(containerState);
            }
            set
            {
                _canvas.Children.Clear();
                Memento memento1 = MementoClone(value);
                foreach (ProjectEntityState item in memento1.ContainerState)
                {
                    ((Shape)item.Element).Stroke = System.Windows.Media.Brushes.Black;
                    _canvas.Children.Add(item.Element);
                }
            }
        }

        public Memento MementoClone(Memento memento)
        {
            List<ProjectEntityState> _ContainerState = new List<ProjectEntityState>();
            foreach (ProjectEntityState item in memento.ContainerState)
            {
                ProjectEntityState newItem = DeepClone(item);
                _ContainerState.Add(newItem);
            }
            return new Memento(_ContainerState);

        }
        private ProjectEntityState DeepClone(ProjectEntityState element)
        {
            string str = XamlWriter.Save(element.Element);
            StringReader stringReader = new StringReader(str);
            XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
            UIElement copyElement = (UIElement)XamlReader.Load(xmlTextReader);
            ProjectEntityState st = new ProjectEntityState(element.X, element.Y, element.Layer, element.RotateDegree, element.TextureId, copyElement);
            return st;
        }
    }
    
    class Caretaker
    {
        private Stack<Memento> UndoStack = new Stack<Memento>();
        private Stack<Memento> RedoStack = new Stack<Memento>();

        public Memento UndoMemento
        {
            get
            {
                if (UndoStack.Count >= 2)
                {
                    RedoStack.Push(UndoStack.Pop());
                    return UndoStack.Peek();
                }
                else
                    return null;
            }
        }
        public Memento RedoMemento
        {
            get
            {
                if (RedoStack.Count != 0)
                {
                    Memento m = RedoStack.Pop();
                    UndoStack.Push(m);
                    return m;
                }
                else
                    return null;
            }
        }
        public void InsertMementoForUndoRedo(Memento memento)
        {
            if (memento != null)
            {
                UndoStack.Push(memento);
                RedoStack.Clear();
            }
        }
        public bool IsUndoPossible
        {
            get
            {
                if (UndoStack.Count >= 2)
                    return true;
                else
                    return false;
            }
        }
        public bool IsRedoPossible
        {
            get
            {
                if (RedoStack.Count != 0)
                    return true;
                else
                    return false;
            }
        }
    }

    interface IUndoRedo
    {
        void Undo(int level);
        void Redo(int level);
        void SetStateForUndoRedo();

    }

    public class UndoRedo : IUndoRedo
    {
        Caretaker _caretaker = new Caretaker();
        MementoOriginator _mementoOriginator = null;
        public event EventHandler EnableDisableUndoRedoFeature;

        public UndoRedo(Canvas container)
        {
            _mementoOriginator = new MementoOriginator(container);
        }
        public void Undo(int level)
        {
            Memento memento = null;
            for (int i = 1; i <= level; i++)
                memento = _caretaker.UndoMemento;
            if (memento != null)
                _mementoOriginator.Memento = memento;
            if (EnableDisableUndoRedoFeature != null)
                EnableDisableUndoRedoFeature(null, null);
        }

        public void Redo(int level)
        {
            Memento memento = null;
            for (int i = 1; i <= level; i++)
                memento = _caretaker.RedoMemento;
            if (memento != null)
                _mementoOriginator.Memento = memento;
            if (EnableDisableUndoRedoFeature != null)
                EnableDisableUndoRedoFeature(null, null);
        }

        public void SetStateForUndoRedo()
        {
            Memento memento = _mementoOriginator.Memento;
            _caretaker.InsertMementoForUndoRedo(memento);
            if (EnableDisableUndoRedoFeature != null)
                EnableDisableUndoRedoFeature(null, null);
        }

        public bool IsUndoPossible
        {
            get
            {
                return _caretaker.IsUndoPossible;
            }
        }
        public bool IsRedoPossible
        {
            get
            {
                return _caretaker.IsRedoPossible;
            }
        }
    }
}
