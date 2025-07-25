using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using  System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Lesson : ValidatableEntity<Lesson>
    {
        public Lesson()
        {
            GuidePath = string.Empty;
        }
        public int LessonId { get; set; }
        public string SubjectName { get; set; }
        public string Grade { get; set; }
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
        
        public int? LessonContentTypeId { get; set; }
        public int? LessonProviderId { get; set; }
        public int SubjectId { get; set; }
        public int LessonFileTypeId { get; set; }
        public string Description { get; set; }
        public int? tUserId { get; set; }
        public DateTime? DateCreated { get; set; }

        public string StandardNumberString { get; set; }
        public string GradeOrderString { get; set; }

    }
}