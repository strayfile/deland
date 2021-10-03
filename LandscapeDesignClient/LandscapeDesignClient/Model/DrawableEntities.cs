using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using LandscapeDesignClient.LandscapeDesignReference;

namespace LandscapeDesignClient.Model
{
    public interface IDrawable
    {
        int X { get; set; }
        int Y { get; set; }
        int Layer { get; set; }
        int Rotate { get; set; }
        int Texture { get; set; }
        bool CanResize { get; }
        bool CanBeOverlapping { get; }
        Geometry Polygon { get; }
        BitmapImage GetTexture();
        void SetPolygon(Geometry polygon);
        void Resize(Geometry polygon);
    }

    public delegate BitmapImage GetImageHandler(int id);
    public abstract class Entity : IDrawable
    {
        private EntityTextures _textures;
        public Entity(int id, int idProject, EntityTextures textures)
        {
            _textures = textures ?? throw new ArgumentNullException("Текстуры недоступны для отображаемого объекта.");
            Id = id;
            IdProject = idProject;
        }
        public BitmapImage GetTexture()
        {
            return _textures.GetTexture(Texture).Image;
        }
        public abstract void SetPolygon(Geometry polygon);
        public abstract void Resize(Geometry polygon);
        public int Id { get; }
        public int IdProject { get; protected set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Layer { get; set; }
        public int Rotate { get; set; }
        public int Texture { get; set; }
        public bool CanResize { get; protected set; }
        public bool CanBeOverlapping { get; protected set; }
        public Geometry Polygon { get; protected set; }

    }
}
