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

                var buffedGoals = BuffOutGoals(goals);


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

        public static IEnumerable<ProductionGoal> BuffOutGoals(IEnumerable<ProductionGoal> goalList)
        {
            var goals = goalList.ToList();
            if(!goals.Any()) return goals;

            var firstGoal = goals.FirstOrDefault();
            var lastGoal = goals.LastOrDefault();

            var difference = MonthDifference(firstGoal.StartDate, lastGoal.StartDate);

            // adds one to account for a difference by 1 month means that the two months are
            // right next to one another
            if (goals.Count >= difference + 1) return goals;

            var toAdd = new List<ProductionGoal>();
            var last = firstGoal.StartDate;

            foreach (var goal in goals)
            {
                if (last.AddMonths(1) == goal.StartDate)
                {
                    last = goal.StartDate;
                    continue;
                }
                while (last.AddMonths(1) < goal.StartDate)
                {
                    last = last.AddMonths(1);
                    toAdd.Add(new ProductionGoal
                    {
                        StartDate = last,
                        Goal = goal.Goal
                    });
                }
            }
            goals.AddRange(toAdd);
            return goals.OrderBy(g => g.StartDate);
        }

    }
}
