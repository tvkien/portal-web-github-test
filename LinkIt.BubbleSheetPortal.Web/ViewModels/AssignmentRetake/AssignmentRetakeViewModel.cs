using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using LinkIt.BubbleSheetPortal.Models.Enums;
using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.AssignmentRetake
{
    public class AssignmentRetakeViewModel
    {
        public string GUID { get; set; }
        public int DistrictId { get; set; }
        public int CurrentVirtualTestID { get; set; }
        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string TestName { get; set; }
        public int OriginalVirtualTestID { get; set; }
        public int TestRetakeNumber { get; set; } = 0;
        public string RetakeType => StudentRetakes.FirstOrDefault()?.RetakeType ?? string.Empty;
        public TestPreferenceModel ObjTestPreferenceModel { get; set; }
        public List<StudentRetakeViewModel> StudentRetakes { get; set; }
        public IEnumerable<string> VirtualTestsDisplay { get; set; }
        public string RetakeStudentIds => string.Join(",", (StudentRetakes ?? new List<StudentRetakeViewModel>()).Select(x => x.StudentID));
    }
    public class StudentRetakeViewModel
    {
        public int ClassID { get; set; }
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public int CurrentVirtualTestID { get; set; }
        public string RetakeType { get; set; }
        public bool IsValid
        {
            get
            {
                var virtualTests = VirtualTests.Where(x => x.VirtualTestID != 0).ToList();
                var currentVirtualTest = VirtualTests.FirstOrDefault(x => x.VirtualTestID == CurrentVirtualTestID);

                if (currentVirtualTest == null)
                    return false;

                var isNotLatestRetake = virtualTests.IndexOf(currentVirtualTest) < virtualTests.Count - 1;
                var isNotCompeled = currentVirtualTest.TestStatus != (int)RetakeStatus.Completed;

                if (isNotLatestRetake || isNotCompeled)
                {
                    return false;
                }
                return true;
            }
        }
        public List<StudentRetakeTestInfoViewModel> VirtualTests { get; set; }
    }
    public class StudentRetakeTestInfoViewModel
    {
        public int VirtualTestID { get; set; }
        public string VirtualTestName { get; set; }
        public string VirtualTestDisplayName { get; set; }
        public int TestStatus { get; set; }
        public string TestStatusDisplay =>
            Enum.IsDefined(typeof(RetakeStatus), TestStatus) ? ((RetakeStatus)TestStatus).GetDisplayName() : string.Empty;
        public string StatusColor
        {
            get
            {
                switch (TestStatus)
                {
                    case (int)RetakeStatus.NotStarted:
                        return RetakeStatusColor.NotStarted;
                    case (int)RetakeStatus.InProgess:
                    case (int)RetakeStatus.InProgess_2:
                        return RetakeStatusColor.InProgess;
                    case (int)RetakeStatus.Pause:
                        return RetakeStatusColor.Pause;
                    case (int)RetakeStatus.PendingReview:
                        return RetakeStatusColor.PendingReview;
                    case (int)RetakeStatus.Completed:
                        return RetakeStatusColor.Completed;
                    default:
                        return RetakeExistColor.NotExist;
                }
            }
        }
        public string TestNameColor
        {
            get
            {
                if (VirtualTestID != 0)
                {
                    return RetakeExistColor.Exist;
                }
                return RetakeExistColor.NotExist;
            }
        }
    }
}
