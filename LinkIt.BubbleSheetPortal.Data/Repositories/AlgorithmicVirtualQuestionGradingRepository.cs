using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AlgorithmicVirtualQuestionGradingRepository : IAlgorithmicVirtualQuestionGradingRepository
    {
        private readonly Table<AlgorithmicVirtualQuestionGradingEntity> table;
        private readonly AlgorithmicContext _context;

        public AlgorithmicVirtualQuestionGradingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AlgorithmicContext.Get(connectionString).GetTable<AlgorithmicVirtualQuestionGradingEntity>();
            _context = AlgorithmicContext.Get(connectionString);
            Mapper.CreateMap<AlgorithmicVirtualQuestionGrading, AlgorithmicVirtualQuestionGradingEntity>();
        }

        public IQueryable<AlgorithmicVirtualQuestionGrading> Select()
        {
            return table.Select(x => new AlgorithmicVirtualQuestionGrading
            {
                AlgorithmID = x.AlgorithmID,
                PointsEarned = x.PointsEarned,
                VirtualQuestionID = x.VirtualQuestionID,
                Expression = x.Expression,
                Order = x.Order,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                Rules = x.Rules
            });
        }

        public void Save(AlgorithmicVirtualQuestionGrading item)
        {
            var entity = table.FirstOrDefault(x => x.AlgorithmID == item.AlgorithmID);

            if (entity == null)
            {
                entity = new AlgorithmicVirtualQuestionGradingEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.AlgorithmID = entity.AlgorithmID;
        }

        public void Delete(AlgorithmicVirtualQuestionGrading item)
        {
            throw new System.NotImplementedException();
        }

        private void MapModelToEntity(AlgorithmicVirtualQuestionGrading item, AlgorithmicVirtualQuestionGradingEntity entity)
        {
            entity.VirtualQuestionID = item.VirtualQuestionID;
            entity.Expression = item.Expression;
            entity.PointsEarned = item.PointsEarned;
            entity.Order = item.Order;
            entity.CreatedBy = item.CreatedBy;
            entity.CreatedDate = item.CreatedDate;
            entity.UpdatedBy = item.UpdatedBy;
            entity.UpdatedDate = item.UpdatedDate;
            entity.Rules = item.Rules;
        }

        public void InsertMultipleRecord(List<AlgorithmicVirtualQuestionGrading> items)
        {
            foreach (var item in items)
            {
                var entity = new AlgorithmicVirtualQuestionGradingEntity();

                MapModelToEntity(item, entity);
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
