namespace LandscapeDesignClient.Model
{
    public delegate void MessageHandler(string text);
    public interface ILandscapeDesignFactory
    {
        event MessageHandler ErrorCatched;
        event MessageHandler RegErrorCatched;
        event MessageHandler AuthErrorCatched;
        event MessageHandler ProfileErrorCatched;
        event MessageHandler ConfirmRegErrorCatched;
        event MessageHandler LoginErrorCatched;
        event ButtonClicked NotConfirmCatched;
        event UserHandler UserChanged;
        event ButtonClicked ToProfile;
        event ButtonClicked UserProjectsChanged;
        event ButtonClicked UserPlantsChanged;
        event ButtonClicked UserBuildingsChanged;
        event ButtonClicked OtherUsersProjectsChanged;
        event ButtonClicked OtherUsersPlantsChanged;
        event ButtonClicked OtherUsersBuildingsChanged;

        void Login(string email, string pass);
        void Register(string email, string pass, string name = null);
        bool EditUserName(string name);
        bool EditUserPassword(string pass, string newpass);
        void SendConfirmKey();
        void ConfirmRegister(string key);

        void LoadOtherUsersProjectsAsync();
        void LoadOtherUsersPlantsAsync();
        void LoadOtherUsersBuildingsAsync();

        void UpdateProfileData();
        void OpeningProfile();
        void AddProject(Project project);
        void EditProject(Project project);
        void ShareProject(int id);
        void DeleteProject(int id);
        void AddPlant(Plant plant);
        void EditPlant(Plant plant);
        void SharePlant(int id);
        void DeletePlant(int id);
        void AddBuilding(Building building);
        void EditBuilding(Building building);
        void ShareBuilding(int id);
        void DeleteBuilding(int id);

        void Close();
    }

}
