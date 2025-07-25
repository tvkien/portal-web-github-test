using System.Linq.Expressions;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables.Parsers
{
    public class ComparableParserExpressionFactory
    {
        public static Expression GetExpression(ConstantExpression value, ParameterExpression parameterExpression, PropertyInfo propertyToSearch, ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.GreaterThan:
                    return Expression.GreaterThan(Expression.Property(Expression.Property(parameterExpression, "Val"),propertyToSearch), value);
                case ComparisonType.LessThan:
                    return Expression.LessThan(Expression.Property(Expression.Property(parameterExpression, "Val"),propertyToSearch), value);
                case ComparisonType.GreaterThanOrEquals:
                    return Expression.GreaterThanOrEqual(Expression.Property(Expression.Property(parameterExpression, "Val"),propertyToSearch), value);
                case ComparisonType.LessThanOrEquals:
                    return Expression.LessThanOrEqual(Expression.Property(Expression.Property(parameterExpression, "Val"),propertyToSearch), value);
                case ComparisonType.NotEquals:
                    return Expression.NotEqual(Expression.Property(Expression.Property(parameterExpression, "Val"),propertyToSearch), value);
                default:
                    return Expression.Equal(Expression.Property(Expression.Property(parameterExpression, "Val"),propertyToSearch), value);
            }
        }
    }
}