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
                var goals = user.Goals.Select(g => new DateDataPoint
                {
                    Date = new DateTime(g.Year, g.Month, 1),
                    Value = g.Goal
                }).OrderBy(g => g.Date);

                var buffedGoals = BuffOut(goals);


                dict.Add(user.Id, new DataSet
                {
                    Label = user.Name,
                    DataPoints = buffedGoals.Select(g => new DataPoint
                    {
                        Label = g.Date,
                        Value = g.Value
                    })
                });
            }

            return dict;
        }



        private static int MonthDifference(DateTime d1, DateTime d2)
        {
            return Math.Abs((d1.Month - d2.Month) + 12*(d1.Year - d2.Year));
        }

        public static IEnumerable<DateDataPoint> BuffOut(IEnumerable<DateDataPoint> list)
        {
            var goals = list.ToList();
            var firstGoal = goals.FirstOrDefault();
            var lastGoal = goals.LastOrDefault();

            var difference = MonthDifference(firstGoal.Date, lastGoal.Date);

            // adds one to account for a difference by 1 month means that the two months are
            // right next to one another
            if (goals.Count() >= difference + 1) return goals;

            var toAdd = new List<DateDataPoint>();
            var last = firstGoal.Date;

            foreach (var goal in goals)
            {
                if (last.AddMonths(1) == goal.Date)
                {
                    last = goal.Date;
                    continue;
                }
                while (last.AddMonths(1) < goal.Date)
                {
                    last = last.AddMonths(1);
                    toAdd.Add(new DateDataPoint
                    {
                        Date = last,
                        Value = goal.Value
                    });
                }
            }
            goals.AddRange(toAdd);
            return goals.OrderBy(g => g.Date);
        }

    }
}
