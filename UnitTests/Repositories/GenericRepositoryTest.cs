using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Repositories
{
    public class GenericRepositoryTest
    {
        
        private readonly IApplicationContext _context;
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
            _context = new AppContextStub();

            //Add some data to something
            _context.UserGroups.AddRange(_seedData);

            _repo = new GenericRepository<UserGroup>(_context);
        }

        [Fact]
        public void GetSuccess()
        {
            var data = new UserGroup {Name = "Test data"};
            var dbData = _context.UserGroups.Add(data);

            var repoDbData = _repo.GetByKey(5); // 5th inserted element
            Assert.Equal(dbData, repoDbData);
        }

        [Fact]
        public void GetFail()
        {
            var result = _repo.GetByKey(-1);
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
            
            _repo.Update(ug => ug.Name = "New test data", 5);

            var newData = _context.UserGroups.Find(5);

            Assert.Equal("New test data", newData.Name);
        }

        [Fact]
        public void UpdateFail()
        {
            
            var result = _repo.Update(ug => ug.Name = "New test data", -1);
            Assert.Null(result);
        }

        [Fact]
        public void DeleteSuccess()
        {
            var data = new UserGroup { Name = "Test data" };
            var dbData = _context.UserGroups.Add(data);

            // Verify the data was added
            Assert.NotNull(_context.UserGroups.Find(5));

            _repo.DeleteByKey(dbData.Id);
            // Check if it was deleted
            Assert.Null(_context.UserGroups.Find(5));
        }

        [Fact]
        public void DeleteSurviveFail()
        {
            // count because of the mocked datastores way of handeling id's
            var id = _context.UserGroups.Count();
            // Verify db state
            Assert.Null(_context.UserGroups.Find(id));

            _repo.DeleteByKey(0);
            // Check if it was deleted
            Assert.Null(_context.UserGroups.Find(id));
        }

        [Fact]
        public void InsertSuccess()
        {
            var data = new UserGroup
            {
                Name = "Test data!!!"
            };
            var dbData = _repo.Insert(data);
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
