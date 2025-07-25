using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassStudentDataService
    {
        private readonly IRepository<ClassStudentData> repository;

        public ClassStudentDataService(IRepository<ClassStudentData> repository)
        {
            this.repository = repository;
        }

        public void Save(ClassStudentData item)
        {
            repository.Save(item);
        }

        public void Delete(ClassStudentData item)
        {
            repository.Delete(item);
        }

        private void MoveStudent(int studentId, int oldClassId, int newClassId)
        {
            ClassStudentData classStudent = repository.Select().FirstOrDefault(c => c.ClassID.Equals(oldClassId) && c.StudentID.Equals(studentId));
            if (classStudent.IsNull())
            {
                classStudent = new ClassStudentData();
                classStudent.StudentID = studentId;
                classStudent.ClassID = newClassId;
                Save(classStudent);
            }
            else
            {
                classStudent.ClassID = newClassId;
                Save(classStudent);
            }
        }

        public void MoveStudents(List<int> movedStudents, int oldClassId, int newClassId)
        {
            foreach (var studentId in movedStudents)
            {
                MoveStudent(studentId, oldClassId, newClassId);
            }
        }

        public ClassStudentData GetClassStudent(int studentId, int classId)
        {
            return repository.Select().FirstOrDefault(c => c.ClassID.Equals(classId) && c.StudentID.Equals(studentId));
        }

        public void RemoveStudentFromClass(int studentId, int classId)
        {
            ClassStudentData classStudent = GetClassStudent(studentId, classId);
            if (classStudent.IsNotNull())
            {
                classStudent.Active = false;
                repository.Save(classStudent);
            }
        }

        public void RemoveStudentsFromClass(List<int> studentIdList, int classId)
        {
            var classStudents = repository.Select().Where(o => o.ClassID.Equals(classId) && studentIdList.Contains(o.StudentID)).ToList();

            foreach (var classStudent in classStudents)
            {
                repository.Delete(classStudent);
            } 
        }

        public bool HasAnyStudentInClass(int classId)
        {
            return repository.Select().Any(c => c.ClassID.Equals(classId));
        }

        public ClassStudentData GetByClassIdStudentId(int classId, int studentId)
        {
            return repository.Select().FirstOrDefault(c => c.ClassID.Equals(classId) && c.StudentID.Equals(studentId));
        }

        public void DeleteClassStudent(int classId, int studentId)
        {
            ClassStudentData classStudentTable = GetByClassIdStudentId(classId, studentId);
            if (classStudentTable.IsNotNull())
            {
                repository.Delete(classStudentTable);
            }
        }

        public void SaveClassStudent(ClassStudentData classStudent)
        {
            repository.Save(classStudent);
        }
    }
}