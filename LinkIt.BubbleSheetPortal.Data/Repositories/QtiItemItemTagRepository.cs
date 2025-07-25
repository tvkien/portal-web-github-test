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
    public class QtiItemItemTagRepository : IRepository<QtiItemItemTag>
    {
        private readonly Table<QtiItemItemTagEntity> table;
        private readonly Table<QtiItemItemTagView> view;
        private readonly ItemTagDataContext dbContext;

        public QtiItemItemTagRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = ItemTagDataContext.Get(connectionString);
            table = dbContext.GetTable<QtiItemItemTagEntity>();
            view = dbContext.GetTable<QtiItemItemTagView>();
        }

        public IQueryable<QtiItemItemTag> Select()
        {
            return view.Select(x => new QtiItemItemTag
                                         {
                                             QtiItemItemTagID = x.QtiItemItemTagID,
                                             QtiItemID = x.QtiItemID,
                                             ItemTagID = x.ItemTagID,
                                             ItemTagCategoryID = x.ItemTagCategoryID,
                                             CategoryName = x.CategoryName,
                                             CategoryDescription = x.CategoryDescription,
                                             TagName = x.TagName,
                                             TagDescription = x.TagDescription
                                         });
        }

        public void Save(QtiItemItemTag item)
        {
            var entity = table.FirstOrDefault(x => x.QtiItemItemTagID.Equals(item.QtiItemItemTagID));

            if (entity.IsNull())
            {
                entity = new QtiItemItemTagEntity();
                table.InsertOnSubmit(entity);
            }
            entity.QtiItemID = item.QtiItemID;
            entity.ItemTagID = item.ItemTagID;
            
            table.Context.SubmitChanges();
            item.QtiItemItemTagID = entity.QtiItemItemTagID;
        }

        public void Delete(QtiItemItemTag item)
        {
            var entity = table.FirstOrDefault(x => x.QtiItemItemTagID.Equals(item.QtiItemItemTagID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}