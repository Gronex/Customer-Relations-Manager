using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using customer_relations_manager.ViewModels.Company;
using customer_relations_manager.ViewModels.User;

namespace customer_relations_manager.ViewModels.Activity
{
    public class ActivityViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required, EmailAddress]
        public string ResponsibleEmail { get; set; }
        public string ResponsibleName { get; set; }
        [Required]
        public bool Done { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyId { get; set; }
        [Required]
        public DateTime? DueDate { get; set; }
        public DateTime? DueTime { get; set; }
    }
}
