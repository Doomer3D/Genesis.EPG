using System;

namespace Genesis.EPG.Model
{
    /// <summary>
    /// дескриптор справочника
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("[{Name,nq}] {Description,ns}")]
    public class DictionaryInfo : IBaseEntity
    {
        /// <summary>
        /// Идентификатор справочника
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Наименование справочника
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
        /// Описание справочника
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Указывает, является ли справочник иерархическим
        /// </summary>
        public string IsHier { get; set; }
    }
}
