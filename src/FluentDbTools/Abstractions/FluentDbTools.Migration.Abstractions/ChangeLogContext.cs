using System;

namespace FluentDbTools.Migration.Abstractions
{
    public class ChangeLogContext
    {
        public string GlobalId { get; set; }
        public string KeyType { get; set; }
        public string ShortName { get; set; }
        public string AnonymousOperation { get; set; }
        public string SchemaPrefix { get; set; }

        public string[] AdditionalContext { get; }

        public ChangeLogContext()
        {
            AnonymousOperation = "NOOP";
            AdditionalContext = Array.Empty<string>();
        }
    }
}