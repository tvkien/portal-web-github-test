using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;
using LinkIt.BubbleSheetPortal.Data.Repositories;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class VirtualTestWithOutTestResultService
    {
        private readonly IReadOnlyRepository<VirtualTestWithOutTestResult> repository;
        private readonly IManageTestRepository _manageTestRepository;
        private readonly IVirtualTestRepository _virtualTestRepository;
        private readonly IBankRepository _bankRepository;

        public VirtualTestWithOutTestResultService(IReadOnlyRepository<VirtualTestWithOutTestResult> repository,
            IManageTestRepository manageTestRepository, IVirtualTestRepository virtualTestRepository, IBankRepository bankRepository)
        {   
            this.repository = repository;
            _manageTestRepository = manageTestRepository;
            _virtualTestRepository = virtualTestRepository;
            _bankRepository = bankRepository;

        }

        public IQueryable<VirtualTestWithOutTestResult> GetVirtualTestWithOutTestResult(int authorId, int districtId)
        {
            if (authorId != 0)
            {
                return repository.Select().Where(o => o.AuthorUserId == authorId && o.DistrictId == districtId).OrderBy(o => o.Name);
            }

            return repository.Select().Where(o => o.DistrictId == districtId).OrderBy(o => o.Name);
        }

        public IQueryable<ListItem> GetVirtualTestWithOutTestResultForPublisher(int districtId)
        {
            return _virtualTestRepository.VirtualTestWithOutTestResultForPublisher(districtId).AsQueryable();
        }
    }
}
