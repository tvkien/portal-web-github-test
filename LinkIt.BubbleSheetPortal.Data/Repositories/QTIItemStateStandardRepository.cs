using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemStateStandardRepository : IQTIItemStateStandardRepository
    {
        private readonly Table<QTIItemStateStandardEntity> table;
        private readonly AssessmentDataContext _assessmentDataContext;
        private readonly IConnectionString _connectionString;

        public QTIItemStateStandardRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
            table = _assessmentDataContext.GetTable<QTIItemStateStandardEntity>();
            Mapper.CreateMap<QTIItemStateStandard, QTIItemStateStandardEntity>();
            _connectionString = conn;
        }

        public IQueryable<QTIItemStateStandard> Select()
        {
            return table.Select(x => new QTIItemStateStandard
                                     {
                                         QTIItemID = x.QTIItemID,
                                         QTIItemStateStandardID = x.QTIItemStateStandardID,
                                         StateStandardID = x.StateStandardID
                                     });
        }

        public void Save(QTIItemStateStandard item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemID.Equals(item.QTIItemID) && x.StateStandardID.Equals(item.StateStandardID));

            if (entity.IsNull())
            {
                entity = new QTIItemStateStandardEntity();
                table.InsertOnSubmit(entity);

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.QTIItemStateStandardID = entity.QTIItemStateStandardID;
            }
        }

        public void Delete(QTIItemStateStandard item)
        {
            var entity =
                table.FirstOrDefault(x => x.QTIItemID == item.QTIItemID && x.StateStandardID == item.StateStandardID);
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
        public List<StateStandardSubject> GetStandardSubjects(string stateCode)
        {
            return _assessmentDataContext.GetStateStandardSubjectsByStateAndArchivedStatus(stateCode,false).Select(
                x => new StateStandardSubject
                         {
                             StateId = x.StateID??0,
                             SubjectName = x.Subject,
                             Year = x.Year.ToString(),
                             StateCode = x.State
                         }).ToList();
        }
        public List<StateSubjectGrade> GetGradeByStateCodeAndSubject(string stateCode, string subject)
        {
            return
                _assessmentDataContext.QTIGetStateStandardGradesByStateAndSubject(stateCode, subject).Select(
                    x => new StateSubjectGrade
                             {
                                 StateCode = x.State,
                                 GradeID = x.GradeID??0,
                                 GradeName = x.Grade,
                                 GradeOrder = x.GradeOrder??0,
                                 SubjectName = x.Subject
                             }).ToList();
        }
        public List<State>  GetStatesQTIItem(int? userId, int? districtId, int? userIdStateForUser)
        {
            return
               _assessmentDataContext.GetStatesQTIItem(userId,districtId,userIdStateForUser).Select(
                   x => new State
                   {
                       Id = x.StateID,
                       Code = x.Code,
                       Name = x.Name,
                       
                   }).ToList();
        }
        
        public List<StateStandardSubject> GetStateStandardSubjectsForItemLibraryFilter(string stateCode,int? userId,int? districtId)
        {
            return _assessmentDataContext.GetStateStandardSubjectsForItemLibraryFilter(stateCode,userId,districtId).Select(
                x => new StateStandardSubject
                {
                    StateId = x.StateID ?? 0,
                    SubjectName = x.Subject,
                    Year = x.Year.ToString(),
                    StateCode = x.State
                }).ToList();
        }
        public List<StateSubjectGrade> GetGradesByStateAndSubjectForItemLibraryFilter(string stateCode, string subject, int? userId, int? districtId)
        {
            return
               _assessmentDataContext.GetGradesByStateAndSubjectForItemLibraryFilter(stateCode, subject,userId,districtId).Select(
                   x => new StateSubjectGrade
                   {
                       StateCode = x.State,
                       GradeID = x.GradeID ?? 0,
                       GradeName = x.Grade,
                       GradeOrder = x.GradeOrder ?? 0,
                       SubjectName = x.Subject
                   }).ToList();
        }
        public List<State> GetStatesQTI3pItem(int? qti3pSourceId, int? userId)
        {
            return _assessmentDataContext.GetStatesQTI3pItem(qti3pSourceId, userId)
                    .Select(
                    x => new State
                    {
                        Id = x.StateID,
                        Code = x.Code,
                        Name = x.Name,

                    }).ToList();
        }

        public void InsertMultipleRecord(List<QTIItemStateStandard> items)
        {
            var bulkHelper = new BulkHelper(_connectionString);
            bulkHelper.BulkCopy(items, "dbo.QTIItemStateStandard");
        }
    }
}
