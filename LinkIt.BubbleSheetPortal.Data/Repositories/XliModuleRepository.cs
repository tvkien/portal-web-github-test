using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class XliModuleRepository : IReadOnlyRepository<XliModule>
    {
        private readonly Table<XLIModuleEntity> table;

        public XliModuleRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<XLIModuleEntity>();
        }

        public IQueryable<XliModule> Select()
        {
            return table.Select(x => new XliModule
                                {
                                    XliModuleId = x.XLIModuleID,
                                    XliAreaId = x.XLIAreaID,
                                    Name = x.Name,
                                    Code = x.Code,
                                    ModuleOrder = x.ModuleOrder,
                                    Path = x.Path,
                                    AllRoles = x.AllRoles,
                                    Restrict = x.Restrict,
                                    DisplayName = x.DisplayName,
                                    DisplayTooltip= x.DisplayTooltip,
                                    HelpText = x.HelpText
                                });
        }
    }
}
