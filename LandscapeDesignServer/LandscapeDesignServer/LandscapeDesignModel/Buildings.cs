using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Runtime.Serialization;

namespace LandscapeDesignServer.LandscapeDesignModel
{
    [Serializable]
    [DataContract]
    public class LDBuilding
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdUser { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Category { get; set; }
        [DataMember]
        public bool Overlappable { get; set; }
        [DataMember]
        public DbGeometry Polygon { get; set; }
        [DataMember]
        public bool Saved { get; set; }
        [DataMember]
        public bool Share { get; set; }
    }

    [Serializable]
    [DataContract]
    public class LDBuildingCharacteristics
    {
        public LDBuildingCharacteristics()
        {
            Category = new Dictionary<int, string>();
            Overlapable = new Dictionary<int, bool>();
        }
        [DataMember]
        public Dictionary<int, string> Category { get; set; }
        [DataMember]
        public Dictionary<int, bool> Overlapable { get; set; }
    }

}
