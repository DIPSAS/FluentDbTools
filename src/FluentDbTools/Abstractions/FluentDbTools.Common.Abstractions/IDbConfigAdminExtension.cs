using System;

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Database Admin extension
    /// </summary>
    public interface IDbConfigAdminExtension
    {
        /// <summary>
        /// <para>Validate AdminUser and AdminPassword.</para>
        /// If both AdminUser and AdminPassword are valid, <see cref="IDbConfig"/> is returned<br/><br/>
        /// If one of them is invalid, an exception will be thrown:<br/>
        /// - If both AdminUser and AdminPassword is invalid, an <see cref="AggregateException"></see> will be thrown<br/>
        /// - If only one of AdminUser or AdminPassword is invalid, an <see cref="ArgumentNullException"></see> or <see cref="ArgumentException"></see> will be thrown
        /// </summary>
        /// <returns>
        /// Return <see cref="IDbConfig"/> if validation succeed.<br/>
        /// If validation fail, exception is thrown. See possible extensions....
        /// </returns>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        IDbConfig ValidateAdminValues();

        /// <summary>
        /// Contain detail information of latest Admin validation<br/>
        /// Null or Empty indicate that everything is ok
        /// </summary>
        InvalidAdminValue[] InvalidAdminValues { get; }

        /// <summary>
        /// Return a TRUE if validation failed, elsewhere FALSE 
        /// </summary>
        bool IsAdminValuesValid { get; }
    }
}