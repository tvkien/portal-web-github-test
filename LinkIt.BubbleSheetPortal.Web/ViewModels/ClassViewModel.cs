using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        public int DistrictTermId { get; set; }
        public List<ListItemsViewModel> ListDistrictTerm { get; set; }
    }

    public class StudentAdding
    {
        public string StudentIdsStr { get; set; }
        public int ClassId { get; set; }
        public string RemoveStudents { get; set; }
        public bool IsCheckAll { get; set; }
        public string ProgramIdList { get; set; }
        public string GradeIdList { get; set; }
        public string SearchBox { get; set; }
        public string ColumnSearchs { get; set; }
        public int Pagging { get; set; }
    }
}
