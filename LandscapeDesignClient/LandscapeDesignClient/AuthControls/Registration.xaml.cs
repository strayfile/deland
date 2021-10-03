using LandscapeDesignClient.Resources;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LandscapeDesignClient.AuthControls
{
    public partial class Registration : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ButtonClicked LoginClick;
        public event ButtonClicked RegClick;
        private string _name;
        private string _email;
        private string _errorText;

        public Registration()
        {
            InitializeComponent();
            gReg.DataContext = this;
            btnReg.IsEnabled = false;
            Validated = false;
        }

        private void ValidateName()
        {
            int error = AuthPerson.ValidateName(tbName.Text);
            if (error != -1)
            {
                tbNameError.Text = "*";
                ErrorText = Texts.Text(error);
            }
            else
            {
                tbNameError.Text = "";
                ErrorText = "";
            }
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
            int error = AuthPerson.ValidatePassword(Password);
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

        private void ValidatePasswordCopy()
        {
            int error = AuthPerson.ValidatePasswordCopy(Password, PasswordConfirm);
            if (error != -1)
            {
                tbPass2Error.Text = "*";
                ErrorText = Texts.Text(error);
            }
            else
            {
                tbPass2Error.Text = "";
                ErrorText = "";
            }
        }

        private bool Validate()
        {
            if (AuthPerson.ValidateName(tbName.Text) != -1 || AuthPerson.ValidateEmail(tbEmail.Text) != -1 || AuthPerson.ValidatePassword(Password) != -1 || AuthPerson.ValidatePasswordCopy(Password, PasswordConfirm) != -1)
                return false;
            return true;
        }

        private void TbName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateName();
            if (Validate())
                btnReg.IsEnabled = true;
            else btnReg.IsEnabled = false;
        }
        private void TbEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateEmail();
            if (Validate())
                btnReg.IsEnabled = true;
            else btnReg.IsEnabled = false;
        }
        private void TbPass_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
            if (Validate())
                btnReg.IsEnabled = true;
            else btnReg.IsEnabled = false;
        }
        private void TbPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password.Length >= 8) TbPass_LostFocus(sender, e);
        }
        private void TbPass2_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePasswordCopy();
            if (Validate())
                btnReg.IsEnabled = true;
            else btnReg.IsEnabled = false;
        }
        private void TbPass2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordConfirm.Length >= 8) TbPass2_LostFocus(sender, e);
        }
        private void Reg_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Validated = Validate();
            if (!Validated)
            {
                ValidateName();
                ValidateEmail();
                ValidatePassword();
                ValidatePasswordCopy();
            }
            else RegClick?.Invoke();
        }
        private void BtnSignInClick(object sender, RoutedEventArgs e)
        {
            LoginClick?.Invoke();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string UserName
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("UserName");
                }
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                if (value != _email)
                {
                    _email = value;
                    OnPropertyChanged("Email");
                }
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
                }
            }
        }
        public string PasswordConfirm
        {
            get { return tbPass2.Password; }
            private set
            {
                if (value != _errorText)
                {
                    tbPass2.Password = value;
                }
            }
        }
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                if (value != _errorText)
                {
                    _errorText = value;
                    OnPropertyChanged("ErrorText");
                }
            }
        }
        public bool Validated { get; private set; }

        
    }
}
