using Newtonsoft.Json;

namespace EntityFrameworkCoreJwtTokenAuth.Models.Auth
{
    public class AuthenticateResponse
    {
        public string PublicId { get; set; }

        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            PublicId = Utilities.HashCode.GeneratePublicId(id: user.Id);
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
