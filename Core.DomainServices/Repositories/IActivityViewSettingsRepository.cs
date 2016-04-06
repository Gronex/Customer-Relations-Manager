using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.ViewSettings;

namespace Core.DomainServices.Repositories
{
    public interface IActivityViewSettingsRepository
    {
        IEnumerable<ActivityViewSettings> GetAll(string userName);
        ActivityViewSettings GetById(int id);
        ActivityViewSettings Create(ActivityViewSettings model, string userName);
        ActivityViewSettings Update(int id, ActivityViewSettings model, string userName);
        void Delete(int id, string userName);
    }
}
