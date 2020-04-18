using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Common.Domain.CommandHandlers;

namespace Blauhaus.Auth.Abstractions.CommandHandler
{
    public interface IAuthenticatedUserCommandHandler<TPayload, in TCommand> 
        : IAuthenticatedCommandHandler<TPayload, TCommand, IAuthenticatedUser>
    {
        
    }
}