namespace LandscapeDesignServer.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LandscapeDesignDb : DbContext
    {
        public LandscapeDesignDb()
            : base("name=LandscapeDesignConnection")
        {
        }

        public virtual DbSet<BuildingCategory> BuildingCategories { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<BuildingTexture> BuildingTextures { get; set; }
        public virtual DbSet<NoresizablePlant> NoresizablePlants { get; set; }
        public virtual DbSet<PlantCare> PlantCares { get; set; }
        public virtual DbSet<PlantCategory> PlantCategories { get; set; }
        public virtual DbSet<PlantLightning> PlantLightnings { get; set; }
        public virtual DbSet<Plant> Plants { get; set; }
        public virtual DbSet<PlantSoil> PlantSoils { get; set; }
        public virtual DbSet<PlantSoilType> PlantSoilTypes { get; set; }
        public virtual DbSet<PlantTexture> PlantTextures { get; set; }
        public virtual DbSet<PlantWatering> PlantWaterings { get; set; }
        public virtual DbSet<ProjectBuilding> ProjectBuildings { get; set; }
        public virtual DbSet<ProjectPlant> ProjectPlants { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<UserKey> UserKeys { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSalt> UserSalts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BuildingCategory>()
                .HasMany(e => e.Buildings)
                .WithRequired(e => e.BuildingCategory)
                .HasForeignKey(e => e.Id_Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Building>()
                .HasMany(e => e.ProjectBuildings)
                .WithRequired(e => e.Building)
                .HasForeignKey(e => e.Id_Building)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BuildingTexture>()
                .Property(e => e.Path)
                .IsUnicode(false);

            modelBuilder.Entity<BuildingTexture>()
                .HasMany(e => e.ProjectBuildings)
                .WithRequired(e => e.BuildingTexture)
                .HasForeignKey(e => e.Id_Texture)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantCare>()
                .HasMany(e => e.Plants)
                .WithRequired(e => e.PlantCare)
                .HasForeignKey(e => e.Id_Care)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantCategory>()
                .HasMany(e => e.Plants)
                .WithRequired(e => e.PlantCategory)
                .HasForeignKey(e => e.Id_Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantLightning>()
                .HasMany(e => e.Plants)
                .WithRequired(e => e.PlantLightning)
                .HasForeignKey(e => e.Id_Lightning)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantLightning>()
               .HasMany(e => e.Projects)
               .WithRequired(e => e.PlantLightning)
               .HasForeignKey(e => e.Id_Lightning)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Plant>()
                .HasOptional(e => e.NoresizablePlant)
                .WithRequired(e => e.Plant)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Plant>()
                .HasMany(e => e.ProjectPlants)
                .WithRequired(e => e.Plant)
                .HasForeignKey(e => e.Id_Plant)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantSoil>()
                .HasMany(e => e.Plants)
                .WithRequired(e => e.PlantSoil)
                .HasForeignKey(e => e.Id_Soil)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantSoil>()
                .HasMany(e => e.Projects)
                .WithRequired(e => e.PlantSoil)
                .HasForeignKey(e => e.Id_Soil)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantSoilType>()
                .HasMany(e => e.Plants)
                .WithRequired(e => e.PlantSoilType)
                .HasForeignKey(e => e.Id_Soil_Type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantSoilType>()
                .HasMany(e => e.Projects)
                .WithRequired(e => e.PlantSoilType)
                .HasForeignKey(e => e.Id_Soil_Type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantTexture>()
                .Property(e => e.Path)
                .IsUnicode(false);

            modelBuilder.Entity<PlantTexture>()
                .HasMany(e => e.Plants)
                .WithRequired(e => e.PlantTexture)
                .HasForeignKey(e => e.Id_Texture)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlantWatering>()
                .HasMany(e => e.Plants)
                .WithRequired(e => e.PlantWatering)
                .HasForeignKey(e => e.Id_Watering)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.ProjectBuildings)
                .WithRequired(e => e.Project)
                .HasForeignKey(e => e.Id_Project);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.ProjectPlants)
                .WithRequired(e => e.Project)
                .HasForeignKey(e => e.Id_Project)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRole>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UserRole)
                .HasForeignKey(e => e.Id_Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Buildings)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.Id_User);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Plants)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.Id_User);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Projects)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.Id_User);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.UserKey)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete();

            modelBuilder.Entity<User>()
                .HasOptional(e => e.UserSalt)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete();
        }
    }

    //Database.SetInitializer<LandscapeDesignDb>(new LandscapeDesignDbInitializer());

    //public sealed class LandscapeDesignDbInitializer : CreateDatabaseIfNotExists<LandscapeDesignDb>
    //{
    //    protected override void Seed(LandscapeDesignDb db)
    //    {
    //        //BuildingCategory bc1 = db.BuildingCategories.Create();
    //        //bc1.Name = "Покрытие";
    //        //bc1.Overlappable = true;
    //        //db.BuildingCategories.Add(bc1);
    //        //BuildingCategory bc2 = db.BuildingCategories.Create();
    //        //bc2.Name = "Постройка";
    //        //bc2.Overlappable = false;
    //        //db.BuildingCategories.Add(bc2);
    //        //BuildingCategory bc3 = db.BuildingCategories.Create();
    //        //bc3.Name = "Забор";
    //        //bc3.Overlappable = false;
    //        //db.BuildingCategories.Add(bc3);
    //        //BuildingCategory bc4 = db.BuildingCategories.Create();
    //        //bc4.Name = "Декоративное сооружение";
    //        //bc4.Overlappable = false;
    //        //db.BuildingCategories.Add(bc4);

    //        //PlantCategory pc1 = db.PlantCategories.Create();
    //        //pc1.Name = "Травы";
    //        //db.PlantCategories.Add(pc1);
    //        //PlantCategory pc2 = db.PlantCategories.Create();
    //        //pc2.Name = "Кустарники";
    //        //db.PlantCategories.Add(pc2);
    //        //PlantCategory pc3 = db.PlantCategories.Create();
    //        //pc3.Name = "Деревья";
    //        //db.PlantCategories.Add(pc3);

    //        //PlantSoil ps1 = db.PlantSoils.Create();
    //        //ps1.Name = "глинистая";
    //        //db.PlantSoils.Add(ps1);
    //        //PlantSoil ps2 = db.PlantSoils.Create();
    //        //ps2.Name = "суглинистая";
    //        //db.PlantSoils.Add(ps2);
    //        //PlantSoil ps3 = db.PlantSoils.Create();
    //        //ps3.Name = "песчаная";
    //        //db.PlantSoils.Add(ps3);
    //        //PlantSoil ps4 = db.PlantSoils.Create();
    //        //ps4.Name = "супесчаная";
    //        //db.PlantSoils.Add(ps4);
    //        //PlantSoil ps5 = db.PlantSoils.Create();
    //        //ps5.Name = "известковая";
    //        //db.PlantSoils.Add(ps5);
    //        //PlantSoil ps6 = db.PlantSoils.Create();
    //        //ps6.Name = "торфяная";
    //        //db.PlantSoils.Add(ps6);
    //        //PlantSoil ps7 = db.PlantSoils.Create();
    //        //ps7.Name = "черноземная";
    //        //db.PlantSoils.Add(ps7);

    //        //PlantSoilType pst1 = db.PlantSoilTypes.Create();
    //        //pst1.Type = "любая";
    //        //pst1.pH = "3.8 - 9.1";
    //        //db.PlantSoilTypes.Add(pst1);
    //        //PlantSoilType pst2 = db.PlantSoilTypes.Create();
    //        //pst2.Type = "сильнокислая";
    //        //pst2.pH = "< 4.5";
    //        //db.PlantSoilTypes.Add(pst2);
    //        //PlantSoilType pst3 = db.PlantSoilTypes.Create();
    //        //pst3.Type = "среднекислая";
    //        //pst3.pH = "4.6 - 5.5";
    //        //db.PlantSoilTypes.Add(pst3);
    //        //PlantSoilType pst4 = db.PlantSoilTypes.Create();
    //        //pst4.Type = "слабокислая";
    //        //pst4.pH = "5.6 - 6.8";
    //        //db.PlantSoilTypes.Add(pst4);

    //        //db.SaveChanges();

    //    }

    //    // ('нейтральная', '6.9 - 7.3'), ('слабощелочная', '7.4 - 8.0'), ('среднещелочная', '8.1 - 8.5'), ('сильнощелочная', '> 8.6')
    //    //INSERT INTO PlantLightning
    //    //VALUES('тень'), ('полутень'), ('среднее'), ('солнце')

    //    //INSERT INTO PlantWatering
    //    //VALUES('слабый'), ('умеренный'), ('обильный')

    //    //INSERT INTO PlantCare
    //    //VALUES('нетребовательный'), ('средний'), ('сложный')

    //    //INSERT INTO PlantTextures
    //    //VALUES(''), (''), ('')

    //    //INSERT INTO Plants
    //    //    (1, 'Луговик дернистый', 1, 18, 25, 2, 1, 4, 3, 1, 0, 0, 1, 1, ''),
    //    //	(1, 'Ракитник', 2, 10, 25, 4, 5, 4, 3, 1, 0, 0, 2, 1, ''), 
    //    //	(1, 'Черешня', 3, 0, 30, 2, 5, 4, 3, 1, 0, 0, 3, 1, ''),
    //    //	(1, 'Полевица', 1, 15, 25, 7, 1, 4, 3, 1, 1, 1, 1, 1, 'Злаковое многолетнее растение, растет довольно медленно. Может обходиться без удобрений и нетребовательно к влажности воздуха.')

    //    //INSERT INTO NoresizablePlants
    //    //VALUES(1, 10, 10), (2, 15, 30), (3, 25, 50)
    //}

}
