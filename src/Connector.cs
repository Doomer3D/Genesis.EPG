using System;
using System.Collections.Generic;

using CONNECTION = Npgsql.NpgsqlConnection;
using COMMAND = Npgsql.NpgsqlCommand;
using PARAMETER = Npgsql.NpgsqlParameter;
using READER = Npgsql.NpgsqlDataReader;

/// <summary>
/// коннектор БД Postgres
/// </summary>
public class Connector : IDisposable
{
    private CONNECTION _conn;               // соединение с БД
    private string _connectionString;       // строка соединения с БД

    /// <summary>
    /// соединение с БД
    /// </summary>
    public CONNECTION Connection { get => _conn; }

    /// <summary>
    /// строка соединения с БД
    /// </summary>
    public string ConnectionString { get { return _connectionString; } set { _connectionString = value; } }

    /// <summary>
    /// конструктор
    /// </summary>
    public Connector()
    {
    }

    /// <summary>
    /// конструктор
    /// </summary>
    /// <param name="connectionString"> строка соединения с БД </param>
    public Connector(string connectionString)
    {
        _connectionString = connectionString;
    }

    #region Open | Close

    /// <summary>
    /// открыть соединение
    /// </summary>
    /// <param name="connectionString"> строка соединения с БД </param>
    /// <returns></returns>
    public CONNECTION Open(string connectionString = null)
    {
        if (_conn != null)
        {
            Close();
        }

        _conn = new CONNECTION(connectionString ?? _connectionString);
        _conn.Open();

        return _conn;
    }

    /// <summary>
    /// закрыть соединение
    /// </summary>
    public void Close()
    {
        if (_conn != null)
        {
            try
            {
                _conn.Close();
                _conn.Dispose();
            }
            catch { }
            _conn = null;
        }
    }

    /// <summary>
    /// уничтожить коннектор
    /// </summary>
    public void Dispose()
    {
        Close();
    }

    #endregion
    #region CreateCommand

    /// <summary>
    /// создать команду
    /// </summary>
    /// <returns></returns>
    public COMMAND CreateCommand() => _conn.CreateCommand();

    /// <summary>
    /// создать команду
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <returns></returns>
    public COMMAND CreateCommand(string sql)
    {
        var command = _conn.CreateCommand();
        command.CommandText = sql;
        return command;
    }

    /// <summary>
    /// создать команду
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="args"> агрументы </param>
    /// <returns></returns>
    public COMMAND CreateCommand(string sql, params object[] args)
    {
        var command = _conn.CreateCommand();
        command.CommandText = sql;

        for (int i = 0, n = args.Length; i < n; i += 2)
        {
            command.Parameters.Add(new PARAMETER(args[i].ToString(), args[i + 1] ?? DBNull.Value));
        }

        return command;
    }

    #endregion
    #region ExecuteNonQuery

    /// <summary>
    /// выполнить запрос
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <returns></returns>
    public int ExecuteNonQuery(string sql)
    {
        using (var command = CreateCommand(sql))
        {
            return command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// выполнить запрос
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="args"> агрументы </param>
    /// <returns></returns>
    public int ExecuteNonQuery(string sql, params object[] args)
    {
        using (var command = CreateCommand(sql, args))
        {
            return command.ExecuteNonQuery();
        }
    }

    #endregion
    #region ExecuteScalar

    /// <summary>
    /// выполнить скалярный запрос
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <returns></returns>
    public object ExecuteScalar(string sql)
    {
        using (var command = CreateCommand(sql))
        {
            return command.ExecuteScalar();
        }
    }

    /// <summary>
    /// выполнить скалярный запрос
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="args"> агрументы </param>
    /// <returns></returns>
    public object ExecuteScalar(string sql, params object[] args)
    {
        using (var command = CreateCommand(sql, args))
        {
            return command.ExecuteScalar();
        }
    }

    /// <summary>
    /// выполнить скалярный запрос
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <returns></returns>
    public T ExecuteScalar<T>(string sql)
    {
        using (var command = CreateCommand(sql))
        {
            var res = command.ExecuteScalar();
            return res == DBNull.Value ? default : (T)res;
        }
    }

    /// <summary>
    /// выполнить скалярный запрос
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="args"> агрументы </param>
    /// <returns></returns>
    public T ExecuteScalar<T>(string sql, params object[] args)
    {
        using (var command = CreateCommand(sql, args))
        {
            var res = command.ExecuteScalar();
            return res == DBNull.Value ? default : (T)res;
        }
    }

    #endregion
    #region ExecuteReader

    /// <summary>
    /// выполнить запрос на чтение
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="action"> действие </param>
    public void ExecuteReader(string sql, Action<READER> action)
    {
        using (var command = CreateCommand(sql))
        {
            using (var reader = command.ExecuteReader())
            {
                action(reader);
            }
        }
    }

    /// <summary>
    /// выполнить запрос на чтение
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="action"> действие </param>
    /// <param name="args"> агрументы </param>
    public void ExecuteReader(string sql, Action<READER> action, params object[] args)
    {
        using (var command = CreateCommand(sql, args))
        {
            using (var reader = command.ExecuteReader())
            {
                action(reader);
            }
        }
    }

    #endregion
    #region ExecuteReaderEach

    /// <summary>
    /// выполнить запрос на чтение с последовательной итерацией строк
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="action"> действие </param>
    public void ExecuteReaderEach(string sql, Action<READER> action)
    {
        using (var command = CreateCommand(sql))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }
    }

    /// <summary>
    /// выполнить запрос на чтение с последовательной итерацией строк
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="action"> действие </param>
    /// <param name="args"> агрументы </param>
    public void ExecuteReaderEach(string sql, Action<READER> action, params object[] args)
    {
        using (var command = CreateCommand(sql, args))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }
    }

    #endregion
    #region ExecuteReaderEach<T>

    /// <summary>
    /// выполнить запрос на чтение с последовательной итерацией строк
    /// </summary>
    /// <typeparam name="T"> тип элемента </typeparam>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="func"> функция преобразования </param>
    /// <returns></returns>
    public IEnumerable<T> ExecuteReaderEach<T>(string sql, Func<READER, T> func)
    {
        using (var command = CreateCommand(sql))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }
    }

    /// <summary>
    /// выполнить запрос на чтение с последовательной итерацией строк
    /// </summary>
    /// <typeparam name="T"> тип элемента </typeparam>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="func"> функция преобразования </param>
    /// <param name="args"> агрументы </param>
    /// <returns></returns>
    public IEnumerable<T> ExecuteReaderEach<T>(string sql, Func<READER, T> func, params object[] args)
    {
        using (var command = CreateCommand(sql, args))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }
    }

    #endregion
    #region Exists

    /// <summary>
    /// выполнить запрос на существование результата
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <returns></returns>
    public bool Exists(string sql)
    {
        var res = ExecuteScalar(sql);
        return !(res == null || res == DBNull.Value);
    }

    /// <summary>
    /// выполнить запрос на существование результата
    /// </summary>
    /// <param name="sql"> SQL запрос </param>
    /// <param name="args"> агрументы </param>
    /// <returns></returns>
    public bool Exists(string sql, params object[] args)
    {
        var res = ExecuteScalar(sql, args);
        return !(res == null || res == DBNull.Value);
    }

    #endregion

    #region Static

    /// <summary>
    /// открыть новый коннектор
    /// </summary>
    /// <returns></returns>
    public static Connector OpenNew(string connectionString = null)
    {
        var conn = new Connector(connectionString);
        conn.Open();
        return conn;
    }

    #endregion
}
