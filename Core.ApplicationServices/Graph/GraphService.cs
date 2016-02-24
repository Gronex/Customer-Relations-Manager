using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Graph;
using Core.DomainModels.Users;
using Core.DomainServices.Services;

namespace Core.ApplicationServices.Graph
{
    public class GraphService : IGraphService
    {
        public IDictionary<string, DataSet> GenerateProductionDataSets(IEnumerable<User> users)
        {
            var dict = new Dictionary<string, DataSet>();

            foreach (var user in users)
            {
                var goals = user.Goals.OrderBy(g => g.StartDate);
                
                var buffedGoals = BuffOutGoals(goals, !user.Active ? user.EndDate : DateTime.UtcNow.Date);


                dict.Add(user.Id, new DataSet
                {
                    Label = user.Name,
                    DataPoints = buffedGoals.Select(g => new DataPoint
                    {
                        Label = g.StartDate,
                        Value = g.Goal
                    })
                });
            }

            return dict;
        }



        private static int MonthDifference(DateTime d1, DateTime d2)
        {
            return Math.Abs((d1.Month - d2.Month) + 12*(d1.Year - d2.Year));
        }

        public static IEnumerable<ProductionGoal> BuffOutGoals(IEnumerable<ProductionGoal> goalList, DateTime? finalDate = null)
        {


            var goals = goalList.ToList();
            if(!goals.Any()) return goals;
            
            var firstGoal = goals.FirstOrDefault();
            var lastGoal = goals.LastOrDefault();

            if(finalDate == null) finalDate = lastGoal.StartDate;


            var difference = MonthDifference(firstGoal.StartDate, finalDate.Value);
            
            if (goals.Count > difference) return goals;

            var toAdd = new List<ProductionGoal>();
            var last = firstGoal;

            foreach (var goal in goals)
            {
                if (last.StartDate.AddMonths(1) == goal.StartDate)
                {
                    last = goal;
                    continue;
                }
                while (last.StartDate.AddMonths(1) < goal.StartDate)
                {
                    last = new ProductionGoal
                    {
                        StartDate = last.StartDate.AddMonths(1),
                        Goal = last.Goal
                    };
                    toAdd.Add(last);
                }
            }
            last = lastGoal;
            var remainder = MonthDifference(lastGoal.StartDate, finalDate.Value);

            for (var i = 1; i <= remainder; i++)
            {
                toAdd.Add(new ProductionGoal
                {
                    StartDate = last.StartDate.AddMonths(i),
                    Goal = last.Goal
                });
            }
            goals.AddRange(toAdd);

            return goals.OrderBy(g => g.StartDate);
        }
    }
}
