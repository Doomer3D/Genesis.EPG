using System;

namespace Genesis.EPG.Model
{
    /// <summary>
    /// информация о таблице
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{SchemaName,nq}.{TableName,nq}")]
    public class TableInfo : ITableInfo
    {
        /// <summary>
        /// наименование схемы
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// наименование таблицы
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// полное наименование таблицы
        /// </summary>
        public string FullTableName => $"{SchemaName}.{TableName}";

        /// <summary>
        /// полное наименование таблицы в кавычках
        /// </summary>
        public object QuotedFullTableName => $@"""{SchemaName}"".""{TableName}""";
    }
}
