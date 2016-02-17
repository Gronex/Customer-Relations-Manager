using System.Linq;
using Core.DomainModels.Customers;

namespace Core.DomainServices.Repositories
{
    public interface ICompanyRepository
    {
        IQueryable<Company> GetAll();
        Company GetById(int id);
        Company Create(Company model);
        Company Update(int id, Company model);
        void Delete(int id);
    }
}
