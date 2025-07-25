using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassUserService
    {
        private readonly IRepository<User> userRepository;
        private readonly IRepository<ClassUser> classUserRepository;
        private readonly IReadOnlyRepository<ClassUserLOE> classUserLOERepository;
        private readonly IReadOnlyRepository<TeacherSchoolClass> teacherSchoolClassRepository;
        private readonly IReadOnlyRepository<ClassExtra> _classExtra;

        public ClassUserService(IRepository<User> userRepository, IRepository<ClassUser> classUserRepository,
            IReadOnlyRepository<ClassUserLOE> classUserLOERepository,
            IReadOnlyRepository<TeacherSchoolClass> teacherSchoolClassRepository,
            IReadOnlyRepository<ClassExtra> classExtra)
        {
            this.userRepository = userRepository;
            this.classUserRepository = classUserRepository;
            this.classUserLOERepository = classUserLOERepository;
            this.teacherSchoolClassRepository = teacherSchoolClassRepository;
            _classExtra = classExtra;
        }

        public IEnumerable<ClassUser> GetClassUsersByClassId(int classId)
        {
            var classUsers = classUserRepository.Select().Where(x => x.ClassId == classId).ToList();

            foreach (var classUser in classUsers)
            {
                classUser.User = userRepository.Select().FirstOrDefault(x => x.Id == classUser.UserId);
                classUser.ClassUserLOE = classUserLOERepository.Select().FirstOrDefault(x => x.Id == classUser.ClassUserLOEId);
            }

            return classUsers;
        }

        public IQueryable<ClassUser> Select()
        {
            return classUserRepository.Select();
        }

        public ClassUser GetClassUserById(int classUserId)
        {
            return classUserRepository.Select().FirstOrDefault(x => x.Id.Equals(classUserId));
        }

        public ClassUser GetPrimaryTeacherByClassId(int classId)
        {
            return classUserRepository.Select().FirstOrDefault(x => x.ClassId.Equals(classId) && x.ClassUserLOEId.Equals(1));
        }

        public List<int> GetPrimaryTeacherByClassIds(List<int> ids)
        {
            if (ids.Count >= 500)
            {
                int numberOfPage = (int)Math.Ceiling((double)ids.Count() / 500);
                var result = new List<int>();
                for (int i = 0; i < numberOfPage; i++)
                {
                    result.AddRange(classUserRepository.Select().Where(x => ids.Skip(i * 500).Take(500).Contains(x.ClassId) && x.ClassUserLOEId.Equals(1)).Select(x => x.UserId).ToList());
                }

                return result;
            }

            return classUserRepository.Select().Where(x => ids.Contains(x.ClassId) && x.ClassUserLOEId.Equals(1)).Select(x => x.UserId).ToList();
        }

        public IQueryable<TeacherSchoolClass> GetTeachersForSchoolByClassId(int classId)
        {
            return teacherSchoolClassRepository.Select().Where(x => x.ClassId == classId);
        }

        public IQueryable<ClassUserLOE> GetLOETypes()
        {
            return classUserLOERepository.Select();
        }

        public void InsertClassUser(ClassUser classUser)
        {
            classUserRepository.Save(classUser);
        }

        public void DeleteClassUser(ClassUser classUser)
        {
            if (classUser.IsNotNull())
            {
                classUserRepository.Delete(classUser);
            }
        }

        public bool ClassDoesNotHavePrimaryTeacher(int classId)
        {
            return GetPrimaryTeacherByClassId(classId).IsNull();
        }

        public bool CheckUserIsPrimaryTeacher(int classId, int userId)
        {
            var classUser = GetPrimaryTeacherByClassId(classId);
            if (classUser != null && classUser.UserId.Equals(userId))
            {
                return true;
            }
            return false;
        }

        public IQueryable<ClassUser> GetClassUsersByUserId(int userId)
        {
            return classUserRepository.Select().Where(x => x.UserId.Equals(userId));
        }

        public void ReplacePrimaryTeacherClassUser(ClassUser classUser)
        {
            var vClassUser = classUserRepository.Select()
                    .FirstOrDefault(o => o.ClassId == classUser.ClassId && o.ClassUserLOEId == (int)ClassUserLoeType.Primary);
            if (vClassUser != null)
            {
                vClassUser.ClassUserLOEId = (int)ClassUserLoeType.CoTeachers;
                classUserRepository.Save(vClassUser);
            }
            var vClassUserSecond = classUserRepository.Select()
                    .FirstOrDefault(o => o.ClassId == classUser.ClassId && o.UserId == classUser.UserId);
            if (vClassUserSecond != null)
            {
                classUserRepository.Delete(vClassUserSecond);
            }
            classUserRepository.Save(classUser);
        }

        /// <summary>
        /// Get All Class By SchoolID, DistrictTermID and UserID in ClassUser
        /// </summary>
        /// <param name="termId"></param>
        /// <param name="userId"></param>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public IQueryable<ClassExtra> GetClassesBySchoolIdTermIdAndUserIdOnClassUser(int termId, int userId,
            int schoolId)
        {
            return
                _classExtra.Select()
                    .Where(o => o.SchoolId == schoolId && o.DistrictTermId == termId && o.UserIdClassUser == userId)
                    .OrderBy(o => o.Name);
        }
    }
}
