using System;

namespace Genesis.EPG.Model
{
    /// <summary>
    /// дескриптор справочника
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("[{Name,nq}] {Description,ns}")]
    public class DictionaryInfo : IBaseEntity, ITableEntity
    {
        /// <summary>
        /// идентификатор справочника
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// наименование справочника
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// наименование схемы
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// наименование таблицы
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// описание справочника
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// указывает, является ли справочник иерархическим
        /// </summary>
        public bool IsHier { get; set; }

        /// <summary>
        /// указывает, что таблица существует в базе
        /// </summary>
        public bool IsTableExists { get; set; }

        /// <summary>
        /// полное наименование таблицы
        /// </summary>
        public string FullTableName => $"{SchemaName}.{TableName}";
    }
}
