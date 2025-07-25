using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TermService
    {
        private readonly IReadOnlyRepository<Term> repository; 

        public TermService(IReadOnlyRepository<Term> repository)
        {
            this.repository = repository;
        }

        public Term GetTermById(int termId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(termId));
        }

        public IQueryable<Term> GetTermsByTeacher(int teacherId)
        {
            return repository.Select().Where(x => x.TeacherId.Equals(teacherId));
        }
    }
}