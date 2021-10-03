using System;
using System.Data.Entity.Spatial;
using System.Runtime.Serialization;

namespace LandscapeDesignServer.LandscapeDesignModel
{
    [Serializable]
    [DataContract]
    public class LDEntity
    {
        public LDEntity(int id, int idProject, int category, bool canresize, bool canbeoverlapping)
        {
            Id = id;
            IdProject = idProject;
            Category = category;
            CanResize = canresize;
            CanBeOverlapping = canbeoverlapping;
        }
        [DataMember]
        public int Id { get; protected set; }
        [DataMember]
        public int IdProject { get; protected set; }
        [DataMember]
        public int Category { get; protected set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Layer { get; set; }
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        [DataMember]
        public int Rotate { get; set; }
        [DataMember]
        public int Texture { get; set; }
        [DataMember]
        public DbGeometry Polygon { get; set; }
        [DataMember]
        public bool CanResize { get; protected set; }
        [DataMember]
        public bool CanBeOverlapping { get; protected set; }
    }
}
