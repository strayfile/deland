using LandscapeDesignClient.AccountControls;
using LandscapeDesignClient.Model;
using LandscapeDesignClient.Resources;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace LandscapeDesignClient.ProjectMenuControls
{
    public partial class ProjectCreatorControl : UserControl
    {
        private ProjectCard _projectCard;
        public event ProjectHandler CreateClick;
        public event ButtonClicked CancelClick;

        public ProjectCreatorControl()
        {
            InitializeComponent();
            spProject.DataContext = this;
            TitleText = Texts.Text(320);
            _projectCard = new ProjectCard();
            spProjectData.DataContext = _projectCard;
            PlantCharacteristics plantCharacteristics = PlantCharacteristics.GetInstance();
            _projectCard.PropertyChanged += PlantCardPropertyChanged;
        }

        private void PlantCardPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            btnSave.IsEnabled = _projectCard.IsValid();
        }

        public string TitleText
        {
            get; private set;
        }


        private void CreateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_projectCard.IsValid())
                CreateClick?.Invoke(_projectCard.GetCreatedProject());
        }

        private void CancelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CancelClick?.Invoke();
        }

        private void NumPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (e.Key == Key.Up || e.Key == Key.OemPlus)
                {
                    if (int.TryParse(tb.Text, out int i))
                        if ((tb.Name == txtTempMin.Name && i < 80) || (tb.Name == txtTempMax.Name && i < 90))
                            tb.Text = (++i).ToString();
                }
                else if (e.Key == Key.Down || e.Key == Key.OemMinus)
                {
                    if (int.TryParse(tb.Text, out int i))
                        if ((tb.Name == txtTempMin.Name && i > -50) || (tb.Name == txtTempMax.Name && i > -50))
                            tb.Text = (--i).ToString();
                }
                else if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    e.Handled = false;
            }
        }
    }
}