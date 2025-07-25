namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables
{
    internal class PostFilterTotal<T> where T : class, new()
    {
        internal int PreTotal { get; set; }
        internal int PostTotal { get; set; }
        internal T Val { get; set; }
    }
}