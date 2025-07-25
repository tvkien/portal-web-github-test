using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.DataLocker;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ResultEntryTemplateSerice
    {
        private readonly IResultEntryTemplateRepository _resultEntryTemplateRepository;

        public ResultEntryTemplateSerice(IResultEntryTemplateRepository resultEntryTemplateRepository)
        {
            _resultEntryTemplateRepository = resultEntryTemplateRepository;
        }
        public IQueryable<ResultEntryTemplateModel> GetTemplate(int userId, int roleId, int districtId, bool archived)
        {
            return _resultEntryTemplateRepository.GetTemplates(userId, roleId, districtId, archived);
        }
    }
}
