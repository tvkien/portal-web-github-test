using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestMaker
{
    public class MultiPartVirtualQuestionExpressionRepository: IReadOnlyRepository<MultiPartVirtualQuestionExpressionDto>
    {
        private readonly Table<MultiPartVirtualQuestionExpressionEntity> _table;
        private readonly MultiPartQuestionContext _context;

        public MultiPartVirtualQuestionExpressionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = MultiPartQuestionContext.Get(connectionString).GetTable<MultiPartVirtualQuestionExpressionEntity>();
            _context = MultiPartQuestionContext.Get(connectionString);
        }

        public IQueryable<MultiPartVirtualQuestionExpressionDto> Select()
        {
            return _table.Select(x => new MultiPartVirtualQuestionExpressionDto
            {
                MultiPartVirtualQuestionExpressionId = x.MultiPartVirtualQuestionExpressionID,
                VirtualQuestionId = x.VirtualQuestionID,
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
    }
}
