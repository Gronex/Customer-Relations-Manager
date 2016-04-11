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

        public GraphTests()
        {
            var context = new AppContextStub();
            var goalRepo = new GenericRepository<ProductionGoal>(context);
            var opportunityRepo = new GenericRepository<Opportunity>(context);
            var activityRepo = new GenericRepository<Activity>(context);
            var service = new GraphService();

            _controller = new GraphController(goalRepo, opportunityRepo, activityRepo, service);

            var user1 = new User {Email = "test1@test.com"};
            var user2 = new User {Email = "test2@test.com"};

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

            context.Goals.AddRange(data);

        }

        [Fact]
        public void GetGoals()
        {
            // If the as fails the result is wrong
            var result = _controller.Goal(null, null, new DateTime(2016,1,1), new DateTime(2017,1,1)) 
                as OkNegotiatedContentResult<GraphEnvelope<IDictionary<string, IEnumerable<DateUserGraphData>>>>;

            var expected = new List<dynamic>
            {
                new {
                    Email = "test1@test.com",
                    Period = new DateTime(2016, 1, 1),
                    Value = 100000
                },
                new {
                    Email = "test1@test.com",
                    Period = new DateTime(2016, 4, 1),
                    Value = 200000
                },
                new {
                    Email = "test2@test.com",
                    Period = new DateTime(2016, 6, 1),
                    Value = 100000
                },
            };
            var formatedResult = ToDynamic(result.Content.Data).ToList();

            var comparison = expected
                .Zip(formatedResult, (ex, r) => ex.Email == r.Email && ex.Period == r.Period && ex.Value == r.Value)
                .All(x => x);

            Assert.True(comparison);
        }

        private static IEnumerable<dynamic> ToDynamic(IDictionary<string, IEnumerable<DateUserGraphData>> data)
        {
            return data.SelectMany(kp => kp.Value.Select(v => new
            {
                v.User.Email,
                v.Period,
                v.Value
            }));
        } 
    }
}
