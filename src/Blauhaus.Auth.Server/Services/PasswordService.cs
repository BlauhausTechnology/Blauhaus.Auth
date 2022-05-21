using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Server.Ioc;
using Blauhaus.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blauhaus.Auth.Server.Services;

public class PasswordService : IPasswordService
{
    private readonly IAnalyticsLogger<PasswordService> _logger;
    private readonly PasswordOptions _passwordOptions;

    private const string SpecialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
    private const string DefaultSalt = "a&6*_0b72a4a547548574be2e53e1821^^3";

    public PasswordService(
        IAnalyticsLogger<PasswordService> logger,
        IOptions<PasswordOptions> options)
    {
        _logger = logger;
        _passwordOptions = options.Value;
    }

     
    public Response<string> CreateHashedPassword(string password)
    {
        if (password.Length < _passwordOptions.RequiredLenth)
        {
            return _logger.LogErrorResponse<string>(PasswordError.TooShort(_passwordOptions.RequiredLenth));
        }

        if (_passwordOptions.RequiredSpecialCharacters != null)
        {
            var actualSpecialCharacters = CountSpecialCharacters(password);
            if (actualSpecialCharacters < _passwordOptions.RequiredSpecialCharacters)
            {
                return _logger.LogErrorResponse<string>(PasswordError.TooFewSpecialCharacters(_passwordOptions.RequiredSpecialCharacters.Value));
            }
        }

        if (_passwordOptions.Salt == null)
        {
            _logger.LogDebug("$No salt provided, using default");
        }
        
        var hashedPassword = GenerateHash(password);

        return Response.Success(hashedPassword);
    }

    public bool IsMatch(string plainTextPassword, string hashedPassword)
    {
        var hashOfInputPassword = GenerateHash(plainTextPassword);
        return string.Equals(hashOfInputPassword, hashedPassword);
    }

    private string GenerateHash(string password)
    {
        var salt = _passwordOptions.Salt ?? DefaultSalt;
        var key = string.Join(":", password, salt);
        using var hmac = HMAC.Create("HmacSHA256");
        hmac!.Key = Encoding.UTF8.GetBytes(salt);
        hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
        return Convert.ToBase64String(hmac.Hash!);
    }

    private static int CountSpecialCharacters(string input)
    {
        var count = 0;
        foreach (var item in SpecialChar)
        {
            if (input.Contains(item))
            {
                count++;
            }
        }

        return count;
    }
}