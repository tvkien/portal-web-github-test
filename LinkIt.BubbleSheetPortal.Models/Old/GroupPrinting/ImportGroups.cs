namespace LinkIt.BubbleSheetPortal.Models.GroupPrinting
{
    public class ImportGroups
    {
        public string GroupName { get; set; }
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public string TermName { get; set; }
        public string ClassName { get; set; }
        public int Index { get; set; }

        public int GroupId { get; set; }
        public int SchoolId { get; set; }
        public int TeacherId { get; set; }
        public int DistrictTermId { get; set; }
        public int ClassId { get; set; }
        public int DistrictId { get; set; }
        public bool IsExpire { get; set; }

        public ImportGroups()
        {
            DistrictId = 0;
            Index = -1;
            GroupName = string.Empty;
            SchoolName = string.Empty;
            TeacherName = string.Empty;
            TermName = string.Empty;
            ClassName = string.Empty;
        }

        public ImportGroups(string[] rowData, int index, int districtId)
        {
            DistrictId = districtId;
            Index = index + 1; //TODO: to show correct order
            GroupName = rowData.Length >= 0 ? rowData[0].Trim() : string.Empty;
            SchoolName = rowData.Length >= 1 ? rowData[1].Trim() : string.Empty;
            TeacherName = rowData.Length >= 2 ? rowData[2].Trim() : string.Empty;
            TermName = rowData.Length >= 3 ? rowData[3].Trim() : string.Empty;
            ClassName = rowData.Length >= 4 ? rowData[4].Trim() : string.Empty;
        }
    }
}