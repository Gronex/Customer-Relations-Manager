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
using Core.DomainModels.Opportunity;
using Core.DomainModels.ViewSettings;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace UnitTests.Controllers
{
    public class OpportunityCategoryController
    {
        private readonly OpportunityCategoriesController _controller;
        private readonly IGenericRepository<OpportunityCategory> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public OpportunityCategoryController()
        {
            _mapper = AutomapperConfig.ConfigMappings().CreateMapper();

            _repo = Substitute.For<IGenericRepository<OpportunityCategory>>();

            _uow = Substitute.For<IUnitOfWork>();
            _controller = new OpportunityCategoriesController(_repo, _uow, _mapper);
        }

        [Fact]
        public void GetAllReturnsAList()
        {
            var data = new List<OpportunityCategory>
            {
                new OpportunityCategory {Id = 1, Name = "1"},
                new OpportunityCategory {Id = 2, Name = "2"},
                new OpportunityCategory {Id = 3, Name = "3"},
                new OpportunityCategory {Id = 4, Name = "4"}
            };
            _repo.Get().ReturnsForAnyArgs(data);

            var result = _controller.Get();
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void GetIsCorrectData()
        {
            var data = new OpportunityCategory { Id = 1, Name = "1" };
            _repo.GetByKeyThrows(Arg.Any<int>()).Returns(x => data);
            var result = _controller.Get(1) as OkNegotiatedContentResult<CategoryViewModel>;
            // Only testing one, since there is no reason the 
            // system should have chosen the same string. 
            // Also object comparison compares on reference
            Assert.Equal("1", result?.Content.Name);
        }

        [Fact]
        public void GetNotFount()
        {
            _repo.GetByKeyThrows(Arg.Any<int>()).ThrowsForAnyArgs(new NotFoundException());
            Assert.Throws<NotFoundException>(() => _controller.Get(1));
        }

        [Fact]
        public void CreateCallsInsert()
        {
            var data = new OpportunityCategory { Id = 1, Name = "1" };
            var dataViewModel = new CategoryViewModel { Name = "1" };

            _repo.Insert(Arg.Any<OpportunityCategory>()).Returns(data);

            _controller.Post(dataViewModel);
            _repo.ReceivedWithAnyArgs().Insert(data);
        }

        [Fact]
        public void CreateSaves()
        {
            var data = new OpportunityCategory { Id = 1, Name = "1" };
            var dataViewModel = new CategoryViewModel { Name = "1" };

            _repo.Insert(Arg.Any<OpportunityCategory>()).Returns(data);

            _controller.Post(dataViewModel);
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void UpdateUpdatesData()
        {
            var data = new OpportunityCategory { Id = 1, Name = "1" };
            var dataViewModel = new CategoryViewModel { Name = "new data" };

            _repo.Update(Arg.Any<Action<OpportunityCategory>>(), 1)
                .Returns(data)
                .AndDoes(ci => ci.Arg<Action<OpportunityCategory>>().Invoke(data));


            _controller.Put(1, dataViewModel);
            Assert.Equal(data.Name, "new data");
        }

        [Fact]
        public void UpdateCallsUpdateInRepo()
        {
            var data = new OpportunityCategory { Id = 1, Name = "1" };
            var dataViewModel = new CategoryViewModel { Name = "1" };

            _repo.Update(Arg.Any<Action<OpportunityCategory>>(), 1).Returns(data);

            _controller.Put(1, dataViewModel);
            _repo.ReceivedWithAnyArgs().Update(Arg.Any<Action<OpportunityCategory>>(), 1);
        }

        [Fact]
        public void UpdateReturnsNotFoundOnBadId()
        {
            var dataViewModel = new CategoryViewModel { Name = "1" };
            _repo.Update(Arg.Any<Action<OpportunityCategory>>(), 1).ThrowsForAnyArgs(new NotFoundException());
            Assert.Throws<NotFoundException>(() => _controller.Put(1, dataViewModel));
        }

        [Fact]
        public void UpdateSaves()
        {
            var data = new OpportunityCategory { Id = 1, Name = "1" };
            var dataViewModel = new CategoryViewModel { Name = "1" };

            _repo.Update(Arg.Any<Action<OpportunityCategory>>(), 1).Returns(data);

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
            var toDelete = new OpportunityCategory
            {
                ProductionViewSettings = new List<ProductionViewSettings>
                {
                    new ProductionViewSettings(),
                    new ProductionViewSettings()
                }
            };

            _repo.GetByKey(1).Returns(toDelete);
            _controller.Delete(1);
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
