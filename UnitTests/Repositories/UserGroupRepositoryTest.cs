using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess;
using NSubstitute;
using Xunit;

namespace UnitTests.Repositories
{
    public class UserGroupRepositoryTest
    {
        private IUserGroupRepository _repo;
        private ApplicationContext _context;
        public UserGroupRepositoryTest()
        {
            var dbConnection = Effort.DbConnectionFactory.CreateTransient();
            _context = new ApplicationContext(dbConnection);

            //Add some data
            _context.UserGroups.AddRange(new List<UserGroup>
            {
                new UserGroup { Name = "Test Group1" },
                new UserGroup { Name = "Test Group2" },
                new UserGroup { Name = "Test Group3" },
                new UserGroup { Name = "Test Group4" },
                new UserGroup { Name = "Test Group5" }
            });
            _context.SaveChanges();
            _repo = new UserGroupRepository(_context);
        }

        [Fact]
        public void CreateAdds()
        {
            var data = new UserGroup {Name = "Added data"};
            var result = _repo.Create(data);
            _context.SaveChanges();

            Assert.Contains(result, _context.UserGroups);
        }

        [Fact]
        public void GetAllReturnsFullTrustAssembliesSectionList()
        {
            var result = _repo.GetAll();
            
            Assert.Equal(_context.UserGroups, result);
        }

        [Theory]
        [InlineData(1, "Test Group1")]
        [InlineData(2, "Test Group2")]
        [InlineData(3, "Test Group3")]
        [InlineData(4, "Test Group4")]
        [InlineData(5, "Test Group5")]
        public void GetByIdGetsData(int id, string name)
        {
            var result = _repo.GetById(id);

            Assert.Equal(name, result.Name);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        public void GetByIdReturnsNullOnNotFound(int id)
        {
            var result = _repo.GetById(id);

            Assert.Equal(result, null);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        public void UpdateReturnsNullOnNotFound(int id)
        {
            var data = new UserGroup {Name = "Updated data"};
            var result = _repo.Update(id, data);

            Assert.Equal(result, null);
        }

        [Fact]
        public void UpdateReturnsUpdatesData()
        {
            var data = new UserGroup { Name = "Updated data" };

            // To verify the test actualy tests anything
            // by making sure that the data in the exisitng object does not
            // have the updated value
            Assert.False(_context.UserGroups.Any(ug => ug.Id == 1 && ug.Name == data.Name));

            var result = _repo.Update(1, data);
            _context.SaveChanges();

            Assert.Equal(result.Name, _context.UserGroups.Single(ug => ug.Id == 1).Name);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(99)]
        public void UpdateSucceedsNoMatterWhat(int id)
        {
            var data = _context.UserGroups.SingleOrDefault(ug => ug.Id == id);
          
            _repo.Delete(id);
            _context.SaveChanges();
            
            Assert.DoesNotContain(data, _context.UserGroups);
        }
    }
}
