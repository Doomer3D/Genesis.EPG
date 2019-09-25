using System;

namespace Genesis.EPG.Model
{
    /// <summary>
    /// дескриптор сущности
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("[{Name,nq}] {Description,ns}")]
    public class EntityInfo : IBaseEntity, ITableEntity
    {
        /// <summary>
        /// идентификатор сущности
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// наименование сущности
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
        /// описание сущности
        /// </summary>
        public string Description { get; set; }

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
