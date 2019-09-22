using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.EPG
{
    /// <summary>
    /// интерфейс базовоч сущности EPG
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Наименование сущности
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Наименование схемы
        /// </summary>
        string SchemaName { get; set; }

        /// <summary>
        /// Наименование таблицы
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Описание сущности
        /// </summary>
        string Description { get; set; }
    }
}
