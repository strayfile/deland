using LandscapeDesignClient.Resources;
using System.Windows;
using System.Windows.Input;

namespace LandscapeDesignClient.ProjectMenuControls
{
    public partial class MessageWindow : Window
    {
        public MessageWindow(string message, bool error, MessageBoxButton button)
        {
            InitializeComponent();
            if (error)
                MessageTitle = Texts.Text(104);
            else MessageTitle = Texts.Text(105);
            if (button == MessageBoxButton.OKCancel)
            {
                btnCancel.Visibility = Visibility.Visible;
                btnCancel.Width = double.NaN;
                btnCancel.Margin = new Thickness(8);
            }
            else if (button == MessageBoxButton.YesNo)
            {
                btnOk.Visibility = Visibility.Hidden;
                btnOk.Width = 0;
                btnOk.Margin = new Thickness(0);
                btnYes.Visibility = Visibility.Visible;
                btnYes.Width = double.NaN;
                btnYes.Margin = new Thickness(8);
                btnNo.Visibility = Visibility.Visible;
                btnNo.Width = double.NaN;
                btnNo.Margin = new Thickness(8);
            }
            gMess.DataContext = this;
            MessageText = message;
        }
        public string MessageTitle { get; set; }
        public string MessageText { get; set; }
        private void OkExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void CancelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }
        
    }
}
