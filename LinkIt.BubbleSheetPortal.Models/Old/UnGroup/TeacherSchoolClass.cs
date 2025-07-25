using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TeacherSchoolClass
    {
        public string TeacherFirstName { get; set; }
        public string TeacherLastName { get; set; }
        public int ClassId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string DisplayName
        {
            get
            {
                var names = new List<string>
                    {
                        TeacherFirstName,
                        TeacherLastName
                    };

                var eNames = names.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                return string.Join(", ", eNames);
            }
        }
        public int? UserStatusId { get; set; }
    }
}