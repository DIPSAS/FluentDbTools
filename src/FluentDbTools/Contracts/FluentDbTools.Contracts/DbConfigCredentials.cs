using System;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.Contracts
{
    /// <inheritdoc />
    public class DbConfigCredentials : IDbConfigCredentials
    {
        private string UserField;
        private string PasswordField;
        private string AdminUserField;
        private string AdminPasswordField;

        /// <summary>
        /// This will be set if <see cref="DbConfigCredentials"/> is initialized by DependencyInjection<br/>
        /// </summary>
        public bool IgnoreManualCallOnConfigurationChanged { get; set; } = false;

        /// <summary>
        /// Placeholder of available Default function
        /// </summary>
        protected DefaultDbConfigValues Defaults;

        /// <summary>
        /// Initialize class by <paramref name="defaultDbConfigValues"/>
        /// </summary>
        /// <param name="defaultDbConfigValues"></param>
        public DbConfigCredentials(DefaultDbConfigValues defaultDbConfigValues = null)
        {
            Defaults = defaultDbConfigValues ?? new DefaultDbConfigValues();

            if (Defaults.GetDefaultSchema == null)
            {
                Defaults.GetDefaultSchema = () => User;
            }

        }

        /// <inheritdoc />
        public virtual string User
        {
            get => UserField ?? Defaults.GetDefaultUser?.Invoke();
            set => UserField = value;
        }

        /// <inheritdoc />
        public virtual string Password
        {
            get => PasswordField ?? Defaults.GetDefaultPassword?.Invoke();
            set => PasswordField = value;
        }

        /// <inheritdoc />
        public virtual string AdminUser
        {
            get => AdminUserField ?? Defaults.GetDefaultAdminUser?.Invoke();
            set => AdminUserField = value;
        }

        /// <inheritdoc />
        public virtual string AdminPassword
        {
            get => AdminPasswordField ?? Defaults.GetDefaultAdminPassword?.Invoke();
            set => AdminPasswordField = value;
        }

        /// <summary>
        /// Can be called when configuration is changed/reloaded
        /// </summary>
        /// <param name="getValueFunc"></param>
        public virtual void OnConfigurationChanged(Func<string[], string> getValueFunc)
        {
            AdminPassword = null;
            AdminUser = null;
            User = null;
            Password = null;
        }
    }
}