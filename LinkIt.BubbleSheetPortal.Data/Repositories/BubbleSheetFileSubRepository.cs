using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetFileSubRepository : IBubbleSheetFileSubRepository
    {
        private readonly Table<BubbleSheetFileSubEntity> table;

        public BubbleSheetFileSubRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetFileSubEntity>();
        }

        public IQueryable<BubbleSheetFileSub> Select()
        {
            return table.Select(x => new BubbleSheetFileSub
                                {
                                    BubblSheetFileSubId = x.BubbleSheetFileSubID,
                                    BubbleSheetFileId = x.BubbleSheetFileID,
                                    CreateDate = x.CreatedDate,
                                    InputFileName = x.InputFileName,
                                    InputFilePath = x.InputFilePath,
                                    OutputFileName = x.OutputFileName,
                                    PageNumber = x.PageNumber,
                                    Resolution = x.Resolution,
                                    PageType = x.PageType
                                });
        }

        public void Save(BubbleSheetFileSub item)
        {
            var entity = table.FirstOrDefault(x => x.BubbleSheetFileID.Equals(item.BubbleSheetFileId));

            if (entity.IsNull())
            {
                entity = new BubbleSheetFileSubEntity();
                table.InsertOnSubmit(entity);
            }

            BindBubbleSheetFileSubEntityToItem(entity, item);
            table.Context.SubmitChanges();
            item.BubbleSheetFileId = entity.BubbleSheetFileID;
        }

        public void Delete(BubbleSheetFileSub item)
        {
            var entity = table.FirstOrDefault(x => x.BubbleSheetFileSubID.Equals(item.BubblSheetFileSubId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void BindBubbleSheetFileSubEntityToItem(BubbleSheetFileSubEntity entity, BubbleSheetFileSub item)
        {
            entity.BubbleSheetFileID = item.BubbleSheetFileId;
            entity.InputFilePath = item.InputFilePath;
            entity.InputFileName = item.InputFileName;
            entity.Resolution = item.Resolution;
            entity.PageNumber = item.PageNumber;
            entity.OutputFileName = item.OutputFileName;
            entity.CreatedDate = DateTime.UtcNow;
            entity.PageType = item.PageType;
        }
    }
}
