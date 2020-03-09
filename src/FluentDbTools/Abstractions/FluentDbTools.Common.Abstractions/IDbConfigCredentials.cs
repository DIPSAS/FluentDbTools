namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// User and AdminUser credentials
    /// </summary>
    public interface IDbConfigCredentials
    {
        /// <summary>
        /// <para>User Name credentials</para>
        /// <remarks>Default config-key: 'database:user' </remarks>
        /// </summary>
        string User { get; }

        /// <summary>
        /// <para>User Password credentials</para>
        /// <remarks>Default config-keys: ['database:password', 'database:user'] </remarks>
        /// </summary>
        string Password { get; }

        /// <summary>
        /// <para>AdminUser Name credentials</para>
        /// <remarks>Default config-key: 'database:adminUser' </remarks>
        /// </summary>
        string AdminUser { get; }

        /// <summary>
        /// <para>AdminUser Password credentials</para>
        /// <remarks>Default config-keys: ['database:adminPassword', 'database:adminUser'] </remarks>
        /// </summary>
        string AdminPassword { get; }
    }
}