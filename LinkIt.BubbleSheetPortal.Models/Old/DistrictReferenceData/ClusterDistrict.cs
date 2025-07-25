using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.DistrictReferenceData
{
    public class ClusterDistrict : ValidatableEntity<ClusterDistrict>
    {
        private string listClusterName = string.Empty;               
        private string subjectName = string.Empty;

        public int DistrictID { get; set; }

        public string ListClusterName
        {
            get { return listClusterName; }
            set { listClusterName = value.ConvertNullToEmptyString(); }
        }

        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value.ConvertNullToEmptyString(); }
        }
    }
}