using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Common.Domain.CommandHandlers;
using Blauhaus.Common.Domain.CommandHandlers.Server;

namespace Blauhaus.Auth.Abstractions.CommandHandler
{
    public interface IAuthenticatedUserCommandHandler<TPayload, TCommand> 
        : ICommandServerHandler<TPayload, TCommand, IAuthenticatedUser>
        where TCommand : notnull
    {
        
    }
}