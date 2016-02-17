using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using customer_relations_manager.ViewModels.Company;
using customer_relations_manager.ViewModels.User;

namespace customer_relations_manager.ViewModels.Opportunity
{
    public class OpportunityViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        
        public UserOverviewViewModel Owner { get; set; }
        [Required]
        public CompanyOverviewViewModel Company { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        [Required]
        public DateTime? ExpectedClose { get; set; }
        public IEnumerable<UserGroupViewModel> Groups { get; set; }
        [Required]
        public double HourlyPrice { get; set; }
    }
}
