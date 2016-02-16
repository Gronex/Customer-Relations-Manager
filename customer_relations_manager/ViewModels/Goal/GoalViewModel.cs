using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace customer_relations_manager.ViewModels.Goal
{
    public class GoalViewModel
    {
        public int Id { get; set; }

        [Required, Range(1, 12)]
        public int Month { get; set; }

        [Required, Range(2000, int.MaxValue)]
        public int Year { get; set; }

        [Required, Range(0, double.MaxValue)]
        public double Goal { get; set; }
    }
}
