using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesignClient.Model
{
    public class PlantCharacteristics
    {
        private static PlantCharacteristics _instance;
        private static readonly object _sync = new Object();

        private Dictionary<int, string> _category;
        private Dictionary<int, string> _soilNames;
        private Dictionary<int, string> _soilTypes;
        private Dictionary<int, string> _soilPH;
        private Dictionary<int, string> _lightning;
        private Dictionary<int, string> _watering;
        private Dictionary<int, string> _careType;

        private PlantCharacteristics()
        {
            _category = new Dictionary<int, string>();
            _soilNames = new Dictionary<int, string>();
            _soilTypes = new Dictionary<int, string>();
            _soilPH = new Dictionary<int, string>();
            _lightning = new Dictionary<int, string>();
            _watering = new Dictionary<int, string>();
            _careType = new Dictionary<int, string>();
        }

        public ObservableCollection<string> Categories
        {
            get
            {
                lock (_category)
                    return new ObservableCollection<string>(_category.Select(c => c.Value));
            }
        }
        public ObservableCollection<string> SoilNames
        {
            get
            {
                lock (_soilNames)
                    return new ObservableCollection<string>(_soilNames.Select(c => c.Value));
            }
        }
        public ObservableCollection<string> SoilTypes
        {
            get
            {
                ObservableCollection<string> c = new ObservableCollection<string>();
                lock (_soilTypes)
                {
                    foreach (var s in _soilTypes)
                        c.Add($"{s.Value} {GetSoilPH(s.Key)} pH");
                }
                return c;
            }
        }

        public ObservableCollection<string> Lightnings
        {
            get { lock (_lightning) return new ObservableCollection<string>(_lightning.Select(c => c.Value)); }
        }
        public ObservableCollection<string> Waterings
        {
            get
            {
                lock (_watering)
                    return new ObservableCollection<string>(_watering.Select(c => c.Value));
            }
        }
        public ObservableCollection<string> CareTypes
        {
            get
            {
                lock (_careType)
                    return new ObservableCollection<string>(_careType.Select(c => c.Value));
            }
        }

        public void AddCategory(int id, string name)
        {
            lock (_category)
                if (!_category.Keys.Contains(id))
                    _category.Add(id, name);
        }
        public void AddSoilName(int id, string name)
        {
            lock (_soilNames)
                if (!_soilNames.Keys.Contains(id))
                    _soilNames.Add(id, name);
        }
        public void AddSoilType(int id, string name)
        {
            lock (_soilTypes)
                if (!_soilTypes.Keys.Contains(id))
                    _soilTypes.Add(id, name);
        }
        public void AddSoilPH(int id, string name)
        {
            lock (_soilPH)
                if (!_soilPH.Keys.Contains(id))
                    _soilPH.Add(id, name);
        }
        public void AddLightning(int id, string name)
        {
            lock (_lightning)
                if (!_lightning.Keys.Contains(id))
                    _lightning.Add(id, name);
        }
        public void AddWatering(int id, string name)
        {
            lock (_watering)
                if (!_watering.Keys.Contains(id))
                    _watering.Add(id, name);
        }
        public void AddCareType(int id, string name)
        {
            lock (_careType)
                if (!_careType.Keys.Contains(id))
                    _careType.Add(id, name);
        }

        public void RemoveCategory(int id)
        {
            lock (_category)
                if (_category.Keys.Contains(id))
                    _category.Remove(id);
        }
        public void RemoveSoilName(int id)
        {
            lock (_soilNames)
                if (_soilNames.Keys.Contains(id))
                    _soilNames.Remove(id);
        }
        public void RemoveSoilType(int id)
        {
            lock (_soilTypes)
                if (_soilTypes.Keys.Contains(id))
                    _soilTypes.Remove(id);
        }
        public void RemoveSoilPH(int id)
        {
            lock (_soilPH)
                if (_soilPH.Keys.Contains(id))
                    _soilPH.Remove(id);
        }
        public void RemoveLightning(int id)
        {
            lock (_lightning)
                if (_lightning.Keys.Contains(id))
                    _lightning.Remove(id);
        }
        public void RemoveWatering(int id)
        {
            lock (_watering)
                if (_watering.Keys.Contains(id))
                    _watering.Remove(id);
        }
        public void RemoveCareType(int id)
        {
            lock (_careType)
                if (_careType.Keys.Contains(id))
                    _careType.Remove(id);
        }

        public string GetCategory(int id)
        {
            lock (_category)
                return _category.ContainsKey(id) ? _category[id] : null;
        }
        public string GetSoilName(int id)
        {
            lock (_soilNames)
                return _soilNames.ContainsKey(id) ? _soilNames[id] : null;
        }
        public string GetSoilType(int id)
        {
            lock (_soilTypes)
                return _soilTypes.ContainsKey(id) ? _soilTypes[id] : null;
        }
        public string GetSoilPH(int id)
        {
            lock (_soilPH)
                return _soilPH.ContainsKey(id) ? _soilPH[id] : null;
        }
        public string GetLightning(int id)
        {
            lock (_lightning)
                return _lightning.ContainsKey(id) ? _lightning[id] : null;
        }
        public string GetWatering(int id)
        {
            lock (_watering)
                return _watering.ContainsKey(id) ? _watering[id] : null;
        }
        public string GetCareType(int id)
        {
            lock (_careType)
                return _careType.ContainsKey(id) ? _careType[id] : null;
        }

        public Dictionary<int, string> Category { get { lock (_category) return _category; } }
        public Dictionary<int, string> SoilName { get { lock (_soilNames) return _soilNames; } }
        public Dictionary<int, string> SoilType { get { lock (_soilTypes) return _soilTypes; } }
        public Dictionary<int, string> SoilPH { get { lock (_soilPH) return _soilPH; } }
        public Dictionary<int, string> Lightning { get { lock (_lightning) return _lightning; } }
        public Dictionary<int, string> Watering { get { lock (_watering) return _watering; } }
        public Dictionary<int, string> CareType { get { lock (_careType) return _careType; } }

        public static PlantCharacteristics GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new PlantCharacteristics();
                }
            }
            return _instance;
        }
    }
}
