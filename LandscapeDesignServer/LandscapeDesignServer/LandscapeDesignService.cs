using System;
using System.ServiceModel;
using LandscapeDesignServer.Model;
using LandscapeDesignServer.LandscapeDesignModel;
using LandscapeDesignServer.ModelAdapter;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LandscapeDesignServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single, ReleaseServiceInstanceOnTransactionComplete=false)]
    public class LandscapeDesignService : ILandscapeDesignService
    {
        private int _id; //текущий пользователь
        private ModelFactory _modelFactory; //класс управления базой данных
        private LogMessages _logs; //логи сервера

        public LandscapeDesignService()
        {
            _modelFactory = ModelFactory.GetInstance();
            _logs = LogMessages.GetInstance();
            _id = -1;
        }

        private void CheckUser()
        {
            if (_id == -1)
            {
                string err = "200";
                InvalidUserFault fault = new InvalidUserFault(err);
                throw new FaultException<InvalidUserFault>(fault, new FaultReason(err));
            }
        }
        
        public LDUser Login(string email, string pass)
        {
            try
            {
                User user = _modelFactory.Login(email, pass);
                LDUser nUser = UserAdapter.GetFromModel(user);
                _id = user.Id;
                _logs.Add($"{email} залогинился.");
                return nUser;
            }
            catch (Exception ex)
            {
                InvalidUserFault fault = new InvalidUserFault(ex.Message);
                throw new FaultException<InvalidUserFault>(fault, new FaultReason(ex.Message));
            }
        }
        
        public async Task<LDUser> RegisterAsync(string email, string pass, string name = null)
        {
            string err;
            try
            {
                User user = _modelFactory.Register(email, pass, name);
                await _modelFactory.SendConfirmKeyAsync(email, pass);
                LDUser nUser = UserAdapter.GetFromModel(user);
                _id = user.Id;
                _logs.Add($"{email} зарегистрировался.");
                return nUser;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                InvalidUserFault fault = new InvalidUserFault(err);
                throw new FaultException<InvalidUserFault>(fault, new FaultReason(err));
            }
        }
        
        public async Task<bool> SendConfirmKeyAsync(string email, string password)
        {
            try
            {
                await _modelFactory.SendConfirmKeyAsync(email, password);
                return true;
            }
            catch (Exception ex)
            {
                string err;
                if (Int32.TryParse(ex.Message, out int i))
                {
                    if (i == 214)
                        return false;
                    else err = ex.Message;
                }
                else err = $"210";
                InvalidUserFault fault = new InvalidUserFault(err);
                throw new FaultException<InvalidUserFault>(fault, new FaultReason(err));
            }
        }
        
        public bool ConfirmRegister(string email, string key)
        {
            try
            {
                bool confirm = _modelFactory.ConfirmRegister(email, key);
                _logs.Add($"{email} подтверждена регистрация.");
                return confirm;
            }
            catch (Exception ex)
            {
                string err;
                if (Int32.TryParse(ex.Message, out int i))
                    err = ex.Message;
                else err = $"214";
                InvalidUserFault fault = new InvalidUserFault(err);
                throw new FaultException<InvalidUserFault>(fault, new FaultReason(err));
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = false)]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void EditUserName(string name)
        {
            CheckUser();
            try
            {
                _modelFactory.EditUserName(_id, name);
                OperationContext.Current.SetTransactionComplete();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                InvalidUserFault fault = new InvalidUserFault(err);
                throw new FaultException<InvalidUserFault>(fault, new FaultReason(err));
            }
        }
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = false)]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void EditUserPassword(string pass, string newpass)
        {
            CheckUser();
            string err;
            try
            {
                _modelFactory.EditUserPassword(_id, pass, newpass);
                OperationContext.Current.SetTransactionComplete();
            }
            catch (Exception ex)
            {
                err = ex.Message;
                InvalidUserFault fault = new InvalidUserFault(err);
                throw new FaultException<InvalidUserFault>(fault, new FaultReason(err));
            }
        }

        //получение всех проектов пользователя
        public async Task<ObservableCollection<LDProject>> GetProjectsAsync()
        {
            CheckUser();
            try
            {
                List<Project> projects = await _modelFactory.GetProjectsAsync(_id);
                ObservableCollection<LDProject> projectsnew = new ObservableCollection<LDProject>();
                if (projects == null)
                    return null;
                foreach (var pl in projects)
                    projectsnew.Add(ProjectAdapter.GetFromModel(pl));
                return projectsnew;
            }
            catch (Exception)
            {
                string err = $"301";
                InvalidProjectsFault fault = new InvalidProjectsFault(err);
                throw new FaultException<InvalidProjectsFault>(fault, new FaultReason(err));
            }
        }
        //получение всех проектов других пользователей
        public async Task<ObservableCollection<LDProject>> GetOtherUsersProjectsAsync()
        {
            CheckUser();
            try
            {
                List<Project> projects = await _modelFactory.GetOtherUsersProjectsAsync(_id);
                ObservableCollection<LDProject> projectsnew = new ObservableCollection<LDProject>();
                if (projects == null)
                    return null;
                foreach (var pl in projects)
                    projectsnew.Add(ProjectAdapter.GetFromModel(pl));
                return projectsnew;
            }
            catch (Exception)
            {
                string err = $"301";
                InvalidProjectsFault fault = new InvalidProjectsFault(err);
                throw new FaultException<InvalidProjectsFault>(fault, new FaultReason(err));
            }
        }
        //получение данных проекта
        public async Task<ObservableCollection<LDEntity>> GetProjectEntitiesAsync(int id)
        {
            CheckUser();
            try
            {
                List<ProjectPlant> plants = await _modelFactory.GetProjectPlantAsync(_id, id);
                List<ProjectBuilding> buildings = await _modelFactory.GetProjectBuildingAsync(_id, id);
                ObservableCollection<LDEntity> projectsnew = new ObservableCollection<LDEntity>();
                if (plants != null)
                    foreach (var pl in plants)
                        projectsnew.Add(ProjectEntitiesAdapter.GetFromModel(pl));
                if (buildings != null)
                    foreach (var pl in buildings)
                        projectsnew.Add(ProjectEntitiesAdapter.GetFromModel(pl));
                return projectsnew;
            }
            catch (Exception)
            {
                string err = $"302";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task<LDEntityTexture> GetPlantTextureAsync(List<int> ids)
        {
            CheckUser();
            try
            {
                return await _modelFactory.GetPlantTextureAsync(_id, ids);
            }
            catch (Exception)
            {
                string err = $"302";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task<LDEntityTexture> GetBuildingTextureAsync(List<int> ids)
        {
            CheckUser();
            if (ids == null || ids.Count == 0)
                return null;
            try
            {
                return await _modelFactory.GetPlantTextureAsync(_id, ids);
            }
            catch (Exception)
            {
                string err = $"302";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task<ObservableCollection<LDPlant>> GetAllUserPlantsAsync()
        {
            CheckUser();
            try
            {
                    ObservableCollection<LDPlant> plants = await _modelFactory.GetUserPlantsAsync(_id);
                    return plants;
            }
            catch (Exception)
            {
                string err = "303";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task<ObservableCollection<LDPlant>> GetOtherUsersPlantsAsync()
        {
            CheckUser();
            try
            {
                return await _modelFactory.GetOtherUsersPlantsAsync(_id);
            }
            catch (Exception)
            {
                string err = $"303";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task<LDPlantCharacteristics> GetPlantsCharacteristicsAsync()
        {
            CheckUser();
            try
            {
                return await _modelFactory.GetPlantsCharacteristicsAsync();
            }
            catch (Exception)
            {
                string err = "303";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task<ObservableCollection<LDBuilding>> GetAllUserBuildingsAsync()
        {
            CheckUser();
            try
            {
                List<Building> buildings = await _modelFactory.GetAllUserBuildingsAsync(_id);
                ObservableCollection<LDBuilding> buildingsnew = new ObservableCollection<LDBuilding>();
                if (buildings == null)
                    return null;
                foreach (var pl in buildings)
                    buildingsnew.Add(BuildingAdapter.GetFromModel(pl));
                return buildingsnew;
            }
            catch (Exception)
            {
                string err = "303";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task<ObservableCollection<LDBuilding>> GetOtherUsersBuildingsAsync()
        {
            CheckUser();
            try
            {
                List<Building> buildings = await _modelFactory.GetOtherUsersBuildingsAsync(_id);
                ObservableCollection<LDBuilding> buildingsnew = new ObservableCollection<LDBuilding>();
                if (buildings == null)
                    return null;
                foreach (var pl in buildings)
                    buildingsnew.Add(BuildingAdapter.GetFromModel(pl));
                return buildingsnew;
            }
            catch (Exception)
            {
                string err = "303";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task<LDBuildingCharacteristics> GetBuildingCharacteristicsAsync()
        {
            CheckUser();
            try
            {
                LDBuildingCharacteristics buildings = await _modelFactory.GetBuildingCharacteristicsAsync();
                return buildings;
            }
            catch (Exception)
            {
                string err = "303";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        
        public async Task<int> AddProjectAsync(LDProject project)
        {
            CheckUser();
            try
            {
                int id = await _modelFactory.AddProjectAsync(project);
                return id;
            }
            catch (Exception)
            {
                string err = "304";
                InvalidProjectsFault fault = new InvalidProjectsFault(err);
                throw new FaultException<InvalidProjectsFault>(fault, new FaultReason(err));
            }
        }
        public async Task EditProjectAsync(LDProject project)
        {
            CheckUser();
            try
            {
                await _modelFactory.EditProjectAsync(_id, project);
            }
            catch (Exception)
            {
                string err = "305";
                InvalidProjectsFault fault = new InvalidProjectsFault(err);
                throw new FaultException<InvalidProjectsFault>(fault, new FaultReason(err));
            }
        }
        public async Task ShareProjectAsync(int id)
        {
            CheckUser();
            try
            {
                await _modelFactory.ShareProjectAsync(_id, id);
            }
            catch (Exception)
            {
                string err = "305";
                InvalidProjectsFault fault = new InvalidProjectsFault(err);
                throw new FaultException<InvalidProjectsFault>(fault, new FaultReason(err));
            }
        }
        public async Task DeleteProjectAsync(int id)
        {
            CheckUser();
            try
            {
                await _modelFactory.DeleteProjectAsync(_id, id);
            }
            catch (Exception)
            {
                string err = "306";
                InvalidProjectsFault fault = new InvalidProjectsFault(err);
                throw new FaultException<InvalidProjectsFault>(fault, new FaultReason(err));
            }
        }
        
        public async Task<int> AddPlantAsync(LDPlant plant)
        {
            CheckUser();
            try
            {
                int id = await _modelFactory.AddPlantAsync(_id, plant);
                return id;
            }
            catch (Exception)
            {
                string err = "307";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task EditPlantAsync(LDPlant plant)
        {
            CheckUser();
            try
            {
                await _modelFactory.EditPlantAsync(_id, plant);
            }
            catch (Exception)
            {
                string err = "308";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task SharePlantAsync(int id)
        {
            CheckUser();
            try
            {
                await _modelFactory.SharePlantAsync(_id, id);
            }
            catch (Exception)
            {
                string err = "308";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task DeletePlantAsync(int id)
        {
            CheckUser();
            try
            {
                await _modelFactory.DeletePlantAsync(_id, id);
            }
            catch (Exception)
            {
                string err = "309";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }

        public async Task<int> AddBuildingAsync(LDBuilding building)
        {
            CheckUser();
            try
            {
                int id = await _modelFactory.AddBuildingAsync(building);
                return id;
            }
            catch (Exception)
            {
                string err = "310";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task EditBuildingAsync(LDBuilding building)
        {
            CheckUser();
            try
            {
                await _modelFactory.EditBuildingAsync(_id, building);
            }
            catch (Exception)
            {
                string err = "311";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task ShareBuildingAsync(int id)
        {
            CheckUser();
            try
            {
                await _modelFactory.ShareBuildingAsync(_id, id);
            }
            catch (Exception)
            {
                string err = "311";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
        public async Task DeleteBuildingAsync(int id)
        {
            CheckUser();
            try
            {
                await _modelFactory.DeleteBuildingAsync(_id, id);
            }
            catch (Exception)
            {
                string err = "312";
                InvalidProjectItemFault fault = new InvalidProjectItemFault(err);
                throw new FaultException<InvalidProjectItemFault>(fault, new FaultReason(err));
            }
        }
    }
}
