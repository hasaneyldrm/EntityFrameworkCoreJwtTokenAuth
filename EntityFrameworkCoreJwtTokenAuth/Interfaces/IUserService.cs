using System.Collections.Generic;
using EntityFrameworkCoreJwtTokenAuth.Models.Auth;

namespace EntityFrameworkCoreJwtTokenAuth.Interfaces
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        IEnumerable<User> GetAll();
        User GetById(int id);
        string GetCompany(int companyId);
    }
}
