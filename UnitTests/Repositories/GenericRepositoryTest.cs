using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Exceptions;
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
            new UserGroup {Id = 3, Name = "Test Group3"},
            new UserGroup {Id = 2, Name = "Test Group2"},
            new UserGroup {Id = 5, Name = "Test Group5"},
            new UserGroup {Id = 1, Name = "Test Group1"},
            new UserGroup {Id = 4, Name = "Test Group4"},
        };

        public GenericRepositoryTest()
        {
            _context = new AppContextStub();

            //Add some data to something
            _context.UserGroups.AddRange(_seedData);

            _repo = new GenericRepository<UserGroup>(_context);
        }

        [Fact]
        public void OrderByString()
        {
            _context.UserGroups.Add(new UserGroup()
            {
                Id = 6,
                Name = "1ab"
            });
            var data = _repo.GetOrderedByStrings(orderBy: new List<string> {"name"});
            Assert.Equal("1ab", data.First().Name);
        }

        [Fact]
        public void OrderBySelector()
        {
            _context.UserGroups.Add(new UserGroup()
            {
                Id = 6,
                Name = "1ab"
            });
            var data = _repo.Get(orderBy: ug => ug.OrderBy(u => u.Name));
            Assert.Equal("1ab", data.First().Name);
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
        public void GetAllReturnsPagedWithTotalCount()
        {
            var result = _repo.GetPaged(orderBy: ug => ug.OrderBy(u => u.Id), page: 1, pageSize: 1);

            Assert.Equal(_seedData.Count(), result.ItemCount);
        }

        [Fact]
        public void GetAllReturnsPagedWithProperPageInfo()
        {
            var result = _repo.GetPaged(orderBy: ug => ug.OrderBy(u => u.Id), page: 3, pageSize: 2);

            Assert.Equal(new {PageNumber = 3, PageSize = 2}, new {result.PageNumber, result.PageSize});
        }

        [Fact]
        public void GetAllReturnsPaged1()
        {
            var result = _repo.GetPaged(orderBy: ug => ug.OrderBy(u => u.Id) ,page: 1, pageSize: 1);

            Assert.Equal("Test Group1", result.Data.Single().Name);
        }

        [Fact]
        public void GetAllReturnsPaged2()
        {
            // only 1 element on second page page
            var result = _repo.GetPaged(orderBy: ug => ug.OrderBy(u => u.Id), page: 2, pageSize: 4);
            
            Assert.Equal("Test Group5", result.Data.Single().Name);
        }

        [Fact]
        public void UpdateSuccess()
        {
            var data = new UserGroup { Name = "Test data" };
            _context.UserGroups.Add(data);
            
            _repo.Update(ug => ug.Name = "New test data", 5);

            var newData = _context.UserGroups.Find(5);

            Assert.Equal("New test data", newData.Name);
        }

        [Fact]
        public void UpdateFail()
        {
            Assert.Throws(typeof (NotFoundException),() => _repo.Update(ug => ug.Name = "New test data", -1));
        }

        [Fact]
        public void UpdateFailWithoutThrowing()
        {
            var result = _repo.Update(ug => ug.Name = "New test data", false, -1);
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
        public void DeleteBySuccess()
        {
            var data = new UserGroup { Name = "Test data" };
            var dbData = _context.UserGroups.Add(data);

            // Verify the data was added
            Assert.NotNull(_context.UserGroups.SingleOrDefault(ug => ug.Name == "Test data"));

            _repo.DeleteBy(ug => ug.Name == "Test data");
            // Check if it was deleted
            Assert.Null(_context.UserGroups.SingleOrDefault(ug => ug.Name == "Test data"));
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

        [Fact]
        public void UpdateByUpdates()
        {
            var data = _repo.UpdateBy(ug =>
            {
                ug.Name = "updated name";
            },ug => ug.Name == "Test Group1");
            Assert.Equal("updated name", data.Name);
        }

        [Fact]
        public void TotalCountCounts()
        {
            var count = _repo.Count();
            Assert.Equal(5, count);
        }

        [Fact]
        public void TotalCountCountsWithFilter()
        {
            var count = _repo.Count(ug => ug.Name.Contains("1"));
            Assert.Equal(1, count);
        }
    }
}
