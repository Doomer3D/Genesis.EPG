using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Genesis.EPG.Services;

namespace Genesis.EPG.Controllers
{
    /// <summary>
    /// файловый контроллер
    /// </summary>
    public class FileController : BaseEPGController
    {
        /// <summary>
        /// сервис файлов
        /// </summary>
        private readonly FileService _fileService;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        /// <param name="fileService"> сервис файлов </param>
        public FileController(
            EPGConfig config,
            FileService fileService) : base(config)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// получить файл
        /// </summary>
        /// <param name="id"> идентификатор файла </param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            using (var conn = GetConnector())
            {
                string filename = default, name = default;

                conn.ExecuteReaderEach(@"
select coalesce(body, '') as filename,
       name
  from meta.file
 where id = @P_ID
",
                    reader =>
                    {
                        filename = reader.GetString(0);
                        name = reader.GetString(1);
                    },
                    "P_ID", id
                );

                if (string.IsNullOrWhiteSpace(filename))
                {
                    return NotFound();
                }
                else
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filename, FileMode.Open))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Position = 0;
                    return File(memory, _fileService.GetContentType(name), Path.GetFileName(name));
                }
            }
        }

        /// <summary>
        /// отправить файл
        /// </summary>
        /// <param name="files"> список файлов </param>
        [HttpPost]
        public ActionResult<int> Post([FromForm(Name = "file")] IFormFileCollection files)
        {
            if (files != default && files.Count != 0)
            {
                using (var conn = GetConnector())
                {
                    using (var tx = conn.BeginTransaction())
                    {
                        foreach (var file in files)
                        {
                            // сохраняем файл в базе
                            var id = conn.ExecuteScalar<int>(@"
insert into meta.file
(name, body)
values
(@P_NAME, @P_BODY)
returning id",
                                "P_NAME", file.FileName,
                                "P_BODY", "filename"
                            );

                            // сохраняем файл в системе
                            var filename = _fileService.GetFileName(id);

                            using (var stream = new FileStream(filename, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            // обновляем имя файла в БД
                            conn.ExecuteNonQuery(@"
update meta.file
   set body = @P_FILENAME
 where id = @P_ID",
                                "P_ID", id,
                                "P_FILENAME", filename
                            );
                        }

                        tx.Commit();
                    }
                }

                return files.Count;
            }
            else
            {
                return 0;
            }
        }
    }
}
