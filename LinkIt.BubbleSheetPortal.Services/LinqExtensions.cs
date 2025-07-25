namespace System.Linq
{
    public static class LinqExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageSize, int startRow)
        {
            return query.Skip(startRow).Take(pageSize);
        }
    }
}
