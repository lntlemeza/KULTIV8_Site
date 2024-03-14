using KULTIV8.Data.DTO;
using KULTIV8.Models;

namespace KULTIV8.Repositories.Abstract
{
    /* Services for Authentication */
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(LoginModel loginModel);
        Task<Status> RegistrationAsync(RegistrationModel registrationModel);
        Task LogoutAsync();
    }
}
