using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Genesis.EPG.Services;

namespace Genesis.EPG
{
    /// <summary>
    /// ядро EPG
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// конфигурировать сервисы
        /// </summary>
        /// <param name="configuration"> конфигурация системы </param>
        /// <param name="services"> сервисы </param>
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            // регистрируем конфиг EPG
            var config = configuration.GetSection(nameof(EPGConfig)).Get<EPGConfig>();
            config.ConnectionString = configuration.GetConnectionString(config.ConnectionStringName ?? "DB");
            services.AddSingleton(config);

            // регистрируем сервисы
            services.AddScoped<DictionaryService>();
            services.AddScoped<EntityService>();

            services
                .AddMvc()
                .AddApplicationPart(typeof(Core).Assembly);
        }
    }
}
