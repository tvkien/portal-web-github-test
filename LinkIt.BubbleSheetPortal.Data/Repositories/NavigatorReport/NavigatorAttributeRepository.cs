using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public class NavigatorAttributeRepository : INavigatorAttributeRepository
    {
        private readonly Table<NavigatorAttributeEntity> table;
        private readonly NavigatorReportDataContext _navigatorReportDataContext;

        public NavigatorAttributeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _navigatorReportDataContext = new NavigatorReportDataContext(connectionString);
            table = _navigatorReportDataContext.GetTable<NavigatorAttributeEntity>();
        }

        public void Delete(NavigatorAttributeDTO item)
        {
        }


        public void Save(NavigatorAttributeDTO item)
        {

        }

        public IQueryable<NavigatorAttributeDTO> Select()
        {
            return table.Select(c => new NavigatorAttributeDTO()
            {
                AttributeType = c.AttributeType,
                Description = c.Description,
                ListOrder = c.ListOrder,
                Name = c.Name,
                NavigatorAttributeID = c.NavigatorAttributeID,
                ShortName = c.ShortName
            });
        }
    }
}
