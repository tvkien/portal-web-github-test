using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class PrintingGroupDataRepository : IReadOnlyRepository<PrintingGroupData>
    {
        private readonly Table<PrintingGroupDataView> table;

        public PrintingGroupDataRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<PrintingGroupDataView>();
        }

        public IQueryable<PrintingGroupData> Select()
        {
            return table.Select(x => new PrintingGroupData
                {
                    CreatedUserID = x.CreatedUserID.GetValueOrDefault(),
                    GroupName = x.GroupName,
                    ClassName = x.ClassName,
                    SchoolName = x.SchoolName,
                    DistrictTermName = x.DistrictTermName,
                    TeacherName = x.NameFirst == string.Empty ? x.NameLast : x.NameLast + ", " + x.NameFirst,
                    UserID = x.UserID.GetValueOrDefault(),
                    GroupID = x.GroupID,
                    ClassID = x.ClassID
                });
        }
    }
}