using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TimingOptionRepository : IRepository<TimingOption>
    {
        private readonly Table<TimingOptionEntity> table;

        public TimingOptionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<TimingOptionEntity>();
            Mapper.CreateMap<School, SchoolEntity>();
        }

        public IQueryable<TimingOption> Select()
        {
            return table.Select(x => new TimingOption
                {
                    Description = x.Description,
                    Name = x.Name,
                    TimingSettingId = x.TimingSettingID,
                    Value = x.Value
                });
        }

        public void Save(TimingOption item)
        {
            var entity = table.FirstOrDefault(x => x.TimingSettingID.Equals(item.TimingSettingId));

            if (entity == null)
            {
                entity = new TimingOptionEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.TimingSettingId = entity.TimingSettingID;
        }

        public void Delete(TimingOption item)
        {
            var entity = table.FirstOrDefault(x => x.TimingSettingID.Equals(item.TimingSettingId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}