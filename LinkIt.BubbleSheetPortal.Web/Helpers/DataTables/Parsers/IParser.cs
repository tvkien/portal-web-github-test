using System.Linq.Expressions;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables.Parsers
{
    public interface IParser
    {
        Expression GetSearchExpression();
    }
}