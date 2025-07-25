using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentTransferService
    {
        private readonly IUnitOfWorkRepository<TestResult> results;
        private readonly IRepository<ClassStudent> classStudents;
        private readonly IRepository<Teacher> teacheRepository;

        public StudentTransferService(IUnitOfWorkRepository<TestResult> results, IRepository<ClassStudent> classStudents, IRepository<Teacher> teacheRepository)
        {
            this.results = results;
            this.classStudents = classStudents;
            this.teacheRepository = teacheRepository;
        }

        public void TransferStudent(Student student, Class fromClass, Class toClass)
        {
            ValidateTransferData(student, fromClass, toClass);
            var oldClass = UpdateOldClass(student, fromClass);
            var newClass = CreateOrUpdateNewClass(student, toClass);

            classStudents.Save(oldClass);
            classStudents.Save(newClass);
        }

        public void TransferTests(Student student, Class fromClass, Class toClass)
        {
            ValidateTransferData(student, fromClass, toClass);
            var teacher = teacheRepository.Select().FirstOrDefault(x => x.Id == toClass.TeacherId);
            int? teacherid = null;
            if (teacher != null)
                teacherid = teacher.Id;
            var relatedResults = results.Select().Where(x => x.ClassId == fromClass.Id && x.StudentId == student.Id);
            foreach (var relatedResult in relatedResults)
            {
                relatedResult.ClassId = toClass.Id;
                relatedResult.DistrictTermId = toClass.DistrictTermId.GetValueOrDefault();
                relatedResult.TermId = toClass.TermId;
                relatedResult.SchoolId = toClass.SchoolId.GetValueOrDefault();
                relatedResult.TeacherId = teacherid;
                relatedResult.UserId = toClass.UserId.GetValueOrDefault();
                results.SaveOnSubmit(relatedResult);
            }
            results.SaveChanges();
        }

        private ClassStudent CreateOrUpdateNewClass(Student student, Class toClass)
        {
            var destinationClass = classStudents.Select().FirstOrDefault(x => x.StudentId == student.Id && x.ClassId == toClass.Id);

            if (destinationClass == null)
            {
                destinationClass = new ClassStudent
                {
                    ClassId = toClass.Id,
                    Active = true,
                    StudentId = student.Id
                };
            }
            else
            {
                destinationClass.Active = true;
            }
            return destinationClass;
        }

        private ClassStudent UpdateOldClass(Student student, Class fromClass)
        {
            var oldClass = classStudents.Select().FirstOrDefault(x => x.StudentId == student.Id && x.ClassId == fromClass.Id);
            if (oldClass == null)
            {
                throw new ArgumentException("No such class, student assignment.");
            }
            oldClass.Active = false;
            return oldClass;
        }

        private static void ValidateTransferData(Student student, Class fromClass, Class toClass)
        {
            if (student == null)
            {
                throw new ArgumentNullException("student");
            }
            if (fromClass == null)
            {
                throw new ArgumentNullException("fromClass");
            }
            if (toClass == null)
            {
                throw new ArgumentNullException("toClass");
            }
        }
    }
}
