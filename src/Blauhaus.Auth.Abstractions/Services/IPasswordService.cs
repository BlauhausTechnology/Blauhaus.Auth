using Blauhaus.Responses;

namespace Blauhaus.Auth.Abstractions.Services;

public interface IPasswordService
{
    Response<string> CreateHashedPassword(string password);
}