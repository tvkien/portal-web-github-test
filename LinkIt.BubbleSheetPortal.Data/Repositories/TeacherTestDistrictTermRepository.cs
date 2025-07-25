using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TeacherTestDistrictTermRepository : IReadOnlyRepository<TeacherTestDistrictTerm>
    {
        private readonly Table<TeacherTestDistrictTermView> table;

        public TeacherTestDistrictTermRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<TeacherTestDistrictTermView>();
        }

        public IQueryable<TeacherTestDistrictTerm> Select()
        {
            return table.Select(x => new TeacherTestDistrictTerm
            {
                DateEnd = x.DateEnd,
                DateStart = x.DateStart,
                DistrictTermId = x.DistrictTermID,
                DistrictTermName = x.DistrictTermName,
                UserId = x.UserID,
                UserName = x.UserName,
                NameFirst = x.NameFirst,
                NameLast = x.NameLast,
                VirtualTestId = x.VirtualTestID,
                ClassId = x.ClassID,
                ClassName = x.ClassName,
                SchoolId = x.SchoolID,
                DistrictId = x.DistrictID,
                SchoolName = x.SchoolName,
                VirtualTestSubTypeId = x.VirtualTestSubTypeID,
                ResultDate = x.ResultDate
            });
        }
    }
}
