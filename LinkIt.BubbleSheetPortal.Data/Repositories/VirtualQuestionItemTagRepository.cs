using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualQuestionItemTagRepository : IVirtualQuestionItemTagRepository
    {
        private readonly Table<VirtualQuestionItemTagEntity> table;
        private readonly Table<VirtualQuestionItemTagView> view;

        public VirtualQuestionItemTagRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualQuestionItemTagEntity>();
            view = dataContext.GetTable<VirtualQuestionItemTagView>();
        }

        public IQueryable<VirtualQuestionItemTag> Select()
        {
            return view.Select(x => new VirtualQuestionItemTag
                {
                    VirtualQuestionItemTagId = x.VirtualQuestionItemTagID,
                    VirtualQuestionId = x.VirtualQuestionID,
                    ItemTagId = x.ItemTagId,
                    Name = x.Name
                });
        }

        public void Save(VirtualQuestionItemTag item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionItemTagID.Equals(item.VirtualQuestionItemTagId));

            if (entity == null)
            {
                entity = new VirtualQuestionItemTagEntity();
                table.InsertOnSubmit(entity);
            }
            entity.VirtualQuestionID = item.VirtualQuestionId;
            entity.ItemTagID = item.ItemTagId;

            table.Context.SubmitChanges();
            item.VirtualQuestionItemTagId = entity.VirtualQuestionItemTagID;
        }

        public void Delete(VirtualQuestionItemTag item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionItemTagID.Equals(item.VirtualQuestionItemTagId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void InsertMultipleRecord(List<VirtualQuestionItemTag> items)
        {
            foreach(var item in items)
            {
                var entity = new VirtualQuestionItemTagEntity
                {
                    ItemTagID = item.ItemTagId,
                    VirtualQuestionID = item.VirtualQuestionId,
                };

                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
