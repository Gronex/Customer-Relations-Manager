using System;
using System.Collections.Generic;
using Core.DomainModels.Users;
using UserRole = Core.DomainServices.DTOs.UserRole;

namespace Core.DomainServices.Repositories
{
    public interface IUserRepository
    {
        PaginationEnvelope<UserRole> GetAll(string orderBy, int? page, int? pageSize);
        UserRole GetById(string id);
    }
}
