namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestAssignmentCriteria
    {
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
        public int? SchoolID { get; set; }
        public bool? PageLoad { get; set; }

        public string ModuleCode { get; set; }
    }
}
