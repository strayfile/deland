using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LandscapeDesignClient.LandscapeDesignReference;

namespace LandscapeDesignClient.Model
{
    public class Texture : LDEntityTexture
    {
        public Texture(int id, int iduser, BitmapImage img, bool share)
        {
            Id = id;
            IdUser = iduser;
            Image = img;
            Share = share;
        }
        public BitmapImage Image { get; private set; }
    }


    public abstract class EntityTextures
    {
        private ObservableCollection<Texture> _textures;
        protected EntityTextures()
        {
            _textures = new ObservableCollection<Texture>();
        }
        public void AddTexture(int id, int iduser, BitmapImage img, bool share)
        {
            lock (_textures)
            {
                if (_textures.FirstOrDefault(t => t.Id == id) != null)
                    return;
                _textures.Add(new Texture(id, iduser, img, share));
            }
        }
        public void AddTexture(Texture texture)
        {
            lock (_textures)
            {
                if (_textures.FirstOrDefault(t => t.Id == texture.Id) != null)
                    return;
                _textures.Add(texture);
            }
        }
        public void RemoveTexture(int id)
        {
            lock (_textures)
            {
                Texture texture = _textures.FirstOrDefault(t => t.Id == id);
                if (texture != null)
                    _textures.Remove(texture);
            }
        }
        public Texture GetTexture(int id)
        {
            lock (_textures)
            {
                return _textures.FirstOrDefault(t => t.Id == id);
            }
        }
        public bool GetShare(int id)
        {
            Texture texture = null;
            lock (_textures)
            {
                texture = _textures.FirstOrDefault(t => t.Id == id);
            }
            if (texture != null)
                return texture.Share;
            return false;
        }
        public ObservableCollection<int> Ids {
            get {
                lock(_textures)
                    return new ObservableCollection<int>(_textures.Select(t => t.Id));
            }
        }
        public ObservableCollection<Texture> Textures
        {
            get
            {
                lock (_textures)
                    return _textures;
            }
        }
    }

    public class PlantTextures : EntityTextures
    {
        private static EntityTextures _instance;
        private static readonly object _sync = new Object();

        private PlantTextures()
        {

        }
        public static EntityTextures GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new PlantTextures();
                }
            }
            return _instance;
        }
    }
    public class BuildingTextures: EntityTextures
    {
        private static EntityTextures _instance;
        private static object _sync = new Object();

        private BuildingTextures()
        {

        }
        public static EntityTextures GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new BuildingTextures();
                }
            }
            return _instance;
        }
    }

}

