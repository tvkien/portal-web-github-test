namespace LinkIt.BubbleSheetPortal.Web.Models.DataTable
{
    public class GetTestClassAssignmentCriteria : GenericDataTableRequest
    {
        public bool PageLoad { get; set; }
        private int? _districtID;
        public int DateTime { get; set; }
        public bool OnlyShowPendingReview { get; set; }
        public bool ShowActiveClassTestAssignment { get; set; }
        public int? QTITestClassAssignmentID { get; set; }
        public int? ProgramID { get; set; }
        public int? DistrictID
        {
            get
            {
                if (_districtID.HasValue && _districtID.Value <= 0) _districtID = null;
                return _districtID;
            }
            set { _districtID = value; }
        }

        public string AssignmentCodes { get; set; }
        public string Code { get; set; }
        public int? SchoolID { get; set; }
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public string TestName { get; set; }
        public string ModuleCode { get; set; }
        public string SessionKey { get; set; }
        public string RetakeAssignmentRequestGuid { get; set; }
    }
}
