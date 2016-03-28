using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoMapper;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Activity;
using Core.DomainModels.Activities;
using Core.DomainModels.Opportunity;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using NSubstitute;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Controllers
{
    public class ActivitesControllerTest
    {
        private readonly ActivitiesController _controller;
        private readonly IActivityRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ActivitesControllerTest()
        {
            _mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IActivityRepository>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new ActivitiesController(_uow, _repo, _mapper);
        }

        [Fact]
        public void GetAllReturnsAList()
        {
            var data = new List<Activity>
            {
                new Activity {Id = 1, Name = "1"},
                new Activity {Id = 2, Name = "2"},
                new Activity {Id = 3, Name = "3"},
                new Activity {Id = 4, Name = "4"}
            };
            _repo.GetAll(Arg.Any<Func<IQueryable<Activity>, IOrderedQueryable<Activity>>>()).ReturnsForAnyArgs(a => new PaginationEnvelope<Activity> {Data = data});

            var result = _controller.GetActivities();
            Assert.Equal(4, result.Data.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new Activity { Id = 1, Name = "1" };
            _repo.GetById(Arg.Any<int>()).Returns(x => data);
            var result = _controller.GetActivity(1) as OkNegotiatedContentResult<ActivityViewModel>;
            // Only testing one, since there is no reason the 
            // system should have chosen the same string. 
            // Also object comparison compares on reference
            Assert.Equal("1", result?.Content.Name);
        }

        [Fact]
        public void GetNotFount()
        {
            _repo.GetById(Arg.Any<int>()).Returns(x => null);

            var result = _controller.GetActivity(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateCallsInsert()
        {
            var data = new Activity { Id = 1, Name = "1" };
            var dataViewModel = new ActivityViewModel { Name = "1" };

            _repo.Create(Arg.Any<Activity>()).Returns(data);

            _controller.PostActivity(dataViewModel);
            _repo.ReceivedWithAnyArgs().Create(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new Activity { Id = 1, Name = "1" };
            var dataViewModel = new ActivityViewModel { Name = "1" };

            _repo.Create(Arg.Any<Activity>()).Returns(data);

            _controller.PostActivity(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateCallsUpdateInRepo()
        {
            var data = new Activity { Id = 1, Name = "1" };
            var dataViewModel = new ActivityViewModel { Name = "1" };

            _repo.Update(1, Arg.Any<Activity>()).Returns(data);

            _controller.PutActivity(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(1, Arg.Any<Activity>());
        }

        [Fact]
        public void UpdateReturnsNotFoundOnBadId()
        {
            var dataViewModel = new ActivityViewModel { Name = "1" };
            Assert.IsType<NotFoundResult>(_controller.PutActivity(1, dataViewModel));

        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new Activity { Id = 1, Name = "1" };
            var dataViewModel = new ActivityViewModel { Name = "1" };

            _repo.Update(1, Arg.Any<Activity>()).Returns(data);

            _controller.PutActivity(1, dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void DeleteSaves()
        {
            _controller.DeleteActivity(1);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void DeleteCallsDelete()
        {
            _controller.DeleteActivity(1);
            _repo.ReceivedWithAnyArgs().DeleteByKey(1);
        }
    }
}
