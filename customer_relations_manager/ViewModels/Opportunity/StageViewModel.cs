using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace customer_relations_manager.ViewModels.Opportunity
{
    public class StageViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required, Range(0,100)]
        public int Value { get; set; }
    }
}
