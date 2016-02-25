using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.Graph;
using Core.DomainModels.Graph;
using Core.DomainModels.Users;
using Core.DomainServices.Services;
using Xunit;

namespace UnitTests.Services
{
    public class GraphServiceTest
    {
        private readonly GraphService _service;
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


            _service = new GraphService();
        }

        [Fact]
        public void ProductionGraphDoesNotAddExtras()
        {
            var data = _service.GenerateGoalDataSets(new List<User>());
            
            Assert.Equal(0, data.Count);
        }

        [Fact]
        public void ProductionGraphSplitsPerMonth()
        {
            var data = _service.GenerateGoalDataSets(_users);
            
            Assert.Collection(data, 
                pair => Assert.Equal(2, pair.Value.DataPoints.Count()),
                pair => Assert.Equal(3, pair.Value.DataPoints.Count()));
        }

        [Fact]
        public void ProductionGraphUsesUserIdAsKey()
        {
            var data = _service.GenerateGoalDataSets(_users);

            Assert.Collection(data,
                pair => Assert.Equal("1", pair.Key),
                pair => Assert.Equal("2", pair.Key));
        }

        [Fact]
        public void ProductionGraphUsesUserNameAsLabel()
        {
            var data = _service.GenerateGoalDataSets(_users);

            Assert.Collection(data,
                pair => Assert.Equal("user1 lastname1", pair.Value.Label),
                pair => Assert.Equal("user2 lastname2", pair.Value.Label));
        }

        [Fact]
        public void MissingMonthsAdded()
        {
            var data = new List<ProductionGoal>
            {
                new ProductionGoal{Goal = 1, StartDate = DateTime.UtcNow.Date},
                new ProductionGoal{Goal = 1, StartDate = DateTime.UtcNow.AddMonths(2).Date}
            };

            var buffedData = GraphService.BuffOutGoals(data).ToList();

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

            var buffedData = GraphService.BuffOutGoals(data).ToList();

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

            var buffedData = GraphService.BuffOutGoals(data).ToList();

            Assert.Collection(buffedData,
                point => Assert.Equal(point.Goal, 1),
                point => Assert.Equal(point.Goal, 1),
                point => Assert.Equal(point.Goal, 2));
        }
    }
}
