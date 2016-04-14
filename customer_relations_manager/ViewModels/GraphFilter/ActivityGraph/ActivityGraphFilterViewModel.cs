using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using customer_relations_manager.ViewModels.User;

namespace customer_relations_manager.ViewModels.GraphFilter.ActivityGraph
{
    public class ActivityGraphFilterViewModel
    {
        [Required]
        public string Name { get; set; }
        public bool Private { get; set; } = true;
        public string OwnerEmail { get; set; }
        public IEnumerable<UserOverviewViewModel> Users { get; set; }
        public IEnumerable<GroupViewModel> UserGroups { get; set; }
    }
}
