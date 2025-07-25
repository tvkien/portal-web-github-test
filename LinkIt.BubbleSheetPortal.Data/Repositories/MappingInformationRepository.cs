using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class MappingInformationRepository : IRepository<MappingInformation>
    {
        private readonly Table<MappingInformationEntity> mappingInformationTable;

        public MappingInformationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            mappingInformationTable = dataContext.GetTable<MappingInformationEntity>();
        }

        public IQueryable<MappingInformation> Select()
        {
            return mappingInformationTable.Select(x => new MappingInformation
                {
                    MapID = x.MapID,
                    Name = x.Name,
                    LoaderType = (MappingLoaderType)x.LoaderType,
                    ProgressStatus = (MappingProgressStatus)x.ProgressStatus,
                    SourceFileContent = x.SourceFileContent,
                    XmlTransform = x.XMLTransform,
                    UserID = x.UserID,
                    CreatedDate = x.CreatedDate,
                    LastestModifiedDate = x.LastestModifiedDate
                });
        }

        public void Save(MappingInformation item)
        {
            var entity = mappingInformationTable.FirstOrDefault(x => x.MapID.Equals(item.MapID));

            if (entity.IsNull())
            {
                entity = new MappingInformationEntity();
                mappingInformationTable.InsertOnSubmit(entity);
            }

            MapToEntity(item, entity);
            mappingInformationTable.Context.SubmitChanges();
            item.MapID = entity.MapID;
        }

        public void Delete(MappingInformation item)
        {
            var entity = mappingInformationTable.FirstOrDefault(x => x.MapID.Equals(item.MapID));

            if (!entity.IsNull())
            {
                mappingInformationTable.DeleteOnSubmit(entity);
                mappingInformationTable.Context.SubmitChanges();
            }
        }

        private void MapToEntity(MappingInformation item, MappingInformationEntity entity)
        {
            if (item.IsNotNull() && entity.IsNotNull())
            {
                entity.Name = item.Name;
                entity.LoaderType = (int)item.LoaderType;
                entity.ProgressStatus = (int)item.ProgressStatus;
                entity.CreatedDate = item.CreatedDate;
                entity.LastestModifiedDate = item.LastestModifiedDate;
                entity.SourceFileContent = item.SourceFileContent;
                entity.UserID = item.UserID;
                entity.XMLTransform = item.XmlTransform;
            }
        }
    }
}