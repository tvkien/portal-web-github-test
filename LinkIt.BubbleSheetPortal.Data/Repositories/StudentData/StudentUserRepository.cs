using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserDefinedTypesDto;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.StudentData
{
    public class StudentUserRepository : IStudentUserRepository
    {
        private readonly Table<StudentUserEntity> table;
        private readonly StudentDataContext _context;

        public StudentUserRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = StudentDataContext.Get(connectionString);
            table = _context.GetTable<StudentUserEntity>();
            Mapper.CreateMap<StudentUser, StudentUserEntity>();
        }

        public IQueryable<StudentUser> Select()
        {
            return table.Select(x => new StudentUser
            {
                StudentId = x.StudentId,
                UserId = x.UserId
            });
        }
        public List<CalculatedMetaData> GetCalculatedMetaData(DataTable calculatedTable, DataTable studentTable)
        {
            var parameters = new List<(string, string, SqlDbType, object, ParameterDirection)>();

            parameters.Add(("CalcConfigration", "StringValueList", SqlDbType.Structured, calculatedTable, ParameterDirection.Input));
            parameters.Add(("studentIds", "IntegerList", SqlDbType.Structured, studentTable, ParameterDirection.Input));
            var res = _context.Query<CalculatedMetaData>(new SqlParameterRequest()
            {
                StoredName = "NS_StudentMetaCalculated",
                Parameters = parameters

            }, out _);
            return res;

        }
        public void Save(StudentUser item)
        {
            var entity = table.FirstOrDefault(x => x.StudentId == item.StudentId && x.UserId == item.UserId);

            if (entity.IsNull())
            {
                entity = new StudentUserEntity()
                {
                    StudentId = item.StudentId,
                    UserId = item.UserId
                };
                table.InsertOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void Delete(StudentUser item)
        {
            var entity = table.FirstOrDefault(x => x.StudentId == item.StudentId && x.UserId == item.UserId);
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public int GetUserIDViaStudentUser(int studentId)
        {
            var studentUser = _context.GetStudentUserForStudentLogon(0, studentId).FirstOrDefault();
            if (studentUser != null)
                return studentUser.UserId;
            return 0;
        }

        public int GetStudentIDViaStudentUser(int userId)
        {
            var studentUser = _context.GetStudentUserForStudentLogon(userId, 0).FirstOrDefault();
            if (studentUser != null)
                return studentUser.StudentId;
            return 0;
        }
    }
}
