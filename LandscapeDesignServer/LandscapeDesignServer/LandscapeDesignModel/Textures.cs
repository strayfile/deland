using System;
using System.Runtime.Serialization;

namespace LandscapeDesignServer.LandscapeDesignModel
{
    [Serializable]
    [DataContract]
    public class LDEntityTexture
    {
        public LDEntityTexture(int id, int iduser, int category, byte[] texture)
        {
            Id = id;
            IdUser = iduser;
            Texture = texture;
            Category = category;
        }
        [DataMember]
        public int Id { get; protected set; }
        [DataMember]
        public int IdUser { get; protected set; }
        [DataMember]
        public byte[] Texture { get; protected set; }
        [DataMember]
        public int Category { get; protected set; }
        [DataMember]
        public bool Share { get; set; }
    }
}
        
