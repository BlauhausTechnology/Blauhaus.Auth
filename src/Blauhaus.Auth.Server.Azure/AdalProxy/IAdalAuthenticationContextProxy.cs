using System.Threading.Tasks;

namespace Blauhaus.Auth.Server.Azure.AdalProxy
{
    public interface IAdalAuthenticationContextProxy
    {
        Task<string> AcquireAccessTokenAsync();
        string GetGraphEndpointForResource(string resource);
    }
}