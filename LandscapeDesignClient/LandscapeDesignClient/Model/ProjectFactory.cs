using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LandscapeDesignClient.LandscapeDesignReference;

namespace LandscapeDesignClient.Model
{
    class ProjectFactory
    {
        private static ProjectFactory _instance;
        private static readonly object _sync = new Object();

        private ProjectFactory()
        {
            Clear();
        }
        
        public void AddProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("Добавляемый проект равен null.");
            if (Projects.FirstOrDefault(p => p.Id == project.Id) != null)
                return;
            Projects.Add(project);
        }
        public void RemoveById(int id)
        {
            var pr = Projects.FirstOrDefault(p => p.Id == id);
            if (pr != null)
                Projects.Remove(pr);
        }
        public Project GetById(int id)
        {
            return Projects.FirstOrDefault(p => p.Id == id);

        }
        public ObservableCollection<Project> GetByUser(int id, bool invert = false)
        {
            ObservableCollection<Project> projects = new ObservableCollection<Project>();
            if (invert)
            {
                foreach (var pl in Projects.Where(p => p.IdUser != id))
                    projects.Add(pl);
            }
            else
            {
                foreach (var pl in Projects.Where(p => p.IdUser == id))
                    projects.Add(pl);
            }
            return projects;
        }
        public ObservableCollection<Project> Projects { get; protected set; }
        public void Clear()
        {
            Projects = new ObservableCollection<Project>();
        }
        public static ProjectFactory GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new ProjectFactory();
                }
            }
            return _instance;
        }
    }
}
