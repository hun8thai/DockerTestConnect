using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IConnections
{

    public enum AppMemDataType
    {
        RedisStringString
    }

    public interface IAppMemConnection
    {
        IAppMemComfiguration Configuration { get; set; }
        bool IsConnect();
        bool ContainsKey(string key);
        bool RemoveKey(string key);

        T ListTryRemoveItem<T>(string key, AppMemDataType dataType = AppMemDataType.RedisStringString);
        bool ListRemoveItem<T>(string key, AppMemDataType dataType = AppMemDataType.RedisStringString);
        T GetItem<T>(string key, AppMemDataType dataType = AppMemDataType.RedisStringString);
        List<T> GetAll<T>(List<string> keys, string prefix = "", AppMemDataType dataType = AppMemDataType.RedisStringString);
        bool InsertItem<T>(string key, T Listitem, AppMemDataType dataType = AppMemDataType.RedisStringString);
        bool UpdateItem<T>(string key, T Listitem, AppMemDataType dataType = AppMemDataType.RedisStringString);
        bool Test();
    }

    public interface IAppMemComfiguration
    {
        string IP { get; set; }
        string Port { get; set; }
        int DB { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
}
