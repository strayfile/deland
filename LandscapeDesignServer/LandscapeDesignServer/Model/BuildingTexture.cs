namespace LandscapeDesignServer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BuildingTexture
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BuildingTexture()
        {
            ProjectBuildings = new HashSet<ProjectBuilding>();
        }

        public int Id { get; set; }

        public int IdUser { get; set; }

        [Required]
        public string Path { get; set; }

        public bool Share { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectBuilding> ProjectBuildings { get; set; }
    }
}
