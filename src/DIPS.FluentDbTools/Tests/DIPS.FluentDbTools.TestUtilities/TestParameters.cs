using System.Collections.Generic;
using DIPS.FluentDbTools.Common.Abstractions;

namespace DIPS.FluentDbTools.TestUtilities
{
    public static class TestParameters
    {
        public static IEnumerable<object[]> DbParameters =>
            new List<object[]>
            {
                new object[] { SupportedDatabaseTypes.Postgres },
                new object[] { SupportedDatabaseTypes.Oracle }
            };
    }
}