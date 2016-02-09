using System.ComponentModel.DataAnnotations;

namespace Core.DomainModels.Activities
{
    public class ActivityCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
