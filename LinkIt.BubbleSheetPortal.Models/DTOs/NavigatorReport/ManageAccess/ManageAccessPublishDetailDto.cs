using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.ManageAccess
{
    public class ManageAccessPublishDetailDto
    {
        public int RoleId { get; set; }
        public string[] ReportNames { get; set; }
        public string RoleName
        {
            get
            {
                switch (RoleId)
                {
                    case (int)RoleEnum.NetworkAdmin:
                    case (int)RoleEnum.DistrictAdmin:
                        return TextConstants.DISTRICT_ADMIN;
                    case (int)RoleEnum.SchoolAdmin:
                        return TextConstants.SCHOOL_ADMIN;
                    case (int)RoleEnum.Teacher:
                        return TextConstants.TEACHER;
                    case (int)RoleEnum.Student:
                        return TextConstants.STUDENT;
                    default:
                        break;
                }
                return string.Empty;
            }
        }
        public int Order
        {
            get
            {
                switch (RoleId)
                {
                    case (int)RoleEnum.NetworkAdmin:
                    case (int)RoleEnum.DistrictAdmin:
                        return 1;
                    case (int)RoleEnum.SchoolAdmin:
                        return 2;
                    case (int)RoleEnum.Teacher:
                        return 3;
                    case (int)RoleEnum.Student:
                        return 4;
                    default:
                        return 5;
                }
            }
        }
    }
}
