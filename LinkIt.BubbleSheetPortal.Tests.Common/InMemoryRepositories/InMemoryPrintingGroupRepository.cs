using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryPrintingGroupRepository : IRepository<PrintingGroup>
    {
        private readonly List<PrintingGroup> table = new List<PrintingGroup>();
        private static int nextUniqueID = 100;

        public InMemoryPrintingGroupRepository()
        {
            table = AddPrintingGroups();
        }

        private List<PrintingGroup> AddPrintingGroups()
        {
            return new List<PrintingGroup>
                       {
                           new PrintingGroup { Id = 1, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 1", IsActive=true },
                           new PrintingGroup { Id = 2, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 2", IsActive=true },
                           new PrintingGroup { Id = 3, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 3", IsActive=true },
                           new PrintingGroup { Id = 4, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 4", IsActive=true },
                           new PrintingGroup { Id = 5, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 5", IsActive=true },
                           new PrintingGroup { Id = 6, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 6", IsActive=true },
                           new PrintingGroup { Id = 7, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 7", IsActive=true },
                           new PrintingGroup { Id = 8, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 8", IsActive=true },
                           new PrintingGroup { Id = 9, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 9", IsActive=true },
                           new PrintingGroup { Id = 10, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 10", IsActive=true },
                           new PrintingGroup { Id = 11, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 11", IsActive=true },
                           new PrintingGroup { Id = 12, CreatedUserId= 4290, DistrictId= 1184, Name="PrintingGroup 12", IsActive=true },
                       };
        }

        public IQueryable<PrintingGroup> Select()
        {
            return table.AsQueryable();
        }

        public void Save(PrintingGroup item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));

            if (entity.IsNull())
            {
                item.Id = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<PrintingGroup, PrintingGroup>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(PrintingGroup item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}
