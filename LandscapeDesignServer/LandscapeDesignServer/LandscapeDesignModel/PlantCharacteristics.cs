using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LandscapeDesignServer.LandscapeDesignModel
{
    [Serializable]
    [DataContract]
    public class LDPlantCharacteristics
    {
        public LDPlantCharacteristics()
        {
            Category = new Dictionary<int, string>();
            SoilNames = new Dictionary<int, string>();
            SoilTypes = new Dictionary<int, string>();
            SoilPH = new Dictionary<int, string>();
            Lightning = new Dictionary<int, string>();
            Watering = new Dictionary<int, string>();
            CareType = new Dictionary<int, string>();
        }
        [DataMember]
        public Dictionary<int, string> Category { get; set; }
        [DataMember]
        public Dictionary<int, string> SoilNames { get; set; }
        [DataMember]
        public Dictionary<int, string> SoilTypes { get; set; }
        [DataMember]
        public Dictionary<int, string> SoilPH { get; set; }
        [DataMember]
        public Dictionary<int, string> Lightning { get; set; }
        [DataMember]
        public Dictionary<int, string> Watering { get; set; }
        [DataMember]
        public Dictionary<int, string> CareType { get; set; }
    }

}
