using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTI3pItemFilters
    {
        public int? FindResultWith { get; set; }
        public string Searchkey { get; set; }
        public int DifficultyId { get; set; }

        public int QTI3pDOKID { get; set; }

        public string Difficulty { get; set; }
        public int BloomsID { get; set; }
        public string Blooms { get; set; }
        public int GradeID { get; set; }
        public int SubjectID { get; set; }
        public string Subject { get; set; }
        public int StateStandardId { get; set; }
        public int PassageId { get; set; }
        public string PassageNumber { get; set; }
        public int TextTypeId { get; set; }
        public int TextSubTypeId { get; set; }
        public int WordCountID { get; set; }
        public int FleschKincaidId { get; set; }
        public int PassageGradeId { get; set; }
        public string PassageSubject { get; set; }
        public string StateStandardIdString { get; set; }
        public bool? ForEmpty { get; set; }
        public int CurrentUserId { get; set; }
        public bool? FirstLoadListItemsFromLibrary { get; set; }
        public string PassageTitle { get; set; }
        public int? Qti3pSourceId { get; set; }
        public int? PassageTypeId { get; set; }
        public int? PassageGenreId { get; set; }
        public bool IsRestricted { get; set; }
        public int ItemTypeId { get; set; }
    }
}
