using System.Windows;
using LandscapeDesignClient.Model;

namespace LandscapeDesignClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LandscapeDesignFactory landDesFactory = LandscapeDesignFactory.GetInstance();
            MainWindowControlsManager mainWindowControlsManager = MainWindowControlsManager.GetInstance();
            mainWindowControlsManager.SetLadscapeDesignFactory(landDesFactory);
            mainWindowControlsManager.ControlDeleted += RemoveControl;
            mainWindowControlsManager.ControlAdded += AddControl;
            mainWindowControlsManager.LanguageControlAdded += AddLanguageControl;
            mainWindowControlsManager.SetLogin();
        }

        public void AddControl(FrameworkElement control)
        {
            gMain.Children.Add(control);
        }
        public void RemoveControl(FrameworkElement control)
        {
            if (control != null && gMain.Children.Contains(control))
                gMain.Children.Remove(control);
        }
        public void AddLanguageControl(FrameworkElement languageControl)
        {
            languageControl.Margin = new Thickness(24);
            languageControl.HorizontalAlignment = HorizontalAlignment.Left;
            languageControl.VerticalAlignment = VerticalAlignment.Top;
            gMain.Children.Add(languageControl);
        }
    }
}
