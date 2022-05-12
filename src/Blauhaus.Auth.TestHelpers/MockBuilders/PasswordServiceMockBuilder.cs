using System;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Auth.TestHelpers.MockBuilders;

public class PasswordServiceMockBuilder : BaseMockBuilder<PasswordServiceMockBuilder, IPasswordService>
{
    public PasswordServiceMockBuilder Where_GenerateHashedPassword_succeeds(string value)
    {
        Mock.Setup(x => x.CreateHashedPassword(It.IsAny<string>())).Returns(Response.Success(value));
        return this;
    }
    public PasswordServiceMockBuilder Where_GenerateHashedPassword_succeeds(string value, string initialValue)
    {
        Mock.Setup(x => x.CreateHashedPassword(initialValue)).Returns(Response.Success(value));
        return this;
    }

    public Error Where_GenerateHashedPassword_fails(Error? error)
    {
        error ??= Error.Create(Guid.NewGuid().ToString());
        Mock.Setup(x => x.CreateHashedPassword(It.IsAny<string>())).Returns(Response.Failure<string>(error.Value));
        return error.Value;
    }
}