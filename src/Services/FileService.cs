using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

using Genesis.EPG.Model;

namespace Genesis.EPG.Services
{
    /// <summary>
    /// сервис файлов
    /// </summary>
    public class FileService : BaseEPGService
    {
        /// <summary>
        /// путь к файлам
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// типы MIME
        /// </summary>
        private static readonly Dictionary<string, string> _mimeTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.ms-word"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "officedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"}
        };

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        /// <param name="environment"> данные окружения </param>
        public FileService(
            EPGConfig config,
            IHostingEnvironment environment) : base(config)
        {
            filePath = Path.Combine(environment.ContentRootPath, config.UploadsFilePath);
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
        }

        /// <summary>
        /// получить MIME-тип
        /// </summary>
        /// <param name="filename"> имя файла </param>
        /// <returns></returns>
        public string GetContentType(string filename)
        {
            return _mimeTypes.TryGetValue(Path.GetExtension(filename).ToLowerInvariant(), out var value) ?
                value :
                "application/octet-stream";
        }

        /// <summary>
        /// получить имя файла
        /// </summary>
        /// <param name="id"> идентификатор </param>
        /// <returns></returns>
        public string GetFileName(int id)
        {
            return Path.Combine(filePath, $"file_{id}");
        }
    }
}
