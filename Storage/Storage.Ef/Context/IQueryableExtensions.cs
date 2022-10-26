using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Zidium.Storage.Ef
{
    public static class IQueryableExtensions
    {
        public static ParametrizedSql ToParametrizedSql(this IQueryable query)
        {
            string relationalCommandCacheText = "_relationalCommandCache";
            string selectExpressionText = "_selectExpression";
            string querySqlGeneratorFactoryText = "_querySqlGeneratorFactory";
            string relationalQueryContextText = "_relationalQueryContext";

            string cannotGetText = "Cannot get";

            var enumerator = query.Provider.Execute<IEnumerable>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private(relationalCommandCacheText) as RelationalCommandCache;
            var queryContext = enumerator.Private<RelationalQueryContext>(relationalQueryContextText) ?? throw new InvalidOperationException($"{cannotGetText} {relationalQueryContextText}");
            var parameterValues = queryContext.ParameterValues;

            string sql;
            IList<SqlParameter> parameters;
            if (relationalCommandCache != null)
            {
                var command = relationalCommandCache.GetRelationalCommandTemplate(parameterValues);
                var parameterNames = new HashSet<string>(command.Parameters.Select(p => p.InvariantName));
                sql = command.CommandText;
                parameters = parameterValues.Where(pv => parameterNames.Contains(pv.Key)).Select(ToSqlParameter).ToList();
            }
            else
            {
                SelectExpression selectExpression = enumerator.Private<SelectExpression>(selectExpressionText) ?? throw new InvalidOperationException($"{cannotGetText} {selectExpressionText}");
                IQuerySqlGeneratorFactory factory = enumerator.Private<IQuerySqlGeneratorFactory>(querySqlGeneratorFactoryText) ?? throw new InvalidOperationException($"{cannotGetText} {querySqlGeneratorFactoryText}");

                var sqlGenerator = factory.Create();
                var command = sqlGenerator.GetCommand(selectExpression);
                sql = command.CommandText;
                parameters = parameterValues.Select(ToSqlParameter).ToList();
            }

            return new ParametrizedSql()
            {
                Sql = sql,
                Parameters = parameters
            };
        }

        private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, bindingFlags)?.GetValue(obj);

        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, bindingFlags)?.GetValue(obj);

        private static SqlParameter ToSqlParameter(KeyValuePair<string, object> pv)
        {
            if (pv.Value is object[] parameters && parameters.Length == 1 && parameters[0] is SqlParameter parameter)
                return parameter;

            var pvValueType = pv.Value.GetType();
            if (pvValueType.IsArray && pvValueType.GetElementType().IsEnum)
                return new SqlParameter("@" + pv.Key, (pv.Value as IEnumerable).Cast<Enum>().Select(t => (int)Convert.ChangeType(t, typeof(int))).ToArray());

            return new SqlParameter("@" + pv.Key, pv.Value);
        }

        public class ParametrizedSql
        {
            public string Sql;

            public IEnumerable<SqlParameter> Parameters;
        }
    }
}
