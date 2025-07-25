using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTITestClassAssignmentData
    {
        public int QTITestClassAssignmentId { get; set; }
        public int VirtualTestId { get; set; }
        public int ClassId { get; set; }
        public DateTime AssignmentDate { get; set; }
        private string _Code;
        public DateTime? CodeTimestamp { get; set; }
        private string _AssignmentGuId;
        private string _TestSetting;
        public int? ComparisonPasscodeLength { get; set; }
        public int Status { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        private string _ModifiedBy;
        public int Type { get; set; }

        public int TutorialMode { get; set; }

        public string Code
        {
            get { return _Code; }
            set { _Code = value.ConvertNullToEmptyString(); }
        }

        public string AssignmentGuId
        {
            get { return _AssignmentGuId; }
            set { _AssignmentGuId = value.ConvertNullToEmptyString(); }
        }

        public string TestSetting
        {
            get { return _TestSetting; }
            set { _TestSetting = value.ConvertNullToEmptyString(); }
        }

        public string ModifiedBy
        {
            get { return _ModifiedBy; }
            set { _ModifiedBy = value.ConvertNullToEmptyString(); }
        }

        public string TestName { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public bool IsHide { get; set; }

        public int DistrictID { get; set; }
        public int? SurveyAssignmentType { get; set; }
        public string ListOfDisplayQuestions { get; set; }
        public DataLockerPreferencesLevel Level { get; set; }
        public string AuthenticationCode { get; set; }
        public DateTime? AuthenticationCodeExpirationDate { get; set; }
        public int StudentID { get; set; }

        public void SetAuthenticationCode(string authenticationCode, DateTime? authenticationCodeExpirationDate)
        {
            AuthenticationCode = authenticationCode;
            AuthenticationCodeExpirationDate = authenticationCodeExpirationDate;
        }
    }
}
