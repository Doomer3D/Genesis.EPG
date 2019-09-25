using System;

namespace Genesis.EPG
{
    /// <summary>
    /// интерфейс табличной сущности
    /// </summary>
    public interface ITableEntity : ITableInfo
    {
        /// <summary>
        /// указывает, что таблица существует в базе
        /// </summary>
        bool IsTableExists { get; set; }
    }
}
