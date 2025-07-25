using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class RoleRepository : IReadOnlyRepository<LinkIt.BubbleSheetPortal.Models.Role>
    {
        private readonly Table<RoleEntity> table;
        private readonly DbDataContext dbContext;

        public RoleRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<RoleEntity>();
            dbContext = DbDataContext.Get(connectionString);
            Mapper.CreateMap<LinkIt.BubbleSheetPortal.Models.Role, RoleEntity>();
        }

        public IQueryable<LinkIt.BubbleSheetPortal.Models.Role> Select()
        {
            var now = DateTime.Now.Date;
            return table.Select(x => new LinkIt.BubbleSheetPortal.Models.Role
                    {
                        RoleId = x.RoleID,
                        Name = x.Name,
                        Display = x.Display
                    });
        }
    }
}