using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Extensions.DbProvider
{
    /// <inheritdoc cref="IConfigurationChangedHandler" />
    /// <inheritdoc cref="IDisposable" />
    public class ConfigurationChangedHandler : IConfigurationChangedHandler, IDisposable
    {
        private readonly IList<Action<Func<string[], string>>> RegisteredConfigurationChangedCallback = new List<Action<Func<string[], string>>>();

        /// <inheritdoc cref="IConfigurationChangedHandler.RegisterConfigurationChangedCallback" />
        public void RegisterConfigurationChangedCallback(Action<Func<string[], string>> getValueFunc)
        {
            UnRegisterConfigurationChangedCallback(getValueFunc);
            RegisteredConfigurationChangedCallback.Add(getValueFunc);
        }

        /// <inheritdoc cref="IConfigurationChangedHandler.UnRegisterConfigurationChangedCallback" />
        public void UnRegisterConfigurationChangedCallback(Action<Func<string[], string>> getValueFunc)
        {
            RegisteredConfigurationChangedCallback.Remove(getValueFunc);
        }

        /// <inheritdoc cref="IConfigurationChangedHandler.RaiseConfigurationChanged" />
        public virtual void RaiseConfigurationChanged(Func<string[], string> getValueFunc)
        {
            foreach (var registeredReloadToken in RegisteredConfigurationChangedCallback.ToArray())
            {
                registeredReloadToken.Invoke(getValueFunc);
            }
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            RegisteredConfigurationChangedCallback.Clear();
        }
    }
}