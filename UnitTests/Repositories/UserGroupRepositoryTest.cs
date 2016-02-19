using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;
using NSubstitute;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Repositories
{
    public class UserGroupRepositoryTest
    {
        private readonly IUserGroupRepository _repo;
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<UserGroup> _generic; 
        public UserGroupRepositoryTest()
        {
            _context = new AppContextStub();
            _generic = Substitute.For<IGenericRepository<UserGroup>>();

            _generic.GetByKey(Arg.Any<object[]>()).Returns(a =>
            {
                var gId = (int)a.Arg<object[]>()[0];
                try
                {
                    return _context.UserGroups.ToList()[gId];
                }
                catch
                {
                    return null;
                }
            });

            //Add some data
            _context.UserGroups.AddRange(new List<UserGroup>
            {
                new UserGroup { Name = "Test Group1" },
                new UserGroup { Name = "Test Group2" },
                new UserGroup { Name = "Test Group3" },
                new UserGroup { Name = "Test Group4" },
                new UserGroup { Name = "Test Group5" }
            });
            _repo = new UserGroupRepository(_context, _generic);
        }

        [Fact]
        public void CreateAdds()
        {
            var data = new UserGroup {Name = "Added data"};
            var result = _repo.Create(data);
            
            Assert.Contains(result, _context.UserGroups);
        }

        [Fact]
        public void GetAllReturnsFullList()
        {
            var result = _repo.GetAll();
            
            Assert.Equal(_context.UserGroups, result);
        }

        [Theory]
        [InlineData(0, "Test Group1")]
        [InlineData(1, "Test Group2")]
        [InlineData(2, "Test Group3")]
        [InlineData(3, "Test Group4")]
        [InlineData(4, "Test Group5")]
        public void GetByIdGetsData(int id, string name)
        {
            var result = _repo.GetById(id);

            Assert.Equal(name, result.Name);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(99)]
        public void GetByIdReturnsNullOnNotFound(int id)
        {
            var result = _repo.GetById(id);

            Assert.Equal(result, null);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(99)]
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


            _generic.Update(Arg.Any<Action<UserGroup>>(), 1).Returns(ci =>
            {
                var action = ci.Arg<Action<UserGroup>>();
                var oldData = new UserGroup { Name = "Pre update"};
                action(oldData);
                return oldData;
            });

            var result = _repo.Update(1, data);

            Assert.Equal(data.Name, result.Name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        public void DeleteSucceedsNoMatterWhat(int id)
        {
            _repo.Delete(id);
            _generic.Received().DeleteByKey(id);
        }
    }
}
