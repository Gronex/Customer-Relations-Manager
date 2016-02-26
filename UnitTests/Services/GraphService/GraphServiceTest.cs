using System;
using System.Collections.Generic;
using System.Linq;
using Core.DomainModels.Users;
using Xunit;

namespace UnitTests.Services.GraphService
{
    public class GraphServiceTest
    {
        private readonly Core.ApplicationServices.Graph.GraphService _service;
        private readonly IEnumerable<User> _users;
        public GraphServiceTest()
        {
            _users = new List<User>()
            {
                new User {Id = "1", FirstName = "user1", LastName = "lastname1", Goals = new List<ProductionGoal>
                {
                    new ProductionGoal { Goal = 1, StartDate = new DateTime(1, 2, 1)},
                    new ProductionGoal { Goal = 2, StartDate = new DateTime(1, 2, 1)}
                }},
                new User {Id = "2", FirstName = "user2", LastName = "lastname2", Goals = new List<ProductionGoal>
                {
                    new ProductionGoal { Goal = 1, StartDate = new DateTime(1, 1, 1)},
                    new ProductionGoal { Goal = 2, StartDate = new DateTime(1, 3, 1)}
                }},
            };


            _service = new Core.ApplicationServices.Graph.GraphService();
        }

        [Fact]
        public void MissingMonthsAdded()
        {
            var data = new List<ProductionGoal>
            {
                new ProductionGoal{Goal = 1, StartDate = DateTime.UtcNow.Date},
                new ProductionGoal{Goal = 1, StartDate = DateTime.UtcNow.AddMonths(2).Date}
            };

            var buffedData = Core.ApplicationServices.Graph.GraphService.BuffOutGoals(data).ToList();

            Assert.Equal(3, buffedData.Count);
        }

        [Fact]
        public void CorrectOrderOnBuffData()
        {
            var today = DateTime.UtcNow.Date;
            var data = new List<ProductionGoal>
            {
                new ProductionGoal{Goal = 1, StartDate = DateTime.UtcNow.Date},
                new ProductionGoal{Goal = 1, StartDate = DateTime.UtcNow.AddMonths(2).Date}
            };

            var buffedData = Core.ApplicationServices.Graph.GraphService.BuffOutGoals(data).ToList();

            Assert.Collection(buffedData, 
                point => Assert.Equal(point.StartDate, today), 
                point => Assert.Equal(point.StartDate, today.AddMonths(1)),
                point => Assert.Equal(point.StartDate, today.AddMonths(2)));
        }

        [Fact]
        public void CorrectValueOnBuffData()
        {
            var data = new List<ProductionGoal>
            {
                new ProductionGoal{Goal = 1, StartDate = DateTime.UtcNow.Date},
                new ProductionGoal{Goal = 2, StartDate = DateTime.UtcNow.AddMonths(2).Date}
            };

            var buffedData = Core.ApplicationServices.Graph.GraphService.BuffOutGoals(data).ToList();

            Assert.Collection(buffedData,
                point => Assert.Equal(point.Goal, 1),
                point => Assert.Equal(point.Goal, 1),
                point => Assert.Equal(point.Goal, 2));
        }
    }
}
