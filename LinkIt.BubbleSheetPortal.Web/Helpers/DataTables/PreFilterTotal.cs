namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables
{
    internal class PreFilterTotal<T> where T : class, new()
    {
        internal int Total { get; set; }
        internal T Val { get; set; }
    }
}