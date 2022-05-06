namespace Blauhaus.Auth.Server.Ioc;

public class PasswordOptions
{
    public int RequiredLenth { get; set; } = 8;
    public string? Salt { get; set; }
    public int? RequiredSpecialCharacters { get; set; }
    public int? RequiredUpperCaseLetters { get; set; }
    public int? RequiredLowerCaseLetters { get; set; }
}