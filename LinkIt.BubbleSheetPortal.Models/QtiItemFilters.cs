using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiItemFilters
    {
        public QtiItemFilters()
        {
            IsPersonalSearch = true;
            IsDistrictSearch = false;
            ForCheckingRight = false;
            QtiItemIdString = string.Empty;
        }
        public int? FindResultWith { get; set; }
        public string Keyword { get; set; }
        public int? QtiBankId { get; set; }
        public int? ItemSetId { get; set; }
        public string Standard { get; set; }
        public string Topic { get; set; }
        public string Skill { get; set; }
        public string Other { get; set; }
        public string Personal { get; set; }
        public string DistrictTag { get; set; }
        public int? DistrictCategoryId { get; set; }
        public string SelectedTags { get; set; }
        public bool? FirstLoadListItemsFromLibraryNew { get; set; }

        public int? PassageId { get; set; }
        public string PassageNumber { get; set; }
        public int? TextTypeId { get; set; }
        public int? TextSubTypeId { get; set; }
        public int? WordCountID { get; set; }
        public int? FleschKincaidId { get; set; }
        public int? PassageGradeId { get; set; }
        public string PassageSubject { get; set; }
        public string StateStandardIdString { get; set; }
        public bool? ForEmpty { get; set; }
        public int CurrentUserId { get; set; }
        public string RefObjectTitle { get; set; }
        public bool IsPersonalSearch { get; set; }
        public bool IsDistrictSearch { get; set; }
        public bool ForCheckingRight { get; set; }
        public string QtiItemIdString { get; set; }// like 1,2,3,4
        public int ItemTypeId { get; set; }

    }
}
