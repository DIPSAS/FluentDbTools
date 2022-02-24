using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Contracts;
using DefaultDbConfigValues = FluentDbTools.Contracts.DefaultDbConfigValues;

// ReSharper disable VirtualMemberCallInConstructor

namespace FluentDbTools.Extensions.DbProvider
{
    /// <inheritdoc cref="IDbConfig" />
    public class DbConfig : DbConnectionStringBuilderConfig, IDbConfig
    {

        private bool? IsDatabaseUserDependedOfUserGrantsField = null;

        /// <inheritdoc />
        public DbConfig(
            DefaultDbConfigValues defaultDbConfigValues = null,
            DbConfigCredentials dbConfigCredentials = null)
            : base(defaultDbConfigValues, dbConfigCredentials)
        {

        }

        private string ConnectionStringField;

        /// <inheritdoc />
        public virtual string ConnectionString
        {
            get => ConnectionStringField ?? Defaults.GetDefaultConnectionString() ?? this.BuildConnectionString(false);
            set => ConnectionStringField = value;
        }

        private string AdminConnectionStringField;

        /// <inheritdoc />
        public virtual string AdminConnectionString
        {
            get => AdminConnectionStringField ?? Defaults.GetDefaultAdminConnectionString() ?? this.BuildAdminConnectionString(false);
            set => AdminConnectionStringField = value;
        }

        /// <inheritdoc />
        public virtual IDictionary<string, string> GetAllDatabaseConfigValues(bool reload = false, string sectionName = null)
        {
            return Defaults?.GetAllDatabaseConfigValues(reload, sectionName) ?? new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public virtual string GetConfigValue(params string[] keys)
        {
            return null;
        }

        /// <inheritdoc />
        public virtual IPrioritizedConfigKeys[] GetPrioritizedConfigKeys()
        {
            return new IPrioritizedConfigKeys[] { new PrioritizedConfigKeys() };
        }

        /// <inheritdoc />
        public string ConfigurationDelimiter { get; set; } = ":";

        /// <inheritdoc />
        public IDbConfig ValidateAdminValues()
        {
            if (IsAdminValuesValid)
            {
                return this;
            }

            InvalidAdminValues.ThrowIfInvalidDatabaseAdminValues();
            return this;
        }

        internal InvalidAdminValue[] InvalidAdminValuesInternal;

        /// <inheritdoc />
        public InvalidAdminValue[] InvalidAdminValues => ValidateDatabaseAdminValues();

        /// <inheritdoc />
        public bool IsAdminValuesValid => InvalidAdminValues.Any() == false;

        /// <inheritdoc />
        protected override void OnConfigurationChanged(Func<string[], string> getValueFunc)
        {

            AdminConnectionStringField = null;
            ConnectionStringField = null;
            base.OnConfigurationChanged(getValueFunc);
        }

        private InvalidAdminValue[] ValidateDatabaseAdminValues()
        {
            try
            {
                return InvalidAdminValuesInternal ?? (InvalidAdminValuesInternal = this.ValidateDatabaseAdminValues(throwIfFail: false) ?? Array.Empty<InvalidAdminValue>());
            }
            catch
            {
                return Array.Empty<InvalidAdminValue>();
            }
        }

        /// <inheritdoc />
        public virtual string[] UserGrants => Array.Empty<string>();

        /// <inheritdoc />
        public virtual string[] AdditionalRolesGrants => Array.Empty<string>();

        /// <inheritdoc />
        public virtual bool IsDatabaseUserDependedOfUserGrants
        {
            get
            {
                IsDatabaseUserDependedOfUserGrantsField = IsDatabaseUserDependedOfUserGrantsField ?? (IsDatabaseUserDependedOfUserGrantsField = User.IsNotEmpty() && UserGrants?.Contains(User, StringComparer.OrdinalIgnoreCase) == true);
                return IsDatabaseUserDependedOfUserGrantsField == true;
            }
        }
    }
}