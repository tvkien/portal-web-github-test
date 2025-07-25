using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SchoolListViewModel
    {
        private string schoolName = string.Empty;
        private string code = string.Empty;
        private string stateCode = string.Empty;

        public int SchoolID { get; set; }
        
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value.ConvertNullToEmptyString(); }
        }

        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }

        public string StateCode
        {
            get { return stateCode; }
            set { stateCode = value.ConvertNullToEmptyString(); }
        }
    }
}