using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.DistrictReferenceData
{
    public class DistrictSearchingObject
    {
        private string keyWords = string.Empty;

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalRecords { get; set; }

        public string KeyWords
        {
            get { return keyWords; }
            set { keyWords = value.ConvertNullToEmptyString(); }
        }
    }
}