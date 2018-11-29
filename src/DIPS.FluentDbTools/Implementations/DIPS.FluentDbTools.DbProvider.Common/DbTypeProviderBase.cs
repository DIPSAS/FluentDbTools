using System.Collections.Generic;
using System.Data;
using System.Linq;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Config;
using DIPS.FluentDbTools.DbProvider.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DIPS.FluentDbTools.DbProvider.Common
{
    public abstract class DatabaseTypeProviderBase : IDatabaseTypeProvider
    {
        private readonly IConfiguration Configuration;

        protected string User => Configuration.GetDbUser();
        protected string Password => Configuration.GetDbPassword();
        protected string AdminUser => Configuration.GetDbAdminUser();
        protected string AdminPassword => Configuration.GetDbAdminPassword();
        protected string Host => Configuration.GetDbHostname();
        protected string Port => Configuration.GetDbPort();
        protected string Database => Configuration.GetDbName();
        protected string AdminDatabase => Configuration.GetDbAdminName();
        protected bool Pooling => Configuration.GetDbPooling();

        private IDbConnection AdminDbConnection;
        private IDbTransaction AdminDbTransaction;
        private IDbConnection UserDbConnection;
        private IDbTransaction UserDbTransaction;

        public abstract SupportedDatabaseTypes DatabaseType { get; }

        public abstract string AdminConnectionString { get; }
        public abstract string ConnectionString { get; }
        
        protected DatabaseTypeProviderBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public IDbConnection GetDbConnection(bool withAdminPrivileges = false)
        {
            return withAdminPrivileges ? GetDbConnectionWithAdminPrivileges() : GetDbConnectionWithUserPrivileges();
        }

        public IDbTransaction GetDbTransaction(bool withAdminPrivileges = false)
        {
            return withAdminPrivileges ? GetDbTransactionWithAdminPrivileges() : GetDbTransactionWithUserPrivileges();
        }
        
        private IDbConnection GetDbConnectionWithUserPrivileges()
        {
            UserDbConnection = UserDbConnection ?? CreateAndOpenDbConnection(false);
            return UserDbConnection;
        }

        private IDbTransaction GetDbTransactionWithUserPrivileges()
        {
            UserDbTransaction = UserDbTransaction ?? GetDbConnectionWithUserPrivileges().BeginTransaction();
            return UserDbTransaction;
        }
        
        private IDbConnection GetDbConnectionWithAdminPrivileges()
        {
            AdminDbConnection = AdminDbConnection ?? CreateAndOpenDbConnection(true);
            return AdminDbConnection;
        }

        private IDbTransaction GetDbTransactionWithAdminPrivileges()
        {
            AdminDbTransaction = AdminDbTransaction ?? GetDbConnectionWithAdminPrivileges().BeginTransaction();
            return AdminDbTransaction;
        }

        private IDbConnection CreateAndOpenDbConnection(bool withAdminPrivileges)
        {
            var dbConnection = CreateDbConnection(withAdminPrivileges);
            dbConnection.Open();
            return dbConnection;
        }

        protected abstract IDbConnection CreateDbConnection(bool withAdminPrivileges);
        
        public void Dispose()
        {
            UserDbTransaction?.Dispose();
            UserDbConnection?.Dispose();
            AdminDbTransaction?.Dispose();
            AdminDbConnection?.Dispose();
        }
    }

    public static class DbTypeTranslators
    {
        private static readonly List<IDbTypeTranslator> TranslatorList = new List<IDbTypeTranslator>();
        public static IReadOnlyList<IDbTypeTranslator> Translators => TranslatorList;

        public static void AddTranslator(IDbTypeTranslator translator)
        {
            if (TranslatorList.All(x => x.DatabaseType != translator.DatabaseType))
            {
                TranslatorList.Add(translator);
            }
        }
    }

}