using System;

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Provide a mechanism for handling configuration-reload of changed config-files
    /// </summary>
    public interface IConfigurationChangedHandler
    {
        /// <summary>
        /// Can be used to register callbacks to be run when configuration is changed/reloaded
        /// </summary>
        /// <param name="getValueFunc"></param>
        void RegisterConfigurationChangedCallback(Action<Func<string[], string>> getValueFunc);

        /// <summary>
        /// Can be used to remove callback registrations
        /// </summary>
        /// <param name="getValueFunc"></param>
        void UnRegisterConfigurationChangedCallback(Action<Func<string[], string>> getValueFunc);

        /// <summary>
        /// Can be called when configuration is changed
        /// </summary>
        /// <param name="getValueFunc"></param>
        void RaiseConfigurationChanged(Func<string[], string> getValueFunc);
    }
}