using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainModels.ViewSettings;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using Infrastructure.DataAccess.Repositories;
using NSubstitute;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Repositories
{
    public class ProductionViewSettingsRepositoryTest
    {
        private readonly IProductionViewSettingsRepository _repo;
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<ProductionViewSettings> _generic;

        public ProductionViewSettingsRepositoryTest()
        {
            _context = new AppContextStub();
            _generic = Substitute.For<IGenericRepository<ProductionViewSettings>>();

            _context.Users.Add(new User
            {
                Id = "1",
                UserName = "test"
            });

            _repo = new ProductionViewSettingsRepository(_context, _generic);
        }

        [Fact]
        public void GetAllGets()
        {
            _context.ProductionViewSettings.AddRange(new List<ProductionViewSettings>
            {
                new ProductionViewSettings(),
                new ProductionViewSettings(),
                new ProductionViewSettings()
            });

            _generic.Get().Returns(_context.ProductionViewSettings);

            Assert.Equal(3, _repo.GetAll().Count());
        }

        [Fact]
        public void GetSingleSuccess()
        {
            var data = new ProductionViewSettings
            {
                Id = 1,
                Name = "test"
            };
            _context.ProductionViewSettings.Add(data);

            var result = _repo.GetById(1);

            Assert.Equal(data, result);
        }

        [Fact]
        public void GetSingleNotFound()
        {
            Assert.Throws<NotFoundException>(() => _repo.GetById(1));
        }

        [Fact]
        public void CreateCreates()
        {
            var data = new ProductionViewSettings();
            _repo.Create(data, "test");
            _generic.ReceivedWithAnyArgs().Insert(data);
        }

        [Fact]
        public void UpdateUpdates()
        {
            var preData = new ProductionViewSettings
            {
                Id = 1,
                Name = "pre",
                Owner = _context.Users.SingleOrDefault(u => u.UserName == "test"),
                OwnerId = "1",
                UserGroups = new List<UserGroup>(),
                Categories = new List<OpportunityCategory>(),
                Users = new List<User>(),
                Departments = new List<Department>(),
                Stages = new List<Stage>(),
                Private = false,
                Weighted = false
            };
            _context.ProductionViewSettings.Add(preData);

            var postData = new ProductionViewSettings
            {
                Id = 1,
                Name = "post",
                Owner = _context.Users.SingleOrDefault(u => u.UserName == "test"),
                OwnerId = "1",
                UserGroups = new List<UserGroup>(),
                Categories = new List<OpportunityCategory>(),
                Users = new List<User>(),
                Departments = new List<Department>(),
                Stages = new List<Stage>(),
                Private = true,
                Weighted = true
            };

            _generic.Update(Arg.Any<Action<ProductionViewSettings>>(), true, 1)
                .ReturnsForAnyArgs(i =>
                {
                    i.Arg<Action<ProductionViewSettings>>().Invoke(preData);
                    return postData;
                });
            _repo.Update(1, postData, "test");

            
            Assert.Equal(
                new { postData.Name, postData.Private, postData.Weighted }, 
                new { preData.Name, preData.Private, preData.Weighted } );
        }

        [Fact]
        public void UpdateNotAllowed()
        {
            var preData = new ProductionViewSettings
            {
                Id = 1,
                Name = "pre",
                Owner = _context.Users.SingleOrDefault(u => u.UserName == "test"),
                OwnerId = "noone",
                UserGroups = new List<UserGroup>(),
                Categories = new List<OpportunityCategory>(),
                Users = new List<User>(),
                Departments = new List<Department>(),
                Stages = new List<Stage>(),
                Private = false,
                Weighted = false
            };
            _context.ProductionViewSettings.Add(preData);

            var postData = new ProductionViewSettings
            {
                Id = 1,
                Name = "post",
                Owner = _context.Users.SingleOrDefault(u => u.UserName == "test"),
                OwnerId = "1",
                UserGroups = new List<UserGroup>(),
                Categories = new List<OpportunityCategory>(),
                Users = new List<User>(),
                Departments = new List<Department>(),
                Stages = new List<Stage>(),
                Private = true,
                Weighted = true
            };

            _generic.Update(Arg.Any<Action<ProductionViewSettings>>(), true, 1)
                .ReturnsForAnyArgs(i =>
                {
                    i.Arg<Action<ProductionViewSettings>>().Invoke(preData);
                    return postData;
                });

            Assert.Throws<NotAllowedException>(() => _repo.Update(1, postData, "test"));
        }
    }
}
