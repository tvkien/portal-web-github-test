namespace LinkIt.BubbleSheetPortal.Models
{
    public static class RoleUtil
    {
        public static bool IsSchoolAdmin(int roleID)
        {
            return roleID == (int)Permissions.SchoolAdmin;
        }

        public static bool IsPublisher(int roleID)
        {
            return roleID == (int)Permissions.Publisher;
        }

        public static bool IsDistrictAdmin(int roleID)
        {
            return roleID == (int)Permissions.DistrictAdmin;
        }

        public static bool IsStudent(int roleID)
        {
            return roleID == (int)Permissions.Student;
        }

        public static bool IsNetworkAdmin(int roleID)
        {
            return roleID == (int)Permissions.NetworkAdmin;
        }

        public static bool IsTeacher(int roleID)
        {
            return roleID == (int)Permissions.Teacher;
        }
    }
}
