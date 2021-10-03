using LandscapeDesignClient.Resources;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LandscapeDesignClient.AuthControls
{
    /// <summary>
    /// Interaction logic for Authorization.xaml
    /// </summary>
    public partial class Authorization : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ButtonClicked LoginClick;
        public event ButtonClicked RegClick;
        private string _email;
        private string _errorText;

        public Authorization()
        {
            InitializeComponent();
            gAuth.DataContext = this;
            Validated = false;
        }
        public Authorization(string email, string pass)
        {
            InitializeComponent();
            gAuth.DataContext = this;
            Email = email;
            Password = pass;
            Validate();
        }

        private void ValidateEmail()
        {
            int error = AuthPerson.ValidateEmail(tbEmail.Text);
            if (error != -1)
            {
                tbEmailError.Text = "*";
                ErrorText = Texts.Text(error);
            }
            else
            {
                tbEmailError.Text = "";
                ErrorText = "";
            }

        }
        private void ValidatePassword()
        {
            int error = AuthPerson.ValidatePassword(tbPass.Password);
            if (error != -1)
            {
                tbPassError.Text = "*";
                ErrorText = Texts.Text(error);
            }
            else
            {
                tbPassError.Text = "";
                ErrorText = "";
            }
    }

        private bool Validate()
        {
            if (AuthPerson.ValidateEmail(tbEmail.Text) == -1 && AuthPerson.ValidatePassword(tbPass.Password) == -1)
                return true;
            return false;
        }
        private void SignIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Validated = Validate();
            if (!Validated)
            {
                ValidateEmail();
                ValidatePassword();
            }
            else LoginClick?.Invoke();
        }
        private void EmailChanged()
        {
            ValidateEmail();
            if (Validate())
                btnSignIn.IsEnabled = true;
            else btnSignIn.IsEnabled = false;
        }
        
        private void TbEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            EmailChanged();
        }
        private void TbPass_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
            if (Validate())
                btnSignIn.IsEnabled = true;
            else btnSignIn.IsEnabled = false;
        }
        private void TbPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password.Length >= 8)
                TbPass_LostFocus(sender, e);
        }
        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            RegClick?.Invoke();
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public string Email {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
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
        public string Password
        {
            get { return tbPass.Password; }
            private set
            {
                if (value != _errorText)
                {
                    tbPass.Password = value;
                    OnPropertyChanged("Password");
                }
            }
        }
        public bool Validated { get; private set; }
        
        
    }
}
