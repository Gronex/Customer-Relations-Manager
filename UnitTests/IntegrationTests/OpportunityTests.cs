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
using Core.DomainServices;
using Core.DomainServices.Filters;
using Infrastructure.DataAccess.Repositories;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.IntegrationTests
{
    public class OpportunityTests
    {
        private readonly OpportunitiesController _controller;
        private readonly IApplicationContext _context;
        public OpportunityTests()
        {
            _context = new AppContextStub();
            var genRepo = new GenericRepository<Opportunity>(_context);

            var repo = new OpportunityRepository(_context, genRepo);
            var uow = new UnitOfWorkStub();
            var mapper = AutomapperConfig.ConfigMappings().CreateMapper();


            var userGroup = new UserGroup {Id = 1, Name = "grp"};
            _context.Users.Add(new User {Id = "1", UserName = "test@test.com", Email = "test@test.com", Groups = new List<UserGroupUser> { new UserGroupUser {UserGroup = userGroup} } });
            _context.OpportunityCategories.Add(new OpportunityCategory { Id = 1});
            _context.Persons.Add(new Person { Id = 1});
            _context.Companies.Add(new Company { Id = 1});
            _context.Departments.Add(new Department { Id = 1});
            _context.Stages.Add(new Stage { Id = 1});
            _context.UserGroups.Add(userGroup);
            
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

        [Fact]
        public void GetAllOpportunities()
        {
            var data = new Opportunity
            {
                Id = 1,
                Amount = 1,
                Category = new OpportunityCategory(),
                Company = new Company {Name = "cmp"},
                Contact = new Person(),
                Description = "test!!!",
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 1, 2),
                ExpectedClose = new DateTime(2016, 1, 3),
                Department = new Department(),
                Percentage = 2,
                Name = "Test",
                Owner = new User {FirstName = "Test", LastName = "pers"},
                HourlyPrice = 3,
                Stage = new Stage { Name = "stage"}
            };
            _context.Opportunities.Add(data);

            var result = _controller.GetAll(new PagedSearchFilter());
            var cast = Assert.IsType<PaginationEnvelope<OpportunityOverviewViewMode>>(result);
            var resultData = cast.Data.FirstOrDefault();

            var expected = new
            {
                Id = 1,
                Name = "Test",
                OwnerName = "Test pers",
                CompanyName = "cmp",
                Amount = 1,
                StartDate = new DateTime(2016,1,1),
                EndDate = new DateTime(2016,1,2),
                ExpectedClose = new DateTime(2016,1,3),
                Percentage = 2,
                StageName = "stage"
            };

            var actual = new
            {
                resultData.Id,
                resultData.Name,
                resultData.OwnerName,
                resultData.CompanyName,
                Amount = (int) resultData.Amount,
                resultData.StartDate,
                resultData.EndDate,
                resultData.ExpectedClose,
                resultData.Percentage,
                StageName = resultData.Stage.Name
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetOpportunity()
        {
            var data = new Opportunity
            {
                Id = 0,
                Amount = 1,
                Category = new OpportunityCategory {Name = "cat!!"},
                Company = new Company { Name = "cmp" },
                Contact = new Person {FirstName = "contact", LastName = "con"},
                Description = "test!!!",
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 1, 2),
                ExpectedClose = new DateTime(2016, 1, 3),
                Department = new Department {Name = "dep"},
                Percentage = 2,
                Name = "Test",
                Owner = new User {FirstName = "Test", LastName = "pers"},
                HourlyPrice = 3,
                Stage = new Stage { Name = "stage" },
                UserGroups = new []{ new UserGroupOpportunity {UserGroup = new UserGroup {Name = "grp"} } }
            };
            _context.Opportunities.Add(data);

            var result = _controller.Get(0);
            var cast = Assert.IsType<OkNegotiatedContentResult<OpportunityViewModel>>(result);
            var resultData = cast.Content;

            var expected = new
            {
                Name = "Test",
                Description = "test!!!",
                OwnerName = "Test pers",
                CompanyName = "cmp",
                ContactName = "contact con",
                Amount = 1,
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 1, 2),
                ExpectedClose = new DateTime(2016, 1, 3),
                Group = "grp", // workaround to make sure all the groups are there
                GroupCount = 1,
                HourlyPrice = 3,
                Percentage = 2,
                StageName = "stage",
                Category = "cat!!",
                Department = "dep"
            };

            var actual = new
            {
                resultData.Name,
                resultData.Description,
                OwnerName = resultData.Owner.Name,
                CompanyName = resultData.Company.Name,
                ContactName = resultData.Contact.Name,
                Amount = (int) resultData.Amount,
                StartDate = resultData.StartDate.Value,
                EndDate= resultData.EndDate.Value,
                ExpectedClose= resultData.ExpectedClose.Value,
                Group = resultData.Groups.Select(g => g.Name).FirstOrDefault(),
                GroupCount =resultData.Groups.Count(),
                HourlyPrice = (int) resultData.HourlyPrice,
                resultData.Percentage,
                StageName = resultData.Stage.Name,
                Category = resultData.Category.Name,
                Department = resultData.Department.Name
            };

            Assert.Equal(expected, actual);
        }
    }
}
