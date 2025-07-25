using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class PrintingGroupJobRepository : IRepository<PrintingGroupJob>
    {
        private readonly Table<PrintingGroupJobEntity> table;

        public PrintingGroupJobRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<PrintingGroupJobEntity>();
            Mapper.CreateMap<PrintingGroupJob, PrintingGroupJobEntity>();
        }

        public IQueryable<PrintingGroupJob> Select()
        {
            return table.Select(x => new PrintingGroupJob
                {
                    PrintingGroupJobID = x.PrintingGroupJobID,
                    GroupID = x.GroupID,
                    DateCreated = x.DateCreated,
                    CreatedUserID = x.CreatedUserID ?? 0
                });
        }

        public void Save(PrintingGroupJob item)
        {
            var entity = table.FirstOrDefault(x => x.GroupID.Equals(item.PrintingGroupJobID));

            if (entity.IsNull())
            {
                entity = new PrintingGroupJobEntity();
                entity.CreatedUserID = item.CreatedUserID;
                entity.DateCreated = item.DateCreated;
                entity.GroupID = item.GroupID;
                table.InsertOnSubmit(entity);
            }
            table.Context.SubmitChanges();
            item.PrintingGroupJobID = entity.PrintingGroupJobID;
        }

        public void Delete(PrintingGroupJob item)
        {
            var entity = table.FirstOrDefault(x => x.PrintingGroupJobID.Equals(item.PrintingGroupJobID));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}