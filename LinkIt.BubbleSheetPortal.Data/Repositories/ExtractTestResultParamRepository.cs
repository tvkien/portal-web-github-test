using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ExtractTestResultParamRepository :  IRepository<ExtractTestResultParam>
    {
        private readonly Table<ExtractTestResultParamEntity> table;
        public ExtractTestResultParamRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table= ExtractTestDataContext.Get(connectionString).GetTable<ExtractTestResultParamEntity>();;
            Mapper.CreateMap<ExtractTestResultParam, ExtractTestResultParamEntity>();
        }

        public IQueryable<ExtractTestResultParam> Select()
        {
            return table.Select(x => new ExtractTestResultParam
            {
                ExtractTestResultParamID = x.ExtractTestResultParamID,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                GradeID = x.GradeID,
                SubjectID = x.SubjectID,
                BankID = x.BankID,
                SchoolID = x.SchoolID,
                TeacherID = x.TeacherID,
                ClassID = x.ClassID,
                StudentID = x.StudentID,
                ListTestIDs = x.ListTestIDs,
                ListIdsUncheck = x.ListIdsUncheck,
                UserID = x.UserID,
                UserRoleID = x.UserRoleID,
                SubjectName = x.SubjectName,
                GeneralSearch = x.GeneralSearch
            });
        }

        public void Save(ExtractTestResultParam item)
        {
            var entity = table.FirstOrDefault(x => x.ExtractTestResultParamID.Equals(item.ExtractTestResultParamID));

            if (entity.IsNull())
            {
                entity = new ExtractTestResultParamEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.ExtractTestResultParamID = entity.ExtractTestResultParamID;
        }

        public void Delete(ExtractTestResultParam item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.ExtractTestResultParamID.Equals(item.ExtractTestResultParamID));
                if (entity != null)
                {
                    table.Context.SubmitChanges();
                }
            }
        }        
    }
}
