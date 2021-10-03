using LandscapeDesignClient.AccountControls;
using LandscapeDesignClient.AuthControls;
using LandscapeDesignClient.Model;
using LandscapeDesignClient.ProjectMenuControls;
using LandscapeDesignClient.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LandscapeDesignClient
{
    public delegate void ButtonClicked();
    public delegate void UserHandler(User user);
    public delegate User LoginBtnClick(string email, string password);
    public delegate User RegBtnClick(string email, string password, string name = null);
    public delegate void ControlChanged(FrameworkElement control);
    public delegate void ProfileLoadHandler();

    class MainWindowControlsManager
    {
        private static MainWindowControlsManager _instance;
        private static readonly object _sync = new Object();
        private ILandscapeDesignFactory _landDesFactory;
        private User _user;
        private List<FrameworkElement> _controls;
        public event ControlChanged ControlDeleted;
        public event ControlChanged ControlAdded;
        public event ControlChanged LanguageControlAdded;

        private MainWindowControlsManager()
        {
            _controls = new List<FrameworkElement>();
        }
        public void SetLadscapeDesignFactory(ILandscapeDesignFactory landscapeDesignFactory)
        {
            _landDesFactory = landscapeDesignFactory;
            SubscribeToErrorsEvents();
        }
        private void SubscribeToErrorsEvents()
        {
            _landDesFactory.ErrorCatched += LandDesFactoryErrorCatched;
            _landDesFactory.RegErrorCatched += LandDesFactoryRegErrorCatched;
            _landDesFactory.AuthErrorCatched += LandDesFactoryAuthErrorCatched;
            _landDesFactory.ProfileErrorCatched += LandDesFactoryProfileErrorCatched;
            _landDesFactory.ConfirmRegErrorCatched += LandDesFactoryConfirmRegErrorCatched;
            _landDesFactory.LoginErrorCatched += LandDesFactoryLoginErrorCatched;
            _landDesFactory.NotConfirmCatched += SetConfirm;
            _landDesFactory.ToProfile += SetProfile;
            _landDesFactory.UserChanged += SetUser;
        }
        private void LandDesFactoryLoginErrorCatched(string email)
        {
            SetLogin(email);
        }
        private void LandDesFactoryConfirmRegErrorCatched(string error)
        {
            if (this["ctrlConfirm"] is ConfirmControl confirm)
                confirm.ErrorText = error;
        }
        private void LandDesFactoryProfileErrorCatched(string error)
        {
            if (this["ctrlProfile"] is ProfileControl profile)
                profile.ErrorText = error;
        }
        private void LandDesFactoryProfileMessageCatched(string mess)
        {
            MessageWindow w = new MessageWindow(mess, false, MessageBoxButton.OK);
            w.ShowDialog();
        }
        private void LandDesFactoryAuthErrorCatched(string error)
        {
            if (this["ctrlAuthorization"] is Authorization auth)
                auth.ErrorText = error;
        }
        public void LandDesFactoryRegErrorCatched(string error)
        {
            if (this["ctrlRegistration"] is Registration reg)
                reg.ErrorText = error;
        }
        public void LandDesFactoryErrorCatched(string error)
        {
            MessageWindow w = new MessageWindow(error, true, MessageBoxButton.OK);
            w.ShowDialog();
        }

        private void CreateAuthorization(string email = null, string password = null)
        {
            Authorization authnew = null;
            if (email != null || password != null)
                authnew = new Authorization(email, password);
            else authnew = new Authorization();
            authnew.Name = "ctrlAuthorization";
            authnew.LoginClick += Login;
            authnew.RegClick += SetReg;
            authnew.Focusable = true;
            this[authnew.Name] = authnew;
            ControlAdded?.Invoke(authnew);
            LanguageControl lang = CreateLanguageControl();
            LanguageControlAdded?.Invoke(lang);
        }
        private void CreateRegistration()
        {
            Registration regnew = new Registration
            {
                Name = "ctrlRegistration"
            };
            regnew.LoginClick += SetLogin;
            regnew.RegClick += Reg;
            regnew.Focusable = true;
            this[regnew.Name] = regnew;
            ControlAdded?.Invoke(regnew);
            LanguageControl lang = CreateLanguageControl();
            LanguageControlAdded?.Invoke(lang);
        }
        private void CreateConfirmControl()
        {
            ConfirmControl confirmnew = new ConfirmControl
            {
                Name = "ctrlConfirm"
            };
            confirmnew.LoginClick += SetLogin;
            confirmnew.RegClick += SetReg;
            confirmnew.ConfirmClick += Confirm;
            confirmnew.Resend += ResendCode;
            confirmnew.Focusable = true;
            this[confirmnew.Name] = confirmnew;
            ControlAdded?.Invoke(confirmnew);
            LanguageControl lang = CreateLanguageControl();
            LanguageControlAdded?.Invoke(lang);
        }
        private LanguageControl CreateLanguageControl()
        {
            LanguageControl lang = new LanguageControl
            {
                Name = "ctrlLanguage"
            };
            this[lang.Name] = lang;
            return lang;
        }
        private void CreateProfile()
        {
            ProfileControl profile = new ProfileControl(_user)
            {
                Name = "ctrlProfile"
            };
            profile.SignOutClick += SignOut;
            profile.SignOutClick += SetLogin;
            profile.CreateNewProjectClick += SetNewProject;
            profile.OpenProjectClick += SetProject;
            profile.OtherUsersProjectsRefresh += _landDesFactory.LoadOtherUsersProjectsAsync;
            profile.OtherUsersPlantsRefresh += _landDesFactory.LoadOtherUsersPlantsAsync;
            profile.OtherUsersBuildingsRefresh += _landDesFactory.LoadOtherUsersBuildingsAsync;

            profile.SaveAccountClick += EditAccount;
            profile.AddPlantClick += SetPlant;
            profile.EditPlantClick += SetEditPlant;
            profile.DelPlantClick += DeletePlantAsync;
            profile.SharePlantClick += SharePlantAsync;

            this[profile.ctrlLanguageProfile.Name] = profile.ctrlLanguageProfile;

            _landDesFactory.UserProjectsChanged += SetProfileUserProjects;
            _landDesFactory.UserPlantsChanged += SetProfileUserPlants;
            _landDesFactory.UserBuildingsChanged += SetProfileUserBuildings;
            _landDesFactory.OtherUsersProjectsChanged += SetProfileOtherUsersProjects;
            _landDesFactory.OtherUsersPlantsChanged += SetProfileOtherUsersPlants;
            _landDesFactory.OtherUsersBuildingsChanged += SetProfileOtherUsersBuildings;

            this[profile.Name] = profile;
            ControlAdded?.Invoke(profile);
        }
        private void SetProfileOtherUsersProjects()
        {
            if (!(this["ctrlProfile"] is ProfileControl profile))
                return;
            ObservableCollection<Project> projects = new ObservableCollection<Project>();
            foreach (var i in ProjectFactory.GetInstance().Projects.Where(pl => pl.IdUser != _user.Id))
                projects.Add(i);
            profile.SetOtherUserProjects(projects);
        }
        private void SetProfileOtherUsersPlants()
        {
            if (!(this["ctrlProfile"] is ProfileControl profile))
                return;
            profile.SetOtherUserPlants(PlantFactory.GetInstance().GetCategoriesByUser(_user.Id, true));
        }
        private void SetProfileOtherUsersBuildings()
        {
            if (!(this["ctrlProfile"] is ProfileControl profile))
                return;
            profile.SetOtherUserBuildings(BuildingFactory.GetInstance().GetCategoriesByUser(_user.Id, true));
        }
        private void SetProfileUserProjects()
        {
            ProfileControl profile = this["ctrlProfile"] as ProfileControl;
            ObservableCollection<Project> projects = new ObservableCollection<Project>();
            foreach (var i in ProjectFactory.GetInstance().Projects.Where(pl => pl.IdUser == _user.Id))
                projects.Add(i);
            profile.SetUserProjects(projects);
        }
        private void SetProfileUserPlants()
        {
            if (!(this["ctrlProfile"] is ProfileControl profile))
                return;
            profile.SetUserPlants(PlantFactory.GetInstance().GetCategoriesByUser(_user.Id));
        }
        private void SetProfileUserBuildings()
        {
            if (!(this["ctrlProfile"] is ProfileControl profile))
                return;
            profile.SetUserBuildings(BuildingFactory.GetInstance().GetCategoriesByUser(_user.Id));
        }

        private void CreateProject()
        {
            ProjectControl project = new ProjectControl()
            {
                Name = "ctrlProject"
            };
            project.ProfileClick += SetProfile;
            project.SignOutClick += SignOut;
            project.SignOutClick += SetLogin;
            project.CreateNewProjectClick += SetNewProject;
            project.OpenProjectClick += SetProject;

            this[project.Name] = project;
            ControlAdded?.Invoke(project);
        }
        private void CreatePlant(Plant plant = null)
        {
            PlantViewControl plantctrl = new PlantViewControl(plant)
            {
                Name = "ctrlPlant"
            };
            if (plant == null)
                plantctrl.SaveClick += AddPlant;
            else plantctrl.SaveClick += EditPlant;
            plantctrl.CancelClick += SetProfile;
            this[plantctrl.Name] = plantctrl;
            ControlAdded?.Invoke(plantctrl);
        }
        
        private void Login()
        {
            Authorization auth = this["ctrlAuthorization"] as Authorization;
            _landDesFactory.Login(auth.Email, auth.Password);
        }
        private void Reg()
        {
            Registration reg = this["ctrlRegistration"] as Registration;
            _landDesFactory.Register(reg.Email, reg.Password, reg.UserName);
        }
        private void Confirm()
        {
            ConfirmControl confirm = this["ctrlConfirm"] as ConfirmControl;
            _landDesFactory.ConfirmRegister(confirm.Code);
        }
        private void ResendCode()
        {
            _landDesFactory.SendConfirmKey();
        }
        private void EditAccount()
        {
            ProfileControl profile = this["ctrlProfile"] as ProfileControl;
            bool edit1 = true, edit2 = true;
            if (!string.IsNullOrEmpty(profile.UserName))
            {
                edit1 = _landDesFactory.EditUserName(profile.UserName);
                 if (edit1)
                    _user.Name = profile.UserName;
            }
            if (!string.IsNullOrEmpty(profile.PasswordOld))
            {
                edit2 = _landDesFactory.EditUserPassword(profile.PasswordOld, profile.Password);
                if (edit2)
                    _user.Password = profile.Password;
            }
            if (edit1 && edit2)
                profile.Clear();
        }

        private void ClearControls()
        {
            lock (_controls)
            {
                for (int i = _controls.Count - 1; i >= 0; i--)
                {
                    FrameworkElement el = _controls[i];
                    if (el != null)
                    {
                        _controls.Remove(el);
                        ControlDeleted?.Invoke(el);
                    }
                }
            }
        }
       
        //обработчик события перехода к авторизации
        public void SetLogin()
        {
            ClearControls();
            CreateAuthorization();
        }
        public void SetLogin(string email)
        {
            ClearControls();
            CreateAuthorization(email);
        }
        //обработчик события перехода к регистрации
        public void SetReg()
        {
            ClearControls();
            CreateRegistration();
        }
        //обработчик события перехода к подтверждению регистрации
        public void SetConfirm()
        {
            ClearControls();
            CreateConfirmControl();
        }
        //обработчик события перехода к профилю
        public void SetProfile()
        {
            ClearControls();
            CreateProfile();
            _landDesFactory.UpdateProfileData();
        }
        public void SetNewProject()
        {
            ClearControls();
            CreateProject();
        }
        public void SetProject()
        {
            //открыть проводник
            //выбрать проект
            ClearControls();
            CreateProject();
        }
        public void SetPlant()
        {
            ClearControls();
            LanguageControl lang = CreateLanguageControl();
            LanguageControlAdded?.Invoke(lang);
            CreatePlant();
        }
        public void SetEditPlant(int id)
        {
            ClearControls();
            LanguageControl lang = CreateLanguageControl();
            LanguageControlAdded?.Invoke(lang);
            CreatePlant(PlantFactory.GetInstance().GetPlantById(id));
        }
        public void SignOut()
        {
            _landDesFactory.Close();
        }

        public void AddProject(Project project)
        {
            _landDesFactory.AddProject(project);
        }
        public void EditProject(Project project)
        {
            _landDesFactory.EditProject(project);
        }
        public void ShareProjectAsync(int id)
        {
            _landDesFactory.ShareProject(id);
        }
        public void DeleteProjectAsync(int id)
        {
            _landDesFactory.DeleteProject(id);
        }
        public void AddPlant(Plant plant)
        {
            _landDesFactory.AddPlant(plant);
        }
        public void EditPlant(Plant plant)
        {
            _landDesFactory.EditPlant(plant);
        }
        public void SharePlantAsync(int id)
        {
            _landDesFactory.SharePlant(id);
        }
        public void DeletePlantAsync(int id)
        {
            _landDesFactory.DeletePlant(id);
        }
        public void AddBuilding(Building building)
        {
            _landDesFactory.AddBuilding(building);
        }
        public void EditBuilding(Building building)
        {
            _landDesFactory.EditBuilding(building);
        }
        public void ShareBuildingAsync(int id)
        {
            _landDesFactory.ShareBuilding(id);
        }
        public void DeleteBuildingAsync(int id)
        {
            _landDesFactory.DeleteBuilding(id);
        }

        private void SetUser(User user)
        {
            _user = user;
        }
        private void SetUserName(string name)
        {
            _user.Name = name;
        }
        public FrameworkElement this[string name]
        {
            get
            {
                lock (_controls)
                {
                     return _controls.FirstOrDefault(c => c.Name == name);
                }
            }
            set
            {
                lock (_controls)
                {
                    if (this[name] == null)
                        _controls.Add(value);
                    else
                    {
                        _controls.Remove(this[name]);
                        _controls.Add(value);
                    }
                }
            }
        }
        public static MainWindowControlsManager GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new MainWindowControlsManager();
                }
            }
            return _instance;
        }
    }
}
