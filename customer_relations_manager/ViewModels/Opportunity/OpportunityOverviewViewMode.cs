using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using customer_relations_manager.ViewModels.Company;
using customer_relations_manager.ViewModels.User;

namespace customer_relations_manager.ViewModels.Opportunity
{
    public class OpportunityOverviewViewMode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string CompanyName { get; set; }
        public double Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpectedClose { get; set; }
        public int Percentage { get; set; }
        public StageViewModel Stage { get; set; }
    }
}
