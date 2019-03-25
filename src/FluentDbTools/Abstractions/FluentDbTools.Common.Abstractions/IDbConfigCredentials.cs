namespace FluentDbTools.Common.Abstractions
{
    public interface IDbConfigCredentials
    {
        string User { get; }

        string Password { get; }

        string AdminUser { get; }

        string AdminPassword { get; }
    }
}