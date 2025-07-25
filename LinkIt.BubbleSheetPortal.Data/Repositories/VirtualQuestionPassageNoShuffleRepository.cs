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
    public class VirtualQuestionPassageNoShuffleRepository : IVirtualQuestionPassageNoShuffleRepository
    {
        private readonly Table<VirtualQuestionPassageNoShuffleEntity> table;
        private readonly TestDataContext dbContext;

        public VirtualQuestionPassageNoShuffleRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = TestDataContext.Get(connectionString);
            table = dbContext.GetTable<VirtualQuestionPassageNoShuffleEntity>();
            Mapper.CreateMap<VirtualQuestionPassageNoShuffle, VirtualQuestionPassageNoShuffleEntity>();
        }

        public IQueryable<VirtualQuestionPassageNoShuffle> Select()
        {
            return
                table.Select(x => new VirtualQuestionPassageNoShuffle
                {

                    VirtualQuestionPassageNoShuffleID = x.VirtualQuestionPassageNoShuffleID,
                    QTI3pPassageID = x.QTI3pPassageId ?? 0,
                    QTIRefObjectID = x.QTIRefObjectID ?? 0,
                    DataFileUploadPassageID = x.DataFileUploadPassageID ?? 0,
                    VirtualQuestionID = x.VirtualQuestionID,
                    PassageURL = x.PassageURL
                });
        }

        public void Save(VirtualQuestionPassageNoShuffle item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionPassageNoShuffleID.Equals(item.VirtualQuestionPassageNoShuffleID));

            if (entity.IsNull())
            {
                entity = new VirtualQuestionPassageNoShuffleEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.VirtualQuestionPassageNoShuffleID = entity.VirtualQuestionPassageNoShuffleID;
        }

        public void Delete(VirtualQuestionPassageNoShuffle item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.VirtualQuestionPassageNoShuffleID.Equals(item.VirtualQuestionPassageNoShuffleID));
                if (entity != null)
                {
                    table.DeleteOnSubmit(entity);
                    table.Context.SubmitChanges();
                }
            }
        }

        public void DeleteAllPassageNoshuffle(int virtualquestionId)
        {
            var entitys = table.Where(x => x.VirtualQuestionID.Equals(virtualquestionId));
            if (entitys.Any())
            {
                table.DeleteAllOnSubmit(entitys);
                table.Context.SubmitChanges();
            }
        }
    }
}