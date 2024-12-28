using IConnections;
using System.Collections;

namespace Connections.MsSql
{

    public class AppMsSqlConfiguration : IAppDBConfiguration
    {
        public string ConnectionString { get; set; } = "";

        public int ConnectionTimeout { get; set; } = 0;

        public string Database { get; set; } = "";

        public override string ToString()
        {
            return $"{ConnectionString} | ConnectionTimeout: {ConnectionTimeout}";
        }
    }
}
