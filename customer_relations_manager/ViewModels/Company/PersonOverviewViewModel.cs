using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace customer_relations_manager.ViewModels.Company
{
    public class PersonOverviewViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // ReSharper disable once ConvertPropertyToExpressionBody
        public string Name { get { return $"{FirstName} {LastName}"; } }
        public string CompanyName { get; set; }
    }
}
