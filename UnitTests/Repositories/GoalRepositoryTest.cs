using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using customer_relations_manager;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NSubstitute;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Repositories
{
    public class GoalRepositoryTest
    {
        private readonly IGoalRepository _repo;
        private readonly IApplicationContext _context;
        private readonly ProductionGoal _data;
        private readonly IGenericRepository<ProductionGoal> _generic;
        
        public GoalRepositoryTest()
        {
            _context = new AppContextStub();
            _generic = Substitute.For<IGenericRepository<ProductionGoal>>();

            _generic.GetByKey(Arg.Any<object[]>()).Returns(a =>
            {
                var gId = (int)a.Arg<object[]>()[0];
                var uId = (string)a.Arg<object[]>()[1];
                try
                {
                    return _context.Goals.SingleOrDefault(g => g.Id == gId && g.UserId == uId);
                }
                catch
                {
                    return null;
                }
            });

            _data = new ProductionGoal {Goal = 10000000, Month = 5, Year = 3000, UserId = "0"};

            //Add some data
            _context.Goals.AddRange(new List<ProductionGoal>
            {
                new ProductionGoal {Id = 0, Month = 1, Year = 2016, Goal = 100, UserId = "0"},
                new ProductionGoal {Id = 1, Month = 1, Year = 2016, Goal = 200, UserId = "0"},
                new ProductionGoal {Id = 2, Month = 1, Year = 2016, Goal = 300, UserId = "1"},
                new ProductionGoal {Id = 3, Month = 1, Year = 2016, Goal = 100000, UserId = "2"},
            });

            _context.Users.Add(new User {Id = "0"});
            _context.Users.Add(new User {Id = "1"});
            _context.Users.Add(new User {Id = "2"});

            _repo = new GoalRepository(_context, _generic);
        }

        [Fact]
        public void CreateAdds()
        {
            var result = _repo.Create("0", _data);

            Assert.Contains(result, _context.Goals);
        }

        [Fact]
        public void GetAllReturnsFullList()
        {
            _generic.Get(filter: Arg.Any<Expression<Func<ProductionGoal, bool>>>())
                .Returns(a => _context.Goals.Where(g => g.UserId == "0"));

            var result = _repo.GetAll("0");

            Assert.Equal(_context.Goals.Where(g => g.UserId == "0"), result);
        }

        [Theory]
        [InlineData("0", 0, 100)]
        [InlineData("0", 1, 200)]
        [InlineData("1", 2, 300)]
        [InlineData("2", 3, 100000)]
        public void GetByIdGetsData(string userId, int id, int goal)
        {
            var result = _repo.GetById(userId, id);

            Assert.Equal(goal, result.Goal);
        }

        [Theory]
        [InlineData("0", -1)]
        [InlineData("0", 6)]
        [InlineData("-1", 1)]
        [InlineData("0", 3)]
        public void GetByIdReturnsNullOnNotFound(string userId, int id)
        {
            var result = _repo.GetById(userId, id);

            Assert.Equal(result, null);
        }

        [Theory]
        [InlineData("0", -1)]
        [InlineData("0", 6)]
        [InlineData("-1", 1)]
        [InlineData("0", 3)]
        public void UpdateReturnsNullOnNotFound(string userId, int id)
        {
            var result = _repo.Update(userId, id, _data);

            Assert.Equal(result, null);
        }

        [Fact]
        public void UpdateReturnsUpdatesData()
        {
            // Actual test is of the action updates what is expected
            _generic
                .UpdateBy(Arg.Any<Action<ProductionGoal>>(), Arg.Any<Expression<Func<ProductionGoal, bool>>>())
                .Returns(ci =>
                {
                    var action = ci.Arg<Action<ProductionGoal>>();
                    var oldData = new ProductionGoal {Goal = -1, Month = -1, Year = -1};
                    action(oldData);
                    return oldData;
                });

            var result = _repo.Update("0", 1, _data);
            
            Assert.Equal(new { _data.Goal, _data.Month, _data.Year }, new { result.Goal, result.Month, result.Year });
        }

        [Theory]
        [InlineData("0", 1)]
        [InlineData("0", 99)]
        [InlineData("-1", 1)]
        [InlineData("-1", 99)]
        public void DeleteSucceedsNoMatterWhat(string userId, int id)
        {
            _repo.Delete(userId, id);
            _generic.Received().DeleteBy(Arg.Any<Expression<Func<ProductionGoal, bool>>>());
        }
    }
}
