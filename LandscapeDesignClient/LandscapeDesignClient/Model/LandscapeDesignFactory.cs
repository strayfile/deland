using System;
using System.Linq;
using System.Threading.Tasks;
using LandscapeDesignClient.LandscapeDesignReference;
using System.ServiceModel;
using System.Collections.ObjectModel;
using LandscapeDesignClient.Resources;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

namespace LandscapeDesignClient.Model
{
    public class LandscapeDesignFactory : ILandscapeDesignFactory, IDisposable
    {
        private static LandscapeDesignFactory _instance;
        private static readonly object _sync = new Object();
        private User _user;
        private ProjectFactory _projectFactory;
        private PlantFactory _plantFactory;
        private BuildingFactory _buildingFactory;
        private readonly PlantCharacteristics _plantCharacteristics;
        private EntityTextures _plantTextures;
        private EntityTextures _buildingTextures;

        private LandscapeDesignServiceClient _ldProxy;
        public event MessageHandler ErrorCatched;
        public event MessageHandler RegErrorCatched;
        public event MessageHandler AuthErrorCatched;
        public event MessageHandler ProfileErrorCatched;
        public event MessageHandler ConfirmRegErrorCatched;
        public event MessageHandler LoginErrorCatched;
        public event ButtonClicked NotConfirmCatched;
        public event UserHandler UserChanged;

        public event ButtonClicked ToProfile;
        public event ButtonClicked UserProjectsChanged;
        public event ButtonClicked UserPlantsChanged;
        public event ButtonClicked UserBuildingsChanged;
        public event ButtonClicked OtherUsersProjectsChanged;
        public event ButtonClicked OtherUsersPlantsChanged;
        public event ButtonClicked OtherUsersBuildingsChanged;

        private LandscapeDesignFactory()
        {
            _plantFactory = PlantFactory.GetInstance();
            _buildingFactory = BuildingFactory.GetInstance();
            _plantCharacteristics = PlantCharacteristics.GetInstance();
            _plantTextures = PlantTextures.GetInstance();
            _buildingTextures = BuildingTextures.GetInstance();
            _projectFactory = ProjectFactory.GetInstance();
        }

        private bool ReLogin()
        {
            try
            {
                _ldProxy = new LandscapeDesignServiceClient();
                LDUser newuser = _ldProxy.Login(_user.Email, _user.Password);
                _user = LandscapeDesignModelAdapter.GetUser(newuser);
                return true;
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                {
                    if (e == 215)
                        NotConfirmCatched?.Invoke();
                    else LoginErrorCatched?.Invoke(_user.Email);
                }
                else LoginErrorCatched?.Invoke(_user.Email);
            }
            catch (FaultException)
            {
                LoginErrorCatched?.Invoke(_user.Email);
            }
            catch (Exception)
            {
                ErrorCatched?.Invoke(Texts.Text(102));
            }
            return false;
        }


