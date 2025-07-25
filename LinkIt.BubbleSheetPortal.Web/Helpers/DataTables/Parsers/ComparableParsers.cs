using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables.Parsers
{
    public class ComparableParsers : IParser
    {
        private readonly SearchExpression expression;
        private readonly PropertyInfo propertyToSearch;
        private readonly ParameterExpression parameterExpression;

        public ComparableParsers(SearchExpression expression, PropertyInfo propertyToSearch, ParameterExpression parameterExpression)
        {
            this.expression = expression;
            this.propertyToSearch = propertyToSearch;
            this.parameterExpression = parameterExpression;
        }

        public Expression GetSearchExpression()
        {
            try
            {
                var type = Convert.ChangeType(expression.Value, propertyToSearch.PropertyType);
                var converted = Expression.Constant(type);
                return ComparableParserExpressionFactory.GetExpression(converted, parameterExpression, propertyToSearch, expression.Type);
            }
            catch (Exception)
            {
                return FalseComparison.ReturnsFalse;
            }
        }
    }
}