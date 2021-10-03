using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LandscapeDesignClient.LandscapeDesignReference;

namespace LandscapeDesignClient.Model
{
    public abstract class Building: LDBuilding
    {
        public Building(int iduser, int idCategory)
        {
            IdUser = iduser;
            Category = idCategory;
        }
    }


    public class Flooring : Building
    {
        public Flooring(int iduser, int idcategory): base(iduser, idcategory)
        {
            Overlappable = true;
        }
    }



    public class House : Building
    {
        public House(int iduser, int idcategory): base(iduser, idcategory)
        {
            Overlappable = false;
        }
    }

    public class Fence : Building
    {
        public Fence(int iduser, int idcategory) : base(iduser, idcategory)
        {
            Overlappable = false;
        }
    }

    public class Decoration: Building
    {
        public Decoration(int iduser, int idcategory) : base(iduser, idcategory)
        {
            Overlappable = false;
        }
    }
}
