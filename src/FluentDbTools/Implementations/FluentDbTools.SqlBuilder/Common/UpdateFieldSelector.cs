using System.Linq;
using FluentDbTools.Common.Abstractions;

namespace FluentDbTools.SqlBuilder.Common
{
    internal class UpdateFieldSelector<TClass> : BaseFieldSetterSelector<TClass>
    {
        public UpdateFieldSelector(IDbConfigSchemaTargets dbConfigConfig, string tableName = null)
            :base(dbConfigConfig, tableName)
        {
        }
        public override string Build()
        {
            var fields = string.Join(", ", FieldsList.Select(x => $"{x.FieldName} = {CreateFieldValue(x)}"));

            return $"UPDATE {TableNameWithSchemaName} SET {fields}";
        }

        private string CreateFieldValue(Field field)
        {
            return field.IsParameterized ? $"{GetParameterPrefix()}{field.Value}" : field.Value;
        }

        private string GetParameterPrefix()
        {
            return SqlBuilderHelper.GetParameterPrefixIfNull(DbConfigConfig?.GetParameterPrefix());
        }

    }
}