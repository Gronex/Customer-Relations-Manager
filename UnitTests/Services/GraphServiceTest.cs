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
        private readonly IGraphService _service;
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

            var expected = new Dictionary<string, DataSet>
            {
                {
                    "1", new DataSet
                    {
                        Label = "user1 lastname1",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint {Label = DateTime.Parse("0001-01-01"), Value = 1},
                            new DataPoint {Label = DateTime.Parse("0001-02-01"), Value = 2}
                        }
                    }
                },
                {
                    "2", new DataSet
                    {
                        Label = "user2 lastname2",
                        DataPoints = new List<DataPoint>
                        {
                            new DataPoint {Label = DateTime.Parse("0001-01-01"), Value = 1},
                            new DataPoint {Label = DateTime.Parse("0001-02-01"), Value = 1},
                            new DataPoint {Label = DateTime.Parse("0002-03-01"), Value = 2}
                        }
                    }
                }
            };

            Assert.Equal(expected, data);
        }
    }
}
