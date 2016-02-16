using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using customer_relations_manager;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Xunit;

namespace UnitTests.Repositories
{
    public class GoalRepositoryTest
    {
        private readonly IGoalRepository _repo;
        private readonly ApplicationContext _context;
        private readonly ProductionGoal _data;
        public GoalRepositoryTest()
        {
            _data = new ProductionGoal {Goal = 10000000, Month = 5, Year = 3000, UserId = "0"};
            var dbConnection = Effort.DbConnectionFactory.CreateTransient();
            _context = new ApplicationContext(dbConnection);

            //Add some data
            var um = new ApplicationUserManager(new UserStore<User>(_context));
            var t1 = um.Create(new User {Id = "0", FirstName = "Test", LastName = "User", UserName = "Test1"});
            um.Create(new User {Id = "1", FirstName = "Test", LastName = "User", UserName = "Test2" });
            um.Create(new User {Id = "2", FirstName = "Test", LastName = "User", UserName = "Test3" });
            _context.SaveChanges();
            
            _context.Goals.AddRange(new List<ProductionGoal>
            {
                new ProductionGoal {Id = 1, Month = 1, Year = 2016, Goal = 100, UserId = "0"},
                new ProductionGoal {Id = 2, Month = 1, Year = 2016, Goal = 200, UserId = "0"},
                new ProductionGoal {Id = 3, Month = 1, Year = 2016, Goal = 300, UserId = "1"},
                new ProductionGoal {Id = 4, Month = 1, Year = 2016, Goal = 100000, UserId = "2"},
            });
            _context.SaveChanges();
            _repo = new GoalRepository(_context, new GenericRepository<ProductionGoal>(_context));
        }

        [Fact]
        public void CreateAdds()
        {
            var result = _repo.Create("0", _data);
            _context.SaveChanges();

            Assert.Contains(result, _context.Goals);
        }

        [Fact]
        public void GetAllReturnsFullList()
        {
            var result = _repo.GetAll("0");

            Assert.Equal(_context.Goals.Where(g => g.UserId == "0"), result);
        }

        [Theory]
        [InlineData("0", 1, 100)]
        [InlineData("0", 2, 200)]
        [InlineData("1", 3, 300)]
        [InlineData("2", 4, 100000)]
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
            // To verify the test actualy tests anything
            // by making sure that the data in the exisitng object does not
            // have the updated value
            Assert.False(_context.Goals.Any(g => g.UserId == "0" && g.Id == 1 && g.Year == _data.Year));

            var result = _repo.Update("0", 1, _data);
            _context.SaveChanges();

            Assert.Equal(result.Year, _context.Goals.Single(g => g.UserId == "0" && g.Id == 1).Year);
        }

        [Theory]
        [InlineData("0", 1)]
        [InlineData("0", 99)]
        [InlineData("-1", 1)]
        [InlineData("-1", 99)]
        public void DeleteSucceedsNoMatterWhat(string userId, int id)
        {
            var data = _context.Goals.SingleOrDefault(g => g.Id == id && userId == g.UserId);

            _repo.Delete(userId, id);
            _context.SaveChanges();

            Assert.DoesNotContain(data, _context.Goals);
        }
    }
}
