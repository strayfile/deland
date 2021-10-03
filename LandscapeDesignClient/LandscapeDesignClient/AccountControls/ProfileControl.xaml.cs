using LandscapeDesignClient.AuthControls;
using LandscapeDesignClient.Model;
using LandscapeDesignClient.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LandscapeDesignClient.AccountControls
{
    public delegate void ProjectItemHandler(int id);
    public delegate void PlantHandler(Plant plant);
    public delegate void BuildingHandler(Building building);
    public delegate void ProjectHandler(Project project);

    public partial class ProfileControl : UserControl, INotifyPropertyChanged
    {
        private User _user;
        private string _errorText;
        public event PropertyChangedEventHandler PropertyChanged;
        public event ButtonClicked SaveAccountClick;
        public event ButtonClicked SignOutClick;
        public event ButtonClicked CreateNewProjectClick;
        public event ButtonClicked OpenProjectClick;

        public event ButtonClicked AddPlantClick;
        public event ProjectItemHandler EditPlantClick;
        public event ProjectItemHandler DelPlantClick;
        public event ProjectItemHandler SharePlantClick;

        public event ButtonClicked AddBuildingClick;
        public event ProjectItemHandler EditBuildingClick;
        public event ProjectItemHandler DelBuildingClick;
        public event ProjectItemHandler ShareBuildingClick;

        public event ButtonClicked DelProjectClick;
        public event ButtonClicked ShareProjectClick;

        public event ButtonClicked OtherUsersProjectsRefresh;
        public event ButtonClicked OtherUsersPlantsRefresh;
        public event ButtonClicked OtherUsersBuildingsRefresh;

        public ProfileControl(User user)
        {
            InitializeComponent();
            _user = user;
            Projects = new ObservableCollection<Project>();
            OtherUsersProjects = new ObservableCollection<Project>();
            Buildings = new ObservableCollection<BuildingsCategory>();
            OtherUsersBuildings = new ObservableCollection<BuildingsCategory>();
            Plants = new ObservableCollection<PlantsCategory>();
            OtherUsersPlants = new ObservableCollection<PlantsCategory>();
            tbError.DataContext = this;
            Clear();
        }

        private void ValidateName()
        {
            int error = AuthPerson.ValidateName(UserName);
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
        private void ValidatePasswordOld()
        {
            int error = AuthPerson.ValidatePassword(PasswordOld);
            if (error != -1)
            {
                tbPassOldError.Text = "*";
                ErrorText = Texts.Text(error);
            }
            else
            {
                tbPassOldError.Text = "";
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
            bool eName = string.IsNullOrEmpty(UserName);
            if (eName && string.IsNullOrEmpty(PasswordOld) && string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(PasswordConfirm))
                return false;
            if (!string.IsNullOrEmpty(UserName) && AuthPerson.ValidateName(UserName) != -1)
                return false;
            if (PasswordOld.Length == 0 && Password.Length == 0 && PasswordConfirm.Length == 0 && !eName && UserName != _user.Name)
                return true;
            else if (AuthPerson.ValidatePassword(PasswordOld) == -1 && AuthPerson.ValidatePassword(Password) == -1 && AuthPerson.ValidatePassword(PasswordConfirm) == -1 && AuthPerson.ValidatePasswordCopy(Password, PasswordConfirm) == -1)
                return true;
            return false;
        }
        private void TbName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateName();
            if (Validate())
                btnSaveAccount.IsEnabled = true;
            else btnSaveAccount.IsEnabled = false;
        }
        private void TbNameChanged(object sender, RoutedEventArgs e)
        {
            TbName_LostFocus(sender, e);
        }
        private void TbPassOldLostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePasswordOld();
            if (Validate())
                btnSaveAccount.IsEnabled = true;
            else btnSaveAccount.IsEnabled = false;
        }
        private void TbPassOldPasswordChanged(object sender, RoutedEventArgs e)
        {
            TbPassOldLostFocus(sender, e);
        }
        private void TbPassLostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
            if (Validate())
                btnSaveAccount.IsEnabled = true;
            else btnSaveAccount.IsEnabled = false;
        }
        private void TbPassPasswordChanged(object sender, RoutedEventArgs e)
        {
            TbPassLostFocus(sender, e);
        }
        private void TbPass2LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePasswordCopy();
            if (Validate())
                btnSaveAccount.IsEnabled = true;
            else btnSaveAccount.IsEnabled = false;
        }
        private void TbPass2PasswordChanged(object sender, RoutedEventArgs e)
        {
            TbPass2LostFocus(sender, e);
        }

        private void ShowUserProfileEditorCard(bool show)
        {
            if (show)
            {
                cardUserProfile.Visibility = Visibility.Hidden;
                cardUserProfileEditer.Visibility = Visibility.Visible;
            }
            else
            {
                cardUserProfile.Visibility = Visibility.Visible;
                cardUserProfileEditer.Visibility = Visibility.Hidden;
                ClearProfileCard();
            }
        }
        private void BtnEditUserProfile_Click(object sender, RoutedEventArgs e)
        {
            ShowUserProfileEditorCard(true);
        }
        private void BtnUserProfileCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            ShowUserProfileEditorCard(false);

        }

        private void ShowProjects(bool user)
        {
            if (user)
            {
                gUserPr.Visibility = Visibility.Visible;
                gOtherPr.Visibility = Visibility.Hidden;
                btnShowProjects.Visibility = Visibility.Hidden;
                btnShowOtherUsersProjects.Visibility = Visibility.Visible;
            }
            else
            {
                gUserPr.Visibility = Visibility.Hidden;
                gOtherPr.Visibility = Visibility.Visible;
                btnShowProjects.Visibility = Visibility.Visible;
                btnShowOtherUsersProjects.Visibility = Visibility.Hidden;
            }
        }
        private void BtnShowProjects_Click(object sender, RoutedEventArgs e)
        {
            ShowProjects(true);
        }
        private void BtnShowOtherUsersProjects_Click(object sender, RoutedEventArgs e)
        {
            ShowProjects(false);
        }
        private void BtnSaveAccount_Click(object sender, RoutedEventArgs e)
        { 
            Validated = Validate();
            if (!Validated)
            {
                ValidateName();
                ValidatePasswordOld();
                ValidatePassword();
                ValidatePasswordCopy();
            }
            else SaveAccountClick?.Invoke();
        }

        private void ButtonAddNewPlant_Click(object sender, RoutedEventArgs e)
        {
            AddPlantClick?.Invoke();
        }
        private void ButtonAddNewBuilding_Click(object sender, RoutedEventArgs e)
        {
            AddBuildingClick?.Invoke();
        }
        
        private void CommandCreateNewProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNewProjectClick?.Invoke();
        }
        private void CommandOpenProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenProjectClick?.Invoke();
        }
        private void CommandSignOutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SignOutClick?.Invoke();
        }
        private void ButtonRefreshOtherUsersProjects_Click(object sender, RoutedEventArgs e)
        {
            OtherUsersProjectsRefresh?.Invoke();
        }
        private void ButtonRefreshOtherUsersPlants_Click(object sender, RoutedEventArgs e)
        {
            OtherUsersPlantsRefresh?.Invoke();
        }
        private void ButtonRefreshOtherUsersBuildings_Click(object sender, RoutedEventArgs e)
        {
            OtherUsersBuildingsRefresh?.Invoke();
        }
        private void ClearProfileCard()
        {
            UserName = _user.Name;
            PasswordOld = "";
            tbPassOld.Password = "";
            Password = "";
            tbPassOldError.Text = "";
            PasswordConfirm = "";
            tbPass2.Password = "";
            tbError.Text = "";
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string UserName
        {
            get { return tbName.Text; }
            set
            {
                if (value != tbName.Text)
                {
                    tbName.Text = value;
                }
            }
        }
        public string PasswordOld
        {
            get { return tbPassOld.Password; }
            private set
            {
                if (value != tbPassOld.Password)
                {
                    tbPassOld.Password = value;
                }
            }
        }
        public string Password
        {
            get { return tbPass.Password; }
            private set
            {
                if (value != tbPass.Password)
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
                if (value != tbPass2.Password)
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
        public Visibility IsNameVisible
        {
            get {
                if (UserName != null)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
        }
        public void ButtonEditPlantClick(int id)
        {
            EditPlantClick?.Invoke(id);
        }
        public void ButtonDeletePlantClick(int id)
        {
            DelPlantClick?.Invoke(id);
        }
        public void ButtonSharePlantClick(int id)
        {
            SharePlantClick?.Invoke(id);
        }
        public void ButtonEditBuildingClick(int id)
        {
            EditBuildingClick?.Invoke(id);
        }
        public void ButtonDeleteBuildingClick(int id)
        {
            DelBuildingClick?.Invoke(id);
        }
        public void ButtonShareBuildingClick(int id)
        {
            ShareBuildingClick?.Invoke(id);
        }
        public void SetUserPlants(ObservableCollection<PlantsCategory> categories)
        {
            Plants = categories;
            tvUserPlants.ItemsSource = Plants;
            tvUserPlants.DataContext = Plants;
            OnPropertyChanged("PlantsCount");
        }
        public void SetOtherUserPlants(ObservableCollection<PlantsCategory> categories)
        {
            OtherUsersPlants = categories;
            tvOtherUsersPlants.ItemsSource = OtherUsersPlants;
            tvOtherUsersPlants.DataContext = OtherUsersPlants;
        }
        public void SetUserBuildings(ObservableCollection<BuildingsCategory> categories)
        {
            Buildings = categories;
            tvUserBuildings.ItemsSource = Buildings;
            tvUserBuildings.DataContext = Buildings;
            OnPropertyChanged("BuildingsCount");
        }
        public void SetOtherUserBuildings(ObservableCollection<BuildingsCategory> categories)
        {
            OtherUsersBuildings = categories;
            tvOtherUsersBuildings.ItemsSource = OtherUsersBuildings;
            tvOtherUsersBuildings.DataContext = OtherUsersBuildings;
        }
        public void SetUserProjects(ObservableCollection<Project> projects)
        {
            Projects = projects;
            lvUserProjects.ItemsSource = Projects;
            lvUserProjects.DataContext = Projects;
            OnPropertyChanged("ProjectsCount");
        }
        public void SetOtherUserProjects(ObservableCollection<Project> projects)
        {
            OtherUsersProjects = projects;
            lvOtherUsersProjects.ItemsSource = OtherUsersProjects;
            lvOtherUsersProjects.DataContext = OtherUsersProjects;
        }
        public void Clear()
        {
            ClearProfileCard();
            gProfile.DataContext = _user;
            UserName = _user.Name;
            gProfileData.DataContext = this;
            gProfileName.DataContext = this;
            tbUserName.DataContext = _user;
            ShowUserProfileEditorCard(false);
        }
        public string ProjectsCount {
            get
            {
                if (Projects != null)
                    return Projects.Count().ToString();
                return "0";
            }
        }
        public string PlantsCount
        {
            get
            {
                if (Plants != null)
                    return Plants.Select(c => c.Plants.Count).Sum().ToString();
                return "0";
            }
        }
        public string BuildingsCount
        {
            get
            {
                if (Buildings != null)
                    return Buildings.Select(c => c.Buildings.Count).Sum().ToString();
                return "0";
            }
        }
        public ObservableCollection<PlantsCategory> Plants { get; private set; }
        public ObservableCollection<PlantsCategory> OtherUsersPlants { get; private set; }
        public ObservableCollection<BuildingsCategory> Buildings { get; private set; }
        public ObservableCollection<BuildingsCategory> OtherUsersBuildings { get; private set; }
        public ObservableCollection<Project> Projects { get; private set; }
        public ObservableCollection<Project> OtherUsersProjects { get; private set; }

        
    }
}
