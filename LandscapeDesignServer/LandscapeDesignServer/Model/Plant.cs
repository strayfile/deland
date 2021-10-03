namespace LandscapeDesignServer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Plant
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Plant()
        {
            ProjectPlants = new HashSet<ProjectPlant>();
        }

        public int Id { get; set; }

        public int Id_User { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public int Id_Category { get; set; }

        public int Temperature_Min { get; set; }

        public int Temperature_Max { get; set; }

        public int Id_Soil { get; set; }

        public int Id_Soil_Type { get; set; }

        public int Id_Lightning { get; set; }

        public int Id_Watering { get; set; }

        public int Id_Care { get; set; }

        public bool Resizable { get; set; }

        public bool Overlappable { get; set; }

        public int Id_Texture { get; set; }

        public bool Share { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public virtual NoresizablePlant NoresizablePlant { get; set; }

        public virtual PlantCare PlantCare { get; set; }

        public virtual PlantCategory PlantCategory { get; set; }

        public virtual PlantLightning PlantLightning { get; set; }

        public virtual PlantSoil PlantSoil { get; set; }

        public virtual PlantSoilType PlantSoilType { get; set; }

        public virtual PlantTexture PlantTexture { get; set; }

        public virtual User User { get; set; }

        public virtual PlantWatering PlantWatering { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProjectPlant> ProjectPlants { get; set; }
    }
}
