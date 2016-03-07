using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.ExtentionMethods;
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
        public void ContainsAllHeaders()
        {
            var users = new List<User>
            {
                new User { FirstName = "user1", LastName = "Last", Opportunities = new List<Opportunity>
                {
                    new Opportunity {Amount = 10, StartDate = DateTime.Today.Date, EndDate = DateTime.Today.Date.AddMonths(1)}
                } },
                new User { FirstName = "user2", LastName = "Last", Opportunities = new List<Opportunity>
                {
                    new Opportunity {Amount = 10, StartDate = DateTime.Today.Date, EndDate = DateTime.Today.Date.AddMonths(1)}
                } },
                new User { FirstName = "user3", LastName = "Last", Opportunities = new List<Opportunity>
                {
                    new Opportunity {Amount = 10, StartDate = DateTime.Today.Date, EndDate = DateTime.Today.Date.AddMonths(1)}
                } },
            };

            var result = _service.GenerateProductionDataTable(users);

            
            var expectedHeaders = users.Select(u => u.Name);

            Assert.Equal(result["header"], expectedHeaders);
        }

        [Fact]
        public void OnlyContainsData()
        {
            var users = new List<User>
            {
                new User { FirstName = "user1", LastName = "Last", Opportunities = new List<Opportunity>
                {
                    new Opportunity {Amount = 10, StartDate = DateTime.Today.Date, EndDate = DateTime.Today.Date.AddMonths(1)}
                } },
                new User { FirstName = "user2", LastName = "Last", Opportunities = new List<Opportunity>()},
                new User { FirstName = "user3", LastName = "Last", Opportunities = new List<Opportunity>()},
            };

            var result = _service.GenerateProductionDataTable(users);


            var expectedHeaders = new List<object> {"user1 Last"};

            Assert.Equal(result["header"], expectedHeaders);
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
                    EndDate = DateTime.Today.Date.AddMonths(1)
                }
            };
            var users = new List<User>
            {
                new User { FirstName = "user1", LastName = "Last", Opportunities =  opportunity}
            };

            var result = _service.GenerateProductionDataTable(users);

            Assert.Equal(new List<object> {10.0}, result[DateTime.Today.RoundToMonth()]);
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
                    EndDate = DateTime.Today.Date.AddMonths(2)
                }
            };
            var users = new List<User>
            {
                new User { FirstName = "user1", LastName = "Last", Opportunities =  opportunity}
            };

            var result = _service.GenerateProductionDataTable(users);

            Assert.Equal(new List<object> { 5.0 }, result[DateTime.Today.RoundToMonth()]);
            Assert.Equal(new List<object> { 5.0 }, result[DateTime.Today.RoundToMonth().AddMonths(1)]);
        }

        [Fact]
        public void DataForLessThanAMonth()
        {
            var opportunity = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 10,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1)
                }
            };
            var users = new List<User>
            {
                new User { FirstName = "user1", LastName = "Last", Opportunities =  opportunity}
            };

            var result = _service.GenerateProductionDataTable(users);

            Assert.Equal(new List<object> { 10.0 }, result[DateTime.Today.RoundToMonth()]);
        }

        [Fact]
        public void DifferentDatesInSameMonthIsSummerized()
        {
            var opportunity = new List<Opportunity>
            {
                new Opportunity
                {
                    Amount = 10,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1)
                },
                new Opportunity
                {
                    Amount = 10,
                    StartDate = DateTime.Today.AddDays(2),
                    EndDate = DateTime.Today.AddDays(5)
                }
            };
            var users = new List<User>
            {
                new User { FirstName = "user1", LastName = "Last", Opportunities =  opportunity}
            };

            var result = _service.GenerateProductionDataTable(users);

            Assert.Equal(new List<object> { 20.0 }, result[DateTime.Today.RoundToMonth()]);
        }

        [Fact]
        public void EvensOutDataInStart()
        {
            var users = new List<User>
            {
                new User { FirstName = "user1", LastName = "Last", Opportunities = new List<Opportunity> {
                    new Opportunity{
                        Amount = 10,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(1)
                    }}
                },
                new User { FirstName = "user2", LastName = "Last", Opportunities = new List<Opportunity> {
                    new Opportunity
                    {
                        Amount = 10,
                        StartDate = DateTime.Today.AddMonths(1),
                        EndDate = DateTime.Today.AddMonths(1)
                    }}
                }
            };

            var result = _service.GenerateProductionDataTable(users);

            Assert.Equal(2, result[DateTime.Today.RoundToMonth()].Count);
        }

        [Fact]
        public void EvensOutDataInEnd()
        {
            var users = new List<User>
            {
                new User { FirstName = "user1", LastName = "Last", Opportunities = new List<Opportunity> {
                    new Opportunity{
                        Amount = 10,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(1)
                    }}
                },
                new User { FirstName = "user2", LastName = "Last", Opportunities = new List<Opportunity> {
                    new Opportunity
                    {
                        Amount = 10,
                        StartDate = DateTime.Today.AddMonths(1),
                        EndDate = DateTime.Today.AddMonths(1)
                    }}
                }
            };

            var result = _service.GenerateProductionDataTable(users);

            Assert.Equal(2, result[DateTime.Today.RoundToMonth().AddMonths(1)].Count);
        }
    }
}
