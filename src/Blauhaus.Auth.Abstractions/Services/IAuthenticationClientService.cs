using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.Models;

namespace Blauhaus.Auth.Abstractions.Services
{
    public interface IAuthenticationClientService
    {
        Task<IUserAuthentication> TryGetLoggedInUserAsync(CancellationToken cancellationToken);
        Task<IUserAuthentication> LoginAsync(CancellationToken cancellationToken);
        Task<IUserAuthentication> RefreshAccessTokenAsync(CancellationToken cancellationToken);
        Task<IUserAuthentication> EditProfileAsync(CancellationToken cancellationToken);
        Task LogoutAsync();
        
    }
}