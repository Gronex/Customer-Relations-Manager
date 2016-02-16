using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;
using Xunit;

namespace UnitTests.Repositories
{
    public class GenericRepositoryTest
    {
        
        private readonly ApplicationContext _context;
        private readonly IGenericRepository<UserGroup> _repo;

        private readonly IEnumerable<UserGroup> _seedData = new List<UserGroup>
        {
            new UserGroup {Name = "Test Group1"},
            new UserGroup {Name = "Test Group2"},
            new UserGroup {Name = "Test Group3"},
            new UserGroup {Name = "Test Group4"},
            new UserGroup {Name = "Test Group5"}
        };

        public GenericRepositoryTest()
        {
            var dbConnection = Effort.DbConnectionFactory.CreateTransient();
            _context = new ApplicationContext(dbConnection);

            //Add some data to something
            _context.UserGroups.AddRange(_seedData);
            _context.SaveChanges();

            _repo = new GenericRepository<UserGroup>(_context);
        }

        [Fact]
        public void GetSuccess()
        {
            var data = new UserGroup {Name = "Test data"};
            var dbData = _context.UserGroups.Add(data);

            var repoDbData = _repo.GetByKey(dbData.Id);
            Assert.Equal(dbData, repoDbData);
        }

        [Fact]
        public void GetFail()
        {
            var result = _repo.GetByKey(0);
            Assert.Null(result);
        }

        [Fact]
        public void GetAllReturnsAll()
        {
            var result = _repo.Get();

            Assert.Equal(_seedData, result);
        }

        [Fact]
        public void GetAllReturnsPaged1()
        {
            var result = _repo.Get(orderBy: ug => ug.OrderBy(u => u.Id) ,page: 1, pageSize: 1);

            Assert.Equal(_seedData.Take(1), result);
        }

        [Fact]
        public void GetAllReturnsPaged2()
        {
            // only 1 element on second page page
            var result = _repo.Get(orderBy: ug => ug.OrderBy(u => u.Id), page: 2, pageSize: 4);
            
            Assert.Equal(_seedData.Last(), result.Single());
        }

        [Fact]
        public void UpdateSuccess()
        {
            var data = new UserGroup { Name = "Test data" };
            var dbData = _context.UserGroups.Add(data);
            _context.SaveChanges();
            
            _repo.Update(ug => ug.Name = "New test data", dbData.Id);
            _context.SaveChanges();

            var newData = _context.UserGroups.Find(dbData.Id);

            Assert.Equal("New test data", newData.Name);
        }

        [Fact]
        public void UpdateFail()
        {
            
            var result = _repo.Update(ug => ug.Name = "New test data", 0);
            _context.SaveChanges();
            Assert.Null(result);
        }

        [Fact]
        public void DeleteSuccess()
        {
            var data = new UserGroup { Name = "Test data" };
            var dbData = _context.UserGroups.Add(data);
            _context.SaveChanges();

            // Verify the data was added
            Assert.NotNull(_context.UserGroups.Find(dbData.Id));

            _repo.DeleteByKey(dbData.Id);
            _context.SaveChanges();
            // Check if it was deleted
            Assert.Null(_context.UserGroups.Find(dbData.Id));
        }

        [Fact]
        public void DeleteSurviveFail()
        {
            // Verify db state
            Assert.Null(_context.UserGroups.Find(0));

            _repo.DeleteByKey(0);
            _context.SaveChanges();
            // Check if it was deleted
            Assert.Null(_context.UserGroups.Find(0));
        }

        [Fact]
        public void InsertSuccess()
        {
            var data = new UserGroup
            {
                Name = "Test data!!!"
            };
            var dbData = _repo.Insert(data);
            _context.SaveChanges();
            // Check if it was deleted
            Assert.NotNull(_context.UserGroups.Find(dbData.Id));
        }

        [Fact]
        public void CreateCreatesEmptyObject()
        {
            var data = _repo.Create();
            Assert.NotNull(data);
        }
    }
}
