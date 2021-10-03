using System;
using System.Collections.ObjectModel;
using System.Linq;
using LandscapeDesignClient.LandscapeDesignReference;

namespace LandscapeDesignClient.Model
{
    public class Project: LDProject
    {
        public Project(int id, int iduser, DateTime createDate)
        {
            Id = id;
            IdUser = iduser;
            CreateDate = createDate;
            Entities = new ObservableCollection<Entity>();
        }
        public void SetChangeDate(DateTime date)
        {
            if (date == null || date < CreateDate)
                ChangeDate = DateTime.Now;
            else ChangeDate = date;
        }

        public void AddEntity(Entity entity)
        {
            if (entity == null || Entities.FirstOrDefault(e => e.Id == entity.Id) != null)
                return;
            Entities.Add(entity);
        }
        public void RemoveEntity(int id)
        {
            Entity en = Entities.FirstOrDefault(e => e.Id == id);
            if (en != null)
                Entities.Remove(en);
        }
        public new ObservableCollection<Entity> Entities { get; protected set; }
    }

    public class CopyOfProject : LDProject
    {
        public CopyOfProject(int id, int iduser, DateTime createDate)
        {
            Id = id;
            IdUser = iduser;
            CreateDate = createDate;
            Entities = new ObservableCollection<Entity>();
        }
        public void SetChangeDate(DateTime date)
        {
            if (date == null || date < CreateDate)
                ChangeDate = DateTime.Now;
            else ChangeDate = date;
        }

        public void AddEntity(Entity entity)
        {
            if (entity == null || Entities.FirstOrDefault(e => e.Id == entity.Id) != null)
                return;
            Entities.Add(entity);
        }
        public void RemoveEntity(int id)
        {
            Entity en = Entities.FirstOrDefault(e => e.Id == id);
            if (en != null)
                Entities.Remove(en);
        }
        public new ObservableCollection<Entity> Entities { get; protected set; }
    }
}
