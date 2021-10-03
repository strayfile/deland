using LandscapeDesignClient.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LandscapeDesignClient.ProjectMenuControls
{
    class ProjectCard : INotifyPropertyChanged
    {
        private PlantCharacteristics _plantCharacteristics;
        private readonly EntityTextures _buildingTextures;

        private int _id;
        private string _name;
        private int _soil;
        private int _soilType;
        private int _lightning;
        private int _temperatureMax;
        private int _temperatureMin;
        public event PropertyChangedEventHandler PropertyChanged;

        public ProjectCard()
        {
            _id = -1;
            _plantCharacteristics = PlantCharacteristics.GetInstance();
            _buildingTextures = BuildingTextures.GetInstance();
            SoilNames = _plantCharacteristics.SoilNames;
            SoilTypes = _plantCharacteristics.SoilTypes;
            Lightnings = _plantCharacteristics.Lightnings;
        }
        public Project GetCreatedProject()
        {
            Project pr = new Project(_id, -1, DateTime.MinValue)
            {
                Name = ProjectName,
                Share = Shared,
                Soil = _soil,
                SoilType = _soilType,
                Lightning = _lightning,
                TemperatureMax = TemperatureMax,
                TemperatureMin = TemperatureMin
            };
            pr.AddEntity(new ProjectBuilding(-1, _id, null) {  });
            return pr;
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id { get { return _id; } }
        public Project Project { get; private set; }

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
        public int Length
        {
            get; private set;
        }
        public int Width
        {
            get; private set;
        }
        public string PH
        {
            get
            {
                return _plantCharacteristics.GetSoilPH(_soilType);
            }
        }
        public string ProjectName { get { return _name; } set { _name = value; OnPropertyChanged("ProjectName"); } }
        public int TemperatureMin { get { return _temperatureMin; } set { _temperatureMin = value; OnPropertyChanged("TemperatureMin"); } }
        public int TemperatureMax { get { return _temperatureMax; } set { _temperatureMax = value; OnPropertyChanged("TemperatureMax"); } }
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
        public string Soil
        {
            get
            {
                return _plantCharacteristics.GetSoilName(_soil);
            }
            set
            {
                if (int.TryParse(value, out int c))
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
                if (int.TryParse(value, out int c))
                {
                    _soilType = c;
                    OnPropertyChanged("SoilType");
                }
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
                if (int.TryParse(value, out int c))
                {
                    _lightning = c;
                    OnPropertyChanged("Lightning");
                }
            }
        }
        public bool Shared { get; set; }

        public bool IsValid()
        {
            if (_name == null || _name.Length == 0 || _name.Length > 250)
                return false;
            if (Length < 1 || Length > 10000)
                return false;
            if (Width < 1 || Width > 10000)
                return false;
            return true;
        }
    }
}
