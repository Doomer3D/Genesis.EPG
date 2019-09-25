using System;

namespace Genesis.EPG
{
    /// <summary>
    /// интерфейс информации о таблице
    /// </summary>
    public interface ITableInfo
    {
        /// <summary>
        /// наименование схемы
        /// </summary>
        string SchemaName { get; set; }

        /// <summary>
        /// наименование таблицы
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// полное наименование таблицы
        /// </summary>
        string FullTableName { get; }
    }
}
