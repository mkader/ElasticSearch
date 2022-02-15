using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using System.Text;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticController : ControllerBase
    {
        //private readonly ElasticClient _esClient;
        private readonly HttpClient _httpClient;
        private readonly string _jsonAccept = "application/json";

        public ElasticController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:9200");
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //_esClient = ESClient();
        }

        #region "private"
        //private static ElasticClient ESClient()
        //{
        //    ConnectionSettings connectionSettings;
        //    StaticConnectionPool connectionPool;

        //    //Connection string for Elasticsearch
        //    /*connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200/")); //local PC
        //    elasticClient = new ElasticClient(connectionSettings);*/

        //    //Multiple node for fail over (cluster addresses)
        //    var nodes = new Uri[]
        //    {
        //        new Uri("http://localhost:9200/"),
        //        //new Uri("Add server 2 address")   //Add cluster addresses here
        //        //new Uri("Add server 3 address")
        //    };

        //    connectionPool = new StaticConnectionPool(nodes);
        //    connectionSettings = new ConnectionSettings(connectionPool)
        //                                .BasicAuthentication("elastic", "es123!");
        //    return new ElasticClient(connectionSettings);
        //}

        //private static string Serialize(object model)
        //{
        //    return JsonConvert.SerializeObject(model);
        //}

        private static T Deserialize<T>(string json)
        {
            if (typeof(T) == typeof(T))
                return (T)Convert.ChangeType(json, typeof(T));

            if (string.Equals(json, "[]") && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                return default;

            return JsonConvert.DeserializeObject<T>(json);
        }
       
        private HttpRequestMessage CreateHttpRequestMessage(Uri uri, System.Net.Http.HttpMethod httpMethod, object? model = null)
        {
            var request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri= uri,
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy(),
                WriteIndented = true
            };

            if (model != null)
            {
                var jsonData = System.Text.Json.JsonSerializer.Serialize(model, options);
                request.Content = new StringContent(jsonData, Encoding.UTF8, _jsonAccept);
            }

            var byteArray = Encoding.ASCII.GetBytes("elastic:es123!");
            string encodeString = Convert.ToBase64String(byteArray);

            request.Headers.Add("Authorization","Basic " +encodeString);
          
            return request;
        }

        //private static (HttpStatusCode, string) ParseProblemDetails(string json)
        //{
        //    var error = Deserialize<ProblemDetails>(json);

        //    var errorCode = error.Status != null ? (HttpStatusCode)error.Status : HttpStatusCode.BadRequest;

        //    var errorMessage = error.Title;

        //    if (!string.IsNullOrWhiteSpace(error.Detail)) 
        //        errorMessage += $" {error.Detail}";

        //    if (error.Extensions.ContainsKey("errors"))
        //            errorMessage += " " + JsonConvert.SerializeObject(error.Extensions["errors"]); 

        //    return (errorCode, errorMessage);
        //}
      
        private async Task<T> SendAsync<T>(Uri uri, System.Net.Http.HttpMethod httpMethod, object? model = null)
        {
            var request = CreateHttpRequestMessage(uri, httpMethod, model);

            var post = await _httpClient.SendAsync(request);

            if (post != null)
            {
                var json = await post.Content.ReadAsStringAsync();

                return Deserialize<T>(json);
            }

            return Deserialize<T>(string.Empty);
        }

        private async Task<T> PostAsync<T>(Uri uri, object? model = null)
        {
            return await SendAsync<T>(uri, System.Net.Http.HttpMethod.Post, model);
        }

        private async Task<T> PutAsync<T>(Uri uri, object? model =  null)
        {
            return await SendAsync<T>(uri, System.Net.Http.HttpMethod.Put, model);
        }

        private async Task<T> DeleteAsync<T>(Uri uri)
        {
            return await SendAsync<T>(uri, System.Net.Http.HttpMethod.Delete, null);
        }

        private async Task<T> GetAsync<T>(Uri uri)
        {
            return await SendAsync<T>(uri, System.Net.Http.HttpMethod.Get, null);
        }
       
        #endregion "private"

        #region "User"

        [HttpPost("User")]
        public async Task<string> CreateUser([FromBody] Models.User user)
        {
            var modelUser = new Models.ElasticUser()
            {
                Email = user.Email,
                Enabled = user.Enabled,
                Full_Name = user.FullName,
                Metadata = new Dictionary<string, int>() { { "intelligence", 7 } },
                Password = user.Password,
                Roles = user.Roles
            };

            return await PostAsync<string>(new Uri("/_security/user/" + user.UserName, UriKind.Relative), modelUser);
        }

        [HttpPost("User/ChangePassword/{username}/{password}")]
        public async Task<string> UserChangePassword(string username, string password)
        {
            var pwd = new Models.ModifyPassword()
            {
                Password = password,
            };

            return await PostAsync<string>(new Uri("/_security/user/" + username +"/_password", UriKind.Relative), pwd);
        }

        [HttpPut("User/{username}")]
        public async Task<string> ModifyUser(string username, [FromBody] Models.ModifyUser user)
        {
            var modelUser = new Models.ElasticUser()
            {
                Email = user.Email,
                Enabled = user.Enabled,
                Full_Name = user.FullName,
                Metadata = new Dictionary<string, int>() { { "intelligence", 7 } },
                Password = user.Password,
                Roles = user.Roles
            };

            return await PutAsync<string>(new Uri("/_security/user/" + username, UriKind.Relative), modelUser);
        }

        [HttpPut("User/Disable/{username}")]
        public async Task<string> DisableUser(string username)
        {
            return await PutAsync<string>(new Uri("/_security/user/" + username+ "/_disable", UriKind.Relative), null);
        }

        [HttpPut("User/Enable/{username}")]
        public async Task<string> EnableUser(string username)
        {
            return await PutAsync<string>(new Uri("/_security/user/" + username + "/_enable", UriKind.Relative), null);
        }

        [HttpGet("User/{username}")]
        public async Task<string> GetUser(string username)
        {
            return await GetAsync<string>(new Uri("/_security/user/" + username, UriKind.Relative));
        }

        [HttpDelete("User/{username}")]
        public async Task<string> DeleteUser(string username)
        {
            return await DeleteAsync<string>(new Uri("/_security/user/" + username, UriKind.Relative));
        }

        #endregion "User"

        #region "Role"

        [HttpPost("Role")]
        public async Task<string> CreateRole(string name, [FromBody] Models.Role role)
        {
            return await PostAsync<string>(new Uri("/_security/role/" + name, UriKind.Relative), role);
        }

        [HttpPut("Role/{name}")]
        public async Task<string> ModifyRole(string name, [FromBody] Models.Role role)
        {
            return await PutAsync<string>(new Uri("/_security/role/" + name, UriKind.Relative), role);
        }

        [HttpGet("Role/{name}")]
        public async Task<string> GetRole(string name)
        {
            return await GetAsync<string>(new Uri("/_security/role/" + name, UriKind.Relative));
        }

        [HttpDelete("Role/{name}")]
        public async Task<string> DeleteRole(string name)
        {
            return await DeleteAsync<string>(new Uri("/_security/role/" + name, UriKind.Relative));
        }

        #endregion "Role"

        #region "Role Mapping"

        [HttpPost("RoleMapping")]
        public async Task<string> CreateRoleMapping(string name, [FromBody] Models.RoleMapping rolemapping)
        {
            return await PostAsync<string>(new Uri("/_security/role_mapping/" + name, UriKind.Relative), rolemapping);
        }

        [HttpPut("RoleMapping/{name}")]
        public async Task<string> ModifyRoleMapping(string name, [FromBody] Models.RoleMapping rolemapping)
        {
            return await PutAsync<string>(new Uri("/_security/role_mapping/" + name, UriKind.Relative), rolemapping);
        }

        [HttpGet("RoleMapping/{name}")]
        public async Task<string> GetRoleMapping(string name)
        {
            return await GetAsync<string>(new Uri("/_security/role_mapping/" + name, UriKind.Relative));
        }

        [HttpDelete("RoleMapping/{name}")]
        public async Task<string> DeleteRoleMapping(string name)
        {
            return await DeleteAsync<string>(new Uri("/_security/role_mapping/" + name, UriKind.Relative));
        }

        #endregion "Role Mapping"


        #region "Privilege"

        [HttpGet("Privilege/{name}")]
        public async Task<string> GetPrivilege(string name)
        {
            return await GetAsync<string>(new Uri("/_security/privilege/" + name, UriKind.Relative));
        }

        #endregion "Privilege"

    }

    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
            name.ToLower();
    }

}
