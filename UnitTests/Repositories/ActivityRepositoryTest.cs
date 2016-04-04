using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Customers;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Repositories;
using NSubstitute;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Repositories
{
    public class ActivityRepositoryTest
    {
        private readonly IActivityRepository _repo;
        private readonly IApplicationContext _context;
        private readonly Activity _data;
        private readonly IGenericRepository<Activity> _generic;

        public ActivityRepositoryTest()
        {
            _context = new AppContextStub();
            _generic = Substitute.For<IGenericRepository<Activity>>();

            _generic.GetByKey(Arg.Any<object[]>()).Returns(a =>
            {
                var gId = (int)a.Arg<object[]>()[0];
                try { return _context.Activities.SingleOrDefault(g => g.Id == gId); }
                catch { return null; }
            });

            _data = new Activity {
                Id = 4,
                Name = "4",
                Done = true,
                PrimaryResponsible = new User { Email = "test", UserName = "test"},
                Category = new ActivityCategory { Name = "test"},
                SecondaryResponsibles = new List<User>(),
                SecondaryContacts = new List<Person>()
            };

            //Add some data
            _context.Activities.AddRange(new List<Activity>
            {
                new Activity {Id = 0, Name = "0", PrimaryResponsible = new User()},
                new Activity {Id = 1, Name = "1", PrimaryResponsible = new User()},
                new Activity {Id = 2, Name = "2", PrimaryResponsible = new User()},
                new Activity {Id = 3, Name = "3", PrimaryResponsible = new User()},
            });

            _context.Users.Add(new User { Id = "0", Email = "test"});
            _context.ActivityCategories.Add(new ActivityCategory {Id = 1, Name = "test"});

            _repo = new ActivityRepository(_context, _generic);
        }

        [Fact]
        public void CreateAdds()
        {
            _generic
                .WhenForAnyArgs(a => a.Insert(Arg.Any<Activity>()))
                .Do(a => _context.Activities.Add(a.Arg<Activity>()));
            _repo.Create(_data);

            Assert.Contains("4", _context.Activities.Select(a => a.Name));
        }

        [Fact]
        public void GetAllReturnsFullList()
        {
            _context.Activities.Add(_data);
            var result = _repo.GetAll(10, null, null);
            Assert.Equal(_context.Activities, result);
        }

        [Fact]
        public void GetAllWithUserNameReturnsOnlyForUser()
        {
            _context.Activities.Add(_data);
            var result = _repo.GetAll(10, "test", null).Single();
            Assert.Equal(_data, result);
        }

        [Fact]
        public void GetAllWithFindFindsSimilar()
        {
            var list = new List<Activity>
            {
                new Activity { Name = "IT minds" },
                new Activity { Name = "IT-minds" },
                new Activity { Name = "itminds" },
                new Activity { Name = "it_minds"},
                new Activity { Name = "it"},
            };

            _context.Activities.AddRange(list);
            var result = _repo.GetAll(10, null, "IT minds").Select(x => x.Name);
            Assert.Equal(new List<string> { "IT minds", "IT-minds", "itminds", "it_minds" }, result);
        }

        [Fact]
        public void GetAllWithFindFindsSimilarAndPartial()
        {
            var list = new List<Activity>
            {
                new Activity { Name = "IT minds" },
                new Activity { Name = "IT-minds" },
                new Activity { Name = "itminds" },
                new Activity { Name = "it_minds"},
                new Activity { Name = "it"},
            };

            _context.Activities.AddRange(list);
            var result = _repo.GetAll(10, null, "IT").Select(x => x.Name);
            Assert.Equal(new List<string> { "IT minds", "IT-minds", "itminds", "it_minds", "it" }, result);
        }

        [Theory]
        [InlineData(0, "0")]
        [InlineData(1, "1")]
        [InlineData(2, "2")]
        [InlineData(3, "3")]
        public void GetByIdGetsData(int id, string name)
        {
            var result = _repo.GetById(id);

            Assert.Equal(name, result.Name);
        }

        [Fact]
        public void GetByIdReturnsNullOnNotFound()
        {
            var result = _repo.GetById(-1);

            Assert.Equal(result, null);
        }

        [Fact]
        public void UpdateReturnsNullOnNotFound()
        {
            var result = _repo.Update(-1, _data);

            Assert.Equal(result, null);
        }

        [Fact]
        public void UpdateReturnsUpdatesData()
        {
            // Actual test is of the action updates what is expected
            _context.Activities.Add(new Activity
            {
                Id = 7,
                Name = "toUpdate",
                Done = false,
                PrimaryResponsible = new User { Email = "test" },
                Category = new ActivityCategory { Name = "test" },
                SecondaryResponsibles = new List<User>(),
                SecondaryContacts = new List<Person>()
            });


            _generic
                .UpdateBy(Arg.Any<Action<Activity>>(), Arg.Any<Expression<Func<Activity, bool>>>())
                .Returns(ci =>
                {
                    var action = ci.Arg<Action<Activity>>();
                    var oldData = new Activity
                    {
                        Id = 4,
                        Name = "old",
                        Done = false,
                        PrimaryResponsible = new User {Email = "test"},
                        Category = new ActivityCategory {Name = "test"},
                        SecondaryResponsibles = new List<User>(),
                        SecondaryContacts = new List<Person>()
                    };
                    action(oldData);
                    return oldData;
                });

            _generic.Update(Arg.Any<Action<Activity>>(), Arg.Any<object[]>()).Returns(a =>
            {
                var db = _context.Activities.SingleOrDefault(ac => ac.Id == (int)a.Arg<object[]>()[0]);

                a.Arg<Action<Activity>>().Invoke(db);
                return db;
            });
            var result = _repo.Update(7, _data);

            Assert.Equal(new { _data.Name, _data.Done}, new { result.Name, result.Done});
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void DeleteSucceedsNoMatterWhat(int id)
        {
            _repo.DeleteByKey(id);
            _generic.Received().DeleteByKey(Arg.Any<object[]>());
        }
    }
}
