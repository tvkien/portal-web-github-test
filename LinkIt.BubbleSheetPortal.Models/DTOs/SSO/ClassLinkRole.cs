using LinkIt.BubbleSheetPortal.Models.Enum;
using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO
{
    public static class ClassLinkRoleExtensions
    {
        private static readonly Dictionary<string, int> ClassLinkRoleDictionary = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Student", (int)RoleEnum.Student },
            { "Parent", (int)RoleEnum.Parent },
            { "Teacher", (int)RoleEnum.Teacher },
            { "Classroom Teacher", (int)RoleEnum.Teacher },
            { "ClassroomTeacher", (int)RoleEnum.Teacher },
            { "School Administrator", (int)RoleEnum.SchoolAdmin },
            { "SchoolAdministrator", (int)RoleEnum.SchoolAdmin },
            { "School Admin", (int)RoleEnum.SchoolAdmin },
            { "SchoolAdmin", (int)RoleEnum.SchoolAdmin },
            { "School", (int)RoleEnum.SchoolAdmin },
            { "District Administrator", (int)RoleEnum.DistrictAdmin },
            { "DistrictAdministrator", (int)RoleEnum.DistrictAdmin },
            { "District Admin", (int)RoleEnum.DistrictAdmin },
            { "DistrictAdmin", (int)RoleEnum.DistrictAdmin },
            { "District", (int)RoleEnum.DistrictAdmin },
            { "Tenant Administrator", (int)RoleEnum.DistrictAdmin },
            { "TenantAdministrator", (int)RoleEnum.DistrictAdmin },
            { "Tenant Admin", (int)RoleEnum.DistrictAdmin },
            { "TenantAdmin", (int)RoleEnum.DistrictAdmin },
            { "Tenant", (int)RoleEnum.DistrictAdmin }
        };

        public static int ToLinkitRole(this string classLinkRole)
        {
            return ClassLinkRoleDictionary.TryGetValue(classLinkRole, out var value) ? value : 0;
        }

        public static bool IsStudentRole(this string classLinkRole)
        {
            var linkItRole = classLinkRole.ToLinkitRole();
            return linkItRole == (int)RoleEnum.Student;
        }
    }
}
