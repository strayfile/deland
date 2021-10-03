using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LandscapeDesignServer.LandscapeDesignModel;
using LandscapeDesignServer.Model;

namespace LandscapeDesignServer.ModelAdapter
{
    internal static class UserAdapter
    {
        public static LDUser GetFromModel(User user)
        {
            return new LDUser(user.Id, user.Email) { Name = user.Name };
        }
    }
    internal static class ProjectAdapter
    {
        public static LDProject GetFromModel(Project project)
        {
            return new LDProject(project.Id, project.Id_User, project.CreateDate,  project.Share) { Name = project.Name, ChangeDate = project.ChangeDate, TemperatureMin = project.Temperature_Min, TemperatureMax = project.Temperature_Max, Lightning = project.Id_Lightning, Soil = project.Id_Soil, SoilType = project.Id_Soil_Type };
        }
    }
    internal static class ProjectEntitiesAdapter
    {
        public static LDEntity GetFromModel(ProjectPlant plant)
        {
            return new LDEntity(plant.Id_Plant, plant.Id_Project, 0, plant.Plant.Resizable, plant.Plant.Overlappable)
            {
                Name = plant.Plant.Name,
                Layer = plant.Layer,
                X = plant.X,
                Y = plant.Y,
                Rotate = 0,
                Texture = plant.Plant.Id_Texture,
                Polygon = plant.Draw
            };
        }
        public static LDEntity GetFromModel(ProjectBuilding building)
        {
            return new LDEntity(building.Id_Building, building.Id_Project, 1, true, true)
            {
                Name = building.Building.Name,
                Layer = building.Layer,
                X = building.X,
                Y = building.Y,
                Rotate = building.Rotate,
                Texture = building.Id_Texture,
                Polygon = building.Building.Draw
            };
        }
    }
    internal static class PlantAdapter
    {
        public static LDPlant GetFromModel(Plant plant)
        {
            LDPlant pl = new LDPlant() { 
                Id = plant.Id, IdUser = plant.Id_User, Name = plant.Name, Category = plant.Id_Category, Resizable = plant.Resizable, Overlappable = plant.Overlappable,
                TemperatureMin = plant.Temperature_Min, TemperatureMax = plant.Temperature_Max, Soil = plant.Id_Soil, SoilType = plant.Id_Soil_Type,
                Lightning = plant.Id_Lightning, Watering = plant.Id_Watering, Care = plant.Id_Care, IdTexture = plant.Id_Texture, Share = plant.Share,
                 Description = plant.Description
            };
            return pl;
        }
    }
    internal static class PlantCharacteristicsAdapter
    {
        public static LDPlantCharacteristics GetFromModel(Plant plant)
        {
            LDPlantCharacteristics c = new LDPlantCharacteristics();
            c.CareType.Add(plant.Id_Care, plant.PlantCare.Name);
            c.Category.Add(plant.Id_Category, plant.PlantCategory.Name);
            c.Lightning.Add(plant.Id_Lightning, plant.PlantLightning.Name);
            c.SoilNames.Add(plant.Id_Soil, plant.PlantSoil.Name);
            c.SoilPH.Add(plant.Id_Soil_Type, plant.PlantSoilType.pH);
            c.SoilTypes.Add(plant.Id_Soil_Type, plant.PlantSoilType.Type);
            c.Watering.Add(plant.Id_Watering, plant.PlantWatering.Name);
            return c;

        }
    }
    internal static class BuildingAdapter
    {
        public static LDBuilding GetFromModel(Building build)
        {
            return new LDBuilding()
            {
                Id = build.Id,
                IdUser = build.Id_User,
                Name = build.Name,
                Category = build.Id_Category,
                Overlappable = build.BuildingCategory.Overlappable,
                Polygon = build.Draw,
                Saved = build.Saved,
                Share = build.Shared
            };
        }
    }
    
}