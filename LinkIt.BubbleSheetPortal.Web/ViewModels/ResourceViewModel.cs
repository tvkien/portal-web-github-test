using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ResourceViewModel: ValidatableEntity<ResourceViewModel>
    {
        public  ResourceViewModel()
        {
            AvailableContentProviderDistricts = new List<SelectListItem>();
            AvailableContentTypes = new List<SelectListItem>();
            AvailableSubjects = new List<SelectListItem>();
            FileNameLesson = string.Empty;
            FileNameGuide = string.Empty;
            FileTypeLesson = string.Empty;
            LessonSelection = string.Empty;
            GuideSelection = string.Empty;
            ErrorMessage = string.Empty;
            RoleId = 0;
            AssignedGradeIdString = string.Empty;
            AssignedStandardIdString = string.Empty;
            SelectedLessonProviderId = 0;
            ResourceMimeTypeString = string.Empty;
            ResourceExtensionString = string.Empty;
        }

        public int CurrentUserDistrictId { get; set; }
        public int RoleId { get; set; }
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
        public string Description { get; set; }
        public int UserId { get; set; }
        public int LessonFileTypeId { get; set; }

        public List<SelectListItem> AvailableContentProviderDistricts { get; set; }
        public List<SelectListItem> AvailableContentTypes { get; set; }
        public List<SelectListItem> AvailableSubjects { get; set; }

        public string FileNameLesson { get; set; }
        public string FileNameGuide { get; set; }
        public string FileTypeLesson { get; set; }
        public string FileTypeGuide { get; set; }
        public string LessonSelection { get; set; }
        public string GuideSelection { get; set; }

        public string ErrorMessage { get; set; }
        public int FileMaxSizeByte { get; set; }
        public int TimeoutSecond { get; set; }
        public string FileMaxSizeMBString { get; set; }
        public int CreatedLessonId { get; set; }
        public string AssignedGradeIdString { get; set; }//AssignedGradeIdString looks like ,-123-,-1234-,-234-
        public string AssignedStandardIdString { get; set; }

        //Filter criteria from the LearningLibraryAdmin/Index.cshtml
        public string GradeSearch { get; set; }
        public int SubjectIdSearch { get; set; }
        public int ContentProviderIdSearch { get; set; }
        public int ResourceTypeIdSearch { get; set; }
        public string FilterSearch { get; set; } // text in the search box

        public int SelectedLessonProviderId { get; set; }
        public string ResourceMimeTypeString { get; set; }
        public string ResourceExtensionString { get; set; }

    }
}
