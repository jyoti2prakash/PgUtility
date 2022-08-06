using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PGUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGUtility.Repository
{
    /// <summary>
    /// This class use to connect and connect PG Database
    /// </summary>
    public static class PgDbRepository
    {
        public static string ConnectionString { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        static PgDbRepository()
        {

            ConnectionString = Program.Configuration.GetConnectionString("PGConnectionString");

        }

        /// <summary>
        /// Set Pg connection
        /// </summary>
        /// <param name="configuration"></param>
        public static void SetPgConnection(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("PGConnectionString");
        }

        /// <summary>
        /// Set pg connection with connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public static void SetPgConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }



        #region ExecuteScalar

        /// <summary>
        /// Execute non query async
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonQueryAsync(string commandText, List<DbParameter> parameters)
        {
            int result = 0;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (DbParameter item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }
                    await connection.OpenAsync();
                    try
                    {
                        result = await cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Execute non query async
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandType, List<DbParameter> parameters)
        {
            int result = 0;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = commandText;
                    cmd.CommandType = commandType;
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (DbParameter item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }
                    await connection.OpenAsync();
                    try
                    {
                        result = await cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result;
        }

        #endregion
        #region ExecuteScalar

        /// <summary>
        /// Execute scalar async
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<Object> ExecuteScalarAsync(string commandText, List<DbParameter> parameters)
        {
            object result = null;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (DbParameter item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }
                    await connection.OpenAsync();
                    try
                    {
                        result = await cmd.ExecuteScalarAsync();
                        cmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  Execute scalar async
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<Object> ExecuteScalarAsync(string commandText, CommandType commandType, List<DbParameter> parameters)
        {
            object result = null;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = commandText;
                    cmd.CommandType = commandType;
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (DbParameter item in parameters)
                        {
                            cmd.Parameters.Add(item);
                        }
                    }
                    await connection.OpenAsync();
                    try
                    {
                        result = await cmd.ExecuteScalarAsync();
                        cmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result;
        }

        #endregion

        #region Execute ExecuteFirstOrDefaultAsync
        /// <summary>
        /// Execute first or default async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<T> ExecuteFirstOrDefaultAsync<T>(string sql, List<DbParameter> parameters)
        {
            T result;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = ConnectionString;

                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.StoredProcedure;
                        var dynamicParameters = new DynamicParameters();
                        foreach (var item in parameters)
                        {
                            dynamicParameters.Add(item.ParameterName, item.Value);
                        }
                        CommandDefinition commandd = new CommandDefinition(sql, dynamicParameters, null, null, CommandType.StoredProcedure);
                        result = await connection.QueryFirstOrDefaultAsync<T>(commandd);
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Execute first or default async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<T> ExecuteFirstOrDefaultAsync<T>(string sql, CommandType commandType, List<DbParameter> parameters)
        {
            T result;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = commandType;
                        var dynamicParameters = new DynamicParameters();
                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (var item in parameters)
                            {
                                dynamicParameters.Add(item.ParameterName, item.Value);
                            }
                        }
                        CommandDefinition commandd = new CommandDefinition(sql, dynamicParameters, null, null, commandType);
                        result = await connection.QueryFirstOrDefaultAsync<T>(commandd);
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result;
        }

        #endregion

        #region Execute List

        /// <summary>
        /// Execute list async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<List<T>> ExecuteListAsync<T>(string sql, List<DbParameter> parameters)
        {
            IEnumerable<T> result = null;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    try
                    {
                        var dynamicParameters = new DynamicParameters();
                        if (parameters != null)
                        {
                            foreach (var item in parameters)
                            {
                                dynamicParameters.Add(item.ParameterName, item.Value);
                            }
                        }
                        await connection.OpenAsync();
                        result = await connection.QueryAsync<T>(sql, dynamicParameters, null, null, CommandType.StoredProcedure);
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result.AsList();
        }

        /// <summary>
        /// Execute list async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<List<T>> ExecuteListAsync<T>(string sql, CommandType commandType, List<DbParameter> parameters)
        {
            IEnumerable<T> result = null;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    try
                    {
                        var dynamicParameters = new DynamicParameters();
                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (var item in parameters)
                            {
                                dynamicParameters.Add(item.ParameterName, item.Value);
                            }
                        }
                        await connection.OpenAsync();
                        result = await connection.QueryAsync<T>(sql, dynamicParameters, null, null, commandType);
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result.AsList();
        }

        /// <summary>
        /// Execute list async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<List<T>> ExecuteListAsync<T>(string connectionstring, string sql, CommandType commandType, List<DbParameter> parameters)
        {
            IEnumerable<T> result = null;
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = connectionstring;
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    try
                    {
                        var dynamicParameters = new DynamicParameters();
                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (var item in parameters)
                            {
                                dynamicParameters.Add(item.ParameterName, item.Value);
                            }
                        }
                        await connection.OpenAsync();
                        result = await connection.QueryAsync<T>(sql, dynamicParameters, null, null, commandType);
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            return result.AsList();
        }

        public static List<T> ExecuteList<T>(IDbConnection connection, string sql)
        {
            IEnumerable<T> result = null;

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
            return result.AsList();
        }



        #region  #region BuilderTool Changes

        /// <summary>
        /// Execute Pocecures
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<DataTable> ExecuteProcedures(string commandText, List<DbParameter> parameters)
        {
            DataTable dt = new DataTable();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                try
                {

                    connection.ConnectionString = ConnectionString;
                    connection.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = commandText;

                    #region Loop Through All Params
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                    #endregion

                    cmd.CommandType = CommandType.StoredProcedure;
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        dt = new DataTable("DataTable1");
                        //Load DataReader into the DataTable.
                        dt.Load(sdr);
                    }
                    cmd.Dispose();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    await connection.CloseAsync();
                }
                return dt;
            }

        }
        #endregion

        #endregion

        /// <summary>
        /// Bulk upload directly to table 
        /// Example for create PostgreSQLCopyHelper<TEntity> object
        /// var postgreSQLCopyHelper = new PostgreSQLCopyHelper<Account>("\"SchemaName\"", "\"TableName\"")
        /// .MapVarchar("col", x => x.AccountName.ToString())
        /// .MapInteger("AccountId", x => x.AccountId))
        /// .MapDate("CreatedDate", x => x.CreatedDate));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="postgreSQLCopyHelper"></param>
        /// <param name="list"></param>
        /// <returns>
        /// ulong value not of effected row
        /// </returns>
        //public static async Task<ulong> BulkUpload<TEntity>(PostgreSQLCopyHelper<TEntity> postgreSQLCopyHelper, List<TEntity> list)
        //{
        //    ulong result = 0;
        //    using (var connection = new NpgsqlConnection(ConnectionString))
        //    {
        //        try
        //        {
        //            await connection.OpenAsync();
        //            result = await postgreSQLCopyHelper.SaveAllAsync(connection, list);
        //        }
        //        catch
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            await connection.CloseAsync();
        //        }

        //    }
        //    return result;
        //}



    }
}
