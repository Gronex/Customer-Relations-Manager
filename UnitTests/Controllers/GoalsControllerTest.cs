using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoMapper;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.Controllers.Users;
using customer_relations_manager.ViewModels;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using NSubstitute;
using Xunit;

namespace UnitTests.Controllers
{
    public class GoalsControllerTest
    {
        private readonly GoalsController _controller;
        private readonly IGoalRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ProductionGoal _data;

        public GoalsControllerTest()
        {
            _data = new ProductionGoal { Id = 1, StartDate = new DateTime(2010, 1, 1), Goal = 100 };
            var mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IGoalRepository>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new GoalsController(_repo, _uow, mapper);
        }

        [Fact]
        public void GetAllSuccessReturnsAList()
        {
            var data = new List<ProductionGoal>
            {
                new ProductionGoal {Id = 1, StartDate = new DateTime(2016, 1, 1), Goal = 100},
                new ProductionGoal {Id = 2, StartDate = new DateTime(2016, 1, 1), Goal = 200},
                new ProductionGoal {Id = 3, StartDate = new DateTime(2016, 1, 1), Goal = 300},
                new ProductionGoal {Id = 4, StartDate = new DateTime(2016, 1, 1), Goal = 100000},
            };

            _repo.GetAll(Arg.Any<string>()).Returns(x => data.AsQueryable());

            var result = _controller.Get("") as OkNegotiatedContentResult<IEnumerable<GoalViewModel>>;
            Assert.Equal(4, result.Content.Count());
        }

        [Fact]
        public void GetAllFailReturnsNotFound()
        {
            _repo.GetAll(Arg.Any<string>()).Returns(x => null);

            var result = _controller.Get("");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetIsCorrectData()
        {
            _repo.GetById(Arg.Any<string>(), Arg.Any<int>()).Returns(x => _data);

            var result = _controller.Get("", 1) as OkNegotiatedContentResult<GoalViewModel>;
            // Only testing one, since there is no reason the 
            // system should have chosen a year in the past, and the objects are not the same refference,
            // just the same data, if all is well
            Assert.Equal(_data.StartDate.Year, result?.Content.Year);
        }

        [Fact]
        public void GetNotFount()
        {
            _repo.GetById(Arg.Any<string>(), Arg.Any<int>()).Returns(x => null);

            var result = _controller.Get("", 1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateCallsCreate()
        {
            var dataViewModel = new GoalViewModel { Id = 1, Month = 1, Year = 2016};

            _repo.Create(Arg.Any<string>(), Arg.Any<ProductionGoal>()).Returns(_data);

            _controller.Post("", dataViewModel);
            _repo.ReceivedWithAnyArgs().Create("", _data);
        }

        [Fact]
        public void CreateSaves()
        {
            var dataViewModel = new GoalViewModel { Id = 1, Month = 1, Year = 2016 };

            _repo.Create(Arg.Any<string>(), Arg.Any<ProductionGoal>()).Returns(_data);

            _controller.Post("", dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void CreateNotFoundDontSave()
        {
            var dataViewModel = new GoalViewModel { Id = 1, Month = 1, Year = 2016 };

            _repo.Create(Arg.Any<string>(), Arg.Any<ProductionGoal>()).Returns(r => null);

            var result = _controller.Post("", dataViewModel);

            // Verify the result was in fact not found
            Assert.IsType<NotFoundResult>(result);

            _uow.DidNotReceiveWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateCallsUpdate()
        {
            var dataViewModel = new GoalViewModel { Id = 1, Month = 1, Year = 2016 };

            _repo.Update("", 1, Arg.Any<ProductionGoal>()).Returns(_data);

            _controller.Put("", 1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update("", 1, _data);
        }

        [Fact]
        public void UpdateSaves()
        {
            var dataViewModel = new GoalViewModel { Id = 1, Month = 1, Year = 2016 };

            _repo.Update("", 1, Arg.Any<ProductionGoal>()).Returns(_data);

            _controller.Put("", 1, dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateNotFoundDontSave()
        {
            var dataViewModel = new GoalViewModel { Id = 1, Month = 1, Year = 2016 };

            _repo.Update("", 1, Arg.Any<ProductionGoal>()).Returns(r => null);

            var result = _controller.Put("", 1, dataViewModel);

            // Verify the result was in fact not found
            Assert.IsType<NotFoundResult>(result);

            _uow.DidNotReceiveWithAnyArgs().Save();
        }

        [Fact]
        public void DeleteSaves()
        {
            _repo.Delete("", 1);

            _controller.Delete("", 1);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void DeleteCallsDelete()
        {
            _controller.Delete("", 1);
            _repo.ReceivedWithAnyArgs().Delete("", 1);
        }
    }
}
