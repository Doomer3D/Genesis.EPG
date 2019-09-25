using System;

namespace Genesis.EPG
{
    /// <summary>
    /// интерфейс базовой сущности
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// идентификатор сущности
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// наименование сущности
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// описание сущности
        /// </summary>
        string Description { get; set; }
    }
}
