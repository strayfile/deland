using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LandscapeDesignServer.ModelAdapter;
using LandscapeDesignServer.Model;

namespace LandscapeDesignServer.LandscapeDesignModel
{
    public class ModelFactory
    {
        private static Regex regexName = new Regex(@"^(?=.*[а-яА-ЯёЁa-zA-Z0-9]$)[а-яА-ЯёЁa-zA-Z][а-яА-ЯёЁa-zA-Z0-9 .'-]{2,50}$");
        //private static Regex regexName = new Regex(@"^[а-яА-ЯёЁa-zA-Z0-9 _-]+$");
        private static Regex regexEmail = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
        private static Regex regexPassword = new Regex(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=])(?=\S+$).{8,20}$");
        private static ModelFactory _instance;
        private static readonly object _sync = new Object();
        private LogMessages _logs;
        private string _adminEmail;
        
        private ModelFactory()
        {
            CheckUnconfirmedUsers();
            _logs = LogMessages.GetInstance();
            _adminEmail = ""; //...set admin email from file...
        }

        public User Login(string email, string password)
        {
            ValidateEmail(email);
            ValidatePassword(password);
            try
            {
                using (var db = new LandscapeDesignDb())
                {
                    User dbuser = db.Users.FirstOrDefault(u => u.Email == email);
                    if (dbuser == null)
                        throw new Exception("208");
                    //если почта не подтверждена
                    if (!dbuser.Confirmed)
                        throw new Exception("215");
                    string salt = dbuser.UserSalt.Salt;
                    if (dbuser.Password != GetPasswordFromSalt(salt, password))
                        throw new Exception("208");
                    return dbuser;
                }
            }
            catch (Exception ex)
            {
                if (!Int32.TryParse(ex.Message, out int e))
                    throw new Exception("100");
                throw new Exception(ex.Message);
            }
        }

        public User Register(string email, string password, string name = null)
        {
            ValidateEmail(email);
            ValidatePassword(password);
            ValidateName(name);
            try
            {
                using (var db = new LandscapeDesignDb())
                {
                    User dbuser = db.Users.FirstOrDefault(u => u.Email == email);
                    if (dbuser != null)
                        throw new Exception("209");
                    try
                    {
                        dbuser = db.Users.Create();
                        dbuser.Name = name;
                        dbuser.Email = email;
                        dbuser.UserRole = db.UserRoles.FirstOrDefault(r => r.Id == 2);

                        List<string> salt = GenerateSalt(password);
                        dbuser.Password = salt[1];
                        dbuser.RegDate = DateTime.Now;
                        db.Users.Add(dbuser);
                        db.SaveChanges();

                        UserSalt us = db.UserSalts.Create();
                        try
                        {
                            us.User = dbuser;
                            us.Salt = salt[0];
                            db.UserSalts.Add(us);
                            db.SaveChanges();
                        }
                        catch (Exception)
                        {
                            db.UserSalts.Remove(us);
                            db.Users.Remove(dbuser);
                            db.SaveChanges();
                            throw new Exception();
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("100");
                    }
                    return dbuser;
                }
            }
            catch (Exception ex)
            {
                if (!Int32.TryParse(ex.Message, out int e))
                    throw new Exception("100");
                throw new Exception(ex.Message);
            }
        }
        private List<string> GenerateSalt(string password)
        {
            byte[] saltbuf = new byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltbuf);
            StringBuilder sb = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
                sb.Append(string.Format("{0:X2}", saltbuf[i]));
            string salt = sb.ToString();
            byte[] pass = Encoding.Unicode.GetBytes(salt + password);
            MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider();
            byte[] byteHash = CSP.ComputeHash(pass);
            StringBuilder hash = new StringBuilder(byteHash.Length);
            for (int i = 0; i < byteHash.Length; i++)
                hash.Append(string.Format("{0:X2}", byteHash[i]));
            return new List<string>() { salt, hash.ToString()};
        }
        private string GetPasswordFromSalt(string salt, string password)
        {
            byte[] pass = Encoding.Unicode.GetBytes(salt + password);
            MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider();
            byte[] byteHash = CSP.ComputeHash(pass);
            StringBuilder hash = new StringBuilder(byteHash.Length);
            for (int i = 0; i < byteHash.Length; i++)
                hash.Append(string.Format("{0:X2}", byteHash[i]));
            return hash.ToString();
        }

