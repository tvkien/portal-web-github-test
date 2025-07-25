using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class XLIAreaService
    {
        private readonly IReadOnlyRepository<XliArea> _repository;

        public XLIAreaService(IReadOnlyRepository<XliArea> repository)
        {
            _repository = repository;
        }

        public IEnumerable<XLIAreaDto> GetXLIAreas()
        {
            return _repository.Select().Where(x => x.Restrict)
                .Select(x => new XLIAreaDto
                {
                    Id = x.XliAreaId,
                    Name = x.Name,
                    Code = x.Code,
                    DisplayTooltip = x.DisplayTooltip
                })
                .ToArray();
        }
    }
}
