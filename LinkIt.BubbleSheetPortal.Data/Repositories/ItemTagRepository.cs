using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ItemTagRepository : IRepository<ItemTag>, IItemTagRepository
    {
        private readonly Table<ItemTagEntity> table;
        private readonly Table<ItemTagView> view;
        private readonly ItemTagDataContext dbContext;

        public ItemTagRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = ItemTagDataContext.Get(connectionString);
            table = dbContext.GetTable<ItemTagEntity>();
            view = dbContext.GetTable<ItemTagView>();
        }

        public IQueryable<ItemTag> Select()
        {
            return view.Select(x => new ItemTag
                                         {
                                             ItemTagID = x.ItemTagID,
                                             ItemTagCategoryID = x.ItemTagCategoryID,
                                             Name = x.TagName,
                                             Description = x.TagDescription,
                                             Category = x.CategoryName,
                                             CategoryDescription = x.CategoryDescription,
                                             DistrictID = x.DistrictID??0,
                                             CountQtiItem = x.CountQtiItem??0
                                         });
        }

        public void Save(ItemTag item)
        {
            var entity = table.FirstOrDefault(x => x.ItemTagID.Equals(item.ItemTagID));

            if (entity.IsNull())
            {
                entity = new ItemTagEntity();
                table.InsertOnSubmit(entity);
            }
            entity.ItemTagCategoryID = item.ItemTagCategoryID;
            entity.Name = item.Name;
            entity.Description = item.Description;
            
            table.Context.SubmitChanges();
            item.ItemTagID = entity.ItemTagID;
        }

        public void Delete(ItemTag item)
        {
            var entity = table.FirstOrDefault(x => x.ItemTagID.Equals(item.ItemTagID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public IList<string> GetSuggestTags(int districtId, string textToSearch)
        {
           return dbContext.GetSuggestTags(districtId, textToSearch).Select(x => x.Name).ToList();
        }
    }
}