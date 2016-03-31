using System;
using System.Collections.Generic;
using System.Linq;
using Core.DomainModels.Opportunity;

namespace Core.DomainServices.Repositories
{
    public interface IOpportunityRepository
    {
        PaginationEnvelope<Opportunity> GetAll(IEnumerable<string> orderBy, int? page = null, int? pageSize = null);
        IEnumerable<Opportunity> GetAll();
        Opportunity GetById(int id);
        Opportunity Create(Opportunity model, string userName);
        Opportunity Update(int id, Opportunity model);
        void Delete(int id);
    }
}
