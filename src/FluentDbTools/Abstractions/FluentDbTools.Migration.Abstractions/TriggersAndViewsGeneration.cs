using System.Diagnostics.CodeAnalysis;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Can be used to specify generic generation of triggers and views<br/>
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")] 
    public enum TriggersAndViewsGeneration
    {

        /// <summary>
        /// Disable all Triggers and Views generation
        /// </summary>
        Disabled=0,

        /// <summary>
        /// Enable both Triggers and Views generation
        /// </summary>
        Both=1,

        /// <summary>
        /// Enable only Triggers generation
        /// </summary>
        Triggers=2,

        /// <summary>
        /// Enable only Views generation
        /// </summary>
        Views=3
    }
}