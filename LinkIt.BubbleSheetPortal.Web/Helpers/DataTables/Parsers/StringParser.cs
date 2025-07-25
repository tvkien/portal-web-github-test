using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables.Parsers
{
    public class StringParser : IParser
    {
        private readonly SearchExpression expression;
        private readonly PropertyInfo propertyToSearch;
        private readonly ParameterExpression paramExpression;
        private readonly bool forceSearchInString;
        public StringParser(SearchExpression expression, PropertyInfo propertyToSearch, ParameterExpression paramExpression, bool forceSearchInString)
        {
            this.expression = expression;
            this.propertyToSearch = propertyToSearch;
            this.paramExpression = paramExpression;
            this.forceSearchInString = forceSearchInString;
        }

        public Expression GetSearchExpression()
        {
            try
            {
                var value = Expression.Constant(expression.Value.ToLower());
                return StringParserExpressionFactory.GetExpression(value, paramExpression, propertyToSearch, expression.Type, forceSearchInString);
            }
            catch (Exception)
            {
                return FalseComparison.ReturnsFalse;
            }
        }
    }
}