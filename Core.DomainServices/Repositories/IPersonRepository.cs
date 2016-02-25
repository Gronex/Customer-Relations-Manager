using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Customers;

namespace Core.DomainServices.Repositories
{
    public interface IPersonRepository
    {
        /// <summary>
        /// Gets all persons in the db
        /// </summary>
        /// <returns></returns>
        IEnumerable<Person> GetAll();
        /// <summary>
        /// Gets a single person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Person GetById(int id);
        /// <summary>
        /// Creates a new person
        /// </summary>
        /// <param name="model">The data of the person to create</param>
        /// <returns></returns>
        Person Create(Person model);
        /// <summary>
        /// Updates the base info of a person
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Person Update(int id, Person model);

        Person AddToCompany(int id, int companyId);

        /// <summary>
        /// Removes the person from a company
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyId">Company to be removed from, if null will remove from all of them</param>
        void Unassign(int id, int? companyId);
    }
}
