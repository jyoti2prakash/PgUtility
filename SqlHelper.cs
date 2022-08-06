using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Dapper;
using System.Runtime.Remoting;
using Npgsql;

namespace PGUtility
{
    public static class SqlHelper
    {
        public static List<SqlConnection> Connections = new List<SqlConnection>();
        static SqlHelper()
        {
            DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
            Connections.Add(new SqlConnection
            {
                ConnectionName = "QA",
                ConnecgtionType = "Npgsql",
                Host = "postgresql-server2.drscloud.net",
                Database = "DroissysCloud_QA_V2",
                Username = "watuser",
                Password = "pGAdmin01#2020",
                SSLMode= SslMode.Disable
            });
            Connections.Add(new SqlConnection
            {
                ConnectionName = "Stage",
                ConnecgtionType = "Npgsql",
                Host = "c.stage-citus.postgres.database.azure.com",
                Database = "citus",
                Username = "citus",
                Password = "pG@dM!n01#2021",
                SSLMode= SslMode.Require
            });
        }


        private static DbProviderFactory GetDbProviderFactory(string type)
        {
            return DbProviderFactories.GetFactory(type);
        }
        public static IDbConnection GetConnection(string connectionName)
        {

            var connection = Connections.FirstOrDefault(x => x.ConnectionName == connectionName);
            DbProviderFactory dbProviderFactory = GetDbProviderFactory(connection.ConnecgtionType);
            IDbConnection instance = dbProviderFactory.CreateConnection();
            instance.ConnectionString = connection.ConnectionString;
            return instance;
        }
        public static IDbConnection GetConnection(SqlConnection connection)
        {
            DbProviderFactory dbProviderFactory = GetDbProviderFactory(connection.ConnecgtionType);
            IDbConnection instance = dbProviderFactory.CreateConnection();
            instance.ConnectionString = connection.ConnectionString;
            return instance;
        }
        public static IDbDataAdapter GetDataAdapter(SqlConnection connection)
        {
            DbProviderFactory dbProviderFactory = GetDbProviderFactory(connection.ConnecgtionType);
            IDbDataAdapter instance = dbProviderFactory.CreateDataAdapter();
            return instance;
        }
        public static List<T> ExecuteList<T>(SqlConnection sqlConnection, string sql)
        {
            IEnumerable<T> result = null;
            using (IDbConnection connection = GetConnection(sqlConnection))
            {
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.Connection = connection;
                    try
                    {
                        connection.Open();
                        result = connection.Query<T>(sql);
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return result.AsList();
        }

        public static T ExecuteFirstOrDefault<T>(SqlConnection sqlConnection, string sql)
        {
            T result;
            using (IDbConnection connection = GetConnection(sqlConnection))
            {
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.Connection = connection;
                    try
                    {
                        connection.Open();
                        result = connection.QueryFirstOrDefault<T>(sql);
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return result;
        }

        public static DataSet ExecuteDataSet(SqlConnection sqlConnection, string sql)
        {
            DataSet result = new DataSet();

            using (IDbConnection connection = GetConnection(sqlConnection))
            {

                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sql;
                    IDbDataAdapter dbDataAdapter = GetDataAdapter(sqlConnection);
                    dbDataAdapter.SelectCommand = cmd;

                    try
                    {
                        dbDataAdapter.Fill(result);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return result;
        }

    }

    public class SqlConnection
    {
        public string ConnectionName { get; set; }
        public string ConnecgtionType { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string CommandTimeout { get; set; }
        public string Timeout { get; set; }
        public string MaxPoolSize { get; set; }
        public SslMode SSLMode { get; set; }
        public string ConnectionString
        {
            get { return $"Host={Host};Username={Username};Password={Password};Database={Database}; Ssl Mode={SSLMode.ToString()}"; }

        }
        public SqlConnection Copy(string dataBase)
        {
            SqlConnection sqlConnection = (SqlConnection)this.MemberwiseClone();
            sqlConnection.Database = dataBase;
            return sqlConnection;
        }

        //"Host=postgresql-server.drscloud.net;Username=watuser;Password=pGAdmin01#2020;Database=DroissysCloud_QA_V2;CommandTimeout=30;Timeout=30;MaxPoolSize=1000;"
    }
}
