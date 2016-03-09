using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace customer_relations_manager.ViewModels.Activity
{
    public class ActivityOverviewViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? DueTime { get; set; }
    }
}
