using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EntityFrameworkCoreJwtTokenAuth.Models.Auth
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
