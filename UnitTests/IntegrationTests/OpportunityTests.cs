using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Company;
using customer_relations_manager.ViewModels.Opportunity;
using customer_relations_manager.ViewModels.User;
using Core.DomainModels.Customers;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Infrastructure.DataAccess.Repositories;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.IntegrationTests
{
    public class OpportunityTests
    {
        private readonly OpportunitiesController _controller;
        public OpportunityTests()
        {
            var context = new AppContextStub();
            var genRepo = new GenericRepository<Opportunity>(context);

            var repo = new OpportunityRepository(context, genRepo);
            var uow = new UnitOfWorkStub();
            var mapper = AutomapperConfig.ConfigMappings().CreateMapper();


            var userGroup = new UserGroup {Id = 1};
            context.Users.Add(new User {Id = "1", UserName = "test@test.com", Email = "test@test.com", Groups = new List<UserGroupUser> { new UserGroupUser {UserGroup = userGroup} } });
            context.OpportunityCategories.Add(new OpportunityCategory { Id = 1});
            context.Persons.Add(new Person { Id = 1});
            context.Companies.Add(new Company { Id = 1});
            context.Departments.Add(new Department { Id = 1});
            context.Stages.Add(new Stage { Id = 1});
            context.UserGroups.Add(userGroup);
            
            _controller = new OpportunitiesController(repo, uow, mapper);
        }

        [Fact]
        public void CreateOpportunity()
        {
            var toCreate = new OpportunityViewModel
            {
                Amount = 1,
                Category = new CategoryViewModel { Id = 1},
                Company = new CompanyOverviewViewModel { Id = 1},
                Contact = new PersonViewModel { Id = 1},
                Description = "test!!!",
                StartDate = new DateTime(2016,1,1),
                EndDate = new DateTime(2016,1,1),
                ExpectedClose = new DateTime(2016,1,1),
                Department = new GroupViewModel { Id = 1},
                Percentage = 1,
                Name = "Test",
                Owner = new UserOverviewViewModel { Id = "1"},
                HourlyPrice = 1,
                Stage = new StageViewModel { Id = 1}
            };

            var result = _controller.Post(toCreate);
            var cast = Assert.IsType<CreatedNegotiatedContentResult<OpportunityViewModel>>(result);
            Assert.NotNull(cast.Content);
        }
    }
}
