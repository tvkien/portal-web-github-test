using System.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SchoolMetaRepository : IRepository<SchoolMeta>
    {
        private readonly Table<SchoolMetaEntity> schoolMetaEntityTable;

        public SchoolMetaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var datacontext = DbDataContext.Get(connectionString);
            schoolMetaEntityTable = datacontext.GetTable<SchoolMetaEntity>();
            Mapper.CreateMap<SchoolMeta, SchoolMetaEntity>();
        }

        public IQueryable<SchoolMeta> Select()
        {
            return schoolMetaEntityTable.Select(x => new SchoolMeta
            {
                SchoolMetaID = x.SchoolMetaID,
                SchoolID = x.SchoolID,
                Name = x.Name,
                Data = x.Data
            });
        }

        public void Save(SchoolMeta item)
        {
            var entity = schoolMetaEntityTable.FirstOrDefault(x => x.SchoolMetaID.Equals(item.SchoolMetaID));

            if (entity.IsNull())
            {
                entity = new SchoolMetaEntity();
                schoolMetaEntityTable.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            schoolMetaEntityTable.Context.SubmitChanges();
        }

        public void Delete(SchoolMeta item)
        {
            var entity = schoolMetaEntityTable.FirstOrDefault(x => x.SchoolMetaID.Equals(item.SchoolMetaID));
            if (entity != null)
            {
                schoolMetaEntityTable.DeleteOnSubmit(entity);
                schoolMetaEntityTable.Context.SubmitChanges();
            }
        }
    }
}
