using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class LessonSearchViewModel
    {
        public int LessonId { get; set; }
        public string SubjectName { get; set; }
        public string GradeOrderString { get; set; }
        public string LessonName { get; set; }
        public string LessonType { get; set; }
        public string Provider { get; set; }
        public string ProviderThumbnail { get; set; }


        public string LessonPath { get; set; }
        public string GuidePath { get; set; }
        public string Keywords { get; set; }
        public string StandardGUIDString { get; set; }
        public string StandardDescriptionString { get; set; }
        public string StandardSubjectString { get; set; }
        public string StandardNumberString { get; set; }
        public string Grade { get; set; }

        public int ActivateInstructionContentType { get; set; } // 0: Is not Activate Instruction data; 1: Document content type; 2: External link
    }
}