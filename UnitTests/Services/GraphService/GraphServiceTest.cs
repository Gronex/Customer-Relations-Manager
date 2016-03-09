using System;
using System.Collections.Generic;
using System.Linq;
using Core.DomainModels.Users;
using Xunit;
using Core.ApplicationServices.Graph;
using Core.ApplicationServices.ServiceInterfaces;

namespace UnitTests.Services.GraphService
{
    public class GraphServiceTest
    {
        private readonly IGraphService _service;
        public GraphServiceTest()
        {
            _service = new Core.ApplicationServices.Graph.GraphService();
        }

        [Fact]
        public void LookaheadFilterTakingGoalBiforeStart()
        {

            var startDate = new DateTime(2016, 1, 1);
            var user = new User {Email = "user1"};
            var goals = new List<ProductionGoal>
            {
                new ProductionGoal {Goal = 1000, StartDate = new DateTime(2015, 1, 1), User = user},
                new ProductionGoal {Goal = 5000, StartDate = new DateTime(2016, 6, 1), User = user},
            };

            var result = _service.GenerateGoalDataTable(goals, startDate);

            Assert.Equal(1000, result["user1"].FirstOrDefault()?.Value);
        }

        [Fact]
        public void LookaheadFilterTakingOneGoalBiforeStart()
        {

            var startDate = new DateTime(2016, 1, 1);
            var user = new User { Email = "user1" };
            var goals = new List<ProductionGoal>
            {
                new ProductionGoal {Goal = 100, StartDate = new DateTime(2014, 1, 1), User = user},
                new ProductionGoal {Goal = 1000, StartDate = new DateTime(2015, 1, 1), User = user},
                new ProductionGoal {Goal = 5000, StartDate = new DateTime(2016, 6, 1), User = user},
            };

            var result = _service.GenerateGoalDataTable(goals, startDate);

            Assert.Equal(1000, result["user1"].FirstOrDefault()?.Value);
        }


        [Fact]
        public void LookaheadFilterNotTakingIfStartCovered()
        {
            var startDate = new DateTime(2016, 1, 1);
            var user = new User { Email = "user1" };
            var goals = new List<ProductionGoal>
            {
                new ProductionGoal {Goal = 100, StartDate = new DateTime(2014, 1, 1), User = user},
                new ProductionGoal {Goal = 1000, StartDate = new DateTime(2015, 1, 1), User = user},
                new ProductionGoal {Goal = 5000, StartDate = startDate, User = user},
            };

            var result = _service.GenerateGoalDataTable(goals, startDate);

            Assert.Equal(5000, result["user1"].FirstOrDefault()?.Value);
        }


        [Fact]
        public void ResultsAreOrdered()
        {
            var startDate = new DateTime(2013, 1, 1);
            var user = new User { Email = "user1" };
            var goals = new List<ProductionGoal>
            {
                new ProductionGoal {Goal = 1000, StartDate = new DateTime(2015, 1, 1), User = user},
                new ProductionGoal {Goal = 5000, StartDate = new DateTime(2016,1,1), User = user},
                new ProductionGoal {Goal = 100, StartDate = new DateTime(2014, 1, 1), User = user},
            };

            var result = _service.GenerateGoalDataTable(goals, startDate);

            Assert.Equal(new List<DateTime> {new DateTime(2014, 1, 1), new DateTime(2015,1,1), new DateTime(2016,1,1)}, result["user1"].Select(r => r.Period));
        }
    }
}
