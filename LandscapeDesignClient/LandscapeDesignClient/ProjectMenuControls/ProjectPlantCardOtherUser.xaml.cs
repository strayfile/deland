using LandscapeDesignClient.Model;
using System.Windows;
using System.Windows.Controls;

namespace LandscapeDesignClient.ProjectMenuControls
{
    public partial class ProjectPlantCardOtherUser : UserControl
    {
        PlantCard _plantcard;
        public ProjectPlantCardOtherUser()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _plantcard = new PlantCard(DataContext as Plant);
            if (_plantcard.Plant.Resizable)
                SetResizablePlant();
            cPlant.DataContext = _plantcard;
            tbDesc.DataContext = this;
        }
        private void SetResizablePlant()
        {
            tbD1.Visibility = Visibility.Hidden;
            tbD1.Height = 0;
            tbD1.Margin = new Thickness(0);
            tbD2.Visibility = Visibility.Hidden;
            tbD2.Height = 0;
            tbD2.Margin = new Thickness(0);
            tbD3.Visibility = Visibility.Hidden;
            tbD3.Height = 0;
            tbD3.Margin = new Thickness(0);
            tbD4.Visibility = Visibility.Hidden;
            tbD4.Height = 0;
            tbD4.Margin = new Thickness(0);
        }
        public Visibility DescriptionVisibility
        {
            get
            {
                if (_plantcard.Plant.Description == null || _plantcard.Description.Trim().Length == 0)
                    return Visibility.Hidden;
                return Visibility.Visible;
            }
        }
    }
}
