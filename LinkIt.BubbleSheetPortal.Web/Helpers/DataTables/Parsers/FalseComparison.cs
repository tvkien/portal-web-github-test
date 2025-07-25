using System.Linq.Expressions;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables.Parsers
{
    public class FalseComparison : IParser
    {
        public static readonly Expression ReturnsFalse = Expression.Equal(Expression.Constant(false), Expression.Constant(true));
        public Expression GetSearchExpression()
        {
            return ReturnsFalse;
        }
    }
}