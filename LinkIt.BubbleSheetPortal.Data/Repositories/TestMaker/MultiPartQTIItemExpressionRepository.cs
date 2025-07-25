using AutoMapper;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;
using System.Data.Linq;
using System.Linq;
using System.Xml.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestMaker
{
    public class MultiPartQtiItemExpressionRepository: IMultiPartQtiItemExpressionRepository
    {
        private readonly Table<MultiPartQTIItemExpressionEntity> _table;
        private readonly MultiPartQuestionContext _context;

        public MultiPartQtiItemExpressionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = MultiPartQuestionContext.Get(connectionString);
            _table = _context.GetTable<MultiPartQTIItemExpressionEntity>();
        }

        public IQueryable<MultiPartQtiItemExpressionDto> Select()
        {
            return _table.Select(x => new MultiPartQtiItemExpressionDto
            {
                MultiPartQtiItemExpressionId = x.MultiPartQTIItemExpressionID,
                QtiItemId = x.QTIItemID,
                Expression = x.Expression,
                EnableElements = x.EnableElements,
                Order = x.Order,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                Rules = x.Rules
            });
        }

        public void SaveExpression(int qtiItemId, int virtualQuestionId, string expressionXML, int userId)
        {
            _context.MultiPartSaveExpression(qtiItemId, virtualQuestionId, GetElement(expressionXML), userId);
        }

        private XElement GetElement(string xml)
        {
            return XElement.Parse(xml);
        }

        public void DuplicateExpression(int oldQtiItemId, int newQtiItemId, int userId)
        {
            _context.DuplicateMultiPartQTIItemExpression(oldQtiItemId, newQtiItemId, userId);
        }
    }
}
