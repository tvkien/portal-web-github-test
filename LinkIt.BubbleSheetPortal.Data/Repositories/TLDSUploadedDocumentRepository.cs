using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSUploadedDocumentRepository : IRepository<TLDSUploadedDocument>
    {
        private readonly Table<TLDSUploadedDocumentEntity> table;

        public TLDSUploadedDocumentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<TLDSUploadedDocumentEntity>();
        }

        public IQueryable<TLDSUploadedDocument> Select()
        {
            return table.Select(x => new TLDSUploadedDocument
            {
                OriginalFileName = x.OriginalFileName,
                ProfileId = x.ProfileID,
                S3FileName = x.S3FileName,
                UploadedDate = x.UploadedDate,
                UploadedDocumentId = x.UploadedDocumentID,
                UploadedUserId = x.UploadedUserID
            });
        }

        public void Save(TLDSUploadedDocument item)
        {
            var entity = table.FirstOrDefault(x => x.UploadedDocumentID.Equals(item.UploadedDocumentId));

            if (entity.IsNull())
            {
                entity = new TLDSUploadedDocumentEntity();
                table.InsertOnSubmit(entity);
            }

            BindEntityToItem(entity, item);
            table.Context.SubmitChanges();
            item.UploadedDocumentId = entity.UploadedDocumentID;
        }

        private void BindEntityToItem(TLDSUploadedDocumentEntity entity, TLDSUploadedDocument item)
        {
            entity.OriginalFileName = item.OriginalFileName;
            entity.ProfileID = item.ProfileId;
            entity.S3FileName = item.S3FileName;
            entity.UploadedDate = item.UploadedDate;
            entity.UploadedUserID = item.UploadedUserId;
        }

        public void Delete(TLDSUploadedDocument item)
        {
            var entity = table.FirstOrDefault(x => x.UploadedDocumentID.Equals(item.UploadedDocumentId));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
