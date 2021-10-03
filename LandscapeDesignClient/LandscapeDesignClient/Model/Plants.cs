using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LandscapeDesignClient.LandscapeDesignReference;

namespace LandscapeDesignClient.Model
{
    public abstract class Plant: LDPlant
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Plant()
        {

        }

        protected void SetData(int category, bool resizable, bool overlappable)
        {
            Category = category;
            Resizable = resizable;
            Overlappable = overlappable;
        }
    }

    public class Grass : Plant
    {
        public Grass(int iduser)
        {
            IdUser = iduser;
            SetData(1, true, true);
        }
    }
    public class Bush : Plant
    {
        public Bush(int iduser)
        {
            IdUser = iduser;
            SetData(2, false, false);
        }
    }
    public class Tree : Plant
    {
        public Tree(int iduser)
        {
            IdUser = iduser;
            SetData(3, false, false);
        }
    }

}
