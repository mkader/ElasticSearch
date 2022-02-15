namespace ElasticSearch.API.Models
{
    public class ModifyUser : UserExtension
    {
        public string? FullName { get; set; }

    }

    public class User : UserExtension
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }

    }

    public class UserExtension : ModifyPassword
    {
        public bool Enabled { get; set; }
        public string[] Roles { get; set; }
        public string? Email { get; set; }
       
    }

    public class ModifyPassword
    {
        public string? Password { get; set; }
    }

    public class ElasticUser : UserExtension
    {
        public string? Full_Name { get; set; }
        public Dictionary<string, int> Metadata { get; set; }

    }

    
}
