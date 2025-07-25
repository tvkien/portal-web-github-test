using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TLDS
{
    public class TLDSDownloadQueueRepository : ITLDSDownloadQueueRepository
    {
        private readonly Table<TLDSDownloadQueueEntity> table;
        private readonly TLDSContextDataContext _tldsContext;

        public TLDSDownloadQueueRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSDownloadQueueEntity>();
            _tldsContext = TLDSContextDataContext.Get(connectionString);
        }

        public void Delete(TLDSDownloadQueue item)
        {
            throw new NotImplementedException();
        }

        public void Save(TLDSDownloadQueue item)
        {
            if (item.TLDSDownloadQueueID > 0)
            {
                var obj = table.FirstOrDefault(m => m.TLDSDownloadQueueID == item.TLDSDownloadQueueID);

                if (obj != null)
                {

                    obj.Status = item.Status;
                    obj.CompletedFiles = item.CompletedFiles;
                    obj.Errors = item.Errors;
                    obj.UpdateDate = DateTime.UtcNow;

                    table.Context.SubmitChanges();
                }
            }
            else
            {
                var model = new TLDSDownloadQueueEntity
                {
                    ProfileIDs = item.ProfileIDs,
                    FileName = item.FileName,
                    Status = item.Status,
                    Total = item.Total,
                    CompletedFiles = item.CompletedFiles,
                    CreatedDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    CreatedUserID = item.CreatedUserID
                };

                table.InsertOnSubmit(model);
                table.Context.SubmitChanges();
                item.TLDSDownloadQueueID = model.TLDSDownloadQueueID;
            }
        }

        public IQueryable<TLDSDownloadQueue> Select()
        {
            var list = table.Select(m => new TLDSDownloadQueue
            {
                TLDSDownloadQueueID = m.TLDSDownloadQueueID,
                ProfileIDs = m.ProfileIDs,
                FileName = m.FileName,
                Status = m.Status,
                Total = m.Total,
                CompletedFiles = m.CompletedFiles,
                CreatedDate = m.CreatedDate,
                UpdateDate = m.UpdateDate,
                CreatedUserID = m.CreatedUserID
            });

            return list;
        }

        public TLDSDownloadQueue GetByFileName(string fileName)
        {
            var obj = table.FirstOrDefault(m => m.FileName == fileName);

            if (obj != null)
            {
                return new TLDSDownloadQueue
                {
                    TLDSDownloadQueueID = obj.TLDSDownloadQueueID,
                    ProfileIDs = obj.ProfileIDs,
                    FileName = obj.FileName,
                    Status = obj.Status,
                    Total = obj.Total,
                    CompletedFiles = obj.CompletedFiles,
                    CreatedDate = obj.CreatedDate,
                    UpdateDate = obj.UpdateDate,
                    CreatedUserID = obj.CreatedUserID
                };
            }

            return null;
        }
    }
}
