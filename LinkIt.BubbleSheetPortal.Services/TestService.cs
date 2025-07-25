using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestService
    {
        private readonly IReadOnlyRepository<Test> repository;

        public TestService(IReadOnlyRepository<Test> repository)
        {
            this.repository = repository;
        }

        public Test GetTestById(int testId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(testId));
        }

        public IQueryable<Test> GetValidTestsByBank(List<int> bankIds, bool isIncludeRetake = false)
        {
            return repository.Select()
                .Where(x => bankIds.Contains(x.BankId) && x.QuestionCount > 0 &&
                            (x.DataSetOriginID == (int)DataSetOriginEnum.Item_Based_Score
                            || (isIncludeRetake && x.DataSetOriginID == (int)DataSetOriginEnum.Item_Based_Score_Retake)));
        }

        public IQueryable<Test> GetNonACTValidTestsByBank(int bankId)
        {
            return repository.Select().Where(x => x.BankId.Equals(bankId) && x.QuestionCount > 0 && x.DataSetOriginID == (int)DataSetOriginEnum.Item_Based_Score);
        }

        public int GetVirtualTestSubTypeIDByTestId(int testId)
        {
            var obj = repository.Select().FirstOrDefault(x => x.Id.Equals(testId));
            if (obj != null)
                return obj.VirtualTestSubTypeId.GetValueOrDefault();
            return 0;
        }

        public IQueryable<Test> GetValidSurveyByBank(int bankId)
        {
            return repository.Select().Where(x => x.BankId.Equals(bankId) && x.QuestionCount > 0 && x.DataSetOriginID == (int)DataSetOriginEnum.Survey);
        }
    }
}
