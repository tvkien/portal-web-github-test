using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ReportRepository : IRepository<Report>
    {
        private readonly Table<ReportEntity> table;

        public ReportRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<ReportEntity>();
            Mapper.CreateMap<Report, ReportEntity>();
        }

        public IQueryable<Report> Select()
        {
            return table.Select(x => new Report
                {
                    Id = x.ID,
                    Name = x.Name,
                    URL = x.URL,
                    DateCreated = x.DateCreated
                });
        }

        public void Save(Report item)
        {
            var entity = table.FirstOrDefault(x => x.ID.Equals(item.Id));
            
            if(entity.IsNull())
            {
                entity = new ReportEntity();
                table.InsertOnSubmit(entity);
            }
            
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.ID;
        }

        public void Delete(Report item)
        {
            var entity = table.FirstOrDefault(x => x.ID.Equals(item.Id));
            if(entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}