using API.Enums;

namespace API.Models.Base
{
    public class Settings
    {
        public string BaseURL { get; set; }
        public BaseClient Client { get; set; }
        public DB DataAccess { get; set; }
        public Service IDP { get; set; }
    }

    public class BaseClient
    {
        public bool Enable { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string ActivationKey { get; set; }
    }

    public class DB
    {
        public DBProvider Provider { get; set; }
        public string ConnectionString { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Service
    {
        public string BaseURL { get; set; }
        public List<string> AllowedGrantTypes { get; set; }
        public List<string> AllowedScopes { get; set; }
        public Token ShortLivedToken { get; set; }
        public Token LongLivedToken { get; set; }
    }
    public class Token
    {
        public bool Enable { get; set; }
        public int LifeSpan { get; set; }
    }
}
