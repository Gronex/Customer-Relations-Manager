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
                    new ProductionGoal { Goal = 1, Month = 1, Year = 1},
                    new ProductionGoal { Goal = 2, Month = 2, Year = 1},
                }},
                new User {Id = "2", FirstName = "user2", LastName = "lastname2", Goals = new List<ProductionGoal>
                {
                    new ProductionGoal { Goal = 1, Month = 1, Year = 1},
                    new ProductionGoal { Goal = 2, Month = 3, Year = 1},
                }},
            };


            _service = new GraphService();
        }

        [Fact]
        public void ProductionGraphDoesNotAddExtras()
        {
            var data = _service.GenerateProductionDataSets(new List<User>());
            
            Assert.Equal(0, data.Count);
        }

        [Fact]
        public void ProductionGraphSplitsPerMonth()
        {
            var data = _service.GenerateProductionDataSets(_users);
            
            Assert.Collection(data, 
                pair => Assert.Equal(2, pair.Value.DataPoints.Count()),
                pair => Assert.Equal(3, pair.Value.DataPoints.Count()));
        }

        [Fact]
        public void ProductionGraphUsesUserIdAsKey()
        {
            var data = _service.GenerateProductionDataSets(_users);

            Assert.Collection(data,
                pair => Assert.Equal("1", pair.Key),
                pair => Assert.Equal("2", pair.Key));
        }

        [Fact]
        public void ProductionGraphUsesUserNameAsLabel()
        {
            var data = _service.GenerateProductionDataSets(_users);

            Assert.Collection(data,
                pair => Assert.Equal("user1 lastname1", pair.Value.Label),
                pair => Assert.Equal("user2 lastname2", pair.Value.Label));
        }

        [Fact]
        public void MissingMonthsAdded()
        {
            var data = new List<DateDataPoint>
            {
                new DateDataPoint{Value = 1, Date = DateTime.UtcNow.Date},
                new DateDataPoint{Value = 1, Date = DateTime.UtcNow.AddMonths(2).Date}
            };

            var expectedData = new List<DateDataPoint>
            {
                new DateDataPoint{Value = 1, Date = DateTime.UtcNow.Date},
                new DateDataPoint{Value = 1, Date = DateTime.UtcNow.AddMonths(1).Date},
                new DateDataPoint{Value = 1, Date = DateTime.UtcNow.AddMonths(2).Date}
            };

            var buffedData = GraphService.BuffOut(data).ToList();

            Assert.Equal(expectedData.Count, buffedData.Count);
        }

        [Fact]
        public void CorrectOrderOnBuffData()
        {
            var today = DateTime.UtcNow.Date;
            var data = new List<DateDataPoint>
            {
                new DateDataPoint{Value = 1, Date = today},
                new DateDataPoint{Value = 1, Date = today.AddMonths(2).Date}
            };

            var buffedData = GraphService.BuffOut(data).ToList();

            Assert.Collection(buffedData, 
                point => Assert.Equal(point.Date, today), 
                point => Assert.Equal(point.Date, today.AddMonths(1)),
                point => Assert.Equal(point.Date, today.AddMonths(2)));
        }
    }
}
