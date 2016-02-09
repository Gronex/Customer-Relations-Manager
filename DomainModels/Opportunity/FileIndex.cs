using System.ComponentModel.DataAnnotations;

namespace Core.DomainModels.Opportunity
{
    public class FileIndex
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OriginalName { get; set; }

        [Required]
        public string FilePath { get; set; }
    }
}
