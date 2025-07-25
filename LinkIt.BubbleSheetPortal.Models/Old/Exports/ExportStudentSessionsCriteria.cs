namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExportStudentSessionsCriteria
    {
        private int? _districtID;
        public int DateTime { get; set; }
        public bool OnlyShowPendingReview { get; set; }
        public bool ShowActiveClassTestAssignment { get; set; }
        public int? QTITestClassAssignmentID { get; set; }
        public int? ProgramID { get; set; }
        public int? UserID { get; set; }
        public int? DistrictID
        {
            get
            {
                if (_districtID.HasValue && _districtID.Value <= 0) _districtID = null;
                return _districtID;
            }
            set { _districtID = value; }
        }
        public string AssignDate { get; set; }
        public string AssignmentCodes { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Bank { get; set; }
        public string Class { get; set; }
        public string Teacher { get; set; }
        public string TestName { get; set; }
        public string Student { get; set; }
        public string Code { get; set; }
        public string SearchBox { get; set; }
        public int? SchoolID { get; set; }
    }
}
