using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator.Expressions;
using FluentMigrator.Model;

namespace DIPS.FluentDbTools.Migration.Postgres
{
    internal static class PostgresHelper
    {
        public static void SetPrivateFieldValue<T>(this T instance, string fieldName, object value) 
        {
            var field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(instance, value);
            }
        }

        public static void ToLower(this ColumnDefinition definition)
        {
            definition.Name = definition.Name.ToLower();
            definition.TableName = definition.TableName.ToLower();
        }

        public static void ToLower(this ConstraintDefinition definition)
        {
            definition.TableName = definition.TableName.ToLower();
            definition.ConstraintName = definition.ConstraintName.ToLower();
            definition.Columns = definition.Columns.ToLower();
        }

        public static void ToLower(this IndexColumnDefinition definition)
        {
            definition.Name = definition.Name.ToLower();
        }

        public static void ToLower(this ForeignKeyDefinition definition)
        {
            definition.Name = definition.Name.ToLower();
            definition.ForeignTable = definition.ForeignTable.ToLower();
            definition.PrimaryTable = definition.PrimaryTable.ToLower();
            definition.ForeignColumns = definition.ForeignColumns.ToLower();
            definition.PrimaryColumns = definition.PrimaryColumns.ToLower();
        }


        public static void ToLower(this ICollection<IndexColumnDefinition> collection)
        {
            foreach (var definition in collection)
            {
                definition.ToLower();
            }
        }

        public static void ToLower(this IndexDefinition definition)
        {
            definition.Name = definition.Name.ToLower();
            definition.TableName = definition.TableName.ToLower();
            definition.Columns.ToLower();
        }
        
        public static AlterTableExpression ToLower(this AlterTableExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();

            return expresstion;
        }

        public static CreateTableExpression ToLower(this CreateTableExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            foreach (var column in expresstion.Columns)
            {
                column.ToLower();
            }
            return expresstion;
        }

        public static AlterColumnExpression ToLower(this AlterColumnExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            expresstion.Column.ToLower();
            return expresstion;
        }

        
        public static CreateForeignKeyExpression ToLower(this CreateForeignKeyExpression expresstion)
        {
            expresstion.ForeignKey.ToLower();
            return expresstion;
        }

        public static CreateIndexExpression ToLower(this CreateIndexExpression expresstion)
        {
            expresstion.Index.ToLower();
            return expresstion;
        }
        

        public static CreateColumnExpression ToLower(this CreateColumnExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            expresstion.Column.ToLower();
            return expresstion;
        }


        public static CreateSequenceExpression ToLower(this CreateSequenceExpression expresstion)
        {
            expresstion.Sequence.Name = expresstion.Sequence.Name.ToLower();
            return expresstion;
        }
        
        public static AlterDefaultConstraintExpression ToLower(this AlterDefaultConstraintExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            expresstion.ColumnName = expresstion.ColumnName.ToLower();
            return expresstion;
        }

        public static CreateConstraintExpression ToLower(this CreateConstraintExpression expresstion)
        {
            expresstion.Constraint.ToLower();
            return expresstion;
        }
        
        public static DeleteColumnExpression ToLower(this DeleteColumnExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            expresstion.ColumnNames = expresstion.ColumnNames.ToLower();
            return expresstion;
        }

        public static DeleteConstraintExpression ToLower(this DeleteConstraintExpression expresstion)
        {
            expresstion.Constraint.ToLower();
            return expresstion;
        }
        
        public static DeleteDefaultConstraintExpression ToLower(this DeleteDefaultConstraintExpression expresstion)
        {
            expresstion.ColumnName = expresstion.ColumnName.ToLower();
            expresstion.TableName = expresstion.TableName.ToLower();
            return expresstion;
        }

        public static DeleteForeignKeyExpression ToLower(this DeleteForeignKeyExpression expresstion)
        {
            expresstion.ForeignKey.ToLower();
            return expresstion;
        }
        

        public static InsertDataExpression ToLower(this InsertDataExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            return expresstion;
        }

        public static DeleteDataExpression ToLower(this DeleteDataExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            return expresstion;
        }

        public static DeleteTableExpression ToLower(this DeleteTableExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            return expresstion;
        }

        public static DeleteIndexExpression ToLower(this DeleteIndexExpression expresstion)
        {
            expresstion.Index.ToLower();
            return expresstion;
        }
        
        public static DeleteSequenceExpression ToLower(this DeleteSequenceExpression expresstion)
        {
            expresstion.SequenceName = expresstion.SequenceName.ToLower();
            return expresstion;
        }

        public static RenameColumnExpression ToLower(this RenameColumnExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            expresstion.NewName = expresstion.NewName.ToLower();
            expresstion.OldName = expresstion.OldName.ToLower();
            return expresstion;
        }
        
        public static RenameTableExpression ToLower(this RenameTableExpression expresstion)
        {
            expresstion.NewName = expresstion.NewName.ToLower();
            expresstion.OldName = expresstion.OldName.ToLower();
            return expresstion;
        }
        
        public static UpdateDataExpression ToLower(this UpdateDataExpression expresstion)
        {
            expresstion.TableName = expresstion.TableName.ToLower();
            return expresstion;
        }


        public static ICollection<string> ToLower(this ICollection<string> collection)
        {
            return collection.Select(x => x.ToLower()).ToArray();
        }
    }
}