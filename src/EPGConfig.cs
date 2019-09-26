using System;
using System.Collections.Generic;

namespace Genesis.EPG
{
    /// <summary>
    /// конфигурация EPG
    /// </summary>
    public class EPGConfig
    {
        /// <summary>
        /// строка соединения с БД
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// имя строки соединения
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// имя схемы метаданных
        /// </summary>
        public string MetadataSchemaName { get; set; } = "meta";

        /// <summary>
        /// имя схемы данных
        /// </summary>
        public string DataSchemaName { get; set; } = "data";

        /// <summary>
        /// путь к папке с файлами для загрузки
        /// </summary>
        public string UploadsFilePath { get; set; } = "uploads";
    }
}
