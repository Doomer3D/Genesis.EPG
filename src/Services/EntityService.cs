using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using Genesis.EPG.Model;
using READER = Npgsql.NpgsqlDataReader;

namespace Genesis.EPG.Services
{
    /// <summary>
    /// сервис сущностей
    /// </summary>
    public class EntityService : BaseEPGService
    {
        private const string META_COLUMNS = @" e.id,
       e.name,
       e.schema_name,
       e.table_name,
       e.description";

        private string _metaQuery;              // базовый запрос к метаданным

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        public EntityService(EPGConfig config) : base(config)
        {
        }

        /// <summary>
        /// получить список всех сущностей
        /// </summary>
        /// <returns></returns>
        public List<EntityInfo> GetAllDescriptors()
        {
            using (var conn = GetConnector())
            {
                return conn.ExecuteReaderEach($@"
{GetMetaQuery()}
", CreateEntityFromReader).ToList();
            }
        }

        /// <summary>
        /// получить сущность по идентификатору
        /// </summary>
        /// <param name="id"> идентификатор сущности </param>
        /// <returns></returns>

        public EntityInfo GetDescriptor(int id)
        {
            using (var conn = GetConnector())
            {
                return conn.ExecuteReaderEach($@"
{GetMetaQuery()}
 where id = :P_ID
", CreateEntityFromReader,
                "P_ID", id).FirstOrDefault();
            }
        }

        /// <summary>
        /// получить сущность по имени
        /// </summary>
        /// <param name="name"> имя сущности </param>
        /// <returns></returns>
        public EntityInfo GetDescriptor(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            using (var conn = GetConnector())
            {
                return conn.ExecuteReaderEach($@"
{GetMetaQuery()}
 where lower(name) = lower(:P_NAME)
", CreateEntityFromReader,
                "P_NAME", name).FirstOrDefault();
            }
        }

        /// <summary>
        /// получить данные
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <returns></returns>
        public JArray GetEntityData(EntityInfo info)
        {
            using (var conn = GetConnector())
            {
                // проверяем существование таблицы
                var (_, table) = CheckTableExists(conn, info, true);

                var res = new JArray();

                // получаем данные
                conn.ExecuteReaderEach($@"
select id,
       extra
  from {table.QuotedFullTableName}
 order by id", reader =>
                {
                    res.Add(ReadObject(reader));
                });

                return res;
            }
        }

        /// <summary>
        /// получить элемент сущности
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <param name="id"> идентификатор </param>
        /// <returns></returns>
        public JObject GetEntityItem(EntityInfo info, int id)
        {
            using (var conn = GetConnector())
            {
                // проверяем существование таблицы
                var (_, table) = CheckTableExists(conn, info, true);

                // получаем данные
                return ReadEntityItem(conn, table, id);
            }
        }

        /// <summary>
        /// вставить элемент сущности
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <param name="item"> значение </param>
        /// <returns></returns>
        public JObject PutEntityItem(EntityInfo info, JObject item)
        {
            var idOrNot = item.TryGetID(true);
            if (idOrNot.HasValue)
            {
                // обновление элемента
                return PostEntityItem(info, item, idOrNot.Value);
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
(extra)
values
(:P_EXTRA::jsonb)
returning id",
                        "P_EXTRA", item.GetExtra()
                    );

                    return ReadEntityItem(conn, table, id);
                }
            }
        }

        /// <summary>
        /// обновить элемент сущности
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <param name="item"> значение </param>
        /// <param name="id"> идентификатор </param>
        /// <returns></returns>
        public JObject PostEntityItem(EntityInfo info, JObject item, int id)
        {
            item.TryGetID(true);

            // обновление элемента
            using (var conn = GetConnector())
            {
                // проверяем существование таблицы
                var (_, table) = CheckTableExists(conn, info, true);

                conn.ExecuteNonQuery($@"
update {table.QuotedFullTableName}
   set extra = :P_EXTRA::jsonb
 where id = :P_ID",
                    "P_EXTRA", item.GetExtra(),
                    "P_ID", id
                );

                return ReadEntityItem(conn, table, id);
            }
        }

        /// <summary>
        /// удалить элемент сущности
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <param name="id"> идентификатор </param>
        public void DeleteEntityItem(EntityInfo info, int id)
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
        private JObject ReadEntityItem(Connector conn, TableInfo table, int id)
        {
            JObject item = default;

            conn.ExecuteReaderEach($@"
select id,
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
        private (bool exists, TableInfo table) CheckTableExists(Connector conn, EntityInfo info, bool createifNotExists)
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
  extra jsonb NULL,
  CONSTRAINT {table.TableName}_pk PRIMARY KEY (id)
)");

                    // добавляем комментарии
                    if (!string.IsNullOrWhiteSpace(info.Description))
                    {
                        conn.ExecuteScalar($@"COMMENT ON TABLE {table.QuotedFullTableName} IS {QuoteString(info.Description)}");
                    }

                    conn.ExecuteScalar($@"COMMENT ON COLUMN {table.QuotedFullTableName}.id IS 'Идентификатор записи'");
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
        /// получить имя таблицы метаданных сущностей
        /// </summary>
        /// <returns></returns>
        private string GetMetaTableName()
        {
            return $"{_config.MetadataSchemaName}.entity";
        }

        /// <summary>
        /// создать сущность
        /// </summary>
        /// <returns></returns>
        private EntityInfo CreateEntityFromReader(READER reader)
        {
            int i = 0;
            return new EntityInfo
            {
                ID = reader.GetInt32(i++),
                Name = reader.GetString(i++),
                SchemaName = reader.GetStringOrDefault(i++),
                TableName = reader.GetString(i++),
                Description = reader.GetString(i++),
                IsTableExists = reader.GetBoolean(i++),
            };
        }
    }
}
