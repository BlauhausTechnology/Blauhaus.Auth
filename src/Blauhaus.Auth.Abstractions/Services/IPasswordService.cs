using Blauhaus.Responses;

namespace Blauhaus.Auth.Abstractions.Services;

public interface IPasswordService
{
    Response<string> CreateHash(string password);
}