using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class LearningLibrarySearchViewModel
    {
        public LearningLibrarySearchViewModel()
        {
            HasParameter = false;
            SubjectIdParameter = 0;
            KeywordsParameter = string.Empty;
            GradeParameter = string.Empty;
            SearchType = string.Empty;
            GUID = string.Empty;
            RoleId = 0;
            ContentProviderIdParameter = 0;
        }

        public int CurrentUserDistrictId { get; set; }
        public int RoleId { get; set; }
        public int SubjectIdParameter { get; set; }
        public string KeywordsParameter { get; set; }
        public string GradeParameter { get; set; }
        public bool HasParameter { get; set; }
        public string SearchType { get; set; }
        public string GUID { get; set; }
        public int ResourceTypeIdParameter { get; set; }
        public int ContentProviderIdParameter { get; set; }
        public bool IsPublisher {
            get { return RoleId.Equals((int)Permissions.Publisher); }
        }
        public bool IsNetworkAdmin
        {
            get { return RoleId.Equals((int)Permissions.NetworkAdmin); }
        }

    }
}