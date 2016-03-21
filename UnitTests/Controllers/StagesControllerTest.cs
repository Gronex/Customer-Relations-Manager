using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Opportunity;
using Core.DomainModels.Opportunity;
using Core.DomainServices;
using Infrastructure.DataAccess.Exceptions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace UnitTests.Controllers
{
    public class StagesControllerTest
    {
        private readonly StagesController _controller;
        private readonly IGenericRepository<Stage> _repo;
        private readonly IUnitOfWork _uow;

        public StagesControllerTest()
        {
            var mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IGenericRepository<Stage>>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new StagesController(_repo, _uow, mapper);
        }

        [Fact]
        public void GetAllReturnsAList()
        {
            var data = new List<Stage>
            {
                new Stage {Id = 1, Name = "1"},
                new Stage {Id = 2, Name = "2"},
                new Stage {Id = 3, Name = "3"},
                new Stage {Id = 4, Name = "4"}
            };
            _repo.Get().ReturnsForAnyArgs(data);

            var result = _controller.GetAll();
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new Stage { Id = 1, Name = "1" };
            _repo.GetByKey(Arg.Any<int>()).Returns(x => data);
            var result = _controller.Get(1) as OkNegotiatedContentResult<StageViewModel>;
            // Only testing one, since there is no reason the 
            // system should have chosen the same string. 
            // Also object comparison compares on reference
            Assert.Equal("1", result?.Content.Name);
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
            var data = new Stage { Id = 1, Name = "1" };
            var dataViewModel = new StageViewModel { Name = "1" };

            _repo.Insert(Arg.Any<Stage>()).Returns(data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Insert(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new Stage { Id = 1, Name = "1" };
            var dataViewModel = new StageViewModel { Name = "1" };

            _repo.Insert(Arg.Any<Stage>()).Returns(data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateUpdatesData()
        {
            var data = new Stage { Id = 1, Name = "1" };
            var dataViewModel = new StageViewModel { Name = "new data" };

            _repo.Update(Arg.Any<Action<Stage>>(), 1)
                .Returns(data)
                .AndDoes(ci => ci.Arg<Action<Stage>>().Invoke(data));


            _controller.Put(1, dataViewModel);
            Assert.Equal(data.Name, "new data");
        }

        [Fact]
        public void UpdateCallsUpdateInRepo()
        {
            var data = new Stage { Id = 1, Name = "1" };
            var dataViewModel = new StageViewModel { Name = "1" };

            _repo.Update(Arg.Any<Action<Stage>>(), 1).Returns(data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(Arg.Any<Action<Stage>>(), 1);
        }

        [Fact]
        public void UpdateReturnsNotFoundOnBadId()
        {
            _repo.Update(Arg.Any<Action<Stage>>(), 1).Throws(new NotFoundException());
            var dataViewModel = new StageViewModel { Name = "1" };
            // A filter will take care of converting the exception to the right result
            Assert.Throws<NotFoundException>(() => _controller.Put(1, dataViewModel));
        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new Stage { Id = 1, Name = "1" };
            var dataViewModel = new StageViewModel { Name = "1" };

            _repo.Update(Arg.Any<Action<Stage>>(), 1).Returns(data);

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
