using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class RequestParameterService
    {
        private readonly IInsertSelect<RequestParameter> repository;

        public RequestParameterService(IInsertSelect<RequestParameter> repository)
        {
            this.repository = repository;
        }

        public void Insert(RequestParameter requestParameter)
        {
            if (requestParameter.IsNotNull())
            {
                repository.Save(requestParameter);
            }
        }
    }
}