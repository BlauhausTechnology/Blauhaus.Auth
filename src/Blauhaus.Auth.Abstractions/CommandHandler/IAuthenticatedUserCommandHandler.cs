using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Common.Domain.CommandHandlers;

namespace Blauhaus.Auth.Abstractions.CommandHandler
{
    public interface IAuthenticatedUserCommandHandler<TPayload, TCommand> 
        : IAuthenticatedCommandHandler<TPayload, TCommand, IAuthenticatedUser>
        where TCommand : notnull
    {
        
    }
}