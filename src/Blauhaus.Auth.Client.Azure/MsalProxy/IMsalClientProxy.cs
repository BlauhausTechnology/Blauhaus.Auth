using System.Threading;
using System.Threading.Tasks;

namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    public interface IMsalClientProxy
    {
        Task<MsalClientResult> AuthenticateSilentlyAsync(CancellationToken cancellationToken, bool forceTokenRefresh = false);
        Task<MsalClientResult> LoginAsync(object clientParentView, bool useEmbeddedWwebView, CancellationToken cancellationToken);
        Task<MsalClientResult> ResetPasswordAsync(object clientParentView, bool useEmbeddedWwebView, CancellationToken cancellationToken);
        Task<MsalClientResult> EditProfileAsync(object clientParentView, CancellationToken cancellationToken);
        Task LogoutAsync();
    }
}