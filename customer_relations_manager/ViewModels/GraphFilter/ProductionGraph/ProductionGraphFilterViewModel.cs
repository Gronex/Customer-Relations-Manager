using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using customer_relations_manager.ViewModels.Opportunity;
using customer_relations_manager.ViewModels.User;

namespace customer_relations_manager.ViewModels.GraphFilter.ProductionGraph
{
    public class ProductionGraphFilterViewModel
    {
        [Required]
        public string Name { get; set; }
        public bool Private { get; set; } = true;
        public bool Weighted { get; set; }
        public IEnumerable<UserOverviewViewModel> Users { get; set; }
        public IEnumerable<GroupViewModel> UserGroups { get; set; }
        public IEnumerable<GroupViewModel> Departments { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public IEnumerable<StageViewModel> Stages { get; set; }
    }
}
