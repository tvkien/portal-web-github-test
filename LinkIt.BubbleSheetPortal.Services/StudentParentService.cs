using System;
using System.Linq;
using System.Transactions;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentParentService
    {
        private readonly IReadOnlyRepository<StudentParent> _repository;
        private readonly IInsertDeleteRepository<StudentParent> _insertDeleteRepository;
        private readonly IStudentParentRepository _studentParentRepository;
        private readonly IRepository<User> _userRepository;


        public StudentParentService(IReadOnlyRepository<StudentParent> repository, IStudentParentRepository studentParentRepository,
            IInsertDeleteRepository<StudentParent> insertDeleteRepository, IRepository<User> userRepository)
        {
            _repository = repository;
            _studentParentRepository = studentParentRepository;
            _insertDeleteRepository = insertDeleteRepository;
            _userRepository = userRepository;
        }

        public IQueryable<StudentParentList> GetStudentParentListByDistrictID(int? districtID)
        {
            return
                _studentParentRepository
                    .GetStudentParentListByDistrictID(districtID)
                    .Select(s => new StudentParentList
                                     {
                                         StudentID =
                                             s.StudentID,
                                         ClassCount =
                                             s.ClassCount,
                                         FirstName =
                                             s.FirstName,
                                         Gender =
                                             s.Gender,
                                         GradeName = s.GradeName,
                                         GradeOrder = s.GradeOrder,
                                         LastName =
                                             s.LastName,
                                         ParentCount =
                                             s.ParentCount,
                                         School =
                                             s.School,
                                         FirstClassName = s.FirstClassName,
                                         FirstParentName = s.FirstParentName
                                     }).ToList().AsQueryable();
        }

        public IQueryable<ClassWithDetail> GetClassWithDetailListByStudentID(int studentID)
        {
            return _studentParentRepository.GetClassesWithDetailByStudentID(studentID).Select(s => new ClassWithDetail
                                                                                                       {
                                                                                                           ClassID =
                                                                                                               s.ClassID,
                                                                                                           ClassName =
                                                                                                               s.Name,
                                                                                                           Course =
                                                                                                               s.Course,
                                                                                                           Section =
                                                                                                               s.Section,
                                                                                                           TeacherFirstName
                                                                                                               =
                                                                                                               s.
                                                                                                               TeacherFirstName,
                                                                                                           TeacherLastName
                                                                                                               =
                                                                                                               s.
                                                                                                               TeacherLastName,
                                                                                                           TermName =
                                                                                                               s.TermName
                                                                                                       }).ToList().
                AsQueryable();
        }

        public IQueryable<ParentDetail> GetParentsByStudentID(int studentID)
        {
            return _studentParentRepository.GetParentsByStudentID(studentID).Select(s => new ParentDetail
                                                                                             {
                                                                                                 FirstName = s.NameFirst,
                                                                                                 LastName = s.NameLast,
                                                                                                 EmailAddress = s.Email,
                                                                                                 MessageNumber =
                                                                                                     s.MessageNumber,
                                                                                                 Phone = s.Phone,
                                                                                                 UserID = s.UserID
                                                                                             }).ToList().AsQueryable();
        }

        public IQueryable<StudentOfParent> GetStudentsByParentID(int parentID)
        {
            return _studentParentRepository.GetStudentsByParentID(parentID).Select(s => new StudentOfParent
                                                                                            {
                                                                                                FirstName = s.FirstName,
                                                                                                LastName = s.LastName,
                                                                                                StudentID = s.StudentID,
                                                                                                Fullname = s.FullName
                                                                                            }).ToList().AsQueryable();
        }

        public IQueryable<ParentDetail> GetNotAssignParentsOfStudent(int studentID)
        {
            return _studentParentRepository.GetNotAssignParentsOfStudent(studentID).Select(s => new ParentDetail
            {
                FirstName = s.NameFirst,
                LastName = s.NameLast,
                EmailAddress = s.Email,
                Phone = s.Phone,
                UserID = s.UserID,
                MessageNumber = s.MessageNumber
            }).ToList().AsQueryable();
        }

        public void DeleteStudentParent(StudentParent studentParent)
        {
            _insertDeleteRepository.Delete(studentParent);
        }

        public void AddStudentParent(int studentId, int parentId, string relationship, bool studentDataAccess)
        {
            var studentParent = GetStudentParentById(studentId, parentId);
            if (studentParent.IsNull())
            {
                studentParent = new StudentParent
                {
                    ParentID = parentId,
                    StudentID = studentId,
                    Relationship = relationship,
                    StudentDataAccess = studentDataAccess
                };
                _insertDeleteRepository.Save(studentParent);
            }
        }

        public StudentParent GetStudentParentById(int studentId, int parentId)
        {
            return _repository.Select().FirstOrDefault(x => x.ParentID.Equals(parentId) && x.StudentID.Equals(studentId));
        }
    }
}
