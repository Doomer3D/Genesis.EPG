using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Genesis.EPG.Model;
using Genesis.EPG.Services;

namespace Genesis.EPG.Controllers
{
    /// <summary>
    /// контроллер справочников
    /// </summary>
    public class DictionaryController : BaseEPGController
    {
        /// <summary>
        /// сервис справочников
        /// </summary>
        private readonly DictionaryService _dataService;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        /// <param name="dataService"> сервис справочников </param>
        public DictionaryController(
            EPGConfig config,
            DictionaryService dataService) : base(config)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// получить список справочников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<DictionaryInfo>> Get()
        {
            return _dataService.GetAllDescriptors();
        }

        /// <summary>
        /// получить данные справочника
        /// </summary>
        /// <param name="name"> имя справочника </param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public ActionResult GetData(string name)
        {
            // получаем дескриптор
            var info = _dataService.GetDescriptor(name);

            if (info == default)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(_dataService.GetDictionaryData(info));
            }
        }

        /// <summary>
        /// получить элемент справочника
        /// </summary>
        /// <param name="name"> имя справочника </param>
        /// <param name="id"> идентификатор </param>
        /// <returns></returns>
        [HttpGet("{name}/{id}")]
        public ActionResult GetItem(string name, int id)
        {
            // получаем дескриптор
            var info = _dataService.GetDescriptor(name);

            if (info == default)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(_dataService.GetDictionaryItem(info, id));
            }
        }

        /// <summary>
        /// вставить элемент справочника
        /// </summary>
        /// <param name="name"> имя справочника </param>
        /// <param name="item"> значение </param>
        /// <returns></returns>
        [HttpPut("{name}")]
        public ActionResult PutItem(string name, [FromBody]JObject item)
        {
            // получаем дескриптор
            var info = _dataService.GetDescriptor(name);

            if (info == default)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(_dataService.PutDictionaryItem(info, item));
            }
        }

        /// <summary>
        /// обновить элемент справочника
        /// </summary>
        /// <param name="name"> имя справочника </param>
        /// <param name="id"> идентификатор </param>
        /// <param name="item"> значение </param>
        /// <returns></returns>
        [HttpPost("{name}/{id}")]
        public ActionResult PostItem(string name, int id, [FromBody]JObject item)
        {
            // получаем дескриптор
            var info = _dataService.GetDescriptor(name);

            if (info == default)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(_dataService.PostDictionaryItem(info, item, id));
            }
        }

        /// <summary>
        /// удалить элемент справочника
        /// </summary>
        /// <param name="name"> имя справочника </param>
        /// <param name="id"> идентификатор </param>
        /// <returns></returns>
        [HttpDelete("{name}/{id}")]
        public ActionResult DeleteItem(string name, int id)
        {
            // получаем дескриптор
            var info = _dataService.GetDescriptor(name);

            if (info == default)
            {
                return NotFound();
            }
            else
            {
                _dataService.DeleteDictionaryItem(info, id);
                return new JsonResult("OK");
            }
        }
    }
}
