using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.Controllers.Users;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Opportunity;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace UnitTests.Controllers
{
    public class OpportunityControllerTest
    {
        private readonly OpportunitiesController _controller;
        private readonly IOpportunityRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly Opportunity _data;

        public OpportunityControllerTest()
        {
            _data = new Opportunity { Id = 1, StartDate = new DateTime(2010, 1, 1), Name = "1" };
            var mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IOpportunityRepository>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new OpportunitiesController(_repo, _uow, mapper);
        }

        [Fact]
        public void GetAllSuccessReturnsAList()
        {
            var data = new List<Opportunity>
            {
                new Opportunity {Id = 1, StartDate = new DateTime(2016, 1, 1), Name = "1"},
                new Opportunity {Id = 2, StartDate = new DateTime(2016, 1, 1), Name = "2"},
                new Opportunity {Id = 3, StartDate = new DateTime(2016, 1, 1), Name = "3"},
                new Opportunity {Id = 4, StartDate = new DateTime(2016, 1, 1), Name = "4"},
            };

            _repo.GetAll(null).ReturnsForAnyArgs(x => new PaginationEnvelope<Opportunity> {Data = data});

            var result = _controller.GetAll(null);
            Assert.Equal(4, result.Data.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            _repo.GetById(Arg.Any<int>()).Returns(x => _data);

            var result = _controller.Get(1) as OkNegotiatedContentResult<OpportunityViewModel>;
            // Only testing one, since there is no reason the 
            // system should have chosen a year in the past, and the objects are not the same refference,
            // just the same data, if all is well
            Assert.Equal("1", result?.Content.Name);
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
            var dataViewModel = new OpportunityViewModel { Name = "test" };

            _repo.Create(Arg.Any<Opportunity>(), Arg.Any<string>()).Returns(_data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Create(_data, "");
        }

        [Fact]
        public void CreateSaves()
        {
            var dataViewModel = new OpportunityViewModel { Name = "test" };

            _repo.Create(Arg.Any<Opportunity>(), Arg.Any<string>()).Returns(_data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void CreateNotFoundDontSave()
        {
            var dataViewModel = new OpportunityViewModel { Name = "test"};

            _repo.Create(Arg.Any<Opportunity>(), Arg.Any<string>()).Throws(new NotFoundException());

            try
            {
                _controller.Post(dataViewModel);
            }
            catch { /* ignored */ }
            
            _uow.DidNotReceiveWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateCallsUpdate()
        {
            var dataViewModel = new OpportunityViewModel { Name = "test" };

            _repo.Update(1, Arg.Any<Opportunity>()).Returns(_data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(1, _data);
        }

        [Fact]
        public void UpdateSaves()
        {
            var dataViewModel = new OpportunityViewModel { Name = "test" };

            _repo.Update(1, Arg.Any<Opportunity>()).Returns(_data);

            _controller.Put(1, dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateNotFoundDontSave()
        {
            var dataViewModel = new OpportunityViewModel { Name = "test" };

            _repo.Update(1, Arg.Any<Opportunity>()).Returns(r => null);

            var result = _controller.Put(1, dataViewModel);

            // Verify the result was in fact not found
            Assert.IsType<NotFoundResult>(result);

            _uow.DidNotReceiveWithAnyArgs().Save();
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
