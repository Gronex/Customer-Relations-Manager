using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Authorization;

namespace Core.DomainServices.Repositories
{
    public interface ITokenRepository
    {
        Task<RefreshToken> GetRefreshTokenAsync(string id);
        RefreshToken AddRefreshToken(RefreshToken token);
        Task<Client> GetClient(string id);
    }
}
