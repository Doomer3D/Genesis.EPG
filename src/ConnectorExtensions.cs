using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using CONNECTION = Npgsql.NpgsqlConnection;
using COMMAND = Npgsql.NpgsqlCommand;
using PARAMETER = Npgsql.NpgsqlParameter;
using READER = Npgsql.NpgsqlDataReader;
using TRANSACTION = Npgsql.NpgsqlTransaction;

public static class ConnectorExtensions
{
    #region Postgres Extensions

    #region Get Or Default

    public static string GetStringOrDefault(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetString(ordinal);

    public static Int16 GetInt16OrDefault(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetInt16(ordinal);

    public static Int32 GetInt32OrDefault(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetInt32(ordinal);

    public static Int64 GetInt64OrDefault(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetInt64(ordinal);

    public static Double GetDoubleOrDefault(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetDouble(ordinal);

    public static Decimal GetDecimalOrDefault(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetDecimal(ordinal);

    public static Boolean GetBooleanOrDefault(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetBoolean(ordinal);

    public static DateTime GetDateTimeOrDefault(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetDateTime(ordinal);

    #endregion
    #region Get Or Null

    public static Int16? GetInt16OrNull(this READER reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return null;
        else return reader.GetInt16(ordinal);
    }

    public static Int32? GetInt32OrNull(this READER reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return null;
        else return reader.GetInt32(ordinal);
    }

    public static Int64? GetInt64OrNull(this READER reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return null;
        else return reader.GetInt64(ordinal);
    }

    public static Decimal? GetDecimalOrNull(this READER reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return null;
        else return reader.GetDecimal(ordinal);
    }

    public static Boolean? GetBooleanOrNull(this READER reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return null;
        else return reader.GetBoolean(ordinal);
    }

    public static DateTime? GetDateTimeOrNull(this READER reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return null;
        else return reader.GetDateTime(ordinal);
    }

    #endregion
    #region GetEnum

    public static T GetEnum<T>(this READER reader, int ordinal) where T : Enum
    {
        return (T)(object)reader.GetInt32(ordinal);
    }

    public static T GetEnumOrDefault<T>(this READER reader, int ordinal) where T : Enum
    {
        if (reader.IsDBNull(ordinal)) return default;
        else return (T)(object)reader.GetInt32(ordinal);
    }

    #endregion
    #region GetDictionary

    public static T GetDictionary<T>(this READER reader, int ordinal, Dictionary<int, T> dic) where T : class
    {
        var key = reader.GetInt32(ordinal);
        if (dic.TryGetValue(key, out var res))
        {
            return res;
        }
        else
        {
            throw new KeyNotFoundException($"Ключ {key} не найден в справочнике");
        }
    }

    public static T GetDictionaryOrDefault<T>(this READER reader, int ordinal, Dictionary<int, T> dic) where T : class
    {
        if (reader.IsDBNull(ordinal)) return default;
        else return dic.TryGetValue(reader.GetInt32(ordinal), out var res) ? res : default;
    }

    public static List<T> GetDictionaryList<T>(this READER reader, int ordinal, Dictionary<int, T> dic) where T : class
    {
        var list = new List<T>();

        var arr = reader.GetInt32Array(ordinal);
        if (arr != default && arr.Length != 0)
        {
            // цикл по массиву ключей
            foreach (var key in arr)
            {
                if (dic.TryGetValue(key, out var item))
                {
                    list.Add(item);
                }
                else
                {
                    throw new KeyNotFoundException($"Ключ {key} не найден в справочнике");
                }
            }
        }

        return list;
    }

    #endregion
    #region GetArray

    public static Int32[] GetInt32Array(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetValue(ordinal) as Int32[];

    public static Double[] GetDoubleArray(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetValue(ordinal) as Double[];

    public static T[] GetEnumArray<T>(this READER reader, int ordinal) where T : Enum
    {
        var arr = GetInt32Array(reader, ordinal);
        if (arr == default) return default;
        else
        {
            return arr.Select(e => (T)(object)e).ToArray();
        }
    }

    #endregion

    public static JObject MergeJson(this READER reader, int ordinal, JObject target)
    {
        if (!reader.IsDBNull(ordinal))
        {
            var json = JObject.Parse(reader.GetString(ordinal));
            target.Merge(json);
            return json;
        }
        else
        {
            return default;
        }
    }

    public static bool GetInt16Boolean(this READER reader, int ordinal) => reader.IsDBNull(ordinal) ? default : reader.GetInt16(ordinal) == 1;

    #endregion

    #region BeginTransaction

    /// <summary>
    /// начать транзакцию
    /// </summary>
    /// <param name="conn"> коннектор </param>
    /// <returns></returns>
    public static TRANSACTION BeginTransaction(this Connector conn)
    {
        return conn.Connection.BeginTransaction();
    }

    #endregion
}
