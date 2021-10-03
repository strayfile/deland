using LandscapeDesignClient.LandscapeDesignReference;
using System;
using System.Linq;

namespace LandscapeDesignClient.Model
{
    internal static class LandscapeDesignModelAdapter
    {
        public static User GetUser(LDUser user)
        {
            if (user == null)
                throw new ArgumentNullException("Пользователь равен null.");
            return new User(user.Id, user.Email) { Name = user.Name };
        }
        public static Plant GetPlant(LDPlant plant)
        {
            if (plant == null)
                throw new ArgumentNullException("Растение равно null.");
            
            PlantCharacteristics plch = PlantCharacteristics.GetInstance();
            Plant pl = null;
            if (plch.GetCategory(plant.Category) == "Травы")
                pl = new Grass(plant.IdUser);
            else if (plch.GetCategory(plant.Category) == "Кустарники")
                pl = new Bush(plant.IdUser);
            else if (plch.GetCategory(plant.Category) == "Деревья")
                pl = new Tree(plant.IdUser);
            else throw new ArgumentException("Категория не найдена.");
            pl.Id = plant.Id;
            pl.Care = plant.Care;
            pl.Category = plant.Category;
            pl.Crown = plant.Crown;
            pl.Description = plant.Description;
            pl.IdTexture = plant.IdTexture;
            pl.Lightning = plant.Lightning;
            pl.Name = plant.Name;
            pl.Radius = plant.Radius;
            pl.Share = plant.Share;
            pl.Soil = plant.Soil;
            pl.SoilType = plant.SoilType;
            pl.TemperatureMax = plant.TemperatureMax;
            pl.TemperatureMin = plant.TemperatureMin;
            pl.Watering = plant.Watering;
            return pl;
        }
        public static Building GetBuilding(LDBuilding building)
        {
            if (building == null)
                throw new ArgumentNullException("Строение равно null.");

            BuildingFactory bldf = BuildingFactory.GetInstance();
            Building bld = null;
            string category = bldf.GetCategoryById(building.Category).Name;
            switch (category)
            {
                case "Покрытие":
                    bld = new Flooring(building.IdUser, building.Category);
                    break;
                case "Постройка":
                    bld = new House(building.IdUser, building.Category);
                    break;
                case "Забор":
                    bld = new Fence(building.IdUser, building.Category);
                    break;
                case "Декоративное сооружение":
                    bld = new Decoration(building.IdUser, building.Category);
                    break;
                default:
                    throw new ArgumentException("319");
            }
            bld.Id = building.Id;
            bld.Name = building.Name;
            bld.Polygon = building.Polygon;
            bld.Saved = building.Saved;
            bld.Share = building.Share;
            return bld; 
        }
        public static Entity GetEntity(LDEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Отображаемый объект равен null.");
            Entity en = null;
            if (entity.Category == 0)
                en = new ProjectPlant(entity.Id, entity.IdProject,  PlantTextures.GetInstance(), entity.CanResize, entity.CanBeOverlapping);
            else if (entity.Category == 1)
                en = new ProjectBuilding(entity.Id, entity.IdProject, BuildingTextures.GetInstance());
            else throw new ArgumentException("Невозможно сохранить как отображаемый объект.");
            en.Name = entity.Name;
            en.Layer = entity.Layer;
            //
            //en.SetPolygon(entity.Polygon);
            en.Rotate = entity.Rotate;
            en.Texture = entity.Texture;
            en.X = entity.X;
            en.Y = entity.Y;
            return en;
        }
        public static Project GetProject(LDProject project)
        {
            if (project == null)
                throw new ArgumentNullException("Добавляемый проект равен null.");

            Project pr = new Project(project.Id, project.IdUser, project.CreateDate)
            {
                ChangeDate = project.ChangeDate,
                Name = project.Name
            };
            foreach (var e in project.Entities)
                pr.AddEntity(LandscapeDesignModelAdapter.GetEntity(e));
            return pr;
        }

        public static void AddPlantsCharacteristics(LDPlantCharacteristics plants)
        {
            PlantCharacteristics plantCharacteristics = PlantCharacteristics.GetInstance();
            foreach (var pl in plants.CareType)
                plantCharacteristics.AddCareType(pl.Key, pl.Value);
            foreach (var pl in plants.Category)
                plantCharacteristics.AddCategory(pl.Key, pl.Value);
            foreach (var pl in plants.Lightning)
                plantCharacteristics.AddLightning(pl.Key, pl.Value);
            foreach (var pl in plants.SoilNames)
                plantCharacteristics.AddSoilName(pl.Key, pl.Value);
            foreach (var pl in plants.SoilPH)
                plantCharacteristics.AddSoilPH(pl.Key, pl.Value);
            foreach (var pl in plants.SoilTypes)
                plantCharacteristics.AddSoilType(pl.Key, pl.Value);
            foreach (var pl in plants.Watering)
                plantCharacteristics.AddWatering(pl.Key, pl.Value);
        }
        public static void AddBuildingsCharacteristics(LDBuildingCharacteristics buildings)
        {
            BuildingFactory buildingFactory = BuildingFactory.GetInstance();
            foreach (var pl in buildings.Category)
                buildingFactory.AddCategory(new BuildingsCategory(pl.Key, pl.Value, buildings.Overlapable.FirstOrDefault(o => o.Key == pl.Key).Value));
        }


        public static LDPlant GetPlant(Plant plant)
        {
            if (plant == null)
                throw new ArgumentNullException("Растение равно null.");

            PlantCharacteristics plch = PlantCharacteristics.GetInstance();
            LDPlant pl = new LDPlant
            {
                Id = plant.Id,
                Care = plant.Care,
                Category = plant.Category,
                Crown = plant.Crown,
                Description = plant.Description,
                IdTexture = plant.IdTexture,
                Lightning = plant.Lightning,
                Name = plant.Name,
                Radius = plant.Radius,
                Share = plant.Share,
                Soil = plant.Soil,
                SoilType = plant.SoilType,
                TemperatureMax = plant.TemperatureMax,
                TemperatureMin = plant.TemperatureMin,
                Watering = plant.Watering,
                IdUser = plant.IdUser,
                Overlappable = plant.Overlappable,
                Resizable = plant.Resizable
            };
            return pl;
        }
    }
}
