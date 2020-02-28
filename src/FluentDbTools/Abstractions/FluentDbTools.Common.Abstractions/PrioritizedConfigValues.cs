namespace FluentDbTools.Common.Abstractions
{
    public class PrioritizedConfigValues : IPrioritizedConfigValues 
    {
        public SupportedDatabaseTypes? GetDbType()
        {
            return null;
        }

        public virtual string GetDbSchema()
        {
            return null;
        }

        public string GetDbSchemaPrefixIdString()
        {
            return null;
        }

        public virtual string GetDbDatabaseName()
        {
            return null;
        }

        public virtual string GetDbUser()
        {
            return null;
        }

        public virtual string GetDbPassword()
        {
            return null;
        }

        public virtual string GetDbAdminUser()
        {
            return null;
        }

        public virtual string GetDbAdminPassword()
        {
            return null;
        }

        public virtual string GetDbHostname()
        {
            return null;
        }

        public virtual string GetDbPort()
        {
            return null;
        }

        public virtual string GetDbDataSource()
        {
            return null;
        }

        public virtual string GetDbConnectionTimeout()
        {
            return null;
        }

        public virtual bool? GetDbPooling()
        {
            return null;
        }

        public virtual string GetDbConnectionString()
        {
            return null;
        }

        public virtual string GetDbAdminConnectionString()
        {
            return null;
        }
    }
}