using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XliAreaRepository : IReadOnlyRepository<XliArea>
    {
        private readonly Table<XLIAreaEntity> table;

        public XliAreaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<XLIAreaEntity>();
        }

        public IQueryable<XliArea> Select()
        {
            return table.Select(x => new XliArea
            {
                XliAreaId = x.XLIAreaID,
                Name = x.Name,
                Code = x.Code,
                Restrict = x.Restrict,
                DisplayTooltip = x.DisplayTooltip,
                DisplayName = x.DisplayName,
                AreaOrder = x.AreaOrder
            });
        }
    }
}
