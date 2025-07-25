using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Repositories.ClassPrinting;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassPrintingGroupService
    {
        private readonly IClassPrintingGroupRepository repository;

        public ClassPrintingGroupService(IClassPrintingGroupRepository repository)
        {
            this.repository = repository;
        }

        public IQueryable<ClassPrintingGroup> GetByGroupId(int groupId)
        {
            return repository.Select().Where(o => o.GroupID == groupId);
        }

        public void SaveClassPrintingGroupByGroupId(int groupId, List<string> lstClassId, List<int> teacherIds)
        {
            if (groupId <= 0 || lstClassId == null) return;
            DeleteClassPrintingGroupByGroupId(groupId);
            for (var i = 0; i < lstClassId.Count; i++)
            {
                int classId;
                if (!int.TryParse(lstClassId[i], out classId)) continue;
                var classPrintingGroup = new ClassPrintingGroup
                                             {
                                                 ClassID = classId,
                                                 GroupID = groupId,
                                                 UserId = teacherIds[i]
                                             };
                repository.Save(classPrintingGroup);
            }
        }

        private void DeleteClassPrintingGroupByGroupId(int groupId)
        {
            var listClassprintinggroup = repository.Select().Where(o => o.GroupID == groupId);
            if (listClassprintinggroup.IsNotNull())
            {
                foreach (ClassPrintingGroup cpg in listClassprintinggroup)
                {
                    repository.Delete(cpg);
                }
            }
        }

        public IQueryable<int> GetClassesByGroupId(int groupId)
        {
            return repository.Select().Where(o => o.GroupID == groupId).Select(g => g.ClassID);
        }

        public void SaveClassInGroup(int groupId, IEnumerable<int> classIds)
        {
            if (groupId > 0 && classIds.IsNotNull() && classIds.Any())
            {   
                foreach (var c in classIds)
                {
                    if (!repository.Select().Any(o=>o.GroupID == groupId && o.ClassID == c))
                    {
                        var classPrintingGroup = new ClassPrintingGroup();
                        classPrintingGroup.ClassID = c;
                        classPrintingGroup.GroupID = groupId;
                        repository.Save(classPrintingGroup);
                    }
                }
            }
        }

        public int CountActiveStudentInGroup(int groupId)
        {
            return repository.CountActiveStudentInGroup(groupId);
        }
    }
}
