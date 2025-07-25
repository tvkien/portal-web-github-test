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
    public class AnswerSubRepository : IRepository<AnswerSubData>
    {
        private readonly Table<AnswerSubEntity> table;

        public AnswerSubRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<AnswerSubEntity>();
            Mapper.CreateMap<AnswerSubData, AnswerSubEntity>();
        }

        public IQueryable<AnswerSubData> Select()
        {
            return table.Select(x => new AnswerSubData
            {
                AnswerSubID = x.AnswerSubID,
                PointsEarned = x.PointsEarned,
                VirtulaQuestionSubID = x.VirtualQuestionSubID,
                AnswerText = x.AnswerText
            });
        }

        public void Save(AnswerSubData item)
        {
            var entity = table.FirstOrDefault(x => x.AnswerSubID.Equals(item.AnswerSubID));

            if(entity.IsNull())
            {
                entity = new AnswerSubEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.AnswerSubID = entity.AnswerSubID;
        }

        public void Delete(AnswerSubData item)
        {
            throw new NotImplementedException();
        }
    }
}
