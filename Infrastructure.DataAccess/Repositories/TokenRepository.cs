using System;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModels.Authorization;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace Infrastructure.DataAccess.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IApplicationContext _context;

        public TokenRepository(IApplicationContext context)
        {
            _context = context;
        }

        public RefreshToken GetRefreshToken(string id)
        {
            return _context.RefreshTokens.Find(id);
        }

        public RefreshToken AddRefreshToken(RefreshToken token)
        {
            var existing = _context.RefreshTokens
                .Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId);
            _context.RefreshTokens.RemoveRange(existing);
            return _context.RefreshTokens.Add(token);
        }

        public Client GetClient(string id)
        {
            return _context.Clients.Find(id);
        }

        public void RemoveRefreshTokens(string subject)
        {
            var tokens = _context.RefreshTokens.Where(r => r.Subject == subject);
            _context.RefreshTokens.RemoveRange(tokens);
        }
        public void RemoveRefreshToken(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);
        }
    }
}
