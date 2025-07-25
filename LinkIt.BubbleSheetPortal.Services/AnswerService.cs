using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AnswerService
    {
        private readonly IAnswerRepository repository;

        public AnswerService(IAnswerRepository repository)
        {
            this.repository = repository;
        }

        public Answer GetAnswerById(int answerId)
        {
            return repository.Select().FirstOrDefault(x => x.AnswerID.Equals(answerId));
        }
        
        public bool RegradeAnswerByListTestResult(List<int> lstTestResultId)
        {   
            if(lstTestResultId !=null && lstTestResultId.Count > 0)
            {
                bool bResult = true;
                foreach (var testResultId in lstTestResultId)
                {
                    //TODO: should be check Regrade success or no ?
                    if (!repository.RegradeTestByTestResultId(testResultId))
                        bResult = false;
                }
                return bResult;
            }
            return false;
        }

        public bool purgeTestByVirtualTestId (int virtualTestId)
        {
            if (virtualTestId > 0)
            {
                return repository.PurgeTest(virtualTestId);
            }
            return false;
        }
         
    }
}
