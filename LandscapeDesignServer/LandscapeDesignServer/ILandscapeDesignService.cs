using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using LandscapeDesignServer.LandscapeDesignModel;

namespace LandscapeDesignServer
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ILandscapeDesignService
    {
        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        LDUser Login(string email, string password);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        Task<LDUser> RegisterAsync(string email, string password, string name = null);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        Task<bool> SendConfirmKeyAsync(string email, string password);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        bool ConfirmRegister(string email, string key);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        void EditUserName(string name);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        void EditUserPassword(string pass, string newpass);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectsFault))]
        Task<ObservableCollection<LDProject>> GetProjectsAsync();

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectsFault))]
        Task<ObservableCollection<LDProject>> GetOtherUsersProjectsAsync();

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<ObservableCollection<LDEntity>> GetProjectEntitiesAsync(int id);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<LDEntityTexture> GetPlantTextureAsync(List<int> ids);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<LDEntityTexture> GetBuildingTextureAsync(List<int> ids);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<ObservableCollection<LDPlant>> GetAllUserPlantsAsync();

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<ObservableCollection<LDPlant>> GetOtherUsersPlantsAsync();

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<LDPlantCharacteristics> GetPlantsCharacteristicsAsync();

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<ObservableCollection<LDBuilding>> GetAllUserBuildingsAsync();

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<ObservableCollection<LDBuilding>> GetOtherUsersBuildingsAsync();

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<LDBuildingCharacteristics> GetBuildingCharacteristicsAsync();

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectsFault))]
        Task<int> AddProjectAsync(LDProject project);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectsFault))]
        Task EditProjectAsync(LDProject project);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task ShareProjectAsync(int id);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectsFault))]
        Task DeleteProjectAsync(int id);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<int> AddPlantAsync(LDPlant plant);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task EditPlantAsync(LDPlant plant);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task SharePlantAsync(int id);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task DeletePlantAsync(int id);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task<int> AddBuildingAsync(LDBuilding building);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task EditBuildingAsync(LDBuilding building);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task ShareBuildingAsync(int id);

        [OperationContract]
        [FaultContract(typeof(InvalidUserFault))]
        [FaultContract(typeof(InvalidProjectItemFault))]
        Task DeleteBuildingAsync(int id);
    }

    [DataContract]
    abstract class InvalidCustomFault
    {
        [DataMember]
        public string CustomError;
        public InvalidCustomFault(string error)
        {
            CustomError = error;
        }
    }

    [DataContract]
    class InvalidUserFault: InvalidCustomFault
    {
        public InvalidUserFault(string error = null): base (error)
        {
            CustomError = error;
        }
    }
    [DataContract]
    class InvalidProjectsFault : InvalidCustomFault
    {
        public InvalidProjectsFault(string error = null) : base(error)
        {
            CustomError = error;
        }
    }
    [DataContract]
    class InvalidProjectItemFault : InvalidCustomFault
    {
        public InvalidProjectItemFault(string error = null) : base(error)
        {
            CustomError = error;
        }
    }


}
