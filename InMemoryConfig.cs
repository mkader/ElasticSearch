using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace IS4.Web
{
    public class InMemoryConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources() =>
           new List<IdentityResource>
           {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email(),
                    new IdentityResource("customScope", new string[] { JwtClaimTypes.WebSite }),
                    new IdentityResource("userName", new string[] { JwtClaimTypes.Id }),
                    new IdentityResource("usergroups", new string[] { "groups" }),
           };
        private static TestUser AddUser(string userName, string fanilyName, string group) =>
            new TestUser
            {
                SubjectId = Guid.NewGuid().ToString(),
                Username = userName,
                Password = userName,
                Claims =
                    {
                        new Claim(JwtClaimTypes.Name, userName+" "+ fanilyName),
                        new Claim(JwtClaimTypes.GivenName, userName),
                        new Claim(JwtClaimTypes.FamilyName, fanilyName),
                        new Claim(JwtClaimTypes.Email, userName+"@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim("groups", JsonSerializer.Serialize(new List<string>(){group}), IdentityServerConstants.ClaimValueTypes.Json)
                    }
            };
        public static List<TestUser> Users()=>
            new List<TestUser>
            {
                AddUser("user1", "Flight", "flight"),
                AddUser("user2", "Ecommerce", "ecom"),
                AddUser("user3", "Log", "log"),
                AddUser("user12", "FlightEcommerce", "flight_ecom")
            };
        public static IEnumerable<ApiScope> ApiScopes() =>
            new List<ApiScope> { 
                new ApiScope("weatherapi", "Weather API"),
                new ApiScope("kibanaapi", "Kibana API")
            };
        public static IEnumerable<ApiResource> ApiResources() =>
            new List<ApiResource>
            {
                new ApiResource("weatherapi", "Weather API")
                {
                    Scopes = { "weatherapi" }
                }
            };
        private static Client AddCodeClient(string clientId, string redirectURL, string postLogoutRedirectURL) =>
           new Client
           {
               ClientId = clientId,
               ClientSecrets = { ClientSecret },

               AllowedGrantTypes = GrantTypes.Code,
               AllowOfflineAccess = true,

               //Kibana callback redirect url
               RedirectUris = { redirectURL },

               RequirePkce = false,

               //Kibana log out url
               PostLogoutRedirectUris = { postLogoutRedirectURL },

               AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "customScope",
                    "userName",
                    "usergroups",
                    "kibanaapi"
                }
           };
        private static Client AddCredentialsClient(string clientId) =>
           new Client
           {
               ClientId = clientId,
               ClientSecrets = { ClientSecret },

               AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
               AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId, "weatherapi"
                    }
           };
        public static IEnumerable<Client> Clients() =>
           new List<Client>
           {
                AddCredentialsClient("api_credentials"),
                AddCodeClient("kibana_local", "http://localhost:5601/api/security/oidc/callback", "http://localhost:5601/logged_out"),
                AddCodeClient("kibana_server", "https://roi-is4-poc.kb.us-central1.gcp.cloud.es.io:9243/api/security/oidc/callback",
                            "https://roi-is4-poc.kb.us-central1.gcp.cloud.es.io:9243/logged_out")
           };
        public static Secret ClientSecret =>
            new Secret("client_secret_123".Sha256());
    }
}
