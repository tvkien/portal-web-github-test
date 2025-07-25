using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.DataFileUpload;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QtiItemDataFileUploadPassageRepository : IRepository<QtiItemDataFileUploadPassage>
    {
        private readonly Table<QtiItemDataFileUploadPassageEntity> table;
        private readonly AssessmentDataContext dbContext;

        public QtiItemDataFileUploadPassageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = AssessmentDataContext.Get(connectionString);
            table = dbContext.GetTable<QtiItemDataFileUploadPassageEntity>();
        }

        public IQueryable<QtiItemDataFileUploadPassage> Select()
        {
            return table.Select(x => new QtiItemDataFileUploadPassage
            {
                QtiItemDataFileUploadPassageID = x.QtiItemDataFileUploadPassageID,
                QtiItemID = x.QtiItemID,
                DataFileUploadPassageID = x.DataFileUploadPassageID,
                
            });
        }

        public void Save(QtiItemDataFileUploadPassage item)
        {
            var entity = table.FirstOrDefault(x => x.QtiItemDataFileUploadPassageID.Equals(item.QtiItemDataFileUploadPassageID));

            if (entity.IsNull())
            {
                entity = new QtiItemDataFileUploadPassageEntity();
                table.InsertOnSubmit(entity);
            }
            entity.DataFileUploadPassageID = item.DataFileUploadPassageID;
            entity.QtiItemID = item.QtiItemID;

            table.Context.SubmitChanges();
            item.QtiItemDataFileUploadPassageID = entity.QtiItemDataFileUploadPassageID;
        }

        public void Delete(QtiItemDataFileUploadPassage item)
        {
            var entity = table.FirstOrDefault(x => x.QtiItemDataFileUploadPassageID.Equals(item.QtiItemDataFileUploadPassageID));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}