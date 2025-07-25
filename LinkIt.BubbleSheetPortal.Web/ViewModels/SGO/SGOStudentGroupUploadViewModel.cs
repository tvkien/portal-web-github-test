using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOStudentGroupUploadViewModel
    {
        public int SGOID { get; set; }
        public IEnumerable<UploadStudentInGroupViewModel> StudentInGroups { get; set; }
        public IEnumerable<UploadDataPointScoreTypeViewModel> DataPointScoreType { get; set; }
    }

    public class UploadStudentInGroupViewModel
    {
        public int StudentId { get; set; }
        public int GroupId { get; set; }
    }

    public class UploadDataPointScoreTypeViewModel
    {
        public int DataPointId { get; set; }
        public int ScoreType { get; set; }
        public int? VirtualTestCustomSubScoreId { get; set; }
    }
}