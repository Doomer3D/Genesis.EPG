using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        private readonly EntityService _entityService;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        /// <param name="entityService"> сервис сущностей </param>
        public EntityController(
            EPGConfig config,
            EntityService entityService) : base(config)
        {
            _entityService = entityService;
        }

        /// <summary>
        /// получить список справочников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return new JsonResult("EntityController.Get");
        }
    }
}
