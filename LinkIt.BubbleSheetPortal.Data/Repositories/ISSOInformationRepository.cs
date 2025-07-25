using LinkIt.BubbleSheetPortal.Models.SSO;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISSOInformationRepository: IRepository<SSOInformation>
    {
        SSOInformation GetByTpe(string type);
    }
}
