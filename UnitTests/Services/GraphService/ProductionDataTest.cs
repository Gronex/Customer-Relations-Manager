using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.ExtentionMethods;
using Core.ApplicationServices.Graph.DataHolders;
using Core.ApplicationServices.ServiceInterfaces;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Xunit;

namespace UnitTests.Services.GraphService
{
    public class ProductionDataTest
    {
        private readonly IGraphService _service;
        public ProductionDataTest()
        {
            _service = new Core.ApplicationServices.Graph.GraphService();
        }

        [Fact]
        public void ContainsUsers()
        {
            var opportunities = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 10, StartDate = DateTime.Today.Date, EndDate = DateTime.Today.Date.AddMonths(1), Owner = new User {Email = "user1"}
                },
                new Opportunity
                {
                    Amount = 10, StartDate = DateTime.Today.Date, EndDate = DateTime.Today.Date.AddMonths(1), Owner = new User {Email = "user1"}
                },
                new Opportunity
                {
                    Amount = 10, StartDate = DateTime.Today.Date, EndDate = DateTime.Today.Date.AddMonths(1), Owner = new User {Email = "user2"}
                }
            };

            var result = _service.GenerateProductionDataTable(opportunities, DateTime.MinValue, DateTime.MaxValue, false);

            
            var users = new List<string> { "user1", "user2"};

            Assert.Equal(result.Keys.ToList(), users);
        }

        [Fact]
        public void RoundingToMonth()
        {
            var date = new DateTime(2016, 3, 15);
            var opportunity = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 10,
                    StartDate = date,
                    EndDate = date.AddMonths(1),
                    Owner = new User { Email = "user1"}
                }
            };

            var targetDate = new DateTime(2016, 3, 20);
            var result = _service.GenerateProductionDataTable(opportunity, targetDate, targetDate.AddMonths(1), false);
            Assert.NotEmpty(result["user1"]);
        }

        [Fact]
        public void ContainsDataForGivenDate()
        {
            var opportunity = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 10,
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date.AddMonths(1),
                    Owner = new User { Email = "user1"}
                }
            };
            var result = _service.GenerateProductionDataTable(opportunity, DateTime.Today, DateTime.Today.AddMonths(1), false);

            Assert.Equal(new List<double> {10.0}, result["user1"].Select(r => r.Value));
        }

        [Fact]
        public void DataSpreadOutOverPeriod()
        {
            var opportunity = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 10,
                    StartDate = DateTime.Today.Date,
                    EndDate = DateTime.Today.Date.AddMonths(2),
                    Owner = new User { Email = "user1"}
                }
            };

            var result = _service.GenerateProductionDataTable(opportunity, DateTime.MinValue, DateTime.MaxValue, false);

            var month1 = result["user1"].SingleOrDefault(r => r.Period == DateTime.Today.RoundToMonth());
            var month2 = result["user1"].SingleOrDefault(r => r.Period == DateTime.Today.RoundToMonth().AddMonths(1));

            Assert.Equal(5.0, month1?.Value);
            Assert.Equal(5.0, month2?.Value);
        }

        [Fact]
        public void DataForLessThanAMonth()
        {
            var startDate = new DateTime(2016,1,1).Date; // Cant do today since it will at the en of the month breake the test
            var opportunity = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 10,
                    StartDate = startDate,
                    EndDate = startDate.AddDays(1),
                    Owner = new User {Email = "user1"}
                }
            };
            var result = _service.GenerateProductionDataTable(opportunity, DateTime.MinValue, DateTime.MaxValue, false);

            Assert.Equal(10.0, result["user1"].SingleOrDefault(r => r.Period == startDate.RoundToMonth())?.Value);
        }

        [Fact]
        public void DifferentDatesInSameMonthIsSummerized()
        {
            var user = new User {Email = "user1"};
            var startDate = new DateTime(2016,1,1).Date; // Cant do today since it will at the en of the month breake the test
            var opportunity = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 10,
                    StartDate = startDate,
                    EndDate = startDate.AddDays(1),
                    Owner = user
                },
                new Opportunity
                {
                    Amount = 10,
                    StartDate = startDate.AddDays(2),
                    EndDate = startDate.AddDays(5),
                    Owner = user
                }
            };
            var result = _service.GenerateProductionDataTable(opportunity, DateTime.MinValue, DateTime.MaxValue, false);

            Assert.Equal(20.0, result["user1"].SingleOrDefault(r => r.Period == startDate.RoundToMonth())?.Value);
        }

        [Fact]
        public void WeightingAsPercentage()
        {
            var user = new User { Email = "user1" };
            var startDate = new DateTime(2016, 1, 1).Date; // Cant do today since it will at the en of the month breake the test
            var opportunity = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 1000,
                    StartDate = startDate,
                    EndDate = startDate.AddDays(1),
                    Owner = user,
                    Percentage = 50
                }
            };
            var result = _service.GenerateProductionDataTable(opportunity, DateTime.MinValue, DateTime.MaxValue, true);

            Assert.Equal(500, result["user1"].SingleOrDefault(r => r.Period == startDate.RoundToMonth())?.Value);
        }
    }
}
