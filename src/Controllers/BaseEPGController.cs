using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Genesis.EPG.Controllers
{
    /// <summary>
    /// базовый контроллер EPG
    /// </summary>
    [Route("epg/[controller]")]
    [ApiController]
    public abstract class BaseEPGController : ControllerBase
    {
        /// <summary>
        /// конфигурация EPG
        /// </summary>
        protected readonly EPGConfig _config;

        /// <summary>
        /// коннектор по умолчанию
        /// </summary>
        protected static Connector _defaultConnector;

        protected static object _lock = new object();

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="config"> конфигурация EPG </param>
        public BaseEPGController(EPGConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// получить коннектор БД
        /// </summary>
        /// <returns></returns>
        protected Connector GetConnector()
        {
            var res = new Connector(_config.ConnectionString);

            res.Open();

            return res;
        }

        /// <summary>
        /// получить коннектор БД по умолчанию
        /// </summary>
        /// <returns></returns>
        protected Connector GetDefaultConnector()
        {
            lock (_lock)
            {
                if (_defaultConnector == null) _defaultConnector = new Connector(_config.ConnectionString);

                var res = _defaultConnector;

                if (res.Connection == null)
                {
                    res.Open();
                }
                else
                {
                    switch (res.Connection.State)
                    {
                        case System.Data.ConnectionState.Broken:
                        case System.Data.ConnectionState.Closed:
                            res.Open();
                            break;
                    }
                }

                return res;
            }
        }
    }
}
