using LinkIt.BubbleSheetPortal.Models.Old.StudentPreferenceDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    public class OptionMatrix
    {
        public int StudentPreferenceID { get; set; }
        public string Level { get; set; }
        public int DistrictID { get; set; }
        public int SchoolID { get; set; }
        public int UserID { get; set; }
        public int TestTypeID { get; set; }
        public int VirtualTestID { get; set; }

        public int Position { get; set; }
        public int ColumnType { get; set; }

        [DisplayName("Enterprise Default")]
        public virtual List<StudentPreferenceDetailDto> Default_Enterprise { get; set; }
        [DisplayName("Enterprise Category")]
        public virtual List<StudentPreferenceDetailDto> TestType_Enterprise { get; set; }
        [DisplayName("Enterprise Test-Specific")]
        public virtual List<StudentPreferenceDetailDto> Specific_Enterprise { get; set; }
        [DisplayName("{district} Default")]
        public virtual List<StudentPreferenceDetailDto> Default_District { get; set; }
        [DisplayName("{district} Category")]
        public virtual List<StudentPreferenceDetailDto> TestType_District { get; set; }
        [DisplayName("{district} Test-Specific")]
        public virtual List<StudentPreferenceDetailDto> Specific_District { get; set; }
        [DisplayName("School Default")]
        public virtual List<StudentPreferenceDetailDto> Default_School { get; set; }
        [DisplayName("School Category")]
        public virtual List<StudentPreferenceDetailDto> TestType_School { get; set; }
        [DisplayName("School Test-Specific")]
        public virtual List<StudentPreferenceDetailDto> Specific_School { get; set; }
        [DisplayName("User Default")]
        public virtual List<StudentPreferenceDetailDto> Default_User { get; set; }
        [DisplayName("User Category")]
        public virtual List<StudentPreferenceDetailDto> TestType_User { get; set; }
        [DisplayName("Class Test-Specific")]
        public virtual List<StudentPreferenceDetailDto> Specific_Class { get; set; }
        [DisplayName("Current Options in Effect")]
        public virtual List<StudentPreferenceDetailDto> Final_Option { get; set; }

        public List<StudentPreferenceMatrix> StudentPreferenceMatrix { get; set; }

        public OptionMatrix(string level, int districtID, int schoolID, int userID, int testTypeID, int virtualTestID)
        {
            Level = level;
            DistrictID = districtID;
            SchoolID = schoolID;
            UserID = userID;
            TestTypeID = testTypeID;
            VirtualTestID = virtualTestID;
            Final_Option = new List<StudentPreferenceDetailDto>();
            StudentPreferenceMatrix = new List<StudentPreferenceMatrix>();
        }
    }
}
