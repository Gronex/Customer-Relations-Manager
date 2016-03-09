using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.ExtentionMethods;
using Core.ApplicationServices.Graph.DataHolders;
using Core.ApplicationServices.ServiceInterfaces;
using Core.DomainModels.Graph;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Core.DomainServices;

namespace Core.ApplicationServices.Graph
{
    public class GraphService : IGraphService
    {

        public IDictionary<object, List<object>> GenerateGoalDataTable(
            IEnumerable<User> users,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var usersWithGoals = users
                .Where(u => u.Goals.Any(g => !endDate.HasValue || g.StartDate <= endDate)).ToList();

            var data = new Dictionary<object, List<object>> {{"header", new List<object>()}};

            var minDate = startDate ?? usersWithGoals.Select(u => u.Goals.Min(g => g.StartDate)).Min();
            var maxDate = endDate ?? usersWithGoals.Select(u => u.Goals.Max(g => g.StartDate)).Max();

            foreach (var user in usersWithGoals)
            {
                var currentDate = minDate;
                data["header"].Add(user.Name);

                var goals = user.Goals
                    .OrderBy(g => g.StartDate);
                var lastGoal = goals.LastOrDefault(g => g.StartDate <= minDate);

                foreach (var goal in goals.Where(g => g.StartDate >= minDate && g.StartDate <= maxDate))
                {
                    while (goal.StartDate > currentDate)
                    {
                        if (lastGoal == null || lastGoal.StartDate < minDate)
                        {
                            AddOrCreate(data, currentDate, null);
                            currentDate = currentDate.AddMonths(1);
                            continue;
                        }
                        AddOrCreate(data, currentDate, lastGoal.Goal);
                        currentDate = currentDate.AddMonths(1);
                    }
                    lastGoal = goal;
                }

                while (maxDate >= currentDate)
                {
                    if (lastGoal == null)
                    {
                        AddOrCreate(data, currentDate, null);
                        continue;
                    }
                    AddOrCreate(data, currentDate, lastGoal.Goal);
                    currentDate = currentDate.AddMonths(1);
                }
            }

            return data;
        }

        public IDictionary<string, IEnumerable<UserGraphData>> GenerateGoalDataTable(IEnumerable<ProductionGoal> goals, DateTime startDate)
        {
            return goals.GroupBy(g => g.User)
                .SelectMany(
                    gr =>
                        gr.OrderBy(g => g.StartDate)
                        .WhereWithLookahead((c, n) => n == null || n.StartDate > startDate)
                        .Select(g =>
                        {
                            if (g.StartDate < startDate) g.StartDate = startDate;
                            return g;
                        }))
                .GroupBy(g => g.User)
                .ToDictionary(g => g.Key.Email, group => group.Select(g => new UserGraphData
                {
                    Value = g.Goal,
                    Period = g.StartDate,
                    User = new SimpleUser
                    {
                        Email = group.Key.Email,
                        FirstName = group.Key.FirstName,
                        LastName = group.Key.LastName
                    }
                }).OrderBy(g => g.Period).AsEnumerable());
        }

        

        public IDictionary<string, IEnumerable<UserGraphData>> GenerateProductionDataTable(IEnumerable<Opportunity> opportunities, DateTime from, DateTime to)
        {
            return opportunities.SelectMany(o =>
            {
                var earningPerMonth = SpreadOutEarnings(o);

                return earningPerMonth.Select(epm => new
                {
                    User = new
                    {
                        o.Owner.FirstName,
                        o.Owner.LastName,
                        o.Owner.Email
                    },
                    Month = epm.Item1,
                    Amount = epm.Item2
                }).Where(epm => epm.Month >= from.RoundToMonth() && epm.Month <= to.RoundToMonth());
            }).GroupBy(o => new {o.Month, o.User}).Select(o => new
            {
                o.Key.User,
                o.Key.Month,
                Sum = o.Sum(os => os.Amount)
            }).GroupBy(o => o.User.Email).ToDictionary(o => o.Key, os => os.OrderBy(o => o.Month).Select(o => new UserGraphData
            {
                User = new SimpleUser
                {
                    Email = o.User.Email,
                    FirstName = o.User.FirstName,
                    LastName = o.User.LastName
                },
                Period = o.Month,
                Value = o.Sum
            }));
        } 

        private static void AddOrCreate(IDictionary<object, List<object>> dict, object key, object value)
        {
            if (dict.ContainsKey(key))
                dict[key].Add(value);
            else
                dict.Add(key, new List<object> { value });
        }


        public IEnumerable<Tuple<DateTime, double>> SpreadOutEarnings(Opportunity opportunity)
        {

            var difference = MonthDifference(opportunity.StartDate, opportunity.EndDate);

            var earningTimeList = new List<Tuple<DateTime, double>>();

            for (var i = 0; i < difference; i++)
            {
                earningTimeList.Add(new Tuple<DateTime, double>(opportunity.StartDate.RoundToMonth().AddMonths(i), opportunity.Amount/difference));
            }

            if (difference == 0)
            {
                earningTimeList.Add(new Tuple<DateTime, double>(opportunity.StartDate.RoundToMonth(), opportunity.Amount));
            }

            return earningTimeList;
        } 
        
        private static int MonthDifference(DateTime d1, DateTime d2)
        {
            var extraMonth = Math.Abs(d1.Day - d2.Day) > DateTime.DaysInMonth(d1.Year, d1.Month)/2 ? 1 : 0;

            return Math.Abs(d1.Month - d2.Month + 12*(d1.Year - d2.Year)) + extraMonth;
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
