using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserGroup;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class XLIModuleService
    {
        private readonly IReadOnlyRepository<XliModule> _repository;

        public XLIModuleService(IReadOnlyRepository<XliModule> repository)
        {
            _repository = repository;
        }

        public IEnumerable<XLIModuleDto> GetXLIModules(int? areaId)
        {
            var modules = _repository.Select().Where(x => x.Restrict);

            if (areaId.HasValue)
                modules = modules.Where(x => x.XliAreaId == areaId);

            return modules.Select(x => new XLIModuleDto
                {
                    Id = x.XliModuleId,
                    XLIAreaId = x.XliAreaId,
                    Name = x.Name,
                    Code = x.Code,
                    DisplayName = x.DisplayName,
                    DisplayTooltip = x.DisplayTooltip,
                    ModuleOrder = x.ModuleOrder
                })
                .ToArray();
        }
    }
}
