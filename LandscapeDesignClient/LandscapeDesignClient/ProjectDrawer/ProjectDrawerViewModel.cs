using LandscapeDesignClient.Resources;
using System.Collections.ObjectModel;

namespace LandscapeDesignClient.ProjectDrawer
{
    public class ProjectDrawerViewModel
    {
        public ProjectDrawerViewModel()
        {
            RBActs = new ObservableCollection<RadioButtonProperies>
            {
                new RadioButtonProperies(0, "CursorDefaultOutline", Texts.Text(321)) { IsChecked = true },
                new RadioButtonProperies(1, "ArrowAll", Texts.Text(322)),
                new RadioButtonProperies(2, "ZoomInOutline", Texts.Text(323)),
                new RadioButtonProperies(3, "VectorPolygon", Texts.Text(324)),
                new RadioButtonProperies(4, "VectorPoint", Texts.Text(325))
            };
            foreach (var i in RBActs)
                i.SelectionIndexUpdated += RaiseSelectionChangedEvent;
        }

        public event SelectionIndexUpdatedEventHandler SelectionIndexUpdated;

        public delegate void SelectionIndexUpdatedEventHandler(int index);

        private void RaiseSelectionChangedEvent(int i)
        {
            SelectionIndexUpdated?.Invoke(i);
        }
        
        public ObservableCollection<RadioButtonProperies> RBActs { get; set; }
        public ObservableCollection<RadioButtonProperies> BActs { get; set; }
    }

    public class RadioButtonProperies
    {
        public RadioButtonProperies(int index, string icon, string text)
        {
            Index = index;
            Icon = icon;
            Text = text;
        }
        private bool _isChecked = false;
        public string Text { get; set; }
        public string Icon { get; private set; }
        public int Index { get; private set; }
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                if (value)
                    SelectionIndexChanged(Index);
            }
        }

        public event SelectionIndexUpdatedEventHandler SelectionIndexUpdated;

        public delegate void SelectionIndexUpdatedEventHandler(int index);

        private void SelectionIndexChanged(int index)
        {
            SelectionIndexUpdated?.Invoke(index);
        }
    }
}