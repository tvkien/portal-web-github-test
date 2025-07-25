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
    public class ItemTagCategoryRepository : IRepository<ItemTagCategory>
    {
        private readonly Table<ItemTagCategoryEntity> table;
        private readonly Table<ItemTagCategoryView> view;
        private readonly ItemTagDataContext dbContext;

        public ItemTagCategoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = ItemTagDataContext.Get(connectionString);
            table = dbContext.GetTable<ItemTagCategoryEntity>();
            view = dbContext.GetTable<ItemTagCategoryView>();
        }

        public IQueryable<ItemTagCategory> Select()
        {
            return view.Select(x => new ItemTagCategory
                                         {
                                             ItemTagCategoryID = x.ItemTagCategoryID,
                                             DistrictID = x.DistrictID??0,
                                             StateId = x.StateID,
                                             District = x.District,
                                             Name = x.Name,
                                             Description = x.Description,
                                             CountQtiItem = x.CountQtiItem
                                         });
        }

        public void Save(ItemTagCategory item)
        {
            var entity = table.FirstOrDefault(x => x.ItemTagCategoryID.Equals(item.ItemTagCategoryID));

            if (entity.IsNull())
            {
                entity = new ItemTagCategoryEntity();
                table.InsertOnSubmit(entity);
            }
            entity.DistrictID = item.DistrictID;
            entity.Name = item.Name;
            entity.Description = item.Description;
            
            table.Context.SubmitChanges();
            item.ItemTagCategoryID = entity.ItemTagCategoryID;
        }

        public void Delete(ItemTagCategory item)
        {
             var entity = table.FirstOrDefault(x => x.ItemTagCategoryID.Equals(item.ItemTagCategoryID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}