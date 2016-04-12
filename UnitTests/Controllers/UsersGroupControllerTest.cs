using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using AutoMapper;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels;
using Core.DomainModels.UserGroups;
using Core.DomainModels.ViewSettings;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using NSubstitute;
using NSubstitute.Core.Arguments;
using Xunit;

namespace UnitTests.Controllers
{
    public class UsersGroupControllerTest
    {
        private readonly UserGroupsController _controller;
        private readonly IGenericRepository<UserGroup> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UsersGroupControllerTest()
        {
            _mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IGenericRepository<UserGroup>>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new UserGroupsController(_repo, _uow, _mapper);
        }

        [Fact]
        public void GetAllReturnsAList()
        {
            var data = new List<UserGroup>
            {
                new UserGroup() {Id = 1, Name = "Grp1"},
                new UserGroup() {Id = 2, Name = "Grp2"},
                new UserGroup() {Id = 3, Name = "Grp3"},
                new UserGroup() {Id = 4, Name = "Grp4"}
            };
            _repo.Get().Returns(x => data);

            var result = _controller.Get();
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new UserGroup() { Id = 1, Name = "Grp1" };
            _repo.GetByKey(Arg.Any<int>()).Returns(x => data);

            var result = _controller.Get(1) as OkNegotiatedContentResult<GroupViewModel>;
            var dataViewModel = _mapper.Map<GroupViewModel>(data);
            // Only testing one, since there is no reason the 
            // system should have chosen the same string and object comparison
            // compares on reference
            Assert.Equal(dataViewModel.Id, result?.Content.Id);
        }

        [Fact]
        public void GetNotFount()
        {
            _repo.GetByKey(Arg.Any<int>()).Returns(x => null);

            var result = _controller.Get(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateCallsInsert()
        {
            var data = new UserGroup{Id = 1, Name = "Grp1"};
            var dataViewModel = new GroupViewModel { Id = 1, Name = "Grp1" };

            _repo.Insert(Arg.Any<UserGroup>()).Returns(data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Insert(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new UserGroup { Id = 1, Name = "Grp1" };
            var dataViewModel = new GroupViewModel { Id = 1, Name = "Grp1" };

            _repo.Insert(Arg.Any<UserGroup>()).Returns(data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateCallsUpdateInRepo()
        {
            var data = new UserGroup { Id = 1, Name = "Grp1" };
            var dataViewModel = new GroupViewModel { Id = 1, Name = "Grp1" };

            _repo.Update(Arg.Any<Action<UserGroup>>(), 1).Returns(data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(Arg.Any<Action<UserGroup>>(), 1);
        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new UserGroup { Id = 1, Name = "Grp1" };
            var dataViewModel = new GroupViewModel { Id = 1, Name = "Grp1" };

            _repo.Update(Arg.Any<Action<UserGroup>>(), 1).Returns(data);

            _controller.Put(1, dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }
        
        [Fact]
        public void DeleteSaves()
        {
            _controller.Delete(1);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void DeleteClears()
        {
            var toDelete = new UserGroup
            {
                ActivityViewSettingses = new List<ActivityViewSettings>
                {
                    new ActivityViewSettings(),
                    new ActivityViewSettings()
                },
                ProductionViewSettings = new List<ProductionViewSettings>
                {
                    new ProductionViewSettings(),
                    new ProductionViewSettings()
                }
            };
            _repo.GetByKey(1).ReturnsForAnyArgs(toDelete);
            _controller.Delete(1);
            Assert.Empty(toDelete.ActivityViewSettingses);
            Assert.Empty(toDelete.ProductionViewSettings);
        }

        [Fact]
        public void DeleteCallsDelete()
        {
            _controller.Delete(1);
            _repo.ReceivedWithAnyArgs().DeleteByKey(1);
        }
    }
}
