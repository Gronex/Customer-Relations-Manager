using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModels.Users
{
    public class ProductionGoal
    {
        [Key]
        public int Id { get; set; }

        //{Month, Year, UserId} is unique
        public int Year { get; set; }
        public int Month { get; set; }
        public double Goal { get; set; }

        [Required, ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
