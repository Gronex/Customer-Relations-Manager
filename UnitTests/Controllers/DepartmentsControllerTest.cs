using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels;
using Core.DomainModels.Customers;
using Core.DomainModels.Opportunity;
using Core.DomainServices;
using NSubstitute;
using Xunit;

namespace UnitTests.Controllers
{
    public class DepartmentsControllerTest
    {
        private readonly DepartmentsController _controller;
        private readonly IGenericRepository<Department> _repo;
        private readonly IUnitOfWork _uow;

        public DepartmentsControllerTest()
        {
            var mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IGenericRepository<Department>>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new DepartmentsController(_repo, _uow, mapper);
        }

        [Fact]
        public void GetAllReturnsAList()
        {
            var data = new List<Department>
            {
                new Department {Id = 1, Name = "1"},
                new Department {Id = 2, Name = "2"},
                new Department {Id = 3, Name = "3"},
                new Department {Id = 4, Name = "4"}
            };
            _repo.Get().ReturnsForAnyArgs(data);

            var result = _controller.Get();
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new Department { Id = 1, Name = "1" };
            _repo.GetByKey(Arg.Any<int>()).Returns(x => data);
            var result = _controller.Get(1) as OkNegotiatedContentResult<GroupViewModel>;
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
            var data = new Department { Id = 1, Name = "1" };
            var dataViewModel = new GroupViewModel { Name = "1" };

            _repo.Insert(Arg.Any<Department>()).Returns(data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Insert(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new Department { Id = 1, Name = "1" };
            var dataViewModel = new GroupViewModel { Name = "1" };

            _repo.Insert(Arg.Any<Department>()).Returns(data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateUpdatesData()
        {
            var data = new Department { Id = 1, Name = "1"};
            var dataViewModel = new GroupViewModel { Name = "new data"};

            _repo.Update(Arg.Any<Action<Department>>(), 1)
                .Returns(data)
                .AndDoes(ci => ci.Arg<Action<Department>>().Invoke(data));


            _controller.Put(1, dataViewModel);
            Assert.Equal(data.Name, "new data");
        }

        [Fact]
        public void UpdateCallsUpdateInRepo()
        {
            var data = new Department { Id = 1, Name = "1" };
            var dataViewModel = new GroupViewModel { Name = "1" };

            _repo.Update(Arg.Any<Action<Department>>(), 1).Returns(data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(Arg.Any<Action<Department>>(), 1);
        }

        [Fact]
        public void UpdateReturnsNotFoundOnBadId()
        {
            var dataViewModel = new GroupViewModel { Name = "1" };
            Assert.IsType<NotFoundResult>(_controller.Put(1, dataViewModel));

        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new Department { Id = 1, Name = "1" };
            var dataViewModel = new GroupViewModel { Name = "1" };

            _repo.Update(Arg.Any<Action<Department>>(), 1).Returns(data);

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
