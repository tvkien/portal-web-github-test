using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassStudentService
    {
        private readonly IReadOnlyRepository<ClassStudent> repository;
        private readonly IRepository<ClassStudent> addEditRepository;
        private readonly IRepository<ClassStudentData> classStudentRepository;
        private readonly IRepository<SGOStudent> sgoStudentRepository;

        public ClassStudentService(IReadOnlyRepository<ClassStudent> repository, 
            IRepository<ClassStudent> addEditRepository,
            IRepository<ClassStudentData> classStudentRepository,
            IRepository<SGOStudent> sgoStudentRepository)
        {
            this.repository = repository;
            this.addEditRepository = addEditRepository;
            this.classStudentRepository = classStudentRepository;
            this.sgoStudentRepository = sgoStudentRepository;
        }

        public IQueryable<ClassStudent> GetClassStudentsByClassId(int classId)
        {
           return repository.Select().Where(x => x.ClassId.Equals(classId));
        }

        public void DeleteClassStudentsOfClass(int classId)
        {
            var classStudents = addEditRepository.Select().Where(x => x.ClassId.Equals(classId));
            foreach (var classStudent in classStudents)
            {                
                addEditRepository.Delete(classStudent);
            }
        }

        public ClassStudent GetClassStudentByStudentId(int studentId)
        {
            return repository.Select().FirstOrDefault(x => x.StudentId.Equals(studentId));
        }

        public IQueryable<ClassStudent> GetClassStudentsByStudentId(int studentId)
        {
            return repository.Select().Where(x => x.StudentId.Equals(studentId));
        }

        public IQueryable<ClassStudent> GetClassStudentsByClassIdList(List<int> listClassIds)
        {
            return repository.Select().Where(x => listClassIds.Contains(x.ClassId));
        }

        public void SaveClassStudent(ClassStudent obj)
        {
            if(obj != null)
                addEditRepository.Save(obj);
        }

        public IQueryable<ClassStudentData> GetClassStudentDataByClassId(int classId)
        {
            return classStudentRepository.Select().Where(x => x.ClassID.Equals(classId));
        }

        public bool HasClassLinkedToSGO(int classID)
        {
            return sgoStudentRepository.Select().Any(c => c.ClassID == classID);
        }

        public bool HasStudentNotInClass(int classId, string studentIdString)
        {
            if (!string.IsNullOrEmpty(studentIdString))
            {
                var studentIds = studentIdString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var classStudents = classStudentRepository.Select().Where(x => x.ClassID.Equals(classId) && studentIds.Contains(x.StudentID.ToString())).ToList() ;
                if (studentIds.Count != classStudents.Count)
                    return false;
            }
            return true;
        }

        public void SurveyInsertClassStudent(int classId, int StudentId)
        {
            var objClassStudent = addEditRepository.Select().FirstOrDefault(o => o.ClassId == classId && o.StudentId == StudentId);
            if(objClassStudent == null)
            {
                objClassStudent = new ClassStudent();
                objClassStudent.ClassId = classId;
                objClassStudent.StudentId = StudentId;
                addEditRepository.Save(objClassStudent);
            }
        }

    }
}
