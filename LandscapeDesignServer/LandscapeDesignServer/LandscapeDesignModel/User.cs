using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace LandscapeDesignServer.LandscapeDesignModel
{
    [Serializable]
    [DataContract]
    public class LDUser
    {
        public LDUser(int id, string email, string pass = null)
        {
            Id = id;
            Password = pass;
            Email = email;
        }
        [DataMember]
        public int Id { get; protected set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Password { get; protected set; }
        [DataMember]
        public string Email { get; protected set; }
    }
}
