using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DistrictReferenceData
{
    public class DistrictReferenceDataViewModel
    {
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
        public string DistrictFullName { get; set; }
        public string StateName { get; set; }
        public List<School> Shools { get; set; }
        public List<Program> Programs { get; set; }
        public List<Race> Races { get; set; }
        public List<DistrictTerm> DistrictTerms { get; set; }
        public List<GenderStudent> Genders { get; set; }
        public List<SubjectDistrict> Subjects { get; set; }
        public List<GradeDistrict> Grades { get; set; }
        public List<Cluster> Clusters { get; set; }
        public List<AchievementLevelSetting> AchievementLevelSettings { get; set; }
    }
}