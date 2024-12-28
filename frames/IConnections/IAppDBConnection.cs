
namespace IConnections
{
    public interface IAppDBConnection
    {
        IAppDBConfiguration Configuration { get; set; }
        bool IsConnected { get; set; }
        int BeginTransaction();
        int BeginTransaction(int il);
        void ChangeDatabase(string databaseName);
        void Close();
        T? GetFirst<T>(string query, Dictionary<string, object> parameters);
        List<T> Get<T>(string query, Dictionary<string, object> parameters);
        int Execute(string query, Dictionary<string, object> parameters);
        int Execute(string query, object data);
        bool Open();
        bool Test();
    }

    //public interface IAppDbTransaction
    //{

    //}

    public interface IAppDBConfiguration
    {
        string ConnectionString { get; set; }
        int ConnectionTimeout { get; }
        string Database { get; }
    }

    public class AppDBHandleException : Exception
    {
        public AppDBHandleException() { }
        public AppDBHandleException(string messsage): base(messsage) { }
        public AppDBHandleException(string messsage, Exception ex): base(messsage, ex) { }
    }
}
