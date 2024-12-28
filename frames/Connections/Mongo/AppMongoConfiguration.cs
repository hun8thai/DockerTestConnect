using IConnections;

namespace Connections.Mongo
{

    public class AppMongoConfiguration : IAppDBConfiguration
    {
        public string ConnectionString { get; set; } = "";

        public int ConnectionTimeout { get; set; } = 0;

        public string Database { get; set; } = "";

        public string CollectionName { get; set; } = "";

        public override string ToString()
        {
            return $"{ConnectionString} | Database: {Database} | CollectionName: {CollectionName}";
        }
    }
}
