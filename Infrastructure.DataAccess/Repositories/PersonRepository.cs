using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Repositories;

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

        public Person GetById(int id)
        {
            return _repo.GetByKey(id);
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
            }, id);
        }

        public Person AddToCompany(int id, int companyId)
        {
            var person = _repo.GetByKey(id);
            if (person == null) return null;
            if (!_context.Companies.Any(c => c.Id == companyId)) return null;

            person.Contracts.Add(new Contract {CompanyId = companyId, StartDate = DateTime.UtcNow.Date});

            _context.SetState(person, EntityState.Modified);
            return person;
        }

        public void Unassign(int id, int? companyId)
        {
            var person = _repo.GetByKey(id);
            if (person == null) return;

            if (companyId.HasValue)
            {
                var contract = person.Contracts.SingleOrDefault(c => c.CompanyId == companyId.Value);
                if (contract == null || contract.EndDate.HasValue) return;
                contract.EndDate = DateTime.UtcNow.Date;
            }
            else
            {
                foreach (var contract in person.Contracts.Where(c => !c.EndDate.HasValue))
                {
                    contract.EndDate = DateTime.UtcNow.Date;
                }
            }
            _context.SetState(person, EntityState.Modified);
        }
    }
}
