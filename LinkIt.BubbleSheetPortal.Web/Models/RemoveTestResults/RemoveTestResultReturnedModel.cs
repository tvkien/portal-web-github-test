using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;

namespace LinkIt.BubbleSheetPortal.Web.Models.RemoveTestResults
{
    public class RemoveTestResultReturnedModel : FormatedList
    {
        public RemoveTestResultReturnedModel(FormatedList list, int totalStudents, int totalVirtualTests, int totalTestResults)
        {
            sEcho = list.sEcho;
            iTotalRecords = list.iTotalRecords;
            iTotalDisplayRecords = list.iTotalDisplayRecords;
            aaData = list.aaData;
            sColumns = list.sColumns;
            iTotalStudents = totalStudents;
            iTotalVirtualTests = totalVirtualTests;
            iTotalTestResults = totalTestResults;
        }

        public int iTotalStudents { get; set; }
        public int iTotalVirtualTests { get; set; }
        public int iTotalTestResults { get; set; }
    }
}
