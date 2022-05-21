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

public class IsMatchTests : BasePasswordServiceTest
{
    public override void Setup()
    {
        base.Setup();
        Services.Configure<PasswordOptions>(options => options.RequiredLenth = 5);
    }

    [Test]
    public void IF_password_is_match_SHOULD_return_true()
    {
        //Arrange
        var hashedPassword = Sut.CreateHashedPassword("myPassword!");

        //Act
        var result = Sut.IsMatch("myPassword!", hashedPassword.Value);

        //Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IF_password_is_not_match_SHOULD_return_false()
    {
        //Arrange
        var hashedPassword = Sut.CreateHashedPassword("myPassword!");

        //Act
        var result = Sut.IsMatch("myPassword", hashedPassword.Value);

        //Assert
        Assert.That(result, Is.False);
    } 
}