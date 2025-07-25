using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using Envoc.Core.Shared.Extensions;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentRepository : IRepository<Student>
    {
        private readonly Table<StudentEntity> table;

        public StudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<StudentEntity>();
            Mapper.CreateMap<Student, StudentEntity>();
        }

        public IQueryable<Student> Select()
        {
            return table.Select(x => new Student
            {
                Id = x.StudentID,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                GenderId = x.GenderID,
                Code = x.Code,
                RaceId = x.RaceID,
                DistrictId = x.DistrictID,
                AltCode = x.AltCode,
                Password = x.Password,
                Email = x.Email,
                Phone = x.Phone,
                LoginCode = x.LoginCode,
                DateOfBirth = x.Dateofbirth,
                PrimaryLanguageId = x.PrimaryLanguageID,
                Status = x.Status,
                SISID = x.SISID,
                StateCode = x.StateCode,
                Note01 = x.Note01,
                CurrentGradeId = x.CurrentGradeID,
                AdminSchoolId = x.AdminSchoolID,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedUser = x.ModifiedUser,
                RegistrationCode = x.RegistrationCode,
                SharedSecret = x.SharedSecret
            });
        }

        public void Save(Student item)
        {
            var entity = table.FirstOrDefault(x => x.StudentID.Equals(item.Id));

            if (entity.IsNull())
            {
                entity = new StudentEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            entity.CurrentGradeID = item.CurrentGradeId;

            table.Context.SubmitChanges();
            item.Id = entity.StudentID;
        }
      
        public void Delete(Student item)
        {
            var entity = table.FirstOrDefault(x => x.StudentID.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
