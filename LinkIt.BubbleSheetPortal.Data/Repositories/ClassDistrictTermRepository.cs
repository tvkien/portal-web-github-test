using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassDistrictTermRepository : IReadOnlyRepository<ClassDistrictTerm>
    {
        private readonly Table<ClassDistrictTermView> table;

        public ClassDistrictTermRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassDistrictTermView>();
        }

        public IQueryable<ClassDistrictTerm> Select()
        {
            return table.Select(x => new ClassDistrictTerm()
            {
                ClassId = x.ClassId,
                ClassName = x.ClassName,
                DistrictId = x.DistrictId,
                DistrictTermId = x.DistrictTermId,
                SchoolId = x.SchoolId,
                SchoolName = x.SchoolName,
                TeacherFirstName = x.TeacherFirstName,
                TeacherLastName = x.TeacherLastName,
                TeacherUserName = x.TeacherUserName,
                TermName = x.TermName,
                UserId = x.UserId
            });
        }
    }
}