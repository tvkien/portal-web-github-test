namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSParentGuardian
    {
        public int TLDSParentGuardianID { get; set; }
        public int TLDSProfileID { get; set; }
        public string ParentGuardianName { get; set; }
        public string ParentGuardianRelationship { get; set; }
        public string ParentGuardianPhone { get; set; }
        public string ParentGuardianEmail { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ParentGuardianName) ||
                !string.IsNullOrEmpty(ParentGuardianRelationship) ||
                !string.IsNullOrEmpty(ParentGuardianPhone) ||
                !string.IsNullOrEmpty(ParentGuardianEmail);
        }
    }
}
