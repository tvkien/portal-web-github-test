namespace LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws
{
    public class ApiResponse<T> where T : class
    {
        public T Data { get; set; }
        public ErrorResponse Error { get; set; }

        public bool IsSuccess
        {
            get
            {
                return (Data != null && Error == null);
            }
        }
    }
}