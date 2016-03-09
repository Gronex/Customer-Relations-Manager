using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using AutoMapper;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Activity;
using Core.DomainModels.Activities;
using Core.DomainModels.UserGroups;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using NSubstitute;
using NSubstitute.Core.Arguments;
using Xunit;

namespace UnitTests.Controllers
{
    public class ActivityCategoryControllerTest
    {
        private readonly ActivityCategoriesController _controller;
        private readonly IGenericRepository<ActivityCategory> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ActivityCategoryControllerTest()
        {
            _mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IGenericRepository<ActivityCategory>>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new ActivityCategoriesController(_repo, _uow, _mapper);
        }

        [Fact]
        public void GetAllReturnsAList()
        {
            var data = new List<ActivityCategory>
            {
                new ActivityCategory {Id = 1, Name = "1"},
                new ActivityCategory {Id = 2, Name = "2"},
                new ActivityCategory {Id = 3, Name = "3"},
                new ActivityCategory {Id = 4, Name = "4"}
            };
            _repo.Get().ReturnsForAnyArgs(x => data);

            var result = _controller.GetAll();
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new ActivityCategory { Id = 1, Name = "1" };
            _repo.GetByKey(Arg.Any<int>()).Returns(x => data);
            var result = _controller.Get(1) as OkNegotiatedContentResult<ActivityCategoryViewModel>;
            // Only testing one, since there is no reason the 
            // system should have chosen the same string. 
            // Also object comparison compares on reference
            Assert.Equal(1, result?.Content.Id);
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
            var data = new ActivityCategory { Id = 1, Name = "1" };
            var dataViewModel = new ActivityCategoryViewModel { Id = 1, Name = "1" };

            _repo.Insert(Arg.Any<ActivityCategory>()).Returns(data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Insert(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new ActivityCategory { Id = 1, Name = "1" };
            var dataViewModel = new ActivityCategoryViewModel { Id = 1, Name = "1" };

            _repo.Insert(Arg.Any<ActivityCategory>()).Returns(data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateUpdatesData()
        {
            var data = new ActivityCategory { Id = 1, Name = "1", Value = 1};
            var dataViewModel = new ActivityCategoryViewModel { Id = 1, Name = "new data", Value = 55};

            _repo.Update(Arg.Any<Action<ActivityCategory>>(), 1)
                .Returns(data)
                .AndDoes(ci => ci.Arg<Action<ActivityCategory>>().Invoke(data));
                

            _controller.Put(1, dataViewModel);
            Assert.Equal(new { data.Name, data.Value}, new {Name = "new data", Value = 55});
        }

        [Fact]
        public void UpdateCallsUpdateInRepo()
        {
            var data = new ActivityCategory { Id = 1, Name = "1" };
            var dataViewModel = new ActivityCategoryViewModel { Id = 1, Name = "1" };

            _repo.Update(Arg.Any<Action<ActivityCategory>>(), 1).Returns(data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(Arg.Any<Action<ActivityCategory>>(), 1);
        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new ActivityCategory { Id = 1, Name = "1" };
            var dataViewModel = new ActivityCategoryViewModel { Id = 1, Name = "1" };

            _repo.Update(Arg.Any<Action<ActivityCategory>>(), 1).Returns(data);

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
        public void DeleteCallsDelete()
        {
            _controller.Delete(1);
            _repo.ReceivedWithAnyArgs().DeleteByKey(1);
        }
    }
}
