using System.ComponentModel.DataAnnotations;

namespace Core.DomainModels.Opportunity
{
    public class Stage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
