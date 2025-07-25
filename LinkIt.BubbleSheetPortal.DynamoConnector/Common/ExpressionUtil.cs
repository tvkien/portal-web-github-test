using System;
using System.Linq.Expressions;

namespace LinkIt.BubbleSheetPortal.DynamoConnector.Common
{
    public class ExpressionUtil<T> where T : class
    {
        public static string GetPath(Expression<Func<T, object>> selector)
        {
            var pathVisitor = new PropertyPathVisitor();
            return pathVisitor.GetPropertyPath(selector);
        }

        public static string GetClassName()
        {
            return typeof(T).Name;
        }
    }
}
