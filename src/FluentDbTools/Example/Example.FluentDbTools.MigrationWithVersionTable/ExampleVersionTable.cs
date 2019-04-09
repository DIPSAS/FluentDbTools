using System;
namespace Example.FluentDbTools.MigrationWithVersionTable
{
    public class ExampleVersionTable
    {
        public string Version { get; set; }
        public DateTimeOffset AppliedOn { get; set; }
        public string Description { get; set; }
    }
}
