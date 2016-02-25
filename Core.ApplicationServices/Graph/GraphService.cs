using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Graph;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Core.DomainServices.Services;

namespace Core.ApplicationServices.Graph
{
    public class GraphService : IGraphService
    {
        public IDictionary<string, DataSet> GenerateGoalDataSets(IEnumerable<User> users)
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

        public IDictionary<object, List<object>> GenerateGoalDataTable(IEnumerable<User> users)
        {
            var usersWithGoals = users.Where(u => u.Goals.Any()).ToList();
            var data = new List<IEnumerable<object>>();

            var testData = new Dictionary<object, List<object>> {{"header", new List<object> {"Date"}}};


            var header = new List<string> {"Date"};

            var minDate = usersWithGoals.Select(u => u.Goals.Min(g => g.StartDate)).Min();
            var maxDate = usersWithGoals.Select(u => u.Goals.Max(g => g.StartDate)).Max();

            foreach (var user in usersWithGoals)
            {
                var currentDate = minDate;
                //header.Add(user.Name);
                testData["header"].Add(user.Name);

                var goals = user.Goals.OrderBy(g => g.StartDate);
                ProductionGoal lastGoal = null;

                foreach (var goal in goals)
                {
                    while (goal.StartDate > currentDate)
                    {
                        if (lastGoal == null)
                        {
                            AddOrCreate(testData, currentDate, null);
                            currentDate = currentDate.AddMonths(1);
                            continue;
                        }
                        AddOrCreate(testData, currentDate, lastGoal.Goal);
                        currentDate = currentDate.AddMonths(1);
                    }
                    lastGoal = goal;
                }

                while (maxDate >= currentDate)
                {
                    if (lastGoal == null)
                    {
                        AddOrCreate(testData, currentDate, null);
                        continue;
                    }
                    AddOrCreate(testData, currentDate, lastGoal.Goal);
                    currentDate = currentDate.AddMonths(1);
                }
            }

            return testData;
        }

        private void AddOrCreate(IDictionary<object, List<object>> dict, object key, object value)
        {
            if (dict.ContainsKey(key))
                dict[key].Add(value);
            else
                dict.Add(key, new List<object> { value });
        }

        public IDictionary<string, DataSet> GenerateProductionDataSets(IEnumerable<User> users)
        {
            var dict = new Dictionary<string, DataSet>();

            foreach (var user in users)
            {
                var production = user.Opportunities
                    .SelectMany(SpreadOutEarnings)
                    .GroupBy(t => t.Item1);

                dict.Add(user.Id, new DataSet
                {
                    Label = user.Name,
                    DataPoints = production.Select(p => new DataPoint
                    {
                        Label = p.Key,
                        Value = p.Sum(t => t.Item2)
                    })
                });
            }

            return dict;
        }


        public IEnumerable<Tuple<DateTime, double>> SpreadOutEarnings(Opportunity opportunity)
        {

            var difference = MonthDifference(opportunity.StartDate, opportunity.EndDate) + 1;

            var earningTimeList = new List<Tuple<DateTime, double>>();

            for (var i = 0; i < difference; i++)
            {
                earningTimeList.Add(new Tuple<DateTime, double>(opportunity.StartDate.AddMonths(i), difference));
            }

            return earningTimeList;
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
