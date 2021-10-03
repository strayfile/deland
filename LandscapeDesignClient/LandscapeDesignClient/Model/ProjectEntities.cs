using System;
using System.Windows.Media;
using LandscapeDesignClient.LandscapeDesignReference;

namespace LandscapeDesignClient.Model
{
    public class ProjectPlant : Entity
    {
        public ProjectPlant(int id, int idProject, EntityTextures textures, bool canresize, bool canbeoverlapping) : base(id, idProject, textures)
        {
            if (textures as PlantTextures == null)
                throw new ArgumentException("Эти текстуры не предназначены для отображения растений.");
            CanResize = canresize;
            CanBeOverlapping = canbeoverlapping;
        }

        public override void Resize(Geometry polygon)
        {
            //создание полигона новых размеров
            //будет реализовано
        }

        public override void SetPolygon(Geometry polygon)
        {
            //установка нового полигона отображаемому растению
            //будет реализовано
        }
    }

    public class ProjectBuilding: Entity
    {
        public ProjectBuilding(int id, int idProject, EntityTextures textures) : base(id, idProject, textures)
        {
            if ((textures as BuildingTextures) == null)
                throw new ArgumentException("Эти текстуры не предназначены для отображения строений.");
            CanResize = true;
            CanBeOverlapping = true;
        }
        public override void Resize(Geometry polygon)
        {
            //создание полигона новых размеров
            //будет реализовано
        }

        public override void SetPolygon(Geometry polygon)
        {
            //установка нового полигона отображаемому строению
            //будет реализовано
        }
    }
}
