using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.GroupPrinting;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class GroupPrintingService
    {
        private readonly IPrintingGroupRepository repository;
        private readonly IReadOnlyRepository<StudentGroup> studentsInGroup;

        public GroupPrintingService(IPrintingGroupRepository repository, IReadOnlyRepository<StudentGroup> studentsInGroup)
        {
            this.repository = repository;
            this.studentsInGroup = studentsInGroup;
        }

        public IQueryable<PrintingGroup> GetAllByCurrentUser(int userId, int districtId)
        {
            if (userId > 0)
            {
                return repository.Select().Where(o => o.IsActive && o.CreatedUserId == userId && o.DistrictId == districtId);
            }

            return repository.Select().Where(o => o.IsActive && o.DistrictId == districtId);
        }

        public bool DeletePrintingGroupById(int printingGroupId)
        {
            if (printingGroupId > 0)
            {
                PrintingGroup printingGroup = repository.Select().FirstOrDefault(o => o.Id == printingGroupId);
                if (printingGroup.IsNotNull())
                {
                    repository.Delete(printingGroup);
                    return true;
                }
            }
            return false;
        }

        public PrintingGroup Save(PrintingGroup printingGroup)
        {
            if (printingGroup.IsNotNull())
            {
                repository.Save(printingGroup);
                return printingGroup;
            }
            return null;
        }

        public PrintingGroup GetById(int printingGroupId)
        {
            return repository.Select().FirstOrDefault(o => o.Id == printingGroupId && o.IsActive);
        }

        public bool CheckUniqueGroupName(int groupId, int userId, string strGroupName)
        {
            if (groupId > 0)
            {
                return !repository.Select().Any(o => o.Id != groupId && o.Name.Equals(strGroupName) && o.CreatedUserId == userId && o.IsActive);
            }

            return !repository.Select().Any(o => o.Name.Equals(strGroupName) && o.CreatedUserId == userId && o.IsActive);
        }

        public List<PrintingGroup> GetPrintingGroupByNames(IEnumerable<string>groupNames, int districtId )
        {
            var query = repository.Select().Where(o => o.DistrictId == districtId && groupNames.Contains(o.Name));
            return query.ToList();
        }

        public IEnumerable<StudentGroup> GetGroupStudents(int groupId)
        {
            var aa = studentsInGroup.Select().Where(x => x.GroupId == groupId).ToList();
            return studentsInGroup.Select().Where(x => x.GroupId == groupId);
        }

        public List<int> GetListGroupIdsByUserId(int districtId, int userId)
        {
            return repository.GetListgroupIdsByUserId(districtId, userId);
        }
    }
}
