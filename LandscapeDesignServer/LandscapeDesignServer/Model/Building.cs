namespace LandscapeDesignServer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Building
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Building()
        {
            ProjectBuildings = new HashSet<ProjectBuilding>();
        }

        public int Id { get; set; }

        public int Id_User { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public int Id_Category { get; set; }

        [Required]
        public DbGeometry Draw { get; set; }

        public bool Saved { get; set; }

        public bool Shared { get; set; }

        public virtual BuildingCategory BuildingCategory { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectBuilding> ProjectBuildings { get; set; }
    }
}
