using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryColumnLookupDataRepository: IReadOnlyRepository<ColumnLookupData>
    {
        private readonly List<ColumnLookupData> table;

        public InMemoryColumnLookupDataRepository()
        {
            table = AddColumnLookupDatas();
        }

        private List<ColumnLookupData> AddColumnLookupDatas()
        {
            return new List<ColumnLookupData> 
            {
                new ColumnLookupData{LookupDataID=1,ColumnID=1,Data="2011"},
                new ColumnLookupData{LookupDataID=2,ColumnID=1,Data="2012"},
                new ColumnLookupData{LookupDataID=3,ColumnID=1,Data="2013"},
                new ColumnLookupData{LookupDataID=3,ColumnID=1,Data="2014"}
            };
        }

        public IQueryable<ColumnLookupData> Select()
        {
            return table.AsQueryable();
        }
    }
}
