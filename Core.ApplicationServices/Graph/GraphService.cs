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
                    Period = DateTime.SpecifyKind(g.StartDate.Date, DateTimeKind.Utc),
                    User = new SimpleUser
                    {
                        Email = group.Key.Email,
                        FirstName = group.Key.FirstName,
                        LastName = group.Key.LastName
                    }
                }).OrderBy(g => g.Period).AsEnumerable());
        }

        

        public IDictionary<string, IEnumerable<UserGraphData>> GenerateProductionDataTable(IEnumerable<Opportunity> opportunities, DateTime from, DateTime to, bool weighted)
        {
            return opportunities.SelectMany(o =>
            {
                var earningPerMonth = SpreadOutEarnings(o);
                var modifyer = weighted ? (o.Percentage/100.0) : 1;

                return earningPerMonth.Select(epm => new
                {
                    User = new
                    {
                        o.Owner.FirstName,
                        o.Owner.LastName,
                        o.Owner.Email
                    },
                    Month = epm.Item1,
                    Amount = epm.Item2 * modifyer
                }).Where(epm => epm.Month >= from.RoundToMonth() && epm.Month <= to.RoundToMonth());
            }).GroupBy(o => new {o.Month, o.User})
            .Select(o => new
            {
                o.Key.User,
                o.Key.Month,
                Sum = o.Sum(os => os.Amount)
            }).GroupBy(o => o.User.Email)
            .ToDictionary(o => o.Key, os => os.OrderBy(o => o.Month).Select(o => new UserGraphData
            {
                User = new SimpleUser
                {
                    Email = o.User.Email,
                    FirstName = o.User.FirstName,
                    LastName = o.User.LastName
                },
                Period = o.Month.Date,
                Value = o.Sum
            }));
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
    }
}
