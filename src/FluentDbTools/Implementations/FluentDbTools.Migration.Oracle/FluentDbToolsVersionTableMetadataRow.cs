using System;
using System.Runtime.Serialization;
#pragma warning disable 1591

namespace FluentDbTools.Migration.Oracle
{
    /// <summary>
    /// Contains FluentMigration upgrade version information. <br/>
    /// </summary>
    [DataContract]
    public class FluentDbToolsVersionTableMetadataRow
    {
        public FluentDbToolsVersionTableMetadataRow(decimal version, DateTime appliedOn, string description)
        {
            Version = version;
            AppliedOn = appliedOn;
            Description = description;
        }

        public FluentDbToolsVersionTableMetadataRow()
        {
            
        }
        /// <summary>
        /// The Unique Version to the migration step.<br/>
        /// </summary>
        [DataMember]
        public decimal Version { get; set; }

        /// <summary>
        /// Timestamp of when the migration step was applied<br/>
        /// </summary>
        [DataMember]
        public DateTime AppliedOn { get; set; }

        /// <summary>
        /// Contains information about the migration step applied<br/>
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}