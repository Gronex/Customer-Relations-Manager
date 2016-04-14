using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using customer_relations_manager.Controllers;
using Core.ApplicationServices.Graph;
using Core.ApplicationServices.Graph.DataHolders;
using Core.DomainModels.Activities;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Core.DomainServices;
using Infrastructure.DataAccess.Repositories;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.IntegrationTests
{
    public class GraphTests
    {
        private readonly GraphController _controller;
        private readonly IApplicationContext _context;

        public GraphTests()
        {
            _context = new AppContextStub();
            var goalRepo = new GenericRepository<ProductionGoal>(_context);
            var opportunityRepo = new GenericRepository<Opportunity>(_context);
            var activityRepo = new GenericRepository<Activity>(_context);
            var service = new GraphService();

            _controller = new GraphController(goalRepo, opportunityRepo, activityRepo, service);
        }

        [Fact]
        public void GetGoals()
        {
            var user1 = new User { Email = "test1@test.com", Active = true };
            var user2 = new User { Email = "test2@test.com", Active = true };

            var data = new List<ProductionGoal>
            {
                new ProductionGoal
                {
                    Id = 1,
                    Goal = 100000,
                    StartDate = new DateTime(2016, 1, 1),
                    User = user1
                },
                new ProductionGoal
                {
                    Id = 2,
                    Goal = 100000,
                    StartDate = new DateTime(2016, 6, 1),
                    User = user2
                },
                new ProductionGoal
                {
                    Id = 3,
                    Goal = 200000,
                    StartDate = new DateTime(2016, 4, 1),
                    User = user1
                }
            };

            _context.Goals.AddRange(data);

            var result = _controller.Goal(null, null, new DateTime(2016,1,1), new DateTime(2017,1,1));
            var cast = Assert.IsType<OkNegotiatedContentResult<GraphEnvelope<IDictionary<string, IEnumerable<DateUserGraphData>>>>>(result).Content;

            var formatedResult = ToDynamic(cast.Data);

            Action<dynamic> c1 = gd => Assert.Equal(new { Email = "test1@test.com", Period = new DateTime(2016, 1, 1), Value = 100000 }, gd);
            Action<dynamic> c2 = gd => Assert.Equal(new { Email = "test1@test.com", Period = new DateTime(2016, 4, 1), Value = 200000 }, gd);
            Action<dynamic> c3 = gd => Assert.Equal(new { Email = "test2@test.com", Period = new DateTime(2016, 6, 1), Value = 100000 }, gd);

            Assert.Collection(formatedResult, c1, c2, c3);
        }

        [Fact]
        public void GetActivities()
        {
            var cat1 = new ActivityCategory {Name = "1", Value = 1};
            var cat2 = new ActivityCategory {Name = "b", Value = 2};
            var cat3 = new ActivityCategory {Name = "III", Value = 3};

            // Not cronoligical to verify that the order is set based on the value
            var inDb = new List<Activity>
            {
                new Activity
                {
                    Category = cat2,
                    DueDate = new DateTime(2016,1,1)
                },
                new Activity
                {
                    Category = cat1,
                    DueDate = new DateTime(2016,1,1)
                },
                new Activity
                {
                    Category = cat3,
                    DueDate = new DateTime(2016,1,1)
                },
                new Activity
                {
                    Category = cat1,
                    DueDate = new DateTime(2016,1,1)
                },
                new Activity
                {
                    Category = cat2,
                    DueDate = new DateTime(2016,1,1)
                }
            };
            _context.Activities.AddRange(inDb);

            var result = _controller.Activities(new int[0], new string[0], new DateTime(2016, 1,1));
            var cast = Assert.IsType <OkNegotiatedContentResult<GraphEnvelope<IEnumerable<GraphData>>>>(result).Content;

            // To get propper desipherable fail message
            var data = cast.Data.Select(d => new {d.Order, Value = (int)d.Value, d.Label});

            Action<dynamic> d1 = gd => Assert.Equal(new { Order = 1, Value = 2, Label = "1"}, gd);
            Action<dynamic> d2 = gd => Assert.Equal(new { Order = 2, Value = 2, Label = "b" }, gd);
            Action<dynamic> d3 = gd => Assert.Equal(new { Order = 3, Value = 1, Label = "III" }, gd);

            Assert.Collection(data, d1, d2, d3);
        }

        [Fact]
        public void GetProduction()
        {
            var user1 = new User { Email = "test1@test.com", Active = true };
            var user2 = new User { Email = "test2@test.com", Active = true };

            var inDb = new List<Opportunity>
            {
                new Opportunity
                {
                    StartDate = new DateTime(2016,1,5),
                    EndDate = new DateTime(2016,1,10),
                    Owner = user1,
                    Amount = 100000,
                    Percentage = 100
                },
                new Opportunity
                {
                    StartDate = new DateTime(2016,1,9),
                    EndDate = new DateTime(2016,1,18),
                    Owner = user1,
                    Amount = 100000,
                    Percentage = 50
                },
                new Opportunity
                {
                    StartDate = new DateTime(2016,1,5),
                    EndDate = new DateTime(2016,1,18),
                    Owner = user2,
                    Amount = 100000,
                    Percentage = 50
                },
                new Opportunity
                {
                    StartDate = new DateTime(2016,1,1),
                    EndDate = new DateTime(2016,2,18),
                    Owner = user2,
                    Amount = 400000,
                    Percentage = 100
                }
            };
            _context.Opportunities.AddRange(inDb);

            var result = _controller.Production(null, null, null, null, null, new DateTime(2016, 1, 1), new DateTime(2017, 1, 1), true);
            var cast = Assert.IsType<OkNegotiatedContentResult<GraphEnvelope<IDictionary<string, IEnumerable<DateUserGraphData>>>>>(result).Content;

            var formatedResult = ToDynamic(cast.Data);

            Action<dynamic> c1 = gd => Assert.Equal(new { Email = "test1@test.com", Period = new DateTime(2016, 1, 1), Value = 150000 }, gd);
            Action<dynamic> c2 = gd => Assert.Equal(new { Email = "test2@test.com", Period = new DateTime(2016, 1, 1), Value = 250000 }, gd);
            Action<dynamic> c3 = gd => Assert.Equal(new { Email = "test2@test.com", Period = new DateTime(2016, 2, 1), Value = 200000 }, gd);

            Assert.Collection(formatedResult, c1, c2, c3);
        }

        private static IEnumerable<dynamic> ToDynamic(IDictionary<string, IEnumerable<DateUserGraphData>> data)
        {
            return data.SelectMany(kp => kp.Value.Select(v => new
            {
                v.User.Email,
                v.Period,
                Value = (int)v.Value
            }));
        } 
    }
}
