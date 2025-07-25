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
    public class VirtualTestMetaRepository : IRepository<VirtualTestMeta>
    {
        private readonly Table<VirtualTestMetaEntity> table;

        public VirtualTestMetaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<VirtualTestMetaEntity>();
        }

        public IQueryable<VirtualTestMeta> Select()
        {
            return table.Select(x => new VirtualTestMeta
            {
                VirtualTestMetaID = x.VirtualTestMetaID,
                VirtualTestID = x.VirtualTestID,
                Name = x.Name,
                Data = x.Data
            });
        }
        public void Save(VirtualTestMeta item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestID.Equals(item.VirtualTestID) && x.Name == item.Name);

            if (entity.IsNull())
            {
                entity = new VirtualTestMetaEntity();
                table.InsertOnSubmit(entity);
            }
            MapVirtualTestMeta(item, entity);
            table.Context.SubmitChanges();
            item.VirtualTestMetaID = entity.VirtualTestMetaID;
        }

        public void Delete(VirtualTestMeta item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.VirtualTestID.Equals(item.VirtualTestID) && x.Name == item.Name);
                if (entity != null)
                {
                    table.DeleteOnSubmit(entity);
                    table.Context.SubmitChanges();
                }
            }
        }

        private void MapVirtualTestMeta(VirtualTestMeta source, VirtualTestMetaEntity destination)
        {
            destination.VirtualTestID = source.VirtualTestID;
            destination.Name = source.Name;
            destination.Data = source.Data;
        }
    }
}
