using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace LinkIt.BubbleSheetPortal.Data.Extensions
{
    public static class QueryableExtensions
    {
        public static IEnumerable<TResult> FilterOnLargeSet<TSource, TResult, TValue>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, TResult>> converter,
            Func<IEnumerable<TValue>, Expression<Func<TSource, bool>>> funcReturnFilterExpression,
            IEnumerable<TValue> filterSet)
        {
            return filterSet.Select((val, ind) => new { val, ind })
                .GroupBy(c => (int)(c.ind / 1000))
                .Select(c =>
                {

                    var filterExpression = funcReturnFilterExpression.Invoke(c.Select(x => x.val).ToArray());
                    return query.Where(filterExpression).Select(converter).ToArray();
                })
                .SelectMany(c => c)
                .ToArray();
        }
        
    }
    
}
