using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetDataService
    {
        private readonly IReadOnlyRepository<BubbleSheetData> repository; 

        public BubbleSheetDataService(IReadOnlyRepository<BubbleSheetData> repository)
        {
            this.repository = repository;
        }

        public BubbleSheetData GetBubbleSheetDataObject(BubbleSheetData model)
        {
            return repository.Select().FirstOrDefault(x =>
                    x.TestId.Equals(model.TestId)
                    && x.BankId.Equals(model.BankId)
                    && x.StateId.Equals(model.StateId)
                    && x.GradeId.Equals(model.GradeId)
                    //&& x.SubjectId.Equals(model.SubjectId)
                    && x.SubjectName.Equals(model.SubjectName)
                    && x.DistrictTermId.Equals(model.DistrictTermId)
                    && x.ClassId.Equals(model.ClassId)
                    && x.SchoolId.Equals(model.SchoolId)
                    && x.UserId.Equals(model.UserId));
        }
    }
}