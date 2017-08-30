namespace Shriek.EventStorage.MongoDB
{
    public class MongoDBOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public bool IsSSL { get; set; }
    }
}