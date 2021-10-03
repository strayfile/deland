namespace LandscapeDesignServer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            ProjectBuildings = new HashSet<ProjectBuilding>();
            ProjectPlants = new HashSet<ProjectPlant>();
        }

        public int Id { get; set; }

        public int Id_User { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ChangeDate { get; set; }

        public bool Share { get; set; }

        public bool CanShared { get; set; }

        public int Temperature_Min { get; set; }

        public int Temperature_Max { get; set; }

        public int Id_Lightning { get; set; }

        public int Id_Soil { get; set; }

        public int Id_Soil_Type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectBuilding> ProjectBuildings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectPlant> ProjectPlants { get; set; }

        public virtual User User { get; set; }

        public virtual PlantLightning PlantLightning { get; set; }

        public virtual PlantSoil PlantSoil { get; set; }

        public virtual PlantSoilType PlantSoilType { get; set; }
    }
}
