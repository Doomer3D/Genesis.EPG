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
    /// контроллер сущностей
    /// </summary>
    public class EntityController : BaseEPGController
    {
        /// <summary>
        /// сервис сущностей
        /// </summary>
        private readonly EntityService _dataService;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        /// <param name="dataService"> сервис сущностей </param>
        public EntityController(
            EPGConfig config,
            EntityService dataService) : base(config)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// получить список сущностей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<EntityInfo>> Get()
        {
            return _dataService.GetAllDescriptors();
        }

        /// <summary>
        /// получить данные сущности
        /// </summary>
        /// <param name="name"> имя сущности </param>
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
                return new JsonResult(_dataService.GetEntityData(info));
            }
        }

        /// <summary>
        /// получить элемент сущности
        /// </summary>
        /// <param name="name"> имя сущности </param>
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
                return new JsonResult(_dataService.GetEntityItem(info, id));
            }
        }

        /// <summary>
        /// вставить элемент сущности
        /// </summary>
        /// <param name="name"> имя сущности </param>
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
                return new JsonResult(_dataService.PutEntityItem(info, item));
            }
        }

        /// <summary>
        /// обновить элемент сущности
        /// </summary>
        /// <param name="name"> имя сущности </param>
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
                return new JsonResult(_dataService.PostEntityItem(info, item, id));
            }
        }

        /// <summary>
        /// удалить элемент сущности
        /// </summary>
        /// <param name="name"> имя сущности </param>
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
                _dataService.DeleteEntityItem(info, id);
                return new JsonResult("OK");
            }
        }
    }
}
