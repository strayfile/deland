using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LandscapeDesignClient.AuthControls
{
    /// <summary>
    /// Interaction logic for ConfirmControl.xaml
    /// </summary>
    public partial class ConfirmControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ButtonClicked LoginClick;
        public event ButtonClicked RegClick;
        public event ButtonClicked ConfirmClick;
        public event ButtonClicked Resend;

        private string _code;
        private string _errorText;

        public ConfirmControl()
        {
            InitializeComponent();
            cConfirm.DataContext = this;
        }

        private void Confirm_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConfirmClick?.Invoke();
        }
        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            RegClick?.Invoke();
        }
        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            LoginClick?.Invoke();
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                OnPropertyChanged("Code");
                Validate();
            }
        }
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }
        private void Validate()
        {
            if (Code != null && Code.Length >= 6)
                btnConfirm.IsEnabled = true;
            else btnConfirm.IsEnabled = false;
        }
        private void BtnResend_Click(object sender, RoutedEventArgs e)
        {
            Resend?.Invoke();
        }
    }
}
