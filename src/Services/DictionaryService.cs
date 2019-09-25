using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using Genesis.EPG.Model;

using READER = Npgsql.NpgsqlDataReader;

namespace Genesis.EPG.Services
{
    /// <summary>
    /// сервис справочников
    /// </summary>
    public class DictionaryService : BaseEPGService
    {
        private const string META_COLUMNS = @" e.id,
       e.name,
       e.schema_name,
       e.table_name,
       e.description,
       e.is_hier";

        private string _metaQuery;              // базовый запрос к метаданным

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        public DictionaryService(EPGConfig config) : base(config)
        {
        }

        /// <summary>
        /// получить список всех справочников
        /// </summary>
        /// <returns></returns>
        public List<DictionaryInfo> GetAllDescriptors()
        {
            using (var conn = GetConnector())
            {
                return conn.ExecuteReaderEach($@"
{GetMetaQuery()}
", CreateDictionaryFromReader).ToList();
            }
        }

        /// <summary>
        /// получить справочник по идентификатору
        /// </summary>
        /// <param name="id"> идентификатор справочника </param>
        /// <returns></returns>

        public DictionaryInfo GetDescriptor(int id)
        {
            using (var conn = GetConnector())
            {
                return conn.ExecuteReaderEach($@"
{GetMetaQuery()}
 where id = :P_ID
", CreateDictionaryFromReader,
                "P_ID", id).FirstOrDefault();
            }
        }

        /// <summary>
        /// получить справочник по имени
        /// </summary>
        /// <param name="name"> имя справочника </param>
        /// <returns></returns>
        public DictionaryInfo GetDescriptor(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            using (var conn = GetConnector())
            {
                return conn.ExecuteReaderEach($@"
{GetMetaQuery()}
 where lower(name) = lower(:P_NAME)
", CreateDictionaryFromReader,
                "P_NAME", name).FirstOrDefault();
            }
        }

        /// <summary>
        /// получить данные
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <returns></returns>
        public JArray GetDictionaryData(DictionaryInfo info)
        {
            using (var conn = GetConnector())
            {
                // проверяем существование таблицы
                var (_, table) = CheckTableExists(conn, info, true);

                var res = new JArray();

                // получаем данные
                conn.ExecuteReaderEach($@"
with recursive dat as (
    select id,
           name,
           pid,
           ord,
           extra,
           array[]::int4[] as hier
      from {table.QuotedFullTableName}
     where pid is null
     --
     union all
     --
    select a.id,
           a.name,
           a.pid,
           a.ord,
           a.extra,
           b.hier || a.pid
      from {table.QuotedFullTableName} as a
      join dat as b on b.id = a.pid
)
select id,
       name,
       pid,
       ord,
       extra
  from dat
 order by hier, ord, name", reader =>
                {
                    res.Add(ReadObject(reader));
                });

                return res;
            }
        }

        /// <summary>
        /// получить элемент справочника
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <param name="id"> идентификатор </param>
        /// <returns></returns>
        public JObject GetDictionaryItem(DictionaryInfo info, int id)
        {
            using (var conn = GetConnector())
            {
                // проверяем существование таблицы
                var (_, table) = CheckTableExists(conn, info, true);

                // получаем данные
                return ReadDictionaryItem(conn, table, id);
            }
        }

        /// <summary>
        /// вставить элемент справочника
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <param name="item"> значение </param>
        /// <returns></returns>
        public JObject PutDictionaryItem(DictionaryInfo info, JObject item)
        {
            var idOrNot = item.TryGetID(true);
            if (idOrNot.HasValue)
            {
                // обновление элемента
                return PostDictionaryItem(info, item, idOrNot.Value);
            }
            else
            {
                // вставка элемента
                using (var conn = GetConnector())
                {
                    // проверяем существование таблицы
                    var (_, table) = CheckTableExists(conn, info, true);

                    var id = conn.ExecuteScalar<int>($@"
insert into {table.QuotedFullTableName}
(name, pid, ord, extra)
values
(:P_NAME, :P_PID, :P_ORD, :P_EXTRA::jsonb)
returning id",
                        "P_NAME", item.GetString("name", true),
                        "P_PID", item.GetInt32OrNull("pid", true),
                        "P_ORD", item.GetInt32OrNull("ord", true),
                        "P_EXTRA", item.GetExtra()
                    );

                    return ReadDictionaryItem(conn, table, id);
                }
            }
        }

        /// <summary>
        /// обновить элемент справочника
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <param name="item"> значение </param>
        /// <param name="id"> идентификатор </param>
        /// <returns></returns>
        public JObject PostDictionaryItem(DictionaryInfo info, JObject item, int id)
        {
            item.TryGetID(true);

            // обновление элемента
            using (var conn = GetConnector())
            {
                // проверяем существование таблицы
                var (_, table) = CheckTableExists(conn, info, true);

                conn.ExecuteNonQuery($@"
update {table.QuotedFullTableName}
   set name = :P_NAME,
       pid = :P_PID,
       ord = :P_ORD,
       extra = :P_EXTRA::jsonb
 where id = :P_ID",
                    "P_NAME", item.GetString("name", true),
                    "P_PID", item.GetInt32OrNull("pid", true),
                    "P_ORD", item.GetInt32OrNull("ord", true),
                    "P_EXTRA", item.GetExtra(),
                    "P_ID", id
                );

                return ReadDictionaryItem(conn, table, id);
            }
        }

        /// <summary>
        /// удалить элемент справочника
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <param name="id"> идентификатор </param>
        public void DeleteDictionaryItem(DictionaryInfo info, int id)
        {
            using (var conn = GetConnector())
            {
                // проверяем существование таблицы
                var (_, table) = CheckTableExists(conn, info, true);

                conn.ExecuteNonQuery($@"
delete from {table.QuotedFullTableName}
 where id = :P_ID", "P_ID", id);
            }
        }

        /// <summary>
        /// прочитать объект
        /// </summary>
        /// <param name="conn"> коннектор </param>
        /// <param name="table"> информация о таблице </param>
        /// <param name="id"> идентификатор </param>
        /// <returns></returns>
        private JObject ReadDictionaryItem(Connector conn, TableInfo table, int id)
        {
            JObject item = default;

            conn.ExecuteReaderEach($@"
select id,
       name,
       pid,
       ord,
       extra
  from {table.QuotedFullTableName}
 where id = :P_ID", reader =>
            {
                item = ReadObject(reader);
            },
           "P_ID", id);

            return item;
        }

        /// <summary>
        /// прочитать объект
        /// </summary>
        /// <param name="reader"> читатель </param>
        /// <returns></returns>
        private JObject ReadObject(READER reader)
        {
            var item = new JObject();

            int i = 0;
            item.Add("id", new JValue(reader.GetInt32(i++)));
            item.Add("name", new JValue(reader.GetString(i++)));
            item.Add("pid", new JValue(reader.GetInt32OrNull(i++)));
            item.Add("ord", new JValue(reader.GetInt32OrNull(i++)));
            reader.MergeJson(i++, item);

            return item;
        }

        /// <summary>
        /// проверить существование таблицы
        /// </summary>
        /// <param name="conn"> коннектор </param>
        /// <param name="info"> дескриптор </param>
        /// <param name="createifNotExists"> указывает, что необходимо создать таблицу в случае ее отсутствия </param>
        /// <returns></returns>
        private (bool exists, TableInfo table) CheckTableExists(Connector conn, DictionaryInfo info, bool createifNotExists)
        {
            if (info == default) throw new ArgumentNullException(nameof(info));

            var table = GetDataTableName(info);

            if (!info.IsTableExists && createifNotExists)
            {
                // необходимо создать таблицу
                using (var tx = conn.BeginTransaction())
                {
                    // создаем таблицу
                    conn.ExecuteScalar($@"
CREATE TABLE {table.QuotedFullTableName}
(
  id int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY,
  name text NOT NULL,
  pid int4 NULL,
  ord int4 NULL,
  extra jsonb NULL,
  CONSTRAINT {table.TableName}_pk PRIMARY KEY (id),
  CONSTRAINT {table.TableName}_fk FOREIGN KEY (pid) REFERENCES {table.QuotedFullTableName}(id) ON DELETE CASCADE ON UPDATE CASCADE
)");

                    // создаем индексы
                    conn.ExecuteScalar($@"CREATE INDEX {table.TableName}_name_idx ON {table.QuotedFullTableName}(name);");
                    conn.ExecuteScalar($@"CREATE INDEX {table.TableName}_pid_idx ON {table.QuotedFullTableName}(pid);");

                    // добавляем комментарии
                    if (!string.IsNullOrWhiteSpace(info.Description))
                    {
                        conn.ExecuteScalar($@"COMMENT ON TABLE {table.QuotedFullTableName} IS {QuoteString(info.Description)}");
                    }

                    conn.ExecuteScalar($@"COMMENT ON COLUMN {table.QuotedFullTableName}.id IS 'Идентификатор записи'");
                    conn.ExecuteScalar($@"COMMENT ON COLUMN {table.QuotedFullTableName}.name IS 'Наименование записи'");
                    conn.ExecuteScalar($@"COMMENT ON COLUMN {table.QuotedFullTableName}.pid IS 'Идентификатор родительской записи'");
                    conn.ExecuteScalar($@"COMMENT ON COLUMN {table.QuotedFullTableName}.ord IS 'Порядок для сортировки'");
                    conn.ExecuteScalar($@"COMMENT ON COLUMN {table.QuotedFullTableName}.extra IS 'Дополнительные данные'");

                    // создаем команду кластеризации
                    conn.ExecuteScalar($@"CLUSTER {table.QuotedFullTableName} USING {table.TableName}_pk;");

                    tx.Commit();
                }
            }

            return (info.IsTableExists, table);
        }

        /// <summary>
        /// получить базовый запрос к метаданным
        /// </summary>
        /// <returns></returns>
        private string GetMetaQuery()
        {
            if (_metaQuery == default)
            {
                _metaQuery = $@"
{GetTablesSubQuery()}
select {META_COLUMNS},
       t is not null as is_table_exists
  from {GetMetaTableName()} as e
{GetTablesSubQueryJoin()}";
            }
            return _metaQuery;
        }

        /// <summary>
        /// получить имя таблицы метаданных справочников
        /// </summary>
        /// <returns></returns>
        private string GetMetaTableName()
        {
            return $"{_config.MetadataSchemaName}.dictionary";
        }

        /// <summary>
        /// создать справочник
        /// </summary>
        /// <returns></returns>
        private DictionaryInfo CreateDictionaryFromReader(READER reader)
        {
            int i = 0;
            return new DictionaryInfo
            {
                ID = reader.GetInt32(i++),
                Name = reader.GetString(i++),
                SchemaName = reader.GetStringOrDefault(i++),
                TableName = reader.GetString(i++),
                Description = reader.GetString(i++),
                IsHier = reader.GetBoolean(i++),
                IsTableExists = reader.GetBoolean(i++),
            };
        }
    }
}
