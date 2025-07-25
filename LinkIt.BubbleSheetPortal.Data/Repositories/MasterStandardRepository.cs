using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class MasterStandardRepository : IMasterStandardRepository
    {
        private readonly Table<MasterStandardEntity> table;
        private readonly AssessmentDataContext dbContext;
        private readonly TestDataContext testContext;
        private readonly Table<Qti3pItemStandardXmlEntity> tableQti3pItemStandardXml;

        public MasterStandardRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<MasterStandardEntity>();
            dbContext = AssessmentDataContext.Get(connectionString);
            testContext = TestDataContext.Get(connectionString);
            tableQti3pItemStandardXml = testContext.GetTable<Qti3pItemStandardXmlEntity>();
            Mapper.CreateMap<MasterStandard, MasterStandardEntity>();
        }
        public IQueryable<MasterStandard> Select()
        {
            return table.Select(x => new MasterStandard
                                     {
                                         Archived = x.Archived,
                                         StateId = x.StateID,
                                         Level = x.Level,
                                         Grade = x.Grade,
                                         Children = x.Children,
                                         Description = x.Description,
                                         Document = x.Document,
                                         GUID = x.GUID,
                                         HiGrade = x.HiGrade,
                                         HighGradeID = x.HighGradeID,
                                         Label = x.Label,
                                         LoGrade = x.LoGrade,
                                         LowGradeID = x.LowGradeID,
                                         MasterStandardID = x.MasterStandardID,
                                         Number = x.Number,
                                         ParentGUID = x.ParentGUID,
                                         State = x.State,
                                         Subject = x.Subject,
                                         Title = x.Title,
                                         Year = x.Year
                                     });
        }

        public IList<StateSubjectGrade> GetStateSubjectGradeByStateAndSubject(string state, string subject)
        {
            var query = testContext.GetStateStandardGradesByStateAndSubject(state, subject);

            var data = query.Select(x => new StateSubjectGrade
                                         {
                                             StateCode = x.State,
                                             SubjectName = x.Subject,
                                             GradeName = x.Grade,
                                             GradeOrder = x.GradeOrder.GetValueOrDefault(),
                                             GradeID = x.GradeID.GetValueOrDefault()
                                         }).ToList();

            return data;
        }

        public IList<StateStandardSubject> GetStateStandardsByStateCode(string stateCode)
        {
            var query = dbContext.GetStateStandardSubjectsByStateAndArchivedStatus(stateCode, false);

            var data = query.Select(x => new StateStandardSubject
                                         {
                                             StateId = x.StateID.GetValueOrDefault(),
                                             StateCode = x.State,
                                             Year = x.Year.ToString(),
                                             SubjectName = x.Subject
                                         }).ToList();

            return data;
        }

        public IList<MasterStandard> GetMasterStandardsByStateCodeAndSubjectAndGradesTopLevelCC(string state, string subject, string grade)
        {
            var query = dbContext.GetStateStandardsByStateAndSubjectAndGradesTopLevelCC(state, subject, grade);

            var data = query.Select(x => new MasterStandard
                                         {
                                             MasterStandardID = x.MasterStandardID.GetValueOrDefault(),
                                             State = x.State,
                                             Document = x.Document,
                                             Subject = x.Subject,
                                             Year = x.Year.GetValueOrDefault(),
                                             Grade = x.Grade,
                                             Level = x.Level.GetValueOrDefault(),
                                             Children = x.Children.GetValueOrDefault(),
                                             Label = x.Label,
                                             Number = x.Number,
                                             Title = x.Title,
                                             Description = x.Description,
                                             GUID = x.GUID,
                                             ParentGUID = x.ParentGUID,
                                             LoGrade = x.LoGrade,
                                             HiGrade = x.HiGrade
                                         }).ToList();
            return data;
        }

        public IList<MasterStandard> GetMasterStandardsByStateCodeAndSubjectAndGradesTopLevel(string state, string subject, string grade)
        {
            var query = dbContext.GetStateStandardsByStateAndSubjectAndGradesTopLevel(state, subject, grade);

            var data = query.Select(x => new MasterStandard
                                         {
                                             MasterStandardID = x.MasterStandardID.GetValueOrDefault(),
                                             State = x.State,
                                             Document = x.Document,
                                             Subject = x.Subject,
                                             Year = x.Year.GetValueOrDefault(),
                                             Grade = x.Grade,
                                             Level = x.Level.GetValueOrDefault(),
                                             Children = x.Children.GetValueOrDefault(),
                                             Label = x.Label,
                                             Number = x.Number,
                                             Title = x.Title,
                                             Description = x.Description,
                                             GUID = x.GUID,
                                             ParentGUID = x.ParentGUID,
                                             LoGrade = x.LoGrade,
                                             HiGrade = x.HiGrade
                                         }).ToList();
            return data;
        }

        public IList<MasterStandard> GetETSStateStandardsByParentGUID(string parentGUID)
        {
            var query = dbContext.GetStateStandardByGUID(parentGUID);

            var data = query.Select(x => new MasterStandard
                                         {
                                             MasterStandardID = x.MasterStandardID,
                                             State = x.State,
                                             Document = x.Document,
                                             Subject = x.Subject,
                                             Year = x.Year,
                                             Grade = x.Grade,
                                             Level = x.Level,
                                             Children = x.Children,
                                             Label = x.Label,
                                             Number = x.Number,
                                             Title = x.Title,
                                             Description = x.Description,
                                             GUID = x.GUID,
                                             ParentGUID = x.ParentGUID,
                                             LoGrade = x.LoGrade,
                                             HiGrade = x.HiGrade
                                         }).ToList();

            return data;
        }

        public IList<MasterStandard> GetStateStandardsForItemLibraryFilterTopLevel(string state, string subject, string grade,int? userId, int? districtId)
        {
            var query = dbContext.GetStateStandardsForItemLibraryFilterTopLevel(state, subject, grade,userId,districtId);

            var data = query.Select(x => new MasterStandard
            {
                MasterStandardID = x.MasterStandardID,
                Level = x.Level,
                Children = x.Children,
                Number = x.Number,
                Description = x.Description,
                GUID = x.GUID,
                ParentGUID = x.ParentGUID,
                DescendantItemCount = x.DescendantItemCount??0
            }).ToList();
            return data;
        }
        public IList<MasterStandard> GetStateStandardsForItemLibraryFilterTopLevelCC(string state, string subject, string grade, int? userId, int? districtId)
        {
            var query = dbContext.GetStateStandardsForItemLibraryFilterTopLevelCC(state, subject, grade,userId,districtId);

            var data = query.Select(x => new MasterStandard
            {
                MasterStandardID = x.MasterStandardID,
                Level = x.Level,
                Children = x.Children,
                Number = x.Number,
                Description = x.Description,
                GUID = x.GUID,
                ParentGUID = x.ParentGUID,
                DescendantItemCount = x.DescendantItemCount ?? 0
            }).ToList();
            return data;
        }
        public IList<MasterStandard> GetStateStandardsForItem3pLibraryFilterTopLevel(string state, string subject, string grade, int? qti3pSourceId)
        {
            var query = dbContext.GetStateStandardsForItem3pLibraryFilterTopLevel(state, subject, grade, qti3pSourceId);

            var data = query.Select(x => new MasterStandard
            {
                MasterStandardID = x.MasterStandardID,
                Level = x.Level,
                Children = x.Children,
                Number = x.Number,
                Description = x.Description,
                GUID = x.GUID,
                ParentGUID = x.ParentGUID,
                DescendantItemCount = x.DescendantItemCount ?? 0
            }).ToList();
            return data;
        }
        public IList<MasterStandard> GetStateStandardsForItem3pLibraryFilterTopLevelCC(string state, string subject, string grade, int? qti3pSourceId)
        {
            var query = dbContext.GetStateStandardsForItem3pLibraryFilterTopLevelCC(state, subject, grade, qti3pSourceId);

            var data = query.Select(x => new MasterStandard
            {
                MasterStandardID = x.MasterStandardID,
                Level = x.Level,
                Children = x.Children,
                Number = x.Number,
                Description = x.Description,
                GUID = x.GUID,
                ParentGUID = x.ParentGUID,
                DescendantItemCount = x.DescendantItemCount ?? 0
            }).ToList();
            return data;
        }
        public IList<MasterStandard> GetStateStandardsNextLevelForItemLibraryFilter(string guid, string state, string subject, string grade, int? userId, int? districtId)
        {
            var query = dbContext.GetStateStandardsNextLevelForItemLibraryFilter(guid, state, subject, grade,userId,districtId);

            var data = query.Select(x => new MasterStandard
            {
                MasterStandardID = x.MasterStandardID,
                Level = x.Level,
                Children = x.Children,
                Number = x.Number,
                Description = x.Description,
                GUID = x.GUID,
                ParentGUID = x.ParentGUID,
                DescendantItemCount = x.DescendantItemCount ?? 0
            }).ToList();
            return data;
        }
        public IList<MasterStandard> GetStateStandardsNextLevelForItem3pLibraryFilter(string guid,string state, string subject, string grade, int? qti3pSourceId)
        {
            var query = dbContext.GetStateStandardsNextLevelForItem3pLibraryFilter(guid, state, subject, grade, qti3pSourceId);

            var data = query.Select(x => new MasterStandard
            {
                MasterStandardID = x.MasterStandardID,
                Level = x.Level,
                Children = x.Children,
                Number = x.Number,
                Description = x.Description,
                GUID = x.GUID,
                ParentGUID = x.ParentGUID,
                DescendantItemCount = x.DescendantItemCount ?? 0
            }).ToList();
            return data;
        }
        public IQueryable<Qti3pItemStandardXml> GetQti3pItemStandardXml()
        {
             return tableQti3pItemStandardXml.Select(x => new Qti3pItemStandardXml
                                     {
                                         Qti3pItemId = x.Qti3pItemID,
                                         MasterStandardXml = x.MasterStandardXML
                                     });
        }


    }
}