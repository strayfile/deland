using LandscapeDesignClient.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace LandscapeDesignClient.ProjectMenuControls
{
    class PlantCard : INotifyPropertyChanged
    {
        private PlantCharacteristics _plantCharacteristics;
        private EntityTextures _plantTextures;
        private int _id;
        private string _name;
        private int _idTexture;
        private int _category;
        private int _temperatureMax;
        private int _temperatureMin;
        private int _soil;
        private int _soilType;
        private int _lightning;
        private int _watering;
        private int _care;
        private int _trunkRadius;
        private int _crownRadius;
        private string _description;
        public event PropertyChangedEventHandler PropertyChanged;

        public PlantCard(Plant plant)
        {
            if (plant != null)
                Plant = plant;
            SetPlant(plant);
            _plantCharacteristics = PlantCharacteristics.GetInstance();
            _plantTextures = PlantTextures.GetInstance();
            Textures = _plantTextures.Textures;
            SoilNames = _plantCharacteristics.SoilNames;
            SoilTypes = _plantCharacteristics.SoilTypes;
            CareTypes = _plantCharacteristics.CareTypes;
            Categories = _plantCharacteristics.Categories;
            Lightnings = _plantCharacteristics.Lightnings;
            Waterings = _plantCharacteristics.Waterings;
        }
        private void SetPlant(Plant plant)
        {
            if (plant == null)
                _id = -1;
            else
            {
                _id = plant.Id;
                PlantName = plant.Name;
                _category = plant.Category;
                TemperatureMin = plant.TemperatureMin;
                TemperatureMax = plant.TemperatureMax;
                _soil = plant.Soil;
                _soilType = plant.SoilType;
                _lightning = plant.Lightning;
                _watering = plant.Watering;
                _care = plant.Care;
                IdTexture = plant.IdTexture;
                Description = plant.Description;
                _trunkRadius = plant.Radius;
                _crownRadius = plant.Crown;
                Resizable = plant.Resizable;
                Overlapable = plant.Overlappable;
                Shared = plant.Share;
            }
        }
        public Plant GetCreatedPlant()
        {
            Plant pl = null;
            if (_plantCharacteristics.GetCategory(_category) == "Травы")
                pl = new Grass(-1);
            else if (_plantCharacteristics.GetCategory(_category) == "Кустарники")
                pl = new Bush(-1);
            else if (_plantCharacteristics.GetCategory(_category) == "Деревья")
                pl = new Tree(-1);
            else throw new ArgumentException("Категория не найдена.");
            pl.Id = _id;
            pl.Care = _care;
            pl.Category = _category;
            pl.Crown = _crownRadius;
            pl.Description = Description;
            pl.IdTexture = IdTexture;
            pl.Lightning = _lightning;
            pl.Name = PlantName;
            pl.Radius = _trunkRadius;
            pl.Share = Shared;
            pl.Soil = _soil;
            pl.SoilType = _soilType;
            pl.TemperatureMax = TemperatureMax;
            pl.TemperatureMin = TemperatureMin;
            pl.Watering = _watering;
            return pl;
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string PlantName { get { return _name; } set { _name = value; OnPropertyChanged("PlantName"); } }
        public string Category
        {
            get { return _plantCharacteristics.GetCategory(_category); }
            set
            {
                if (Int32.TryParse(value, out int c))
                {
                    _category = c;
                    OnPropertyChanged("Category");
                }
            }
        }
        public int TemperatureMin { get { return _temperatureMin; } set { _temperatureMin = value; OnPropertyChanged("TemperatureMin"); } }
        public int TemperatureMax { get { return _temperatureMax; } set { _temperatureMax = value; OnPropertyChanged("TemperatureMax"); } }
        public string Soil
        {
            get
            {
                return _plantCharacteristics.GetSoilName(_soil);
            }
            set
            {
                if (Int32.TryParse(value, out int c))
                {
                    _soil = c;
                    OnPropertyChanged("Soil");
                }
            }
        }
        public string SoilType
        {
            get
            {
                return _plantCharacteristics.GetSoilType(_soilType);
            }
            set
            {
                if (Int32.TryParse(value, out int c))
                {
                    _soilType = c;
                    OnPropertyChanged("SoilType");
                }
            }
        }
        public string PH
        {
            get
            {
                return _plantCharacteristics.GetSoilPH(_soilType);
            }
        }
        public string Lightning
        {
            get
            {
                return _plantCharacteristics.GetLightning(_lightning);
            }
            set
            {
                if (Int32.TryParse(value, out int c))
                {
                    _lightning = c;
                    OnPropertyChanged("Lightning");
                }
            }
        }
        public string Watering
        {
            get
            {
                return _plantCharacteristics.GetWatering(_watering);
            }
            set
            {
                if (Int32.TryParse(value, out int c))
                {
                    _watering = c;
                    OnPropertyChanged("Watering");
                }
            }
        }
        public string Care
        {
            get
            {
                return _plantCharacteristics.GetCareType(_care);
            }
            set
            {
                if (Int32.TryParse(value, out int c))
                {
                    _care = c;
                    OnPropertyChanged("Care");
                }
            }
        }
        public int IdTexture
        {
            get { return _idTexture; }
            set
            {
                _idTexture = value;
                OnPropertyChanged("IdTexture");

            }
        }
        public BitmapImage Texture
        {
            get {
                Texture texture = _plantTextures.GetTexture(IdTexture);
                if (texture != null)
                    return texture.Image;
                return null;
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
        public int TrunkDiameter
        {
            get
            {
                return _trunkRadius * 2;
            }
            set
            {
                if (value < 6)
                    value = 6;
                _trunkRadius = value / 2;
                OnPropertyChanged("TrunkDiameter");
            }
        }
        public int CrownDiameter
        {
            get
            {
                return _crownRadius * 2;
            }
            set
            {
                if (value < 8)
                    value = 8;
                _crownRadius = value / 2;
                OnPropertyChanged("CrownDiameter");
            }
        }
        public bool Resizable { get; private set; }
        public bool Overlapable { get; private set; }
        public bool Shared { get; set; }

        public Plant Plant { get; private set; }

        public ObservableCollection<string> Categories
        {
            get; private set;
        }
        public ObservableCollection<string> SoilNames
        {
            get; private set;
        }
        public ObservableCollection<string> SoilTypes
        {
            get; private set;
        }
        public ObservableCollection<string> Lightnings
        {
            get; private set;
        }
        public ObservableCollection<string> Waterings
        {
            get; private set;
        }
        public ObservableCollection<string> CareTypes
        {
            get; private set;
        }
        public ObservableCollection<Texture> Textures
        {
            get; private set;
        }
        public int CategoryIndex
        {
            get {
                return Categories.IndexOf(Category);
                    }
            set {
                string name = Categories[value];
                Category = _plantCharacteristics.Category.FirstOrDefault(c => c.Value == name).Key.ToString();

            }
        }
        public int SoilIndex
        {
            get
            {
                return SoilNames.IndexOf(Soil);
            }
            set
            {
                string name = SoilNames[value];
                Soil = _plantCharacteristics.SoilName.FirstOrDefault(c => c.Value == name).Key.ToString();
            }
        }
        public int SoilTypeIndex
        {
            get
            {
                return SoilTypes.IndexOf($"{SoilType} {PH} pH");
            }
            set
            {
                string name = SoilTypes[value];
                SoilType = _plantCharacteristics.SoilType.FirstOrDefault(c => c.Value == name.Substring(0, name.IndexOf(" "))).Key.ToString();
            }
        }
        public int LightningIndex
        {
            get
            {
                return Lightnings.IndexOf(Lightning);
            }
            set
            {
                string name = Lightnings[value];
                Lightning = _plantCharacteristics.Lightning.FirstOrDefault(c => c.Value == name).Key.ToString();
            }
        }
        public int WateringIndex
        {
            get
            {
                return Waterings.IndexOf(Watering);
            }
            set
            {
                string name = Waterings[value];
                Watering = _plantCharacteristics.Watering.FirstOrDefault(c => c.Value == name).Key.ToString();
            }
        }
        public int CareTypeIndex
        {
            get
            {
                return CareTypes.IndexOf(Care);
            }
            set
            {
                string name = CareTypes[value];
                Care = _plantCharacteristics.CareType.FirstOrDefault(c => c.Value == name).Key.ToString();
            }
        }
        public int TextureIndex
        {
            get
            {
                return Textures.IndexOf(_plantTextures.GetTexture(IdTexture));
            }
            set
            {
                Texture texture = Textures[value];
                IdTexture = texture.Id;
            }
        }

        public bool IsValid()
        {
            if (_name == null || _name.Length < 2 || _lightning == 0 || _watering == 0 || _care == 0 || _soil == 0 || _soilType == 0 || _idTexture == 0)
                return false;
            if (_category == 0)
                return false;
            if (_category != 1 && (_trunkRadius < 3 || _crownRadius < 4))
                return false;
            return true;
        }
    }
}
