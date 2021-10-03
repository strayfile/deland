namespace LandscapeDesignServer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProjectBuilding
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_Project { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_Building { get; set; }

        public int Id_Texture { get; set; }

        public int Rotate { get; set; }

        public int Layer { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public virtual Building Building { get; set; }

        public virtual BuildingTexture BuildingTexture { get; set; }

        public virtual Project Project { get; set; }
    }
}
