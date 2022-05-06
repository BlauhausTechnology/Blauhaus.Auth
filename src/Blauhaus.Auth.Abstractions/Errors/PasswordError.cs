using Blauhaus.Errors;

namespace Blauhaus.Auth.Abstractions.Errors;

public static class PasswordError
{
    public static Error TooShort(int minimum) => Error.Create($"Password must be at least {minimum} characters");
    public static Error TooFewSpecialCharacters(int minimum) => Error.Create($"Password must have at least {minimum} special characters");
}