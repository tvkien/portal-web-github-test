using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassTestResultListService
    {
        private readonly IReadOnlyRepository<ClassTestResultList> repository;

        public ClassTestResultListService(IReadOnlyRepository<ClassTestResultList> repository)
        {
            this.repository = repository;
        }

        public IQueryable<ClassTestResultList> GetByClassId(int classId)
        {
            return repository.Select().Where(x => x.ClassId.Equals(classId));
        }

        public IQueryable<ClassTestResultList> GetByClassIdList(List<int> listClassIds)
        {
            return repository.Select().Where(x => x.ClassId.HasValue && listClassIds.Contains(x.ClassId.Value));
        }
    }
}