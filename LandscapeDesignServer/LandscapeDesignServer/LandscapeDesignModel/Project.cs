using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace LandscapeDesignServer.LandscapeDesignModel
{
    [Serializable]
    [DataContract]
    public class LDProject
    {
        public LDProject(int id, int iduser, DateTime createDate, bool share)
        {
            Id = id;
            IdUser = iduser;
            CreateDate = createDate;
            Share = share;
            Entities = new ObservableCollection<LDEntity>();
        }
        [DataMember]
        public int Id { get; protected set; }
        [DataMember]
        public int IdUser { get; protected set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool Share { get; set; }
        [DataMember]
        public DateTime CreateDate { get; protected set; }
        [DataMember]
        public DateTime ChangeDate { get; set; }
        [DataMember]
        public int TemperatureMin { get; set; }
        [DataMember]
        public int TemperatureMax { get; set; }
        [DataMember]
        public int Lightning { get; set; }
        [DataMember]
        public int Soil { get; set; }
        [DataMember]
        public int SoilType { get; set; }

        [DataMember]
        public ObservableCollection<LDEntity> Entities { get; protected set; }
    }
}