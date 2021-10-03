using LandscapeDesignClient.AccountControls;
using LandscapeDesignClient.Model;
using LandscapeDesignClient.Resources;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LandscapeDesignClient.ProjectMenuControls
{
    /// <summary>
    /// Interaction logic for ProjectPlantCard.xaml
    /// </summary>

    public partial class ProjectPlantCard : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private PlantCard _plantcard;
        public event ProjectItemHandler EditClick;
        public event ProjectItemHandler DeleteClick;
        public event ProjectItemHandler ShareClick;

        public ProjectPlantCard()
        {
            InitializeComponent();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditClick?.Invoke(_plantcard.Plant.Id);
        }
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            DeleteClick?.Invoke(_plantcard.Plant.Id);
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
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool Shared {
            get
            {  if (_plantcard.Plant.Share)
                    return true;
                return false;
            }
        }
        public bool IsShareEnabled
        {
            get
            {
                if (_plantcard.Plant.Share)
                    return false;
                return true;
            }
        }
        public Visibility DescriptionVisibility
        {
            get
            {
                if (_plantcard.Description == null || _plantcard.Description.Trim().Length == 0)
                    return Visibility.Hidden;
                return Visibility.Visible;
            }
        }
        private void ShareChecked(object sender, RoutedEventArgs e)
        {
            if (_plantcard.Plant.Share)
            {
                (sender as CheckBox).IsEnabled = false;
                btnDel.IsEnabled = false;
                return;
            }
            MessageWindow w = new MessageWindow($"{Texts.Text(313)} \"{_plantcard.Plant.Name}\"\n{Texts.Text(106)}", false, MessageBoxButton.YesNo);
            var res = w.ShowDialog();
            if (res == true)
            {
                ShareClick?.Invoke(_plantcard.Plant.Id);
                _plantcard.Plant.Share = true;
                btnDel.IsEnabled = false;
                (sender as CheckBox).IsEnabled = false;
            }
        }
    }
}
