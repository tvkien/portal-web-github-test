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
    public class DataFileUploadPassageRepository : IRepository<DataFileUploadPassage>
    {
        private readonly Table<DataFileUploadPassageEntity> table;
        private readonly AssessmentDataContext dbContext;

        public DataFileUploadPassageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = AssessmentDataContext.Get(connectionString);
            table = dbContext.GetTable<DataFileUploadPassageEntity>();
        }

        public IQueryable<DataFileUploadPassage> Select()
        {
            return table.Select(x => new DataFileUploadPassage
            {
                DataFileUploadPassageID = x.DataFileUploadPassageID,
                DataFileUploadTypeID = x.DataFileUploadTypeID,
                FileName = x.FileName,
                Fullpath = x.Fullpath,
                DataFileUploadLogID = x.DataFileUploadLogID,
                PassageTitle = x.PassageTitle
                
            });
        }

        public void Save(DataFileUploadPassage item)
        {
            var entity = table.FirstOrDefault(x => x.DataFileUploadPassageID.Equals(item.DataFileUploadPassageID));

            if (entity.IsNull())
            {
                entity = new DataFileUploadPassageEntity();
                table.InsertOnSubmit(entity);
            }
            entity.DataFileUploadTypeID = item.DataFileUploadTypeID;
            entity.FileName = item.FileName;
            entity.Fullpath = item.Fullpath;
            entity.DataFileUploadLogID = item.DataFileUploadLogID;
            table.Context.SubmitChanges();
            item.DataFileUploadPassageID = entity.DataFileUploadPassageID;
        }

        public void Delete(DataFileUploadPassage item)
        {
            throw new NotImplementedException();
        }
    }
}
