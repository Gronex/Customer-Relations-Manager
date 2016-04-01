using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;

namespace Infrastructure.DataAccess.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<Person> _repo;

        public PersonRepository(IApplicationContext context, IGenericRepository<Person> repo)
        {
            _context = context;
            _repo = repo;
        }

        public IEnumerable<Person> GetAll()
        {
            return _repo.Get();
        }

        public PaginationEnvelope<Person> GetAll(IEnumerable<string> orderBy, int? page = null, int? pageSize = null)
        {
            return _repo.GetPaged(orderBy, page, pageSize);
        }

        public Person GetById(int id)
        {
            return _repo.GetByKeyThrows(id);
        }

        public Person Create(Person model)
        {
            return _repo.Insert(model);
        }

        public Person Update(int id, Person model)
        {
            return _repo.Update(p =>
            {
                p.FirstName = model.FirstName;
                p.LastName = model.LastName;
                p.Email = model.Email;
                p.PhoneNumber = model.PhoneNumber;

                if (p.StartDate.HasValue || p.CompanyId.HasValue || !model.CompanyId.HasValue)
                {
                    Unassign(p);
                }
                p.CompanyId = model.CompanyId;
                p.StartDate = DateTime.UtcNow.Date;
            }, id);
        }

        public Person AddToCompany(int id, int companyId)
        {
            var person = _repo.GetByKeyThrows(id);
            if (!_context.Companies.Any(c => c.Id == companyId)) throw new NotFoundException("Company");
            if (person.StartDate.HasValue) return null;
            
            person.CompanyId = companyId;
            person.StartDate = DateTime.UtcNow.Date;

            _context.SetState(person, EntityState.Modified);
            return person;
        }

        public void Unassign(int id)
        {
            var person = _repo.GetByKey(id);
            if (person == null) return;
            Unassign(person);
            
            _context.SetState(person, EntityState.Modified);
        }

        private void Unassign(Person person)
        {
            // If they are not either both null or not null, then we have a problem and need to fix it
            if (!person.StartDate.HasValue || !person.CompanyId.HasValue)
            {
                person.StartDate = null;
                person.CompanyId = null;
                return;
            }

            var contract = new Contract
            {
                Person = person,
                CompanyId = person.CompanyId.Value,
                StartDate = person.StartDate.Value,
                EndDate = DateTime.UtcNow.Date
            };

            person.StartDate = null;
            person.CompanyId = null;

            foreach (var opportunity in _context
                .Opportunities
                .Where(o => o.ContactId == person.Id))
            {
                opportunity.ContactId = null;
                _context.SetState(opportunity, EntityState.Modified);
            }

            person.Contracts.Add(contract);
        }
    }
}
