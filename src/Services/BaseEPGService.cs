using System;
using System.Text;

using Genesis.EPG.Model;

namespace Genesis.EPG.Services
{
    /// <summary>
    /// базовый сервис EPG
    /// </summary>
    public abstract class BaseEPGService
    {
        /// <summary>
        /// конфигурация EPG
        /// </summary>
        protected readonly EPGConfig _config;

        /// <summary>
        /// коннектор по умолчанию
        /// </summary>
        protected static Connector _defaultConnector;

        protected static object _lock = new object();

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        public BaseEPGService(EPGConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// получить коннектор БД
        /// </summary>
        /// <returns></returns>
        protected Connector GetConnector()
        {
            var res = new Connector(_config.ConnectionString);

            res.Open();

            return res;
        }

        /// <summary>
        /// получить подзапрос на список таблиц
        /// </summary>
        /// <returns></returns>
        protected string GetTablesSubQuery()
        {
            return @"
with tables as (
    select ns.nspname as schema_name,
           c.relname as table_name
      from pg_catalog.pg_class as c
 left join pg_catalog.pg_namespace as ns on ns.oid = c.relnamespace
     where ns.nspname !~ '^pg_'
       and ns.nspname <> 'information_schema'
       and c.relkind in ('r','p','v','m')
)";
        }

        /// <summary>
        /// получить join-часть запроса на список таблиц
        /// </summary>
        /// <returns></returns>
        protected string GetTablesSubQueryJoin()
        {
            return $@"
  left join tables as t on t.schema_name = coalesce(e.schema_name, '{_config.DataSchemaName}')
                       and t.table_name = e.table_name";
        }

        /// <summary>
        /// получить информацию о таблице данных
        /// </summary>
        /// <param name="info"> дескриптор </param>
        /// <returns></returns>
        protected TableInfo GetDataTableName(ITableEntity info)
        {
            return new TableInfo
            {
                SchemaName = (string.IsNullOrWhiteSpace(info.SchemaName) ? _config.DataSchemaName : info.SchemaName).ToLowerInvariant(),
                TableName = info.TableName.ToLowerInvariant()
            };
        }

        /// <summary>
        /// заключить строку в кавычки
        /// </summary>
        /// <param name="value"> значение </param>
        /// <returns></returns>
        protected string QuoteString(string value)
        {
            if (value == default)
            {
                return "'NULL'";
            }
            else if (value == string.Empty)
            {
                return "''";
            }
            else
            {
                var sb = new StringBuilder(value.Length + 2);
                sb.Append('\'');
                char c;
                int count = value.Length;
                for (int i = 0; i < count; i++)
                {
                    if ((c = value[i]) == '\'')
                    {
                        sb.Append("''");
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                sb.Append('\'');
                return sb.ToString();
            }
        }
    }
}
