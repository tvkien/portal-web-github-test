using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestUtilitiesDefineTemplates;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public interface IConversionSetService
    {
        List<ConversionSet> GetAllConversionSet();
        int SaveConversionSet(int? conversionSetId, string fileName, List<ConversionSetDetailDto> details);
    }
}
