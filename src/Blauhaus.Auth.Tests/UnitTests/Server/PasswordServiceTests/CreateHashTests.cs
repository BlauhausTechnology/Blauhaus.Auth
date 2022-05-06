using Blauhaus.Auth.Tests.UnitTests.Server.PasswordServiceTests.Base;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using Blauhaus.Auth.Abstractions.Errors;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using Blauhaus.Auth.Server.Ioc;

namespace Blauhaus.Auth.Tests.UnitTests.Server.PasswordServiceTests;

public class CreateHashTests : BasePasswordServiceTest
{

    [Test]
    public void IF_password_is_too_short_SHOULD_return_error()
    {
        //Arrange
        Services.Configure<PasswordOptions>(options => options.RequiredLenth = 12);

        //Act
        var result = Sut.CreateHash("121213131");

        //Assert
        MockLogger.VerifyLogErrorResponse(PasswordError.TooShort(3), result);
    }

    [Test]
    public void IF_password_has_too_few_special_characters_SHOULD_fail()
    {
        //Arrange
        Services.Configure<PasswordOptions>(options => options.RequiredSpecialCharacters = 2);

        //Act
        var result = Sut.CreateHash("12121313$");

        //Assert
        MockLogger.VerifyLogErrorResponse(PasswordError.TooFewSpecialCharacters(2), result);
    }

    [Test]
    public void IF_Salt_is_provided_SHOULD_use_it()
    {
        //Arrange
        Services.Configure<PasswordOptions>(options => options.Salt = "12345");

        //Act
        var result = Sut.CreateHash("12123413");

        //Assert
        var expectedHash = GenerateHash("12123413", "12345");
        Assert.That(result.Value, Is.EqualTo(expectedHash));
    }

    [Test]
    public void IF_Salt_is_not_provided_SHOULD_use_default()
    {
        //Arrange
        Services.Configure<PasswordOptions>(options => options.Salt = null);

        //Act
        var result = Sut.CreateHash("12123413");

        //Assert
        var expectedHash = GenerateHash("12123413", "a&6*_0b72a4a547548574be2e53e1821^^3");
        Assert.That(result.Value, Is.EqualTo(expectedHash));
    }

    private static string GenerateHash(string password, string salt)
    {
        var key = string.Join(":", password, salt);

        using var hmac = HMAC.Create("HmacSHA256");
        hmac!.Key = Encoding.UTF8.GetBytes(salt);
        hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
        return Convert.ToBase64String(hmac.Hash!);
    }
}