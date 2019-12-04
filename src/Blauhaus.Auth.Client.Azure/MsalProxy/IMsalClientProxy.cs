using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Blauhaus.Auth.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] 
namespace Blauhaus.Auth.Client.Azure.MsalProxy
{
    internal interface IMsalClientProxy
    {
        Task<MsalClientResult> AuthenticateSilentlyAsync(CancellationToken cancellationToken);
        Task<MsalClientResult> LoginAsync(object clientParentView, CancellationToken cancellationToken);
        Task<MsalClientResult> ResetPasswordAsync(object clientParentView, CancellationToken cancellationToken);
        Task LogoutAsync();
    }
}