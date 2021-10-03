namespace LandscapeDesignServer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NoresizablePlant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_Plant { get; set; }

        public int Radius { get; set; }

        public int Crown { get; set; }

        public virtual Plant Plant { get; set; }
    }
}
