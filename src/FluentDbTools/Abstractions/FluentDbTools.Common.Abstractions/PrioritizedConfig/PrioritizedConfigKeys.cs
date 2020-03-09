namespace FluentDbTools.Common.Abstractions.PrioritizedConfig
{
    /// <summary>
    /// <inheritdoc cref="IPrioritizedConfigKeys"/>
    /// <para>_______________________________________________________________________________</para>
    /// <para>All <see cref="IPrioritizedConfigKeys"/>-methods in this class return <c>null</c></para>
    /// <para>It is recommended to use this class as a base-class and override those methods you want to implement..</para>
    /// </summary>
    public class PrioritizedConfigKeys : IPrioritizedConfigKeys
    {
        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbTypeKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbTypeKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbSchemaKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbSchemaKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbSchemaPasswordKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbSchemaPasswordKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbSchemaPrefixIdStringKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbSchemaPrefixIdStringKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbSchemaUniquePrefixIdStringKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbSchemaUniquePrefixIdStringKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbDatabaseNameKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbDatabaseNameKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbUserKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbUserKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbPasswordKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbPasswordKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbAdminUserKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbAdminUserKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbAdminPasswordKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbAdminPasswordKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbHostnameKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbHostnameKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbPortKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbPortKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbDataSourceKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbDataSourceKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbConnectionTimeoutKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbConnectionTimeoutKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbPoolingKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbPoolingKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbConnectionStringKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbConnectionStringKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.GetDbAdminConnectionStringKeys"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>This method return <c>null</c>.</para>
        /// <para>Override this method to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDbAdminConnectionStringKeys()
        {
            return null;
        }

        /// <summary>
        /// <inheritdoc cref="IPrioritizedConfigKeys.Order"/>
        /// <para>_______________________________________________________________________________</para>
        /// <para>The default Order-values is <c>99</c>.</para>
        /// <para>Override this property to customize it for your implementation</para>
        /// </summary>
        /// <returns></returns>
        public virtual int Order { get; set; } = 99;
    }
}