        private async void LoadUserProjectsAsync()
        {
            string err = null;
            int e = -1;
            try
            {
                var projects = await _ldProxy.GetProjectsAsync();
                if (projects == null)
                    return;
                foreach (var pl in projects)
                    if (_projectFactory.Projects.FirstOrDefault(p => p.Id == pl.Id) == null)
                        _projectFactory.Projects.Add(LandscapeDesignModelAdapter.GetProject(pl));
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException<InvalidProjectsFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
                });
                return;
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReLogin())
                        LoadUserProjectsAsync();
                });

                return;
            }
            if (err == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserProjectsChanged?.Invoke();
                });
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e != -1)
                    ProfileErrorCatched?.Invoke(Texts.Text(e));
                else ProfileErrorCatched?.Invoke(err);
            });
        }
        public async void LoadOtherUsersProjectsAsync()
        {
            string err = null;
            int e = -1;
            try
            {
                var projects = await _ldProxy.GetOtherUsersProjectsAsync();
                if (projects == null)
                    return;
                foreach (var pl in projects)
                    if (_projectFactory.Projects.FirstOrDefault(p => p.Id == pl.Id) == null)
                        _projectFactory.Projects.Add(LandscapeDesignModelAdapter.GetProject(pl));
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException<InvalidProjectsFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
                });
                return;
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReLogin())
                        LoadOtherUsersProjectsAsync();
                });
                return;
            }
            if (err == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OtherUsersProjectsChanged?.Invoke();
                });
                return;
            }
            if (e != -1)
                ProfileErrorCatched?.Invoke(Texts.Text(e));
            else ProfileErrorCatched?.Invoke(err);
        }

        private async void LoadUserPlantsAsync()
        {
            string err = null;
            int e = -1;
            try
            {
                ObservableCollection<LDPlant> plants = await _ldProxy.GetAllUserPlantsAsync();
                if (plants == null)
                    return;
                foreach (var pl in plants)
                    if (_plantFactory.GetPlantById(pl.Id) == null)
                        _plantFactory.AddPlant(LandscapeDesignModelAdapter.GetPlant(pl));
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
                });
                return;
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReLogin())
                        LoadUserPlantsAsync();
                });
                return;
            }
            if (err == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserPlantsChanged?.Invoke();
                });
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e != -1)
                    ProfileErrorCatched?.Invoke(Texts.Text(e));
                else ProfileErrorCatched?.Invoke(err);
            });
        }
        public async void LoadOtherUsersPlantsAsync()
        {
            string err = null;
            int e = -1;
            try
            {
                ObservableCollection<LDPlant> plants = await _ldProxy.GetOtherUsersPlantsAsync();
                if (plants == null)
                    return;
                foreach (var pl in plants)
                    if (_plantFactory.GetPlantById(pl.Id) == null)
                        _plantFactory.AddPlant(LandscapeDesignModelAdapter.GetPlant(pl));
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
                });
                return;
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReLogin())
                        LoadOtherUsersPlantsAsync();
                });
                return;
            }
            if (err == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OtherUsersPlantsChanged?.Invoke();
                });
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e != -1)
                    ProfileErrorCatched?.Invoke(Texts.Text(e));
                else ProfileErrorCatched?.Invoke(err);
            });
        }
        
        private async void LoadUserBuildingsAsync()
        {
            string err = null;
            int e = -1;
            try
            {
                ObservableCollection<LDBuilding> buildings = await _ldProxy.GetAllUserBuildingsAsync();
                if (buildings != null)
                {
                    foreach (var pl in buildings)

                        if (_buildingFactory.GetBuildingById(pl.Id) == null)
                            _buildingFactory.AddBuilding(LandscapeDesignModelAdapter.GetBuilding(pl));
                }
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
                });
                return;
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReLogin())
                        LoadUserBuildingsAsync();
                });
                return;
            }
            if (err == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserBuildingsChanged?.Invoke();
                });
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e != -1)
                    ProfileErrorCatched?.Invoke(Texts.Text(e));
                else ProfileErrorCatched?.Invoke(err);
            });
        }
        public async void LoadOtherUsersBuildingsAsync()
        {
            string err = null;
            int e = -1;
            try
            {
                ObservableCollection<LDBuilding> buildings = await _ldProxy.GetOtherUsersBuildingsAsync();
                if (buildings == null)
                    return;
                foreach (var pl in buildings)
                    if (_buildingFactory.GetBuildingById(pl.Id) == null)
                        _buildingFactory.AddBuilding(LandscapeDesignModelAdapter.GetBuilding(pl));
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
                });
                return;
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReLogin())
                        LoadOtherUsersBuildingsAsync();
                });
                return;
            }
            if (err == null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OtherUsersBuildingsChanged?.Invoke();
                });
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e != -1)
                    ProfileErrorCatched?.Invoke(Texts.Text(e));
                else ProfileErrorCatched?.Invoke(err);
            });
        }
       
        private async Task LoadUserPlantsCharacteristicsAsync()
        {
            var t = await _ldProxy.GetPlantsCharacteristicsAsync();
            LandscapeDesignModelAdapter.AddPlantsCharacteristics(t);
        }
        private async Task LoadPlantsTexturesAsync()
        {
            LDEntityTexture texture = null;
            do
            {
                texture = await _ldProxy.GetPlantTextureAsync(_plantTextures.Ids);
                if (texture == null)
                    return;
                try
                {
                    using (var stream = new MemoryStream(texture.Texture))
                    {
                        //BitmapFrame bitmap = BitmapFrame.Create(stream);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        _plantTextures.AddTexture(texture.Id, texture.IdUser, bitmap, texture.Share);
                    }
                }
                catch (Exception)
                {
                    ProfileErrorCatched(Texts.Text(314));
                }

            } while (texture != null);
        }
        private async Task LoadUserBuildingCharacteristicsAsync()
        {
            LDBuildingCharacteristics bldch = await _ldProxy.GetBuildingCharacteristicsAsync();
            LandscapeDesignModelAdapter.AddBuildingsCharacteristics(bldch);
        }
        private async Task LoadBuildingTexturesAsync()
        {
            LDEntityTexture texture = null;
            do
            {
                texture = await _ldProxy.GetPlantTextureAsync(_plantTextures.Ids);
                if (texture == null)
                    return;
                try
                {
                    using (var stream = new MemoryStream(texture.Texture))
                    {
                        //BitmapFrame bitmap = BitmapFrame.Create(stream);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        _buildingTextures.AddTexture(texture.Id, texture.IdUser, bitmap, texture.Share);
                    }
                }
                catch (Exception)
                {
                    ProfileErrorCatched(Texts.Text(314));
                }

            } while (texture != null);
        }
        
        private void ErrorMessage(string err)
        {
            if (Int32.TryParse(err, out int e))
                err = Texts.Text(e);
            else err = Texts.Text(100);
            ErrorCatched?.Invoke(err);
        }
        public void UpdateProfileData()
        {
            UserProjectsChanged?.Invoke();
            UserPlantsChanged?.Invoke();
            UserBuildingsChanged?.Invoke();
            OtherUsersProjectsChanged?.Invoke();
            OtherUsersPlantsChanged?.Invoke();
            OtherUsersBuildingsChanged?.Invoke();
        }

        public void Login(string email, string pass)
        {
            try
            {
                _user = new User(-1, email) { Password = pass };
                _ldProxy = new LandscapeDesignServiceClient();
                LDUser newuser = _ldProxy.Login(email, pass);
                _user = LandscapeDesignModelAdapter.GetUser(newuser);
                _user.Password = pass;
                UserChanged?.Invoke(_user);
                ToProfile?.Invoke();
                OpeningProfile();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                {
                    if (e == 215)
                    {
                        _user = new User(-1, email) { Password = pass };
                        NotConfirmCatched?.Invoke();
                    }
                    else AuthErrorCatched?.Invoke(Texts.Text(e));
                }
                else AuthErrorCatched?.Invoke(ex.Message);
            }
            catch (FaultException ex)
            {
                AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
            }
            catch (Exception)
            {
                ErrorCatched?.Invoke(Texts.Text(102));
            }
        }
        public async void Register(string email, string pass, string name = null)
        {
            try
            {
                _user = new User(-1, email) { Password = pass };
                _ldProxy = new LandscapeDesignServiceClient();
                LDUser user = null;
                await Task.Run(async () => {
                    user = await _ldProxy.RegisterAsync(email, pass, name);
                });
                _user = LandscapeDesignModelAdapter.GetUser(user);
                _user.Password = pass;
                UserChanged?.Invoke(_user);
                NotConfirmCatched?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    RegErrorCatched?.Invoke(Texts.Text(e));
                else RegErrorCatched?.Invoke(ex.Message);
            }
            catch (FaultException ex)
            {
                RegErrorCatched?.Invoke($"{Texts.Text(100)}\n{ex.Message}");
            }
            catch (Exception)
            {
                ErrorCatched?.Invoke(Texts.Text(102));
            }
        }
        public async void SendConfirmKey()
        {
            try
            {
                await Task.Run(async () => {
                    await _ldProxy.SendConfirmKeyAsync(_user.Email, _user.Password);
                });
                ConfirmRegErrorCatched?.Invoke(Texts.Text(216));
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    ConfirmRegErrorCatched?.Invoke(Texts.Text(e));
                else ConfirmRegErrorCatched?.Invoke(ex.Message);
            }
            catch (FaultException ex)
            {
                ConfirmRegErrorCatched?.Invoke($"{Texts.Text(100)}\n{ex.Message}");
            }
            catch (Exception)
            {
                ReLogin();
            }
        }
        public void ConfirmRegister(string key)
        {
            try
            {
                _ldProxy.ConfirmRegister(_user.Email, key);
                Login(_user.Email, _user.Password);
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                {
                    ConfirmRegErrorCatched?.Invoke(Texts.Text(e));
                }
                else ConfirmRegErrorCatched?.Invoke(ex.Message);
            }
            catch (FaultException ex)
            {
                ConfirmRegErrorCatched?.Invoke($"{Texts.Text(100)}\n{ex.Message}");
            }
            catch (Exception)
            {
                ErrorCatched?.Invoke(Texts.Text(102));
            }
        }

        public bool EditUserName(string name)
        {
            try
            {
                if (name == _user.Name)
                    return true;
                Task.Run(async () =>
                {
                    await _ldProxy.EditUserNameAsync(name);
                }).GetAwaiter().GetResult();
                return true;
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    ProfileErrorCatched?.Invoke(Texts.Text(e));
                else ProfileErrorCatched?.Invoke(ex.Message);
            }
            catch (FaultException ex)
            {
                ProfileErrorCatched?.Invoke($"{Texts.Text(100)}\n{ex.Message}");
            }
            catch (Exception)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                    if (ReLogin())
                        EditUserName(name);
            }
            return false;
        }
        public bool EditUserPassword(string pass, string newpass)
        {
            try
            {
                Task.Run(async () =>
                {
                    await _ldProxy.EditUserPasswordAsync(pass, newpass);
                }).GetAwaiter().GetResult();
                return true;
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    ProfileErrorCatched?.Invoke(Texts.Text(e));
                else ProfileErrorCatched?.Invoke(ex.Message);
            }
            catch (FaultException ex)
            {
                ProfileErrorCatched?.Invoke($"{Texts.Text(100)}\n{ex.Message}");
            }
            catch (Exception)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                    if (ReLogin())
                        EditUserPassword(pass, newpass);
            }
            return false;
        }
        public async void LoadPlants()
        {
            string err = null;
            int e = -1;
            try
            {
                await LoadUserPlantsCharacteristicsAsync();
                await LoadPlantsTexturesAsync();
                LoadUserPlantsAsync();
                LoadOtherUsersPlantsAsync();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException ex)
            {
                AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
                return;
            }
            catch (Exception)
            {
                if (ReLogin())
                    LoadPlants();
                return;
            }
            if (err == null)
            {
                UserPlantsChanged?.Invoke();
                return;
            }
            if (e != -1)
                ProfileErrorCatched?.Invoke(Texts.Text(e));
            else ProfileErrorCatched?.Invoke(err);
        }
        public async void LoadBuildings()
        {
            string err = null;
            int e = -1;
            try
            {
                await LoadUserBuildingCharacteristicsAsync();
                await LoadBuildingTexturesAsync();
                LoadUserBuildingsAsync();
                LoadOtherUsersBuildingsAsync();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                if (!Int32.TryParse(ex.Message, out e))
                    err = ex.Message;
            }
            catch (FaultException ex)
            {
                AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
                return;
            }
            catch (Exception)
            {
                if (ReLogin())
                    LoadBuildings();
                return;
            }
            if (err == null)
            {
                UserBuildingsChanged?.Invoke();
                return;
            }
            if (e != -1)
                ProfileErrorCatched?.Invoke(Texts.Text(e));
            else ProfileErrorCatched?.Invoke(err);
        }
        public void OpeningProfile()
        {
            LoadUserProjectsAsync();
            LoadOtherUsersProjectsAsync();
            LoadPlants();
            LoadBuildings();
        }
        //public async void RefreshOtherUsersProjects()
        //{
        //    string err = null;
        //    int e = -1;
        //    try
        //    {
        //        await Task.Run(async () =>
        //        {
        //            await LoadOtherUsersProjectsAsync();
        //        });
        //        OtherUsersProjectsChanged?.Invoke();
        //    }
        //    catch (FaultException<InvalidUserFault> ex)
        //    {
        //        if (!Int32.TryParse(ex.Message, out e))
        //            err = ex.Message;
        //    }
        //    catch (FaultException<InvalidProjectsFault> ex)
        //    {
        //        if (!Int32.TryParse(ex.Message, out e))
        //            err = ex.Message;
        //    }
        //    catch (FaultException ex)
        //    {
        //        AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
        //        return;
        //    }
        //    catch (Exception)
        //    {
        //        if (ReLogin())
        //            RefreshOtherUsersProjects();
        //        return;
        //    }
        //    if (err == null)
        //    {
        //        OtherUsersProjectsChanged?.Invoke();
        //        return;
        //    }
        //    if (e != -1)
        //        ProfileErrorCatched?.Invoke(Texts.Text(e));
        //    else ProfileErrorCatched?.Invoke(err);
        //}
        //public async void RefreshOtherUsersPlants()
        //{
        //    string err = null;
        //    int e = -1;
        //    try
        //    {
        //        await Task.Run(async () =>
        //        {
        //            LoadUserPlantsCharacteristicsAsync().GetAwaiter();
        //            await LoadOtherUsersPlantsAsync();
        //        });
        //        OtherUsersPlantsChanged?.Invoke();
        //    }
        //    catch (FaultException<InvalidUserFault> ex)
        //    {
        //        if (!Int32.TryParse(ex.Message, out e))
        //            err = ex.Message;
        //    }
        //    catch (FaultException<InvalidProjectItemFault> ex)
        //    {
        //        if (!Int32.TryParse(ex.Message, out e))
        //            err = ex.Message;
        //    }
        //    catch (FaultException ex)
        //    {
        //        AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
        //        return;
        //    }
        //    catch (Exception)
        //    {
        //        if (ReLogin())
        //            RefreshOtherUsersPlants();
        //        return;
        //    }
        //    if (err == null)
        //    {
        //        OtherUsersPlantsChanged?.Invoke();
        //        return;
        //    }
        //    if (e != -1)
        //        ProfileErrorCatched?.Invoke(Texts.Text(e));
        //    else ProfileErrorCatched?.Invoke(err);
        //}
        //public async void RefreshOtherUsersBuildings()
        //{

        //    string err = null;
        //    int e = -1;
        //    try
        //    {
        //        await Task.Run(async () =>
        //        {
        //            LoadUserBuildingCharacteristicsAsync().GetAwaiter();
        //            await LoadOtherUsersBuildingsAsync();
        //        });
        //        OtherUsersBuildingsChanged?.Invoke();
        //    }
        //    catch (FaultException<InvalidUserFault> ex)
        //    {
        //        if (!Int32.TryParse(ex.Message, out e))
        //            err = ex.Message;
        //    }
        //    catch (FaultException<InvalidProjectItemFault> ex)
        //    {
        //        if (!Int32.TryParse(ex.Message, out e))
        //            err = ex.Message;
        //    }
        //    catch (FaultException ex)
        //    {
        //        AuthErrorCatched?.Invoke($"{Texts.Text(100)}.\n{ex.Message}");
        //        return;
        //    }
        //    catch (Exception)
        //    {
        //        if (ReLogin())
        //            RefreshOtherUsersBuildings();
        //        return;
        //    }
        //    if (err == null)
        //    {
        //        OtherUsersBuildingsChanged?.Invoke();
        //        return;
        //    }
        //    if (e != -1)
        //        ProfileErrorCatched?.Invoke(Texts.Text(e));
        //    else ProfileErrorCatched?.Invoke(err);
        //}
        public async void AddProject(Project project)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int id = await _ldProxy.AddProjectAsync(LandscapeDesignModelAdapter.GetProject(project));
                    project.Id = id;
                });
                _projectFactory.AddProject(project);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectsFault> ex)
            {
                throw new Exception(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        AddProject(project);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void EditProject(Project project)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await _ldProxy.EditProjectAsync(LandscapeDesignModelAdapter.GetProject(project));
                });
                _projectFactory.RemoveById(project.Id);
                _projectFactory.AddProject(project);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                ErrorMessage(ex.Message);

            }
            catch (FaultException<InvalidProjectsFault> ex)
            {
                throw new Exception(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        EditProject(project);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void ShareProject(int id)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await _ldProxy.ShareProjectAsync(id);
                });
                _projectFactory.GetById(id).Share = true;
                UserProjectsChanged?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectsFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        ShareProject(id);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void DeleteProject(int id)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await _ldProxy.DeleteProjectAsync(id);
                });
                _projectFactory.RemoveById(id);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectsFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        DeleteProject(id);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void AddPlant(Plant plant)
        {
            try
            {
                await Task.Run(async () =>
                {
                    plant.IdUser = _user.Id;
                    int id = await _ldProxy.AddPlantAsync(LandscapeDesignModelAdapter.GetPlant(plant));
                    plant.Id = id;
                });
                _plantFactory.AddPlant(plant);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        AddPlant(plant);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void EditPlant(Plant plant)
        {
            try
            {
                plant.IdUser = _user.Id;
                await Task.Run(async () =>
                {
                    await _ldProxy.EditPlantAsync(LandscapeDesignModelAdapter.GetPlant(plant));
                });
                _plantFactory.RemovePlantById(plant.Id);
                _plantFactory.AddPlant(plant);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        EditPlant(plant);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void SharePlant(int id)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await _ldProxy.SharePlantAsync(id);
                });
                _plantFactory.GetPlantById(id).Share = true;
                UserPlantsChanged?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        SharePlant(id);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void DeletePlant(int id)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await _ldProxy.DeletePlantAsync(id);
                });
                _plantFactory.RemovePlantById(id);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        DeletePlant(id);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void AddBuilding(Building building)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int id = await _ldProxy.AddBuildingAsync(LandscapeDesignModelAdapter.GetBuilding(building));
                    building.Id = id;
                });
                _buildingFactory.AddBuilding(building);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        AddBuilding(building);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void EditBuilding(Building building)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await _ldProxy.EditBuildingAsync(LandscapeDesignModelAdapter.GetBuilding(building));
                });
                _buildingFactory.RemoveBuildingById(building.Id);
                _buildingFactory.AddBuilding(building);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        EditBuilding(building);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void ShareBuilding(int id)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await _ldProxy.ShareBuildingAsync(id);
                });
                _buildingFactory.GetBuildingById(id).Share = true;
                UserBuildingsChanged?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        ShareBuilding(id);
                    else ErrorMessage(ex.Message);
                }
            }
        }
        public async void DeleteBuilding(int id)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await _ldProxy.DeleteBuildingAsync(id);
                });
                _buildingFactory.RemoveBuildingById(id);
                ToProfile?.Invoke();
            }
            catch (FaultException<InvalidUserFault> ex)
            {
                if (Int32.TryParse(ex.Message, out int e))
                    LoginErrorCatched?.Invoke(Texts.Text(e));
                else throw new Exception(Texts.Text(100));

            }
            catch (FaultException<InvalidProjectItemFault> ex)
            {
                ErrorMessage(ex.Message);
            }
            catch (FaultException)
            {
                throw new Exception(Texts.Text(100));
            }
            catch (Exception ex)
            {
                if (_ldProxy.State == CommunicationState.Faulted)
                {
                    if (ReLogin())
                        DeleteBuilding(id);
                    else ErrorMessage(ex.Message);
                }
            }
        }

        public void Close()
        {
            Dispose();
            _user = null;
            BuildingFactory.GetInstance().Clear();
            PlantFactory.GetInstance().Clear();
            ProjectFactory.GetInstance().Clear();
        }
        public void Dispose()
        {
            if (_ldProxy != null)
            {
                try
                {
                    if (_ldProxy.State == CommunicationState.Opened)
                    {
                        _ldProxy.Close();
                        ((IDisposable)_ldProxy).Dispose();
                    }
                }
                catch (Exception)
                {
                    _ldProxy.Abort();
                }
            }
        }
        public static LandscapeDesignFactory GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new LandscapeDesignFactory();
                }
            }
            return _instance;
        }
    }
}
