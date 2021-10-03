using LandscapeDesignClient.AccountControls;
using LandscapeDesignClient.Model;
using System.Windows;
using System.Windows.Controls;

namespace LandscapeDesignClient.ProjectMenuControls
{
    public partial class ProjectBuildingCard : UserControl
    {
        Building _building;
        public event ProjectItemHandler EditClick;
        public event ProjectItemHandler DeleteClick;
        public event ProjectItemHandler ShareClick;

        public ProjectBuildingCard()
        {
            InitializeComponent();
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditClick?.Invoke(_building.Id);
        }
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            DeleteClick?.Invoke(_building.Id);
        }
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _building = DataContext as Building;
            cBuilding.DataContext = _building;
        }
        public bool Shared
        {
            get { return _building.Share; }
            set
            {
                if (!value)
                    return;
                var res = MessageBox.Show($"{Properties.Resources.ResourceManager.GetString("m_p_Shared")} \"{_building.Name}\".\n{Properties.Resources.ResourceManager.GetString("m_p_Shared")}", Properties.Resources.ResourceManager.GetString("m_Notitfication"), MessageBoxButton.YesNo);
                if (res == MessageBoxResult.No)
                    return;
                ShareClick?.Invoke(_building.Id);
            }
        }
        public bool IsShareEnabled
        {
            get
            {
                return !_building.Share;
            }
        }
    }
}
