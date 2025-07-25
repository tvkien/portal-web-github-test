using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables.Parsers
{
    public class StringParserExpressionFactory
    {
        public static Expression GetExpression(ConstantExpression value, ParameterExpression parameterExpression, PropertyInfo propertyToSearch, ComparisonType comparisonType,bool forceSearchInString)
        {
            Expression notNullExp = Expression.NotEqual(Expression.Property(Expression.Property(parameterExpression, "Val"), propertyToSearch), Expression.Constant(null, typeof(string)));
            Expression searchExp;
            if (forceSearchInString)
            {
                searchExp = GetLowweredExpression(value, parameterExpression, propertyToSearch, "Contains", new[] { typeof(string) });
                return Expression.AndAlso(notNullExp, searchExp);
            }
            else
            {

                switch (comparisonType)
                {
                    case ComparisonType.NotEquals:
                    case ComparisonType.Equals:
                    case ComparisonType.GreaterThanOrEquals:
                    case ComparisonType.GreaterThan:
                    case ComparisonType.LessThanOrEquals:
                    case ComparisonType.LessThan:
                        //searchExp = ComparableParserExpressionFactory.GetExpression(value, parameterExpression,
                        //                                                       propertyToSearch, comparisonType);
                        searchExp = GetLowweredExpression(value, parameterExpression, propertyToSearch, "Equals", new[] { typeof(string) });
                        break;
                    case ComparisonType.StartsWith:
                        searchExp = GetLowweredExpression(value, parameterExpression, propertyToSearch, "StartsWith",
                                                     new[] { typeof(string) });
                        break;
                    case ComparisonType.EndsWith:
                        searchExp = GetLowweredExpression(value, parameterExpression, propertyToSearch, "EndsWith",
                                                     new[] { typeof(string) });
                        break;
                    case ComparisonType.Contains: //TuanVo :Add new search contains
                        searchExp = GetLowweredExpression(value, parameterExpression, propertyToSearch, "Contains",
                                                     new[] { typeof(string) });
                        break;
                    default:
                        searchExp = GetLowweredExpression(value, parameterExpression, propertyToSearch, "StartsWith",
                                                     new[] { typeof(string) });
                        break;
                }

                return Expression.AndAlso(notNullExp, searchExp);
            }
        }

        private static Expression GetLowweredExpression(ConstantExpression value, ParameterExpression parameterExpression, PropertyInfo propertyToSearch, string type, Type[] paramters = null)
        {
            var toLowerCall = Expression.Call(Expression.Property(Expression.Property(parameterExpression, "Val"), propertyToSearch), "ToLower", new Type[0]);
            return Expression.Call(toLowerCall, GetMethod(type, paramters), value);
        }

        private static MethodInfo GetMethod(string type, Type[] paramters)
        {
            return paramters == null
                       ? typeof (string).GetMethod(type)
                       : typeof (string).GetMethod(type, paramters);
        }
    }
}