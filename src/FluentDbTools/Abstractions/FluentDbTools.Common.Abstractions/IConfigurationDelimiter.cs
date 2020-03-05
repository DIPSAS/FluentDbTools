using System.Diagnostics.CodeAnalysis;

namespace FluentDbTools.Common.Abstractions
{
    /// <summary>
    /// Configuration delimiter Api
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public interface IConfigurationDelimiter
    {
        /// <summary>
        /// The configuration delimiter - Default ":"
        /// </summary>
        string Delimiter { get; }

        /// <summary>
        /// <para>The configuration delimiter to convert from</para>
        /// <para> ex: ":" Can be replaced with "."</para>
        /// <para> ex: "." Can be replaced with ":"</para>
        /// </summary>
        string DelimiterToConvertFrom { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        string[] ReplaceDelimiter(params string[] keys);
    }
}