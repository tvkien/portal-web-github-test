using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Repositories.StudentData;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using System.Collections.Generic;
using System.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserDefinedTypesDto;
using LinkIt.BubbleSheetPortal.Data.Extensions;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentMetaService
    {
        private readonly IRepository<StudentMeta> repository;
        private readonly IStudentUserRepository _studentUserRepository;

        public StudentMetaService(IRepository<StudentMeta> repository, IStudentUserRepository studentUserRepository)
        {
            this.repository = repository;
            this._studentUserRepository = studentUserRepository;
        }

        public IQueryable<StudentMeta> GetStudentsMetaByStudentId(int studentId)
        {
            return repository.Select().Where(x => x.StudentID.Equals(studentId));
        }

        public List<CalculatedMetaData> GetCalculatedMetaData(IEnumerable<StringValueListDto> calculatedConfigs, int [] studentIds)
        {
            var studentDataTable = studentIds.Select(id => new IntegerListDto { Id = id }).ToDataTable();

            return _studentUserRepository.GetCalculatedMetaData(calculatedConfigs.ToDataTable(), studentDataTable);
        }

        public StudentMeta GetStudentsMetaByStudentIdAndName(int studentId, string name)
        {
            var sm = repository.Select().Where(x => x.StudentID.Equals(studentId) && x.Name.Equals(name));
            if (sm.Any())
                return sm.FirstOrDefault();
            return null;
        }

        public StudentMeta GetStudentMetaById(int studentMetaId)
        {
            return repository.Select().FirstOrDefault(x => x.StudentMetaID.Equals(studentMetaId));
        }

        public void Save(StudentMeta student)
        {
            repository.Save(student);
        }

        public int GetUserIdByStudentId(int studentId)
        {            
            return _studentUserRepository.GetUserIDViaStudentUser(studentId);
        }

        public void AddOrUpdateStudentMeta(int studentId, string metaName, string metaValue)
        {
            var studentMetaData = repository.Select().FirstOrDefault(x => x.StudentID == studentId && x.Name.ToLower() == metaName.ToLower());
            if (studentMetaData != null)
            {
                if (string.IsNullOrWhiteSpace(metaValue))
                {
                    repository.Delete(studentMetaData);
                    return;
                }
                studentMetaData.Data = metaValue;
                repository.Save(studentMetaData);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(metaValue))
                {
                    studentMetaData = new StudentMeta()
                    {
                        StudentID = studentId,
                        Name = metaName,
                        Data = metaValue
                    };
                    repository.Save(studentMetaData);
                }
            }
        }

        public int GetUserIdViaStudentUser(int studentId)
        {
            return _studentUserRepository.GetUserIDViaStudentUser(studentId);
        }
        public int GetStudentIdViaStudentUser(int userId)
        {
            return _studentUserRepository.GetStudentIDViaStudentUser(userId);
        }
    }
}
