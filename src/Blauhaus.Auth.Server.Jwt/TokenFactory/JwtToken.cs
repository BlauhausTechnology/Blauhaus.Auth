namespace Blauhaus.Auth.Server.Jwt.TokenFactory
{
    public class JwtToken
    {
        public JwtToken(string token, string username, TimeSpan validity, string refreshToken, Guid id, string emailId, Guid guidId, DateTime expiredTime)
        {
            Token = token;
            Username = username;
            Validity = validity;
            RefreshToken = refreshToken;
            Id = id;
            EmailId = emailId;
            GuidId = guidId;
            ExpiredTime = expiredTime;
        }

        public string Token { get; }

        public string Username { get; }

        public TimeSpan Validity { get; }

        public string RefreshToken { get; }

        public Guid Id { get; }

        public string EmailId { get; }

        public Guid GuidId { get; }

        public DateTime ExpiredTime { get; }
    }
}