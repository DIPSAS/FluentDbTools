namespace FluentDbTools.Common.Abstractions
{
    public interface IDbConfig
    {
        SupportedDatabaseTypes DbType { get; }
        
        string User{ get; }
        
        string Password { get; }
        
        string AdminUser { get; }
        
        string AdminPassword { get; }
        
        string Hostname { get; }
        
        string Port { get; }
        
        string DatabaseConnectionName { get; }
        
        bool Pooling { get; }
        
        string Schema { get; }
        
        string DefaultTablespace { get; }
        
        string TempTablespace { get; }
    }
}