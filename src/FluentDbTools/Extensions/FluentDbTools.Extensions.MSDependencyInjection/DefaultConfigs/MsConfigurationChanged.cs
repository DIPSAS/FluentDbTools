using System;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    /// <inheritdoc />
    internal class MsConfigurationChangedHandler : ConfigurationChangedHandler
    {
        private readonly IConfiguration Configuration;
        private readonly IDisposable ChangeCallback;
        private IChangeToken ChangeTokenField;
        private IChangeToken ChangeToken => ChangeTokenField ??  (ChangeTokenField = Configuration.GetReloadToken());

        /// <summary>
        /// The constructor will register configuration callback <br/>
        /// Resolve <see cref="IChangeToken"/> from <see cref="IConfiguration.GetReloadToken()"/><br/>
        /// Finally register Action <see cref="IConfigurationChangedHandler.RaiseConfigurationChanged"/> to be called when config reloads with {<see cref="IChangeToken.RegisterChangeCallback"/>}
        /// </summary>
        /// <param name="configuration"></param>
        public MsConfigurationChangedHandler(IConfiguration configuration)
        {
            Configuration = configuration;
            ChangeCallback = ChangeToken.RegisterChangeCallback(state => RaiseConfigurationChanged(args => Configuration.GetConfigValue(args)), Configuration);
        }

        /// <inheritdoc />
        public override void RaiseConfigurationChanged(Func<string[], string> getValueFunc)
        {
            if (ChangeToken.ActiveChangeCallbacks && ChangeToken.HasChanged)
            {
                base.RaiseConfigurationChanged(getValueFunc);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            ChangeCallback?.Dispose();
        }
    }
}