using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Toprope.Infrastructure.Storage
{
    /// <summary>
    /// Represents a repository. This class cannot be inherited.
    /// </summary>
    public sealed class Repository : IDisposable
    {
        #region Properties

        private static object _lock = new object();
        private static RepositorySettings _settings = null;

        /// <summary>
        /// Gets the repository settings that are global for all repository instances.
        /// </summary>
        public static RepositorySettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    lock (_lock)
                    {
                        if (_settings == null)
                            _settings = new RepositorySettings();
                    }
                }

                return _settings;
            }
        }

        /// <summary>
        /// Gets or sets the database connection.
        /// </summary>
        private IDbConnection Connection { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of an object.
        /// </summary>
        public Repository()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Repository"].ConnectionString;
            
            Connection = new SqlConnection(connectionString);
            Connection.Open();
        }

        /// <summary>
        /// Executes the given SQL statement and returns a data reader that corresponds to matching result set.
        /// </summary>
        /// <param name="sql">SQL statement.</param>
        /// <returns>A data reader that corresponds to matching result set.</returns>
        public IDataReader Select(string sql)
        {
            return Select(sql, null);
        }

        /// <summary>
        /// Executes the given SQL statement and returns a data reader that corresponds to matching result set.
        /// </summary>
        /// <param name="sql">SQL statement.</param>
        /// <param name="parameters">Query parameters.</param>
        /// <returns>A data reader that corresponds to matching result set.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="sql">sql</paramref> represents an invalid SQL statement.</exception>
        /// <exception cref="System.ObjectDisposedException">Associated database connection is closed.</exception>
        public IDataReader Select(string sql, params Parameter[] parameters)
        {
            IDataReader ret = null;

            if (string.IsNullOrEmpty(sql))
                throw new ArgumentException("SQL statement is invalid.", "sql");
            else if (Connection == null)
                throw new ObjectDisposedException("Connection");
            else
            {
                using (IDbCommand command = Connection.CreateCommand())
                {
                    command.CommandText = sql;

                    if (parameters != null)
                    {
                        foreach (Parameter p in parameters)
                            command.Parameters.Add(new SqlParameter("@" + p.Name, p.Value));
                    }

                    ret = command.ExecuteReader();
                }
            }

            return ret;
        }

        /// <summary>
        /// Selects the given record by its Id.
        /// </summary>
        /// <typeparam name="T">Record type.</typeparam>
        /// <param name="id">Record Id.</param>
        /// <returns>Record.</returns>
        public T Select<T>(Guid id) where T : DbRecord, new()
        {
            return id != Guid.Empty ? Select<T>(SqlBuilder.Select(GetTableName<T>(), "[Id] = @id"),
                new Parameter("id", id)) : default(T);
        }

        /// <summary>
        /// Selects the given record by using the custom SQL statement.
        /// </summary>
        /// <typeparam name="T">Record type.</typeparam>
        /// <param name="sql">SQL statement.</param>
        /// <returns>Record.</returns>
        public T Select<T>(string sql) where T : DbRecord, new()
        {
            return Select<T>(sql, null);
        }

        /// <summary>
        /// Selects the given record by using the custom SQL statement.
        /// </summary>
        /// <typeparam name="T">Record type.</typeparam>
        /// <param name="sql">SQL statement.</param>
        /// <param name="parameters">SQL statement parameters.</param>
        /// <returns>Record.</returns>
        /// <exception cref="System.ObjectDisposedException">Associated database connection is closed.</exception>
        public T Select<T>(string sql, params Parameter[] parameters) where T : DbRecord, new()
        {
            T ret = default(T);

            if (!string.IsNullOrEmpty(sql))
            {
                if (Connection == null)
                    throw new ObjectDisposedException("Connection");
                else
                {
                    using (IDbCommand command = Connection.CreateCommand())
                    {
                        command.CommandText = sql;

                        if (parameters != null)
                        {
                            foreach (Parameter p in parameters)
                                command.Parameters.Add(new SqlParameter("@" + p.Name, p.Value));
                        }

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                ret = Read<T>(reader);
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Updates the given record (either inserts a new one or updates existing).
        /// </summary>
        /// <typeparam name="T">Record type.</typeparam>
        /// <param name="record">Record to update.</param>
        /// <returns>Updated record.</returns>
        public T Update<T>(T record) where T: DbRecord
        {
            return (T)Update((DbRecord)record);
        }

        /// <summary>
        /// Updates the given record (either inserts a new one or updates existing).
        /// </summary>
        /// <param name="record">Record to update.</param>
        /// <returns>Updated record.</returns>
        /// <exception cref="System.ObjectDisposedException">Associated database connection is closed.</exception>
        public DbRecord Update(DbRecord record)
        {
            DataRow row = null;
            DataTable tab = null;
            DbRecord ret = record;
            bool isNewRow = false;

            if (record != null)
            {
                if (Connection == null)
                    throw new ObjectDisposedException("Connection");
                else
                {
                    using (DataSet set = new DataSet())
                    {
                        using (IDbCommand command = Connection.CreateCommand())
                        {
                            command.CommandText = SqlBuilder.Select(GetTableName(record.GetType()), "[Id] = @id");
                            command.Parameters.Add(new SqlParameter("@id", record.Id));

                            using (SqlDataAdapter adapter = new SqlDataAdapter())
                            {
                                using (SqlCommandBuilder builder = new SqlCommandBuilder(adapter))
                                {
                                    adapter.SelectCommand = command as SqlCommand;

                                    adapter.InsertCommand = builder.GetInsertCommand();
                                    adapter.UpdateCommand = builder.GetUpdateCommand();

                                    adapter.Fill(set);

                                    if (set != null && set.Tables != null && set.Tables.Count > 0)
                                    {
                                        tab = set.Tables[0];

                                        if (tab.Rows.Count > 0)
                                            row = tab.Rows[0];
                                        else
                                        {
                                            row = tab.NewRow();
                                            isNewRow = true;
                                        }

                                        FillRow(row, record);

                                        if (record.Id == Guid.Empty)
                                            row["Id"] = record.Id = Guid.NewGuid();
                                        else
                                            row["Id"] = record.Id;

                                        if (isNewRow)
                                            tab.Rows.Add(row);

                                        adapter.Update(set);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Searches the repository.
        /// </summary>
        /// <typeparam name="T">Search result type.</typeparam>
        /// <param name="query">Search query.</param>
        /// <returns>Search result.</returns>
        public Search.SearchResult<T> Search<T>(Search.SearchQuery query) where T : DbRecord, new()
        {
            Search.SearchResult<T> ret = null;

            using (Search.Specialized.DbSearchEngine se = new Search.Specialized.DbSearchEngine(this))
                ret = se.Search<T>(query);

            return ret;
        }

        /// <summary>
        /// Fills table row with record property values.
        /// </summary>
        /// <param name="row">Table row.</param>
        /// <param name="record">Record instance.</param>
        private void FillRow(DataRow row, DbRecord record)
        {
            object value = null;
            ValueConverter converter = null;

            foreach (System.Reflection.PropertyInfo p in Settings.GetPropertyMappings(record.GetType()).Values)
            {
                if (p.CanRead && string.Compare(p.Name, "Id", StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    value = p.GetValue(record, null);

                    if(value == null)
                        value = DBNull.Value;
                    else
                    {
                        converter = Repository.Settings.GetMatchingConverters(p.PropertyType, null).FirstOrDefault();

                        if (converter != null)
                        {
                            value = converter.Convert(value);
                            if (value == null)
                                value = DBNull.Value;
                        }
                    }

                    if (value != null && p.PropertyType.IsEnum)
                        value = (int)value;

                    row[p.Name] = value;
                }
            }
        }

        /// <summary>
        /// Returns the name of the database table for a given record type.
        /// </summary>
        /// <typeparam name="T">Record type.</typeparam>
        /// <returns>The name of the database table for a given record type.</returns>
        private string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        /// <summary>
        /// Returns the name of the database table for a given record type.
        /// </summary>
        /// <param name="t">Record type.</param>
        /// <returns>The name of the database table for a given record type.</returns>
        private string GetTableName(Type t)
        {
            return t != null ? t.Name : string.Empty;
        }

        /// <summary>
        /// Disposes all resources used by this repository.
        /// </summary>
        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();

                Connection = null;
            }
        }

        #region Static methods

        /// <summary>
        /// Reads data from the given data reader and returns an object instance with property values filled with retrieved data.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="reader">Reader to read data from.</param>
        /// <returns>An object whose state corresponds to data from the given data reader.</returns>
        public static T Read<T>(IDataReader reader) where T : new()
        {
            return Read<T>(reader, string.Empty);
        }

        /// <summary>
        /// Reads data from the given data reader and returns an object instance with property values filled with retrieved data.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="reader">Reader to read data from.</param>
        /// <param name="fieldPrefix">Optional field prefix.</param>
        /// <returns>An object whose state corresponds to data from the given data reader.</returns>
        public static T Read<T>(IDataReader reader, string fieldPrefix) where T : new()
        {
            return Read<T>(Read(reader), fieldPrefix);
        }

        /// <summary>
        /// Reads data from the given data reader and returns an object instance with property values filled with retrieved data.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="data">Object data.</param>
        /// <returns>An object whose state corresponds to data from the given data reader.</returns>
        public static T Read<T>(IDictionary<string, object> data) where T : new()
        {
            return Read<T>(data, string.Empty);
        }

        /// <summary>
        /// Reads data from the given data reader and returns an object instance with property values filled with retrieved data.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="data">Object data.</param>
        /// <param name="fieldPrefix">Optional field prefix.</param>
        /// <returns>An object whose state corresponds to data from the given data reader.</returns>
        public static T Read<T>(IDictionary<string, object> data, string fieldPrefix) where T : new()
        {
            T ret = default(T);
            object value = null;
            string key = string.Empty;
            ValueConverter converter = null;
            
            if (data != null)
            {
                ret = new T();

                foreach (KeyValuePair<string, System.Reflection.PropertyInfo> p in Settings.GetPropertyMappings<T>())
                {
                    key = p.Key;

                    if (!string.IsNullOrEmpty(fieldPrefix))
                        key = fieldPrefix + p.Key;

                    if (data.ContainsKey(key))
                    {
                        value = data[key];

                        if (value != null)
                        {
                            converter = Repository.Settings.GetMatchingConverters(p.Value.PropertyType, value.GetType()).FirstOrDefault();

                            if (converter != null)
                                value = converter.ConvertBack(value);
                        }

                        if (value != null && p.Value.PropertyType.IsEnum)
                        {
                            try
                            {
                                value = Enum.Parse(p.Value.PropertyType, value.ToString(), true);
                            }
                            catch (ArgumentException) { }
                        }

                        p.Value.SetValue(ret, value, null);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Reads data from the given data reader and puts it into the dictionary.
        /// </summary>
        /// <param name="reader">Reader to read data from.</param>
        /// <returns>A dictionary containing data from the given data reader.</returns>
        public static IDictionary<string, object> Read(IDataReader reader)
        {
            int columnsCount = 0;
            string columnName = string.Empty;
            IDictionary<string, object> ret = new Dictionary<string, object>();

            if (reader != null && !reader.IsClosed)
            {
                columnsCount = reader.FieldCount;

                for (int i = 0; i < columnsCount; i++)
                {
                    columnName = reader.GetName(i);
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        if (ret.ContainsKey(columnName))
                            ret[columnName] = reader[i];
                        else
                            ret.Add(columnName, reader[i]);

                        if (ret[columnName] == DBNull.Value)
                            ret[columnName] = null;
                    }
                }
            }

            return ret;
        }

        #endregion
    }
}