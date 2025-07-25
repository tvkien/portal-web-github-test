using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables.Parsers;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables
{
    public class ParseFactory
    {
        public static IParser GetParser(string searchText, PropertyInfo propertyToSearch, ParameterExpression paramExpression,bool forceSearchInString, bool isDefaultContain = false)
        {
            var expression = GetExpression(searchText, isDefaultContain);
            if (propertyToSearch.PropertyType == typeof(string))
            {
                var value = new StringParser(expression, propertyToSearch, paramExpression, forceSearchInString);
                return value;
            }
            if (typeof(IComparable).IsAssignableFrom(propertyToSearch.PropertyType))
            {
                return new ComparableParsers(expression, propertyToSearch, paramExpression);
            }

            return new FalseComparison();
        }

        private static SearchExpression GetExpression(string searchText, bool isDefaultContain = false)
        {
            if(searchText.Length > 2)
            switch (searchText.Substring(0,2))
            {
                case "==":
                    return new SearchExpression { Type = ComparisonType.Equals, Value = searchText.Substring(2) };
                case ">=":
                    return new SearchExpression { Type = ComparisonType.GreaterThanOrEquals, Value = searchText.Substring(2) };
                case "<=":
                    return new SearchExpression { Type = ComparisonType.LessThanOrEquals, Value = searchText.Substring(2) };
                case "!=":
                    return new SearchExpression { Type = ComparisonType.NotEquals, Value = searchText.Substring(2) };
            }
            //TuanVo: Add search contains
            if(searchText.Length>2 && searchText.StartsWith("*") && searchText.EndsWith("*"))
            {
                return new SearchExpression { Type = ComparisonType.Contains, Value = searchText.Substring(1,searchText.Length-2) };
            }
           
            if (searchText.Length > 1)
            {
                switch (searchText.Substring(0, 1))
                {
                    case "*":
                        return new SearchExpression { Type = ComparisonType.EndsWith, Value = searchText.Substring(1) };
                    case "=":
                        return new SearchExpression { Type = ComparisonType.Equals, Value = searchText.Substring(1) };
                    case ">":
                        return new SearchExpression { Type = ComparisonType.GreaterThan, Value = searchText.Substring(1) };
                    case "<":
                        return new SearchExpression { Type = ComparisonType.LessThan, Value = searchText.Substring(1) };
                    case "-":
                    case "!":
                        return new SearchExpression { Type = ComparisonType.NotEquals, Value = searchText.Substring(1) };
                }
                switch (searchText.Last())
                {
                    case '*':
                        return new SearchExpression { Type = ComparisonType.StartsWith, Value = searchText.Substring(0,searchText.Length-1) };
                }
            }

            if(isDefaultContain)
            {
                return new SearchExpression { Type = ComparisonType.Contains, Value = searchText };
            }
            return new SearchExpression {Type = ComparisonType.Default, Value = searchText};
        }
    }
}