using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoMapper;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels.Activity;
using customer_relations_manager.ViewModels.Company;
using Core.DomainModels.Activities;
using Core.DomainModels.Customers;
using Core.DomainServices;
using NSubstitute;
using Xunit;

namespace UnitTests.Controllers
{
    public class CompaniesControllerTest
    {
        private readonly CompaniesController _controller;
        private readonly IGenericRepository<Company> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CompaniesControllerTest()
        {
            _mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IGenericRepository<Company>>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new CompaniesController(_repo, _uow, _mapper);
        }

        [Fact]
        public void GetAllReturnsAList()
        {
            var data = new List<Company>
            {
                new Company {Id = 1, Name = "1"},
                new Company {Id = 2, Name = "2"},
                new Company {Id = 3, Name = "3"},
                new Company {Id = 4, Name = "4"}
            };
            _repo.GetPaged(Arg.Any<string[]>()).ReturnsForAnyArgs(x => new PaginationEnvelope<Company> {
                Data = data
            });

            var result = _controller.GetAll(new string[0]);
            Assert.Equal(4, result.Data.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new Company { Id = 1, Name = "1" };
            _repo.GetByKey(Arg.Any<int>()).Returns(x => data);
            var result = _controller.Get(1) as OkNegotiatedContentResult<CompanyViewModel>;
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
            var data = new Company { Id = 1, Name = "1" };
            var dataViewModel = new CompanyViewModel { Name = "1" };

            _repo.Insert(Arg.Any<Company>()).Returns(data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Insert(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new Company { Id = 1, Name = "1" };
            var dataViewModel = new CompanyViewModel { Name = "1" };

            _repo.Insert(Arg.Any<Company>()).Returns(data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateUpdatesData()
        {
            var data = new Company { Id = 1, Name = "1", Address = "test street"};
            var dataViewModel = new CompanyViewModel { Name = "new data", Address = "updated test street"};

            _repo.Update(Arg.Any<Action<Company>>(), 1)
                .Returns(data)
                .AndDoes(ci => ci.Arg<Action<Company>>().Invoke(data));


            _controller.Put(1, dataViewModel);
            Assert.Equal(new { data.Name, data.Address }, new { Name = "new data", Address = "updated test street" });
        }

        [Fact]
        public void UpdateCallsUpdateInRepo()
        {
            var data = new Company { Id = 1, Name = "1" };
            var dataViewModel = new CompanyViewModel { Name = "1" };

            _repo.Update(Arg.Any<Action<Company>>(), 1).Returns(data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(Arg.Any<Action<Company>>(), 1);
        }

        [Fact]
        public void UpdateReturnsNotFoundOnBadId()
        {
            var dataViewModel = new CompanyViewModel { Name = "1" };
            Assert.IsType<NotFoundResult>(_controller.Put(1, dataViewModel));

        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new Company { Id = 1, Name = "1" };
            var dataViewModel = new CompanyViewModel { Name = "1" };

            _repo.Update(Arg.Any<Action<Company>>(), 1).Returns(data);

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

        [Fact]
        public void PersonGetsEmployees()
        {
            var data = new Company
            {
                Id = 1, Name = "1",
                Employees = new List<Person>
                {
                    new Person {FirstName = "1"},
                    new Person {FirstName = "2"}
                }
            };
            _repo.GetByKey(Arg.Any<int>()).Returns(x => data);
            var result = _controller.Persons(1) as OkNegotiatedContentResult<IEnumerable<PersonViewModel>>;
            Assert.Equal(2, result?.Content.Count());
        }

        [Fact]
        public void PersonReturnsNotFoundOnBadCompanyId()
        {
            Assert.IsType<NotFoundResult>(_controller.Persons(2));
        }
    }
}