        public void EditUserName(int id, string name = null)
        {
            ValidateName(name);
            using (LandscapeDesignDb db = new LandscapeDesignDb())
            {
                User dbuser = db.Users.FirstOrDefault(u => u.Id == id);
                if (dbuser == null)
                    throw new Exception("100");
                try
                {
                    dbuser.Name = name;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    throw new Exception($"100");
                }
            }
        }
        public void EditUserPassword(int id, string password, string newpassword)
        {
            ValidatePassword(password);
            using (LandscapeDesignDb db = new LandscapeDesignDb())
            {
                User dbuser = db.Users.FirstOrDefault(u => u.Id == id);
                if (dbuser == null)
                    throw new Exception("100");
                try
                {
                    string pass1 = GetPasswordFromSalt(dbuser.UserSalt.Salt, password);
                    if (pass1 != dbuser.Password)
                        throw new Exception("207");
                    List<string> salt = GenerateSalt(password);
                    dbuser.Password = salt[1];
                    dbuser.UserSalt.Salt = salt[0];
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (Int32.TryParse(ex.Message, out int e))
                        throw new Exception(ex.Message);
                    throw new Exception($"100");
                }
            }
        }
        public async void DeleteUserAsync(int id)
        {
            await Task.Run(() =>
            {
                using (LandscapeDesignDb db = new LandscapeDesignDb())
                {
                    User dbuser = db.Users.FirstOrDefault(u => u.Id == id);
                    if (dbuser == null)
                        throw new Exception("Пользователь не найден.");
                    try
                    {
                        db.Users.Remove(dbuser);
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Невозможно удалить пользователя с id {id}.");
                    }
                }
            });
        }
        //отправка кода на email для подтверждения регистрации
        private async Task SendMessage(string email, string code)
        {
            try
            {
                MailAddress from = new MailAddress(_adminEmail, "DeLand");
                MailAddress to = new MailAddress(email);
                MailMessage m = new MailMessage(from, to)
                {
                    Subject = "Verification Key",
                    Body = $"Your verification code: {code}"
                };
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_adminEmail, "") //set code
                };
                await smtp.SendMailAsync(m);
                _logs.Add($"{email} отправлен код подтверждения регистрации.");
            }
            catch (Exception)
            {
                throw new Exception("100");
            }
        }
        public async Task SendConfirmKeyAsync(string email, string password)
        {
            ValidateEmail(email);
            ValidatePassword(password);
            string code = null;
            try
            {
                using (var db = new LandscapeDesignDb())
                {
                    User dbuser = db.Users.FirstOrDefault(u => u.Email == email);
                    if (dbuser == null)
                        throw new Exception("208");
                    string salt = dbuser.UserSalt.Salt;
                    if (dbuser.Password != GetPasswordFromSalt(salt, password))
                        throw new Exception("208");
                    //если почта не подтверждена
                    if (dbuser.Confirmed)
                        throw new Exception("214");
                    //генерация кода для подтверждения email
                    string guid = Guid.NewGuid().ToString();
                    code = guid.Substring(0, 8);
                    email = dbuser.Email;
                    bool created = false;
                    UserKey uk = db.UserKeys.FirstOrDefault(u => u.Id_User == dbuser.Id);
                    if (uk == null)
                    {
                        uk = db.UserKeys.Create();
                        uk.User = dbuser;
                        created = true;
                    }
                    uk.Key = code;
                    if (created)
                        db.UserKeys.Add(uk);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (!Int32.TryParse(ex.Message, out int e))
                    throw new Exception("100");
                throw new Exception(ex.Message);
            }
            await SendMessage(email, code);
        }

        public bool ConfirmRegister(string email, string key)
        {
            using (var db = new LandscapeDesignDb())
            {
                User user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    throw new Exception("213");
                if (user.Confirmed)
                    throw new Exception("217");
                else
                {
                    UserKey ukey = db.UserKeys.FirstOrDefault(k => k.Id_User == user.Id);
                    if (ukey == null)
                        throw new Exception("Ошибка подтверждения регистрации.");
                    //если дата регистрации была больше 30-ти минут назад
                    if (user.RegDate <= DateTime.Now.Subtract(new TimeSpan(0, 30, 0)))
                    {
                        db.UserKeys.Remove(ukey);
                        db.Users.Remove(user);
                        db.SaveChanges();
                    }
                    else
                    {
                        if (key != ukey.Key)
                            throw new ArgumentException("212");
                        db.UserKeys.Remove(ukey);
                        user.Confirmed = true;
                        db.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public void CheckUnconfirmedUsers()
        {
            using (var db = new LandscapeDesignDb())
            {
                DateTime enddate = DateTime.Now.Subtract(new TimeSpan(0, 30, 0));
                var users = db.Users.Where(u => !u.Confirmed && u.RegDate <= enddate);
                if (users != null && users.Any())
                    db.Users.RemoveRange(users);
            }
        }
        public void ValidateEmail(string email)
        {
            if (email == null || email.Length == 0 || !regexEmail.IsMatch(email))
                throw new Exception("202");
        }
        public void ValidatePassword(string password)
        {
            if (password == null || password.Length == 0 || !regexPassword.IsMatch(password))
                throw new Exception("204");
        }
        public void ValidateName(string name)
        {
            if (name == null)
                return;
            if (name.Length == 0 || name.Length > 50 || !regexName.IsMatch(name))
                throw new Exception("205");
        }

        public async Task<List<Project>> GetProjectsAsync(int id)
        {
            return await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    User user = db.Users.FirstOrDefault(u => u.Id == id);
                    if (user == null)
                        throw new Exception("Невозможно получить проекты этого пользователя.");
                    if (user.Projects != null)
                        return user.Projects.ToList();
                }
                return null;
            });
        }
        public async Task<List<Project>> GetOtherUsersProjectsAsync(int id)
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    User user = db.Users.FirstOrDefault(u => u.Id == id);
                    if (user == null)
                        throw new Exception("Невозможно получить проекты этого пользователя.");
                    var pr = db.Projects.Where(p => p.Id_User != id && p.Share);
                    if (pr != null)
                        return pr.ToList();
                }
                return null;
            });
        }

        public async Task<List<ProjectPlant>> GetProjectPlantAsync(int user, int id)
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    Project project = db.Projects.FirstOrDefault(u => u.Id == id);
                    if (project == null || (project.Id_User != user && !project.Share))
                        throw new Exception("Невозможно получить данные этого проекта.");
                    if (project.ProjectPlants != null)
                        return project.ProjectPlants.ToList();
                }
                return null;
            });
        }
        public async Task<List<ProjectBuilding>> GetProjectBuildingAsync(int user, int id)
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    Project project = db.Projects.FirstOrDefault(u => u.Id == id);
                    if (project == null || (project.Id_User != user && !project.Share))
                        throw new Exception("Невозможно получить данные этого проекта.");
                    if (project.ProjectBuildings != null)
                        return project.ProjectBuildings.ToList();
                }
                return null;
            });
        }
        public async Task<List<BuildingTexture>> GetProjectBuildingTexturesAsync(int user, int id)
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    Project project = db.Projects.FirstOrDefault(u => u.Id == id);
                    if (project == null || (project.Id_User != user && !project.Share))
                        throw new Exception("Невозможно получить данные этого проекта.");
                    if (project.ProjectBuildings != null)
                        return db.BuildingTextures.Join(project.ProjectBuildings, t => t.Id, p => p.Id_Texture, (t, p) => t).ToList();
                }
                return null;
            });
        }
        public async Task<List<PlantTexture>> GetProjectPlantTexturesAsync(int user, int id)
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    Project project = db.Projects.FirstOrDefault(u => u.Id == id);
                    if (project == null || (project.Id_User != user && !project.Share))
                        throw new Exception("Невозможно получить данные этого проекта.");
                    if (project.ProjectPlants != null)
                        return db.PlantTextures.Join(project.ProjectPlants, t => t.Id, p => p.Plant.Id_Texture, (t, p) => t).ToList();
                }
                return null;
            });
        }
        public async Task<LDEntityTexture> GetPlantTextureAsync(int iduser, List<int> ids)
        {
            return await Task.Run(() => {
                if (ids == null)
                    ids = new List<int>();
                using (var db = new LandscapeDesignDb())
                {
                    var tPaths = db.PlantTextures.Where(t => !ids.Contains(t.Id) && t.Share);
                    if (tPaths == null || !tPaths.Any())
                        return null;
                    var pt = tPaths.FirstOrDefault();
                    if (pt == null)
                        return null;
                    //получение пути папки проекта
                    string path = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
                    //получение пути к файлу тектсуры
                    path = $@"{path}PlantTextures\{pt.Path}";
                    try
                    {
                        byte[] data = File.ReadAllBytes(path);
                        return new LDEntityTexture(pt.Id, pt.IdUser, 0, data) { Share = pt.Share };
                    }
                    catch (Exception) //в случае если тектсура не была найдена в папке и не смогла загрузиться
                    {   //не добавлять ее к возвращаемым текстурам и продолжить загрузку остальных
                        db.PlantTextures.Remove(pt); //удалить запись о ней из базы
                        db.SaveChanges();
                        return null;
                    }
                }
            });
        }
        public async Task<LDEntityTexture> GetBuildingTextureAsync(int iduser, List<int> ids)
        {
            return await Task.Run(() => {
                if (ids == null)
                    ids = new List<int>();
                using (var db = new LandscapeDesignDb())
                {
                    var tPaths = db.BuildingTextures.Where(t => !ids.Contains(t.Id) && t.Share);
                    if (tPaths == null || !tPaths.Any())
                        return null;
                    var pt = tPaths.FirstOrDefault();
                    //получение пути папки проекта
                    string path = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
                    //получение пути к файлу тектсуры
                    path = $@"{path}BuildingTextures\{pt.Path}";
                    try
                    {
                        byte[] data = File.ReadAllBytes(path);
                        return new LDEntityTexture(pt.Id, pt.IdUser, 1, data) { Share = pt.Share };
                    }
                    catch (Exception) //в случае если тектсура не была найдена в папке и не смогла загрузиться
                    {   //не добавлять ее к возвращаемым текстурам и продолжить загрузку остальных
                        db.BuildingTextures.Remove(pt); //удалить запись о ней из базы
                        db.SaveChanges();
                        return null;
                    }
                }
            });
        }
        public async Task<ObservableCollection<LDPlant>> GetUserPlantsAsync(int id)
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    var plants = db.Plants.Where(plant => plant.Id_User == id);
                    ObservableCollection<LDPlant> pl = new ObservableCollection<LDPlant>();
                    if (plants != null && plants.Any())
                    {
                        foreach (var p in plants)
                        {
                            LDPlant p1 = PlantAdapter.GetFromModel(p);
                            if (!p.Resizable)
                            {
                                p1.Radius = p.NoresizablePlant.Radius;
                                p1.Crown = p.NoresizablePlant.Crown;
                            }
                            pl.Add(p1);
                        }
                    }
                    return pl;
                }
            });
        }
        public async Task<ObservableCollection<LDPlant>> GetOtherUsersPlantsAsync(int id)
        {
            return await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    var plants = db.Plants.Where(plant => plant.Id_User != id && plant.Share);
                    ObservableCollection<LDPlant> pl = new ObservableCollection<LDPlant>();
                    if (plants != null && plants.Any())
                    {
                        foreach (var p in plants)
                        {
                            LDPlant p1 = PlantAdapter.GetFromModel(p);
                            if (!p.Resizable)
                            {
                                p1.Radius = p.NoresizablePlant.Radius;
                                p1.Crown = p.NoresizablePlant.Crown;
                            }
                            pl.Add(p1);
                        }
                    }
                    return pl;
                }
            });
        }
        public async Task<LDPlantCharacteristics> GetPlantsCharacteristicsAsync()
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    LDPlantCharacteristics plchs = new LDPlantCharacteristics();
                    if (db.PlantCares.Any())
                        foreach (var pl in db.PlantCares)
                            plchs.CareType.Add(pl.Id, pl.Name);
                    if (db.PlantCategories.Any())
                        foreach (var pl in db.PlantCategories)
                            plchs.Category.Add(pl.Id, pl.Name);
                    if (db.PlantLightnings.Any())
                        foreach (var pl in db.PlantLightnings)
                            plchs.Lightning.Add(pl.Id, pl.Name);
                    if (db.PlantSoils.Any())
                        foreach (var pl in db.PlantSoils)
                            plchs.SoilNames.Add(pl.Id, pl.Name);
                    if (db.PlantSoilTypes.Any())
                        foreach (var pl in db.PlantSoilTypes)
                            plchs.SoilPH.Add(pl.Id, pl.pH);
                    if (db.PlantSoilTypes.Any())
                        foreach (var pl in db.PlantSoilTypes)
                            plchs.SoilTypes.Add(pl.Id, pl.Type);
                    if (db.PlantWaterings.Any())
                        foreach (var pl in db.PlantWaterings)
                            plchs.Watering.Add(pl.Id, pl.Name);
                    return plchs;
                }
            });
        }
        public async Task<List<Building>> GetAllUserBuildingsAsync(int id)
        {
            return await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    var buildings = db.Buildings.Where(pl => pl.Id_User == id);
                    if (buildings != null && buildings.Any())
                        return buildings.ToList();
                }
                return null;
            });
        }
        public async Task<List<Building>> GetOtherUsersBuildingsAsync(int id)
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    var buildings = db.Buildings.Where(pl => pl.Id_User != id && pl.Shared);
                    if (buildings != null && buildings.Any())
                        return buildings.ToList();
                }
                return null;
            });
        }
        public async Task<LDBuildingCharacteristics> GetBuildingCharacteristicsAsync()
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    LDBuildingCharacteristics plchs = new LDBuildingCharacteristics();
                    if (db.BuildingCategories.Any())
                        foreach (var pl in db.BuildingCategories)
                        {
                            plchs.Category.Add(pl.Id, pl.Name);
                            plchs.Overlapable.Add(pl.Id, pl.Overlappable);
                        }
                    return plchs;
                }
            });
        }

        public async Task<int> AddProjectAsync(LDProject project)
        {
            return await Task.Run(() => {
                return -1;
                //using (var db = new LandscapeDesignDb())
                //{

                //}
            });
        }
        public async Task EditProjectAsync(int user, LDProject project)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    var pr = db.Projects.FirstOrDefault(p => p.Id == project.Id);
                    if (pr == null || pr.Id_User != user)
                        throw new Exception("305");
                    //редактирование проекта
                }
            });
        }
        public async Task ShareProjectAsync(int user, int id)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    Project dbproject = db.Projects.FirstOrDefault(p => p.Id == id);
                    if (dbproject == null || dbproject.Id_User != user)
                        throw new Exception("305");
                    dbproject.Share = true;
                    db.SaveChanges();
                }
            });
        }
        public async Task DeleteProjectAsync(int user, int id)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    Project dbproject = db.Projects.FirstOrDefault(p => p.Id == id);
                    if (dbproject == null || dbproject.Id_User != user || (dbproject.Share && dbproject.User.Id_Role != 1))
                        throw new Exception("306");
                    db.Projects.Remove(dbproject);
                    db.SaveChanges();
                }
            });
        }

        public async Task<int> AddPlantAsync(int iduser, LDPlant plant)
        {
            return await Task.Run(() => {
                using (var db = new LandscapeDesignDb())
                {
                    Plant dbplant = db.Plants.Create();
                    dbplant.Name = plant.Name;
                    dbplant.User = db.Users.FirstOrDefault(u => u.Id == iduser);
                    dbplant.PlantCategory = db.PlantCategories.FirstOrDefault(p => p.Id == plant.Category);
                    dbplant.Temperature_Min = plant.TemperatureMin;
                    dbplant.Temperature_Max = plant.TemperatureMax;
                    dbplant.PlantSoil = db.PlantSoils.FirstOrDefault(p => p.Id == plant.Soil);
                    dbplant.PlantSoilType = db.PlantSoilTypes.FirstOrDefault(p => p.Id == plant.SoilType);
                    dbplant.PlantLightning = db.PlantLightnings.FirstOrDefault(p => p.Id == plant.Lightning);
                    dbplant.PlantWatering = db.PlantWaterings.FirstOrDefault(p => p.Id == plant.Watering);
                    dbplant.PlantCare = db.PlantCares.FirstOrDefault(p => p.Id == plant.Care);
                    dbplant.Resizable = plant.Resizable;
                    dbplant.Overlappable = plant.Overlappable;
                    dbplant.PlantTexture = db.PlantTextures.FirstOrDefault(p => p.Id == plant.IdTexture);
                    dbplant.Share = plant.Share;
                    dbplant.Description = plant.Description;
                    db.Plants.Add(dbplant);
                    db.SaveChanges();
                    if (!plant.Resizable)
                    {
                        NoresizablePlant nsp = db.NoresizablePlants.Create();
                        nsp.Plant = dbplant;
                        nsp.Radius = plant.Radius;
                        nsp.Crown = plant.Crown;
                        db.NoresizablePlants.Add(nsp);
                        db.SaveChanges();
                    }
                    return dbplant.Id;
                }
            });
        }
        public async Task EditPlantAsync(int user, LDPlant plant)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    Plant dbplant = db.Plants.FirstOrDefault(p => p.Id == plant.Id);
                    if (dbplant == null || dbplant.Id_User != user)
                        throw new Exception("308");
                    dbplant.Name = plant.Name;
                    dbplant.PlantCategory = db.PlantCategories.FirstOrDefault(p => p.Id == plant.Category);
                    dbplant.Temperature_Min = plant.TemperatureMin;
                    dbplant.Temperature_Max = plant.TemperatureMax;
                    dbplant.PlantSoil = db.PlantSoils.FirstOrDefault(p => p.Id == plant.Soil);
                    dbplant.PlantSoilType = db.PlantSoilTypes.FirstOrDefault(p => p.Id == plant.SoilType);
                    dbplant.PlantLightning = db.PlantLightnings.FirstOrDefault(p => p.Id == plant.Lightning);
                    dbplant.PlantWatering = db.PlantWaterings.FirstOrDefault(p => p.Id == plant.Watering);
                    dbplant.PlantCare = db.PlantCares.FirstOrDefault(p => p.Id == plant.Care);
                    dbplant.Resizable = plant.Resizable;
                    dbplant.Overlappable = plant.Overlappable;
                    dbplant.PlantTexture = db.PlantTextures.FirstOrDefault(p => p.Id == plant.IdTexture);
                    dbplant.Share = plant.Share;
                    dbplant.Description = plant.Description;
                    db.SaveChanges();
                    if (!plant.Resizable)
                    {
                        bool created = false;
                        NoresizablePlant nsp = db.NoresizablePlants.FirstOrDefault(p => p.Id_Plant == plant.Id);
                        if (nsp == null)
                        {
                            nsp = db.NoresizablePlants.Create();
                            nsp.Plant = dbplant;
                            created = true;
                        }
                        
                        nsp.Radius = plant.Radius;
                        nsp.Crown = plant.Crown;
                        if (created)
                            db.NoresizablePlants.Add(nsp);
                        db.SaveChanges();
                    }
                }
            });
        }
        public async Task SharePlantAsync(int user, int id)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    Plant dbplant = db.Plants.FirstOrDefault(p => p.Id == id);
                    if (dbplant == null || dbplant.Id_User != user)
                        throw new Exception("308");
                    dbplant.Share = true;
                    db.SaveChanges();
                }
            });
        }
        public async Task DeletePlantAsync(int user, int id)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    Plant dbplant = db.Plants.FirstOrDefault(p => p.Id == id);
                    if (dbplant == null || dbplant.Id_User != user || (dbplant.Share && dbplant.User.Id_Role != 1))
                        throw new Exception("308");
                    db.Plants.Remove(dbplant);
                    db.SaveChanges();
                }
            });
        }

        public async Task<int> AddBuildingAsync(LDBuilding building)
        {
            return await Task.Run(() => {
                return -1;
            });
        }
        public async Task EditBuildingAsync(int user, LDBuilding building)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    Building dbbuilding = db.Buildings.FirstOrDefault(p => p.Id == building.Id);
                    if (dbbuilding == null || dbbuilding.Id_User != user)
                        throw new Exception("308");
                    //
                    //db.SaveChanges();
                }
            });
        }
        public async Task ShareBuildingAsync(int user, int id)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    Building dbbuilding = db.Buildings.FirstOrDefault(p => p.Id == id);
                    if (dbbuilding == null || dbbuilding.Id_User != user)
                        throw new Exception("311");
                    dbbuilding.Shared = true;
                    db.SaveChanges();
                }
            });
        }
        public async Task DeleteBuildingAsync(int user, int id)
        {
            await Task.Run(() =>
            {
                using (var db = new LandscapeDesignDb())
                {
                    Building dbbuilding = db.Buildings.FirstOrDefault(p => p.Id == id);
                    if (dbbuilding == null || dbbuilding.Id_User != user || (dbbuilding.Shared && dbbuilding.User.Id_Role != 1))
                        throw new Exception("311");
                    db.Buildings.Remove(dbbuilding);
                    db.SaveChanges();
                }
            });
        }

        public static ModelFactory GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new ModelFactory();
                }
            }
            return _instance;
        }
    }
}

