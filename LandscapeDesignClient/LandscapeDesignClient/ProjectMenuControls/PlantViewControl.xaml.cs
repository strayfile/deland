using LandscapeDesignClient.AccountControls;
using LandscapeDesignClient.Model;
using LandscapeDesignClient.Resources;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LandscapeDesignClient.ProjectMenuControls
{
    public partial class PlantViewControl : UserControl
    {
        private PlantCard _plantCard;
        public event PlantHandler SaveClick;
        public event ButtonClicked CancelClick;
        public PlantViewControl(Plant plant)
        {
            InitializeComponent();
            spPlant.DataContext = this;
            if (plant == null)
                TitleText = Texts.Text(315);
            else TitleText = Texts.Text(316);
            _plantCard = new PlantCard(plant);
            spPlantData.DataContext = _plantCard;
            if (_plantCard.Resizable)
            {
                spNoResizable.Visibility = Visibility.Hidden;
                spNoResizable.Height = 0;
            }
            PlantCharacteristics plantCharacteristics = PlantCharacteristics.GetInstance();
            _plantCard.PropertyChanged += PlantCardPropertyChanged;
        }

        private void PlantCardPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            btnSave.IsEnabled = _plantCard.IsValid();
        }

        public string TitleText {
            get; private set;
        }


        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_plantCard.IsValid())
                SaveClick?.Invoke(_plantCard.GetCreatedPlant());
        }

        private void CancelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CancelClick?.Invoke();
        }
        
        private void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_plantCard.Category == "Травы")
            {
                spNoResizable.Visibility = Visibility.Hidden;
                spNoResizable.Height = 0;
            }
            else
            {
                spNoResizable.Visibility = Visibility.Visible;
                spNoResizable.Height = double.NaN;
            }
        }
        private void NumPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (e.Key == Key.Up || e.Key == Key.OemPlus)
                {
                    if (int.TryParse(tb.Text, out int i))
                        if ((tb.Name == txtTrunk.Name && i < 500) || (tb.Name == txtCrown.Name && i < 1000) || (tb.Name == txtTempMin.Name && i < 80) || (tb.Name == txtTempMax.Name && i < 90))
                            tb.Text = (++i).ToString();
                }
                else if (e.Key == Key.Down || e.Key == Key.OemMinus)
                {
                    if (int.TryParse(tb.Text, out int i))
                        if ((tb.Name == txtTrunk.Name && i > 6) || (tb.Name == txtCrown.Name && i > 8) || (tb.Name == txtTempMin.Name && i > -50) || (tb.Name == txtTempMax.Name && i > -50))
                            tb.Text = (--i).ToString();
                }
                else if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    e.Handled = false;
            }
        }
    }
}
