using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using customer_relations_manager.App_Start;
using Core.DomainModels.Authorization;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess;
using Microsoft.Owin.Security.Infrastructure;
using Ninject;

namespace customer_relations_manager.Providers
{
    public class ApplicationRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];
            if (string.IsNullOrEmpty(clientId))
                return;

            var refreshTokenId = Guid.NewGuid().ToString("N");
            
            var kernel = NinjectWebCommon.Kernel;
            var repo = (ITokenRepository)kernel.TryGet(typeof(ITokenRepository));
            var uow = (IUnitOfWork)kernel.TryGet(typeof(IUnitOfWork));
            var refreshTokenLifetime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken
            {
                Id = Helpers.GetHash(refreshTokenId),
                ClientId = clientId,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifetime))
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();
            repo.AddRefreshToken(token);
            await uow.SaveAsync();

            context.SetToken(refreshTokenId);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var hashedTokenId = Helpers.GetHash(context.Token);
            
            var kernel = NinjectWebCommon.Kernel;
            var repo = (ITokenRepository)kernel.TryGet(typeof(ITokenRepository));
            var uow = (IUnitOfWork)kernel.TryGet(typeof(IUnitOfWork));
            var refreshToken = repo.GetRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                repo.RemoveRefreshToken(refreshToken);
                await uow.SaveAsync();
            }
        }
    }
}
