﻿using System.Collections.Generic;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    public class MsDbConfig : DbConfig
    {
        private readonly IConfiguration Configuration;

        public MsDbConfig(
            IConfiguration configuration, 
            MsDefaultDbConfigValues defaultDbConfigValues = null) :
            base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration))
        {
            Configuration = configuration;
        }

        public override IDictionary<string, string> GetAllDatabaseConfigValues() =>
            Configuration.GetDbSection().GetDbAllConfigValues();

    }
}