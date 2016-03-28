using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.ViewSettings;

namespace Core.DomainServices.Repositories
{
    public interface IProductionViewSettingsRepository
    {
        IEnumerable<ProductionViewSettings> GetAll(string userName);
        ProductionViewSettings GetById(int id);
        ProductionViewSettings Create(ProductionViewSettings model, string userName);
        ProductionViewSettings Update(int id, ProductionViewSettings model, string userName);
        void Delete(int id, string userName);
    }
}
