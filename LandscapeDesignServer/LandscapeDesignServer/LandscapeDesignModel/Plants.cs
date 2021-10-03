using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LandscapeDesignServer.LandscapeDesignModel
{
    [Serializable]
    [DataContract]
    public class LDPlant
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
        public bool Resizable { get; set; }
        [DataMember]
        public bool Overlappable { get; set; }
        [DataMember]
        public int TemperatureMin { get; set; }
        [DataMember]
        public int TemperatureMax { get; set; }
        [DataMember]
        public int Soil { get; set; }
        [DataMember]
        public int SoilType { get; set; }
        [DataMember]
        public int Lightning { get; set; }
        [DataMember]
        public int Watering { get; set; }
        [DataMember]
        public int Care { get; set; }
        [DataMember]
        public int IdTexture { get; set; }
        [DataMember]
        public bool Share { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Radius { get; set; }
        [DataMember]
        public int Crown { get; set; }
    }

}
