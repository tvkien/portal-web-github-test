using System.Data.Linq;
using System.Linq;
using System.Xml.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AlgorithmQTIItemGradingRepository : IAlgorithmQTIItemGradingRepository
    {
        private readonly Table<AlgorithmQTIItemGradingEntity> table;
        private readonly AlgorithmicContext _context;

        public AlgorithmQTIItemGradingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AlgorithmicContext.Get(connectionString).GetTable<AlgorithmQTIItemGradingEntity>();
            _context = AlgorithmicContext.Get(connectionString);
            Mapper.CreateMap<AlgorithmQTIItemGrading, AlgorithmQTIItemGradingEntity>();
        }

        public IQueryable<AlgorithmQTIItemGrading> Select()
        {
            return table.Select(x => new AlgorithmQTIItemGrading
            {
                AlgorithmID = x.AlgorithmID,
                PointsEarned = x.PointsEarned,
                QTIItemID = x.QTIItemID,
                Expression = x.Expression,
                Order = x.Order,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                Rules = x.Rules
            });
        }

        public void AlgorithmicSaveExpression(int qtiItemId, int virtualQuestionId, string expressionXML, int userId)
        {
            _context.AlgorithmicSaveExpression(qtiItemId, virtualQuestionId, GetElement(expressionXML), userId);
        }

        private XElement GetElement(string xml)
        {
            return XElement.Parse(xml);
        }

        public void Save(AlgorithmQTIItemGrading item)
        {
            var entity = table.FirstOrDefault(x => x.AlgorithmID == item.AlgorithmID);

            if (entity == null)
            {
                entity = new AlgorithmQTIItemGradingEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.AlgorithmID = entity.AlgorithmID;
        }

        public void Delete(AlgorithmQTIItemGrading item)
        {
            throw new System.NotImplementedException();
        }

        private void MapModelToEntity(AlgorithmQTIItemGrading item, AlgorithmQTIItemGradingEntity entity)
        {
            entity.QTIItemID = item.QTIItemID;
            entity.Expression = item.Expression;
            entity.PointsEarned = item.PointsEarned;
            entity.Order = item.Order;
            entity.CreatedBy = item.CreatedBy;
            entity.CreatedDate = item.CreatedDate;
            entity.UpdatedBy = item.UpdatedBy;
            entity.UpdatedDate = item.UpdatedDate;
            entity.Rules = item.Rules;
        }
    }
}
