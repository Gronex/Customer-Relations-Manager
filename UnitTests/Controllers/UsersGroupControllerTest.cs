using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using AutoMapper;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels;
using Core.DomainModels.UserGroups;
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
        private readonly IUserGroupRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UsersGroupControllerTest()
        {
            _mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IUserGroupRepository>();

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
            _repo.GetAll().Returns(x => data.AsQueryable());

            var result = _controller.Get();
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new UserGroup() { Id = 1, Name = "Grp1" };
            _repo.GetById(Arg.Any<int>()).Returns(x => data);

            var result = _controller.Get(1) as OkNegotiatedContentResult<UserGroupViewModel>;
            var dataViewModel = _mapper.Map<UserGroupViewModel>(data);
            // Only testing one, since there is no reason the 
            // system should have chosen the same string and object comparison
            // compares on reference
            Assert.Equal(dataViewModel.Id, result?.Content.Id);
        }

        [Fact]
        public void GetNotFount()
        {
            _repo.GetById(Arg.Any<int>()).Returns(x => null);

            var result = _controller.Get(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateCallsCreate()
        {
            var data = new UserGroup{Id = 1, Name = "Grp1"};
            var dataViewModel = new UserGroupViewModel { Id = 1, Name = "Grp1" };

            _repo.Create(Arg.Any<UserGroup>()).Returns(data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Create(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new UserGroup { Id = 1, Name = "Grp1" };
            var dataViewModel = new UserGroupViewModel { Id = 1, Name = "Grp1" };

            _repo.Create(Arg.Any<UserGroup>()).Returns(data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        // TODO: Fugire out how to do this right
        //[Fact]
        //public void CreateFailsOnMissingName()
        //{
        //    var dataViewModel = new UserGroupViewModel { Id = 1 };
        //    var result = _controller.Post(dataViewModel);
        //    Assert.IsType<BadRequestResult>(result);
        //}

        //[Fact]
        //public void CreateFailsOnEmptyName()
        //{
        //    var dataViewModel = new UserGroupViewModel { Id = 1, Name = ""};

        //    var result = _controller.Post(dataViewModel);
        //    Assert.IsType<BadRequestResult>(result);
        //}

        [Fact]
        public void UpdateCallsCreate()
        {
            var data = new UserGroup { Id = 1, Name = "Grp1" };
            var dataViewModel = new UserGroupViewModel { Id = 1, Name = "Grp1" };

            _repo.Update(1, Arg.Any<UserGroup>()).Returns(data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(1, data);
        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new UserGroup { Id = 1, Name = "Grp1" };
            var dataViewModel = new UserGroupViewModel { Id = 1, Name = "Grp1" };

            _repo.Update(1,Arg.Any<UserGroup>()).Returns(data);

            _controller.Put(1, dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }
        
        [Fact]
        public void DeleteSaves()
        {
            _repo.Delete(1);

            _controller.Delete(1);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void DeleteCallsDelete()
        {
            _controller.Delete(1);
            _repo.ReceivedWithAnyArgs().Delete(1);
        }
    }
}
