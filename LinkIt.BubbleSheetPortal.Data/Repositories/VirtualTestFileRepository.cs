using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public  class VirtualTestFileRepository : IRepository<VirtualTestFile>
    {
        private readonly Table<VirtualTestFileEntity> table;

        public VirtualTestFileRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<VirtualTestFileEntity>();
        }

        public IQueryable<VirtualTestFile> Select()
        {
            return table.Select(en => new VirtualTestFile
                {
                    FileKey = en.FileKey,
                    FileType = en.FileType,
                    FileUrl = en.FileUrl,
                    UploadByUserId = en.UploadByUserID,
                    UploadDate = en.UploadDate,
                    VirtualTestFileId = en.VirtualTestFileID,
                    VirtualTestId = en.VirtualTestID,
                    FileName = en.FileName
                });
        }

        public void Save(VirtualTestFile item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestFileID.Equals(item.VirtualTestFileId));
            if (entity.IsNull())
            {
                entity = new VirtualTestFileEntity();
                table.InsertOnSubmit(entity);
            }
            MapObject(item, entity);
            table.Context.SubmitChanges();
            item.VirtualTestFileId = entity.VirtualTestFileID;
        }

        public void Delete(VirtualTestFile item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestFileID.Equals(item.VirtualTestFileId));
            if (entity.IsNull()) return;
            table.DeleteOnSubmit(entity);
            table.Context.SubmitChanges();
        }

        private void MapObject(VirtualTestFile item, VirtualTestFileEntity entity)
        {
            entity.FileKey = item.FileKey;
            entity.FileName = item.FileName;
            entity.FileType = item.FileType;
            entity.FileUrl = item.FileUrl;
            entity.UploadByUserID = item.UploadByUserId;
            entity.UploadDate = item.UploadDate;
            entity.VirtualTestID = item.VirtualTestId;
        }
    }
}