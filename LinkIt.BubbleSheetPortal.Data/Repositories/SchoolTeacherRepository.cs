using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SchoolTeacherRepository : ISchoolTeacherRepository
    {
        private readonly Table<SchoolTeacherView> table;
        private readonly UserDataContext _context;

        public SchoolTeacherRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<SchoolTeacherView>();
            _context = UserDataContext.Get(connectionString);
        }

        public IQueryable<SchoolTeacher> Select()
        {
            return table.Select(x => new SchoolTeacher
                {
                    UserId = x.UserID,
                    SchoolId = x.SchoolID,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast,
                    TeacherName = string.IsNullOrEmpty(x.NameFirst) ? x.NameLast : x.NameLast + ", " + x.NameFirst,
                    UserName = x.UserName
                });
        }

        public List<ListItem> GetListTeacherBySchoolIdAndDistrictTermId(int schoolId, int districttermId, int userId = 0, int roleId = 0)
        {
            if (schoolId > 0 && districttermId > 0)
            {
                var query = _context.GetTeacherBySchoolIDAndDistricttermID(schoolId, districttermId).
                    Where(m=> roleId != 2 || (roleId == 2 && m.UserID == userId)).ToList();
                if (query.Count > 0)
                    return query.Select(o => new ListItem() { Id = o.UserID, Name = o.FullName }).ToList();
            }
            
            return new List<ListItem>();
        }
        public List<ListItem> GetAllListTeacherBySchoolIdAndDistrictTermId(int schoolId, int districttermId, int userId = 0, int roleId = 0)
        {
            if (schoolId > 0 && districttermId > 0)
            {
                var query = _context.GetAllTeacherBySchoolIDAndDistrictTermID(schoolId, districttermId).
                    Where(m => roleId != (int)Permissions.Teacher || (roleId == (int)Permissions.Teacher && m.UserID == userId)).ToList();

                return query.Select(o => new ListItem() { Id = o.UserID, Name = o.FullName }).ToList();
            }

            return new List<ListItem>();
        }
    }
}
