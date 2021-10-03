using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LandscapeDesignClient.Model
{
    public class PlantFactory
    {
        private static PlantFactory _instance;
        private static readonly object _sync = new Object();
        private ObservableCollection<PlantsCategory> _categories;
        private PlantFactory()
        {
            _categories = new ObservableCollection<PlantsCategory>();
        }
        public void AddCategory(PlantsCategory category)
        {
            if (_categories == null) return;
            lock (_categories)
            {
                if (_categories.FirstOrDefault(p => p.Id == category.Id) != null)
                    return;
                _categories.Add(category);
            }
        }
        public void AddPlant(Plant plant)
        {
            if (plant == null)
                throw new ArgumentNullException("Добавляемое растение равно null.");

            lock (_categories)
            {
                if (_categories.FirstOrDefault(cat => cat.Id == plant.Category) == null)
                {
                    string name = PlantCharacteristics.GetInstance().GetCategory(plant.Category);
                    if (name == null)
                        throw new ArgumentNullException("Категория растения неизвестна.");
                    AddCategory(new PlantsCategory(plant.Category, name));
                }
                PlantsCategory c = _categories.FirstOrDefault(p => p.Id == plant.Category);
                if (c != null && c.GetById(plant.Id) == null)
                    c.Add(plant);
            }
        }
        public void RemoveCategoryById(int id)
        {
            lock (_categories)
            {
                PlantsCategory c = _categories.FirstOrDefault(p => p.Id == id);
                if (c != null)
                    _categories.Remove(c);
            }
        }
        public void RemovePlantById(int id)
        {
            lock (_categories)
            {
                PlantsCategory c = _categories.FirstOrDefault(p => p.GetById(id) != null);
                if (c != null)
                    c.RemoveById(id);
            }
        }
        public Plant GetPlantById(int id)
        {
            lock (_categories)
            {
                PlantsCategory c = _categories.FirstOrDefault(p => p.GetById(id) != null);
                if (c != null)
                    return c.GetById(id) as Plant;
            }
            return null;
        }
        public PlantsCategory GetCategoryById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }
        public ObservableCollection<Plant> GetPlantsByUser(int id, bool invert = false)
        {
            lock (_categories)
            {
                foreach (var c in _categories)
                {
                    IEnumerable<Plant> plants = null;
                    if (invert)
                        plants = (c as PlantsCategory).Plants.Where(p => p.IdUser != id);
                    else
                        plants = (c as PlantsCategory).Plants.Where(p => p.IdUser == id);
                    if (plants != null && plants.Any())
                        return new ObservableCollection<Plant>(plants);
                }
            }
            return null;
        }
        public ObservableCollection<PlantsCategory> GetCategoriesByUser(int id, bool invert = false)
        {
            ObservableCollection<PlantsCategory> categories = new ObservableCollection<PlantsCategory>();
            lock (_categories)
            {
                
                foreach (var c in _categories)
                {
                    PlantsCategory category = null;
                    IEnumerable<Plant> plants = null;
                    if (invert)
                        plants = c.Plants.Where(p => p.IdUser != id);
                    else
                        plants = c.Plants.Where(p => p.IdUser == id);
                    if (plants != null && plants.Any())
                    {
                        category = new PlantsCategory(c.Id, c.Name);
                        foreach (var b in plants)
                            category.Add(b);
                    }
                    if (category != null)
                        categories.Add(category);
                }
            }
            return categories;
        }
        public ObservableCollection<PlantsCategory> PlantCategories { get { lock (_categories) return _categories; } }
        public void Clear()
        {
            lock (_categories)
                _categories.Clear();
        }
        public static PlantFactory GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new PlantFactory();
                }
            }
            return _instance;
        }
    }

    public class PlantsCategory
    {
        private ObservableCollection<Plant> _plants;
        public PlantsCategory(int id, string name)
        {
            _plants = new ObservableCollection<Plant>();
            Id = id;
            Name = name;
        }

        public void Add(object item)
        {
            if (!(item is Plant plant))
                throw new ArgumentNullException("Добавляемое растение равно null.");
            lock (_plants)
            {
                if (plant.Category != Id || _plants.FirstOrDefault(p => p.Id == plant.Id) != null)
                    return;
                _plants.Add(plant);
            }
        }
        public void RemoveById(int id)
        {
            lock (_plants)
            {
                Plant i = _plants.FirstOrDefault(p => p.Id == id);
                if (i != null)
                    _plants.Remove(i);
            }
        }
        public object GetById(int id)
        {
            lock (_plants)
            {
                return _plants.FirstOrDefault(p => p.Id == id);
            }
        }
        public void Clear()
        {
            lock (_plants)
                _plants.Clear();
        }
        public ObservableCollection<Plant> Plants { get { lock (_plants) return _plants; } }
        public string Name { get; protected set; }
        public int Id { get; protected set; }
    }
}
