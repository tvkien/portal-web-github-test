using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualTestTimingRepository : IRepository<VirtualTestTiming>
    {
        private readonly Table<VirtualTestTimingEntity> table;

        public VirtualTestTimingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<VirtualTestTimingEntity>();
            Mapper.CreateMap<VirtualTestTiming, VirtualTestTimingEntity>();
        }

        public IQueryable<VirtualTestTiming> Select()
        {
            return table.Select(x => new VirtualTestTiming
                {
                    DistrictId = x.DistrictID,
                    TimingOptionId = x.TimingOptionID,
                    Value = x.Value,
                    VirtualTestTimingId = x.VirtualTestTimingID,
                    VirtualTestId = x.VirtualTestID
                });
        }

        public void Save(VirtualTestTiming item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestTimingID.Equals(item.VirtualTestTimingId));

            if (entity == null)
            {
                entity = new VirtualTestTimingEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.VirtualTestTimingId = entity.VirtualTestTimingID;
        }

        public void Delete(VirtualTestTiming item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestTimingID.Equals(item.VirtualTestTimingId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}