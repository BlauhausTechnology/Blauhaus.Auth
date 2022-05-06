using System;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Server.Ioc;
using Blauhaus.Auth.Server.Services;
using Blauhaus.Auth.Tests.UnitTests.Base;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Blauhaus.Auth.Tests.UnitTests.Server.PasswordServiceTests.Base;

public class BasePasswordServiceTest : BaseAuthTest<PasswordService>
{
    public override void Setup()
    {
        base.Setup();
    }
}