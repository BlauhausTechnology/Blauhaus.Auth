﻿
using Blauhaus.Errors;

namespace Blauhaus.Auth.Abstractions.Errors
{
    public static class AuthErrors
    {
        public static Error AuthenticationCancelled = Error.Create("User has cancelled the authentication process");
        public static Error NotAuthenticated = Error.Create("The current user has not been successfully authenticated");
        public static Error InvalidIdentity = Error.Create("The current user record is invalid");
        public static Error InvalidToken = Error.Create("The token provided could not be read");
        public static Error NotAuthorized = Error.Create("The current user is not allowed to perform this action");
    }
}