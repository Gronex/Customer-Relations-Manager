using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModels.Users
{
    public class ProductionGoal
    {
        [Key]
        public int Id { get; set; }

        //{Month, Year, UserId} is unique
        [Index("YearMonthUser", 1, IsUnique = true)]
        public int Year { get; set; }
        [Index("YearMonthUser", 2, IsUnique = true)]
        public int Month { get; set; }
        public double Goal { get; set; }

        [Required, ForeignKey(nameof(User))]
        [Index("YearMonthUser", 3, IsUnique = true)]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
