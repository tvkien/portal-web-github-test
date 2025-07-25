using System.Data.Linq;
using System.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.MasterData
{
    public class ShortLinkRepository : IShortLinkRepository
    {

        private readonly Table<ShortLinkEntity> _table;

        public ShortLinkRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<ShortLinkEntity>();
            Mapper.CreateMap<ShortLinkDto, ShortLinkEntity>();
            Mapper.CreateMap<ShortLinkEntity, ShortLinkDto>();
        }

        public void Add(ShortLinkDto model)
        {
            var entity = new ShortLinkEntity()
            {
                Code = model.Code,
                FullLink = model.FullLink,
                QTITestClassAssignmentId = model.QTITestClassAssignmentId
            };

            _table.InsertOnSubmit(entity);
            _table.Context.SubmitChanges();
            model.ShortLinkID = entity.ShortLinkID;
        }

        public string GetFullLinkByCode(string code)
        {
            var fullLink = string.Empty;
            var entity = _table.FirstOrDefault(m => m.Code == code);

            if (entity != null)
            {
                fullLink = entity.FullLink;
            }

            return fullLink;
        }
    }
}
