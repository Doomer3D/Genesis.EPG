using System;

namespace Genesis.EPG.Model
{
    /// <summary>
    /// дескриптор сущности
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("[{Name,nq}] {Description,ns}")]
    public class EntityInfo : IBaseEntity
    {
        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Наименование сущности
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Наименование схемы
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Наименование таблицы
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Описание сущности
        /// </summary>
        public string Description { get; set; }
    }
}
