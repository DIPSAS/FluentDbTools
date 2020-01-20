using FluentDbTools.Common.Abstractions;
using FluentDbTools.SqlBuilder.Abstractions.Common;

namespace FluentDbTools.SqlBuilder.Common
{
    internal class WhereFieldSelectorWithSelect<TClass> : WhereFieldSelector<TClass>
    {
        private readonly string Prefix;

        public WhereFieldSelectorWithSelect(string prefix, IDbConfigSchemaTargets dbConfigConfig) : base(dbConfigConfig)
        {
            Prefix = prefix;
        }

        protected override string CreateWhereFieldStringForParameter(string field, string paramName, OP whereOperator)
        {
            return $"{GetAliasForType()}.{field} {SqlBuilderHelper.GetStringForOperator(whereOperator)} {DbConfigConfig.WithParameters(paramName)}";
        }

        protected override string CreateWhereFieldStringForValue(string field, OP whereOperator, params string[] value)
        {
            if (whereOperator == OP.NULL_OR_DI)
            {
                return string.Format(
                    "({0}.{1} IS NULL OR {0}.{1} {2} {3})",
                    GetAliasForType(),
                    field,
                    SqlBuilderHelper.GetStringForOperator(whereOperator),
                    value);
            }
            if (whereOperator == OP.NOT_IN && value.Length == 0)
            {
                return "/* NOT IN () */ 1 = 1";
            }

            if (whereOperator == OP.IN && value.Length == 0)
            {
                return "/* IN () */ 1 = 0";
            }

            return
                $"{GetAliasForType()}.{field} {SqlBuilderHelper.GetStringForOperator(whereOperator)} {(value != null ? GetStringValueByOperator(whereOperator, value) : GetNullStringValueByOperator(whereOperator))}";
        }

        private string GetAliasForType()
        {
            return Prefix ?? SqlBuilderHelper.GetAliasForType<TClass>();
        }
    }
}