using System.Collections.Generic;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Newtonsoft.Json;

namespace Bromine.RestServiceWrapper.Models
{
    public class CreateOidcTokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty(PropertyName = "id_token")]
        public string IdToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is CreateOidcTokenResponse)) return false;
            var source = this;
            var compare = (CreateOidcTokenResponse)obj;
            return source.AccessToken.EqualsIgnoreCase(compare.AccessToken) &&
                   source.RefreshToken.EqualsIgnoreCase(compare.RefreshToken) &&
                   source.IdToken.EqualsIgnoreCase(compare.IdToken) &&
                   source.TokenType.EqualsIgnoreCase(compare.TokenType) &&
                   source.ExpiresIn == compare.ExpiresIn;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class CreateOidcTokenResponseComparer : IEqualityComparer<CreateOidcTokenResponse>
    {
        public bool Equals(CreateOidcTokenResponse source, CreateOidcTokenResponse compare)
        {
            if (source == null && compare == null) return true;
            if (source == null || compare == null) return false;

            if (source.Equals(compare)) return true;
            var logger = LogHelper.GetLogger();
            logger.Info($"Expected '{source}' \nActual '{compare}'");
            return false;
        }

        public int GetHashCode(CreateOidcTokenResponse obj)
        {
            return obj.AccessToken.GetHashCode() + obj.AccessToken.GetHashCode();
        }
    }
}
