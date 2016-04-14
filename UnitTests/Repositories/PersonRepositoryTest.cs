using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Customers;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using Infrastructure.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Repositories
{
    public class PersonRepositoryTest
    {
        private readonly IPersonRepository _repo;
        private readonly IApplicationContext _context;
        private readonly Person _data;
        private readonly IGenericRepository<Person> _generic;

        public PersonRepositoryTest()
        {
            _context = new AppContextStub();
            _generic = Substitute.For<IGenericRepository<Person>>();

            _generic.GetByKeyThrows(Arg.Any<object[]>()).Returns(a =>
            {
                var pId = (int)a.Arg<object[]>()[0];
                try { return _context.Persons.Single(p => p.Id == pId); }
                catch{ throw new NotFoundException(); }
            });

            _generic.GetByKey(Arg.Any<object[]>()).Returns(a =>
            {
                var pId = (int)a.Arg<object[]>()[0];
                try { return _context.Persons.Single(p => p.Id == pId); }
                catch { return null; }
            });

            _data = new Person { FirstName = "test", LastName = "testsen", Email = "test@test.test", PhoneNumber = "123123123"};

            //Add some data
            _context.Persons.AddRange(new List<Person>
            {
                new Person { FirstName = "t1", LastName = "t1", Id = 1, Contracts = new List<Contract>()},
                new Person { FirstName = "t2", LastName = "t2", Id = 2},
                new Person { FirstName = "t3", LastName = "t3", Id = 3},
                new Person { FirstName = "t4", LastName = "t4", Id = 4},
                new Person { FirstName = "t5", LastName = "t5", Id = 5},
            });

            _repo = new PersonRepository(_context, _generic);
        }

        [Fact]
        public void CreateAdds()
        {
            _generic.Insert(Arg.Any<Person>()).Returns(p =>
            {
                var person = p.Arg<Person>();
                _context.Persons.Add(person);
                return person;
            });

            var result = _repo.Create(_data);

            Assert.Contains(result, _context.Persons);
        }

        [Fact]
        public void AddToFailsOnNoCompany()
        {
            var msg = Assert.Throws<NotFoundException>(() => _repo.AddToCompany(1, -1)).Message;
            Assert.Equal("Company", msg);
        }

        [Fact]
        public void AddToSucceeds()
        {
            var company = new Company {Id = 0};
            _context.Companies.Add(company);

            _repo.AddToCompany(1, 0);

            var person = _context.Persons.SingleOrDefault(c => c.Id == 1);
            Assert.True(person.CompanyId.HasValue);
        }

        [Fact]
        public void AddToFailsOnNoPerson()
        {
            _context.Companies.Add(new Company { Id = 0});
            Assert.Throws<NotFoundException>(() => _repo.AddToCompany(-1, 0));
        }

        [Fact]
        public void GetAllReturnsFullList()
        {
            _generic.Get().Returns(a => _context.Persons);

            var result = _repo.GetAll();

            Assert.Equal(_context.Persons, result);
        }

        [Theory]
        [InlineData(1, "t1")]
        [InlineData(2, "t2")]
        [InlineData(3, "t3")]
        [InlineData(4, "t4")]
        [InlineData(5, "t5")]
        public void GetByIdGetsData(int id, string firstName)
        {
            var result = _repo.GetById(id);

            Assert.Equal(firstName, result.FirstName);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        public void GetByIdThrowsOnNotFound(int id)
        {
            Assert.Throws<NotFoundException>(() => _repo.GetById(id));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        public void UpdateThrowsOnNotFound(int id)
        {
            _generic.Update(Arg.Any<Action<Person>>()).ThrowsForAnyArgs(new NotFoundException());
            Assert.Throws<NotFoundException>(() => _repo.Update(id, _data));
        }

        [Fact]
        public void UpdateReturnsUpdatesData()
        {
            _generic
                .Update(Arg.Any<Action<Person>>(), Arg.Any<object[]>())
                .Returns(ci =>
                {
                    var action = ci.Arg<Action<Person>>();
                    var oldData = new Person { FirstName = "OldFirstname", LastName = "OldLastname", Email = "oldEmail", PhoneNumber = "oldPhone", };
                    action(oldData);
                    return oldData;
                });

            var result = _repo.Update(1, _data);
            Assert.Equal(new { _data.FirstName, _data.LastName, _data.Email, _data.PhoneNumber }, new { result.FirstName, result.LastName, result.Email, result.PhoneNumber});
        }

        [Fact]
        public void UnassignDoesNotCrashOnNotFound()
        {
            var company = new Company {Id = 0};
            _context.Companies.Add(company);

            _repo.AddToCompany(1, 0);

            _repo.Unassign(-1);
            // If we get this far without crashing, we are not going to crash because of this call
            Assert.True(true);
        }
        

        [Fact]
        public void UnassignAddsToContracts()
        {
            _context.Companies.Add(new Company { Id = 0 });

            _repo.AddToCompany(1, 0);

            _repo.Unassign(1);

            var person = _context.Persons.SingleOrDefault(p => p.Id == 1);
            Assert.Equal(0, person.Contracts.SingleOrDefault(c => c.Person.Id == 1).CompanyId);
        }

        [Fact]
        public void UnassignRemovesTarget()
        {
            _context.Companies.Add(new Company { Id = 0 });

            _repo.AddToCompany(1, 0);

            _repo.Unassign(1);

            var person = _context.Persons.SingleOrDefault(p => p.Id == 1);
            Assert.False(person.CompanyId.HasValue);
        }
    }
}
