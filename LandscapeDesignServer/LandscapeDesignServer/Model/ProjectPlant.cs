namespace LandscapeDesignServer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProjectPlant
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_Project { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_Plant { get; set; }

        [Required]
        public DbGeometry Draw { get; set; }

        public int Layer { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public virtual Plant Plant { get; set; }

        public virtual Project Project { get; set; }
    }
}
