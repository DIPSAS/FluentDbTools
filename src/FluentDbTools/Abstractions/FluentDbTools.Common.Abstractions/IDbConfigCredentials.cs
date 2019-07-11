namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// User and AdminUser credentials
    /// </summary>
    public interface IDbConfigCredentials
    {
        /// <summary>
        /// User Name credentials
        /// </summary>
        string User { get; }

        /// <summary>
        /// User Password credentials
        /// </summary>
        string Password { get; }

        /// <summary>
        /// AdminUser Name credentials
        /// </summary>
        string AdminUser { get; }

        /// <summary>
        /// AdminUser Password credentials
        /// </summary>
        string AdminPassword { get; }
    }
}