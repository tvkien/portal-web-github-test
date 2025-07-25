using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassStudentCustomService 
    {
        private readonly IReadOnlyRepository<ClassStudentCustom> repository;

        public ClassStudentCustomService(IReadOnlyRepository<ClassStudentCustom> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Get Student infor from ClassStudent from classstudent table with student status is active
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public IQueryable<ClassStudentCustom> GetStudentActiveByClassId(int classId)
        {
            return repository.Select().Where(o => o.ClassId == classId && o.Status == 1);
        }

        public IQueryable<ClassStudentCustom> GetStudentActiveByClassIds(List<int> classIds)
        {
            return repository.Select().Where(o => classIds.Contains(o.ClassId)  && o.Status == 1);
        }
		public List<string> GetStudentsExistCodeDiffName(int districtId, int classId, List<Student> students)
        {
            List<string> studentCodes = students.Select(en => en.Code).ToList();

            // Get all duplicate students in database by student code
            var studentCodeExistsDiffClass =
                repository.Select().Where(x => x.DistrictId.Equals(districtId)
                && studentCodes.Contains(x.Code.ToLower())
                && x.ClassId != classId).ToList();
             List<string> lstReturn = new List<string>();
            foreach (var s in studentCodeExistsDiffClass)
            {
                var st =
                    students.FirstOrDefault(o => o.Code.ToLower().Equals(s.Code.ToLower()) &&
                                ( !o.LastName.ToUpper().Equals(s.LastName.ToUpper()) ||
                                  !o.FirstName.ToUpper().Equals(s.FirstName.ToUpper())) 
                            );
                if(st != null)
                    lstReturn.Add(st.AltCode);
            }
            return lstReturn;
        }
    }
}
