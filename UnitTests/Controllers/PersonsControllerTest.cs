using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels.Company;
using Core.DomainModels.Activities;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using NSubstitute;
using Xunit;

namespace UnitTests.Controllers
{
    public class PersonsControllerTest
    {
        private readonly PersonsController _controller;
        private readonly IPersonRepository _repo;
        private readonly IUnitOfWork _uow;

        public PersonsControllerTest()
        {
            var mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IPersonRepository>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new PersonsController(_repo, _uow, mapper);
        }

        [Fact]
        public void GetAllReturnsAList()
        {
            var data = new List<Person>
            {
                new Person {Id = 1, FirstName = "1"},
                new Person {Id = 2, FirstName = "2"},
                new Person {Id = 3, FirstName = "3"},
                new Person {Id = 4, FirstName = "4"}
            };
            _repo.GetAll(Arg.Any<IEnumerable<string>>()).ReturnsForAnyArgs(a => new PaginationEnvelope<Person> { Data = data });

            var result = _controller.GetAll(new string[0]);
            Assert.Equal(4, result.Data.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new Person { Id = 1, FirstName = "1" };
            _repo.GetById(Arg.Any<int>()).Returns(x => data);
            var result = _controller.Get(1) as OkNegotiatedContentResult<PersonViewModel>;
            // Only testing one, since there is no reason the 
            // system should have chosen the same string. 
            // Also object comparison compares on reference
            Assert.Equal("1", result?.Content.FirstName);
        }

        [Fact]
        public void GetNotFount()
        {
            _repo.GetById(Arg.Any<int>()).Returns(x => null);

            var result = _controller.Get(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateCallsInsert()
        {
            var data = new Person { Id = 1, FirstName = "1" };
            var dataViewModel = new PersonViewModel { FirstName = "1" };

            _repo.Create(Arg.Any<Person>()).Returns(data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Create(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new Person { Id = 1, FirstName = "1" };
            var dataViewModel = new PersonViewModel { FirstName = "1" };

            _repo.Create(Arg.Any<Person>()).Returns(data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateCallsUpdateInRepo()
        {
            var data = new Person { Id = 1, FirstName = "1" };
            var dataViewModel = new PersonViewModel { FirstName = "1" };

            _repo.Update(1, Arg.Any<Person>()).Returns(data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(1, Arg.Any<Person>());
        }

        [Fact]
        public void UpdateReturnsNotFoundOnBadId()
        {
            var dataViewModel = new PersonViewModel { FirstName = "1" };
            Assert.IsType<NotFoundResult>(_controller.Put(1, dataViewModel));

        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new Person { Id = 1, FirstName = "1" };
            var dataViewModel = new PersonViewModel { FirstName = "1" };

            _repo.Update(1, Arg.Any<Person>()).Returns(data);

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
        public void DeleteCallsUnassign()
        {
            _controller.Delete(1);
            _repo.ReceivedWithAnyArgs().Unassign(1);
        }

        [Fact]
        public void AddToCompanySaves()
        {
            _repo.AddToCompany(1, 1).Returns(new Person());
            _controller.AddToCompany(1, 1);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void AddToCompanyCallsRepo()
        {
            _controller.AddToCompany(1, 1);
            _repo.ReceivedWithAnyArgs().AddToCompany(1, 1);
        }
        
    }
}
