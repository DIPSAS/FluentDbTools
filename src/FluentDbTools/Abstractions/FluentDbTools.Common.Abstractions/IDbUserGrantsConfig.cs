namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Hold User grants configuration
    /// </summary>
    public interface IDbUserGrantsConfig
    {
        /// <summary>
        /// Required User grants
        /// </summary>
        string[] UserGrants { get; }

        /// <summary>
        /// Additional grants to add to selected users
        /// </summary>
        string[] AdditionalRolesGrants { get; }

        /// <summary>
        /// Tells if 'Database:User' is in <see cref="UserGrants"/> array. If true, Connection with 'Database:User' should be delayed until AddUserGrant migration is executed
        /// </summary>
        bool IsDatabaseUserDependedOfUserGrants { get; }
    }
}