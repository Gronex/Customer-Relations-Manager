using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DomainModels.Activities
{
    public class ActivityCategory
    {
        [Key]
        public int Id { get; set; }

        [Required, Index(IsUnique = true)]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
