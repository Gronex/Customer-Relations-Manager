using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Customers;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using Infrastructure.DataAccess.Repositories;
using NSubstitute;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Repositories
{
    public class OpportunityRepositoryTest
    {
        private readonly IOpportunityRepository _repo;
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<Opportunity> _generic;

        public OpportunityRepositoryTest()
        {
            _context = new AppContextStub();
            _generic = Substitute.For<IGenericRepository<Opportunity>>();
            
            _repo = new OpportunityRepository(_context, _generic);

            _generic
                .Update(Arg.Any<Action<Opportunity>>(), Arg.Any<object[]>())
                .ReturnsForAnyArgs(i =>
                {
                    var old = new Opportunity {Department = new Department {Id = 2} };
                    i.Arg<Action<Opportunity>>().Invoke(old);
                    return old;
                });
        }

        [Fact]
        public void GetAllGets()
        {
            SeedData();

            _generic.Get().ReturnsForAnyArgs(_context.Opportunities);

            var result = _repo.GetAll();

            Assert.Same(_context.Opportunities, result);
        }

        [Fact]
        public void GetAllGetsPaged()
        {
            _repo.GetAll(os => os.OrderBy(o => o.Name));
            _generic
                .ReceivedWithAnyArgs()
                .GetPaged(Arg.Any<Func<IQueryable<Opportunity>, IOrderedQueryable<Opportunity>>>());
        }

        [Fact]
        public void GetSingleSuccess()
        {
            SeedData();

            _generic.GetByKey().ReturnsForAnyArgs(i =>
            {
                var id = (int) i.Arg<object[]>()[0];
                return _context.Opportunities.SingleOrDefault(o => o.Id == id);
            });

            var data = _repo.GetById(1);

            Assert.Equal("1", data.Name);
        }

        [Fact]
        public void CreateInserts()
        {
            SeedData();
            _repo.Create(new Opportunity
            {
                Stage = new Stage { Id = 1},
                Category = new OpportunityCategory { Id = 1},
                Department = new Department { Id = 1},
                Company = new Company { Id = 1},
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ExpectedClose = DateTime.Now,
            }, "test");

            _generic
                .ReceivedWithAnyArgs()
                .Insert(Arg.Any<Opportunity>());
        }

        [Theory]
        [InlineData(2,1,1,1,"test",false)]
        [InlineData(1,2,1,1,"test",false)]
        [InlineData(1,1,2,1,"test",false)]
        [InlineData(1,1,1,2,"test",false)]
        [InlineData(1,1,1,1,"bad user",false)]
        [InlineData(1,1,1,1, "test", true)]
        public void CreateFailsOnBadIds(int sId, int cId, int dId, int coId, string username, bool user)
        {
            SeedData();

            Assert.Throws<NotFoundException>(() => _repo.Create(new Opportunity
                {
                    Stage = new Stage { Id = sId },
                    Category = new OpportunityCategory { Id = cId },
                    Department = new Department { Id = dId },
                    Company = new Company { Id = coId },
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    ExpectedClose = DateTime.Now,
                    Owner = user ? new User {Id = "2"} : null
                }, username));
        }

        [Fact]
        public void UpdateUpdates()
        {
            SeedData();
            var result = _repo.Update(1, new Opportunity
            {
                Stage = new Stage { Id = 1 },
                Category = new OpportunityCategory { Id = 1 },
                Department = new Department { Id = 1 },
                Company = new Company { Id = 1 },
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date,
                ExpectedClose = DateTime.Now.Date,
                Owner = new User { Id = "1"},
                Amount = 100,
                Contact = new Person { Id = 1},
                Description = "desc",
                HourlyPrice = 100,
                Name = "name",
                Percentage = 100
            });

            Assert.Equal(new
                {
                    Stage = 1,
                    Category = 1,
                    Department = 2, //Should not update
                    Company = 1,
                    StartDate = DateTime.Now.Date,
                    EndDate = DateTime.Now.Date,
                    ExpectedClose = DateTime.Now.Date,
                    Owner = "1",
                    Amount = 100,
                    Contact = 1,
                    Description = "desc",
                    HourlyPrice = 100,
                    Name = "name",
                    Percentage = 100
            }, new
                {
                    Stage = result.Stage.Id,
                    Category = result.Category.Id,
                    Department = result.Department.Id,
                    Company = result.Company.Id,
                    StartDate = DateTime.Now.Date,
                    EndDate = DateTime.Now.Date,
                    ExpectedClose = DateTime.Now.Date,
                    Owner = result.Owner.Id,
                    Amount = 100,
                    Contact = result.Contact.Id,
                    Description = "desc",
                    HourlyPrice = 100,
                    Name = "name",
                    Percentage = 100
            });
        }
        
        [Theory]
        [InlineData(2, 1, 1, false)]
        [InlineData(1, 2, 1, false)]
        [InlineData(1, 1, 2, false)]
        [InlineData(1, 1, 1, true)]
        public void UpdateFailsOnBadIds(int sId, int cId, int coId, bool user)
        {
            SeedData();

            Assert.Throws<NotFoundException>(() => _repo.Update(1, new Opportunity
                {
                    Stage = new Stage {Id = sId},
                    Category = new OpportunityCategory {Id = cId},
                    Company = new Company {Id = coId},
                    StartDate = DateTime.Now.Date,
                    EndDate = DateTime.Now.Date,
                    ExpectedClose = DateTime.Now.Date,
                    Owner = user ? new User {Id = "2"} : new User {Id = "1"}
                }));
        }

        [Fact]
        public void DeleteDeletes()
        {
            _repo.Delete(1);

            _generic
                .ReceivedWithAnyArgs()
                .DeleteByKey();
        }

        private void SeedData()
        {
            _context.Opportunities.AddRange(new List<Opportunity>
            {
                new Opportunity {Id = 6, Name = "6"},
                new Opportunity {Id = 1, Name = "1"},
                new Opportunity {Id = 3, Name = "3"}
            });
            
            _context.Users.Add(new User
            {
                Id = "1",
                UserName = "test",
                Groups = new List<UserGroupUser>
                {
                    new UserGroupUser {UserGroup = new UserGroup {Id = 1}},
                    new UserGroupUser {UserGroup = new UserGroup {Id = 2}}
                }
            });
            
            _context.Stages.Add(new Stage{ Id = 1 });
            _context.OpportunityCategories.Add(new OpportunityCategory { Id = 1 });
            _context.Departments.Add(new Department { Id = 1 });
            _context.Companies.Add(new Company { Id = 1 });
            _context.Persons.Add(new Person { Id = 1, CompanyId = 1});
        }
    }
}
