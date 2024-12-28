using IConnections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
//using Microsoft.Data.SqlClient;
namespace Connections.MsSql
{
    public class AppMSSQLConnection : IAppDBConnection
    {
        [Obsolete]
        private SqlConnection _conn { get; set; }
        private ILogger<AppMSSQLConnection> _logger { get; set; }

        public IAppDBConfiguration Configuration { get; set; }
        public bool IsConnected { get; set; }

        public AppMSSQLConnection(IConfiguration configuration, ILogger<AppMSSQLConnection> logger)
        {
            var connstring = configuration?.GetConnectionString("MsSql");
            if (string.IsNullOrEmpty(connstring))
                throw new AppDBHandleException("Connection string of SQL Server is empty");

            _logger = logger;
            _logger?.LogDebug("AppMSSQLConnection - constructure");
            Configuration = new AppMsSqlConfiguration
            {
                ConnectionString = connstring,
                Database = string.Empty,
                ConnectionTimeout = 0
            };
            _conn = new SqlConnection(connstring);
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public bool Open()
        {
            try
            {
                _conn = new SqlConnection(Configuration.ConnectionString);
                _conn.Open();
                return _conn?.State == ConnectionState.Open;
            }
            finally
            {
            }
        }

        public int BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public int BeginTransaction(int il)
        {
            throw new NotImplementedException();
        }


        public void Close()
        {
            try
            {
                if (_conn?.State == ConnectionState.Open)
                {
                    _conn.Close();
                }
            }
            finally
            {
            }
        }

        public int Execute(string query, Dictionary<string, object> parameters)
        {
            SqlConnection myconn = null;
            int reval = 0;

            try
            {
                myconn = new SqlConnection(Configuration.ConnectionString);
                myconn.Open();
                using (var sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = myconn;
                    sqlcmd.CommandText = query;
                    sqlcmd.Parameters.Clear();
                    if (parameters != null)
                        foreach (var parameter in parameters)
                        {
                            sqlcmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
                        }
                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    reval = sqlcmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (myconn != null)
                    myconn.Close();
            }

            return reval;
        }

        public int Execute(string query, object data)
        {
            SqlConnection myconn = null;
            int reval = 0;
            Dictionary<string, string> parameters = [];
            try
            {
                myconn = new SqlConnection(Configuration.ConnectionString);
                myconn.Open();
                using (var sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = myconn;
                    sqlcmd.CommandText = query;
                    sqlcmd.Parameters.Clear();
                    if (data != null)
                        parameters = _convertObjectToDictionary(data);
                        foreach (var parameter in parameters)
                        {
                            sqlcmd.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
                        }
                    sqlcmd.CommandType = CommandType.StoredProcedure;

                    reval = sqlcmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (myconn != null)
                    myconn.Close();
            }

            return reval;
        }

        public List<T> Get<T>(string query, Dictionary<string, object> parameters)
        {
            var data = _executeReader(query, parameters);
            if (data == null) return [];

            var realData = new List<T>();
            foreach (var item in data)
            {
                realData.Add(_createDynamicObject<T>(item));
            }
            
            return realData;
        }

        public T? GetFirst<T>(string query, Dictionary<string, object> parameters)
        {
            var data = _executeReader(query, parameters);
            if (data == null) return default;

            var firstDics = data.FirstOrDefault();
            if (firstDics == null) return default;

            var realData = _createDynamicObject<T>(firstDics);
            return realData;
        }

        public bool Test()
        {
            SqlConnection myconn = null;

            try
            {
                myconn = new SqlConnection(Configuration.ConnectionString);
                myconn.Open();

                using (var cmd = new SqlCommand("SELECT name, database_id, create_date FROM sys.databases;", myconn))
                {

                    using (var reader = cmd.ExecuteReader())
                    {
                        var cols = reader.GetColumnSchema()
                            .Select((r, index) => (index, r.ColumnName))
                            .ToList();

                        var res = new List<Dictionary<string, object?>>();

                        while (reader.Read())
                        {
                            res.Add(
                                cols.ToDictionary
                                (
                                    col => col.ColumnName,
                                    col => reader.IsDBNull(col.index) ? _getDefault(reader.GetFieldType(col.index)) : reader.GetValue(col.index)
                                )
                            );
                        }

                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (myconn != null)
                    myconn.Close();
            }
        }

        #region Private functions
        List<Dictionary<string, object>> _executeReader(string sqlText, Dictionary<string, object> parameters)
        {
            SqlConnection myconn = null;

            try
            {
                myconn = new SqlConnection(Configuration.ConnectionString);
                myconn.Open();

                using (var cmd = new SqlCommand(sqlText, myconn))
                {

                    using (var reader = cmd.ExecuteReader())
                    {
                        var cols = reader.GetColumnSchema()
                            .Select((r, index) => (index, r.ColumnName))
                            .ToList();

                        var res = new List<Dictionary<string, object?>>();

                        while (reader.Read())
                        {
                            res.Add(
                                cols.ToDictionary
                                (
                                    col => col.ColumnName,
                                    col => reader.IsDBNull(col.index) ? _getDefault(reader.GetFieldType(col.index)) : reader.GetValue(col.index)
                                )
                            );
                        }

                        return res;
                    }
                }
            }
            finally
            {
                if (myconn != null)
                    myconn.Close();
            }

        }

        //private List<string> _getObjectKeys<T>()
        //{
        //    var listPropertyNames = typeof(T).GetProperties()
        //                     .Select(x => x.Name + "").ToList();
        //    return listPropertyNames;
        //}

        T _createDynamicObject<T>(Dictionary<string, object> source)
        {
            var name = typeof(T).Name;
            var someObject = (T)Activator.CreateInstance(typeof(T));

            var someObjectType = someObject.GetType();

            string[] notStoreProperty = { "TeamLimitObject", "ClientLimit", "GroupLimitObject", "BranchLimitObject", "FirmLimitObject" };
            foreach (var item in source)
            {
                if (!notStoreProperty.Any(x => x == item.Key))
                {
                    try
                    {
                        someObjectType
                            .GetProperty(item.Key)?
                            .SetValue(someObject, _convertBasicType(someObjectType.GetProperty(item.Key).PropertyType, item.Value + ""), null);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
            }

            return someObject;
        }

        object? _convertBasicType(Type propertyType, string value)
        {
            var data = Nullable.GetUnderlyingType(propertyType);
            Type tmpType = propertyType;
            bool isNullable = false;
            if (data != null)
            {
                isNullable = true;
                tmpType = data;
            }

            switch (tmpType.FullName)
            {
                case "System.Int64":
                    return Int64.TryParse(value, out Int64 resultInt64) ? resultInt64 : (isNullable ? null : 0);
                case "System.Boolean":
                    return bool.TryParse(value, out bool resultbool) ? resultbool : (isNullable ? null : 0);
                case "System.Int32":
                    return int.TryParse(value, out int resultValue) ? resultValue : (isNullable ? null : 0);
                case "System.Int16":
                    return Int16.TryParse(value, out Int16 resultInt16Value) ? resultInt16Value : (isNullable ? null : 0);
                case "System.Decimal":
                    return decimal.TryParse(value, out decimal resultdecimal) ? resultdecimal : (isNullable ? null : 0);
                case "System.String":
                    return value;
                case "System.Char":
                    return char.TryParse(value, out char resultChar) ? resultChar : (isNullable ? null : 0);
                case "System.DateTime":
                    return DateTime.TryParse(value, out DateTime resultDate) ? resultDate : (isNullable ? null : 0);
                default:
                    return value;
            }
        }

        Dictionary<string, string> _convertObjectToDictionary(object data)
        {
            Dictionary<string, string> dictionary = [];
            if (data == null) return dictionary;
            dictionary = data.GetType().GetProperties()
                             .ToDictionary(x => x.Name, x => x.GetValue(data)?.ToString() ?? ""); // convert null string to empty string
            return dictionary;
        }

        object? _getDefault(Type type)
        {
            return DefaultVal.ContainsKey(type.FullName??"") ? DefaultVal[type.FullName??""] : null;
        }
        static Dictionary<string, object> DefaultVal = new Dictionary<string, object>
        {
            { "System.String", string.Empty },
            { "System.Int32", 0 },
            { "System.Int64", 0 },
            { "System.Decimal",  0.0 },
            { "System.Double",  0.0 },
            { "System.Money",  0.0 },
            { "System.DateTime", DateTime.MinValue },
        };
        #endregion Private functions
    }
}
