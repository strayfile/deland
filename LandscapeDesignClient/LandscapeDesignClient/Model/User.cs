using System;
using System.Collections.ObjectModel;
using LandscapeDesignClient.LandscapeDesignReference;

namespace LandscapeDesignClient.Model
{
    public class User: LDUser
    {
        public User(int id, string email)
        {
            Id = id;
            Email = email;
        }
    }
	
}
