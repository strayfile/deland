using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LandscapeDesignClient.Model
{
    public class BuildingFactory
    {
        private static BuildingFactory _instance;
        private static readonly object _sync = new Object();
        private ObservableCollection<BuildingsCategory> _buildingsCategories;
        private BuildingFactory()
        {
            _buildingsCategories = new ObservableCollection<BuildingsCategory>();
        }

        public void AddCategory(BuildingsCategory category)
        {
            if (_buildingsCategories == null) return;
            lock (_buildingsCategories)
            {
                if (_buildingsCategories.FirstOrDefault(p => p.Id == category.Id) != null)
                    return;
                _buildingsCategories.Add(category);
            }
        }
        public void RemoveCategoryById(int id)
        {
            lock (_buildingsCategories)
            {
                BuildingsCategory c = _buildingsCategories.FirstOrDefault(p => p.Id == id);
                if (c != null)
                    _buildingsCategories.Remove(c);
            }
        }

        public void AddBuilding(Building building)
        {
            if (building == null)
                throw new ArgumentNullException("Добавляемое строение равно null.");

            lock (_buildingsCategories)
            {
                BuildingsCategory c = _buildingsCategories.FirstOrDefault(p => p.Id == building.Category);
                if (c != null && c.GetById(building.Id) == null)
                    c.Add(building);
            }
        }
        public void RemoveBuildingById(int id)
        {
            lock (_buildingsCategories)
            {
                BuildingsCategory c = _buildingsCategories.FirstOrDefault(p => p.GetById(id) != null);
                if (c != null)
                    c.RemoveById(id);
            }
        }
        public Building GetBuildingById(int id)
        {
            lock (_buildingsCategories)
            {
                BuildingsCategory c = _buildingsCategories.FirstOrDefault(p => p.GetById(id) != null);
                if (c != null)
                    return c.GetById(id) as Building;
            }
            return null;
        }
        public BuildingsCategory GetCategoryById(int id)
        {
            return _buildingsCategories.FirstOrDefault(c => c.Id == id);
        }
        public ObservableCollection<Building> GetBuildingsByUser(int id, bool invert = false)
        {
            lock (_buildingsCategories)
            {
                foreach (var c in _buildingsCategories)
                {
                    IEnumerable<Building> buildings = null;
                    if (invert)
                        buildings = (c as BuildingsCategory).Buildings.Where(p => p.IdUser != id);
                    else 
                        buildings = (c as BuildingsCategory).Buildings.Where(p => p.IdUser == id);
                    if (buildings != null && buildings.Any())
                        return new ObservableCollection<Building>(buildings);
                }
            }
            return null;
        }
        public ObservableCollection<BuildingsCategory> GetCategoriesByUser(int id, bool invert = false)
        {
            ObservableCollection<BuildingsCategory> categories = new ObservableCollection<BuildingsCategory>();
            lock (_buildingsCategories)
            {
                BuildingsCategory category = null;
                foreach (var c in _buildingsCategories)
                {
                    IEnumerable<Building> buildings = null;
                    if (invert)
                        buildings = (c as BuildingsCategory).Buildings.Where(p => p.IdUser != id && p.Saved);
                    else
                        buildings = (c as BuildingsCategory).Buildings.Where(p => p.IdUser == id && p.Saved);
                    if (buildings != null && buildings.Any())
                    {
                        category = new BuildingsCategory(c.Id, c.Name, c.Overlappable);
                        foreach (var b in buildings)
                            category.Add(b);
                    }
                    if (category != null)
                        categories.Add(category);
                }
            }
            return categories;
        }
        public ObservableCollection<BuildingsCategory> BuildingsCategories { get { lock (_buildingsCategories) return _buildingsCategories; } }
        public void Clear()
        {
            lock (_buildingsCategories)
                _buildingsCategories.Clear();
        }
        public static BuildingFactory GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new BuildingFactory();
                }
            }
            return _instance;
        }
    }

    public class BuildingsCategory
    {
        private ObservableCollection<Building> _buildings;
        public BuildingsCategory(int id, string name, bool overlappable)
        {
            _buildings = new ObservableCollection<Building>();
            Id = id;
            Name = name;
            Overlappable = overlappable;
        }

        public void Add(object item)
        {
            if (!(item is Building build))
                throw new ArgumentNullException("Добавляемое строение равно null.");
            lock (_buildings)
            {
                if (build.Category != Id || _buildings.FirstOrDefault(p => p.Id == build.Id) != null)
                    return;
                _buildings.Add(build);
            }
        }
        public void RemoveById(int id)
        {
            lock (_buildings)
            {
                Building i = _buildings.FirstOrDefault(p => p.Id == id);
                if (i != null)
                    _buildings.Remove(i);
            }
        }
        public object GetById(int id)
        {
            lock (_buildings)
            {
                return _buildings.FirstOrDefault(p => p.Id == id);
            }
        }
        public void Clear()
        {
            lock (_buildings)
                _buildings.Clear();
        }
        public ObservableCollection<Building> Buildings { get { lock(_buildings) return _buildings; } }
        public string Name { get; protected set; }
        public bool Overlappable { get; protected set; }
        public int Id { get; protected set; }
    }
}
