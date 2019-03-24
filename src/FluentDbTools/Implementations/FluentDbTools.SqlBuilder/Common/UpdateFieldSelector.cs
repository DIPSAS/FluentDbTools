using System.Linq;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.SqlBuilder.Common
{
    internal class UpdateFieldSelector<TClass> : BaseFieldSetterSelector<TClass>
    {
        public UpdateFieldSelector(IDbConfigDatabaseTargets dbConfig)
            :base(dbConfig)
        {
        }
        public override string Build()
        {
            var fields = string.Join(", ", FieldsList.Select(x => $"{x.FieldName} = {CreateFieldValue(x)}"));

            return $"UPDATE {GetTableNameWithSchemaNamePrefix} SET {fields}";
        }

        private string CreateFieldValue(Field field)
        {
            return field.IsParameterized ? GetParameterPrefix() + field.Value : field.Value;
        }

        private string GetParameterPrefix()
        {
            return SqlBuilderHelper.GetParameterPrefixIfNull(DbConfig?.GetParameterPrefix());
        }

    }
}