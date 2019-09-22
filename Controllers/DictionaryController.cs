using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        private readonly DictionaryService _dictionaryService;

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        /// <param name="dictionaryService"> сервис справочников </param>
        public DictionaryController(
            EPGConfig config,
            DictionaryService dictionaryService) : base(config)
        {
            _dictionaryService = dictionaryService;
        }

        /// <summary>
        /// получить список справочников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return new JsonResult("DictionaryController.Get");
        }
    }
}
