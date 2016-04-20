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
        RefreshToken GetRefreshToken(string id);
        RefreshToken AddRefreshToken(RefreshToken token);
        Client GetClient(string id);
        void RemoveRefreshTokens(string subject);
        void RemoveRefreshToken(RefreshToken token);
    }
}
