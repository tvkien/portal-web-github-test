using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualQuestionStateStandardRepository : IVirtualQuestionStateStandardRepository
    {
        private readonly Table<VirtualQuestionStateStandardEntity> table;
        private readonly IConnectionString _connectionString;

        public VirtualQuestionStateStandardRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualQuestionStateStandardEntity>();
            _connectionString = conn;
        }

        public IQueryable<VirtualQuestionStateStandard> Select()
        {
            return table.Select(x => new VirtualQuestionStateStandard
                {
                    VirtualQuestionStateStandardId = x.VirtualQuestionStateStandardID,
                    VirtualQuestionId = x.VirtualQuestionID,
                    StateStandardId = x.StateStandardID
                });
        }

        public void Save(VirtualQuestionStateStandard item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionStateStandardID.Equals(item.VirtualQuestionStateStandardId));

            if (entity == null)
            {
                entity = new VirtualQuestionStateStandardEntity();
                table.InsertOnSubmit(entity);
            }
            entity.VirtualQuestionID = item.VirtualQuestionId;
            entity.StateStandardID = item.StateStandardId;

            table.Context.SubmitChanges();
            item.VirtualQuestionStateStandardId = entity.VirtualQuestionStateStandardID;
        }

        public void Delete(VirtualQuestionStateStandard item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionStateStandardID.Equals(item.VirtualQuestionStateStandardId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void InsertMultipleRecord(List<VirtualQuestionStateStandard> items)
        {
            var bulkHelper = new BulkHelper(_connectionString);
            bulkHelper.BulkCopy(items, "dbo.VirtualQuestionStateStandard");
        }
    }
}
