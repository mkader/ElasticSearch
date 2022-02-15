namespace ElasticSearch.API.Models
{
    public class All
    {
        public Dictionary<string, string> Field { get; set; }
    }
    public class Rules
    {
        public List<All> All { get; set; }
    }

    //{"mak_role_mapping":{"enabled":true,"roles":["MAK TEST"],"rules":{"all":[{"field":{"groups":"*"}}]},"metadata":{}}}

    public class RoleMapping
    {
        public bool Enabled { get; set; }
        public string[] Roles { get; set; }
        public Rules Rules { get; set; }
        public Dictionary<string, int> Metadata { get; set; }

    }
}
