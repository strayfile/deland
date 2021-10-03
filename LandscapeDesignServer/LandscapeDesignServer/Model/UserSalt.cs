namespace LandscapeDesignServer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserSalt")]
    public partial class UserSalt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id_User { get; set; }

        [Required]
        [StringLength(255)]
        public string Salt { get; set; }

        public virtual User User { get; set; }
    }
}
