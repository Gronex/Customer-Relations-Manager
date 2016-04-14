using System;
using System.Collections.Generic;
using System.Linq;
using Core.DomainModels.Opportunity;
using Core.DomainServices.Filters;

namespace Core.DomainServices.Repositories
{
    public interface IOpportunityRepository
    {
        PaginationEnvelope<Opportunity> GetAll(PagedSearchFilter filter);
        IEnumerable<Opportunity> GetAll();
        Opportunity GetById(int id);
        Opportunity Create(Opportunity model, string userName);
        Opportunity Update(int id, Opportunity model);
        void Delete(int id);
    }
}
