using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModels.Users
{
    public class ProductionGoal
    {
        [Key]
        public int Id { get; set; }
        public double Goal { get; set; }
        [Index("DateUser", 1, IsUnique = true)]
        public DateTime StartDate { get; set; }
        [Required, ForeignKey(nameof(User))]
        [Index("DateUser", 2, IsUnique = true)]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
