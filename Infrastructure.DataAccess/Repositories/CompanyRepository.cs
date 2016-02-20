using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace Infrastructure.DataAccess.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<Company> _repo;

        public CompanyRepository(IApplicationContext context, IGenericRepository<Company> repo)
        {
            _context = context;
            _repo = repo;
        }

        public IQueryable<Company> GetAll()
        {
            return _repo.Get();
        }

        public Company GetById(int id)
        {
            return _repo.GetByKey(id);
        }

        public Company Create(Company model)
        {
            return _repo.Insert(model);
        }

        public Company Update(int id, Company model)
        {
            return _repo.Update(c =>
            {
                c.Address = model.Address;
                c.City = model.City;
                c.Country = model.Country;
                c.Name = model.Name;
                c.PhoneNumber = model.PhoneNumber;
                c.PostalCode = model.PostalCode;
                c.WebSite = model.WebSite;
            }, id);
        }

        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
        }
    }
}
