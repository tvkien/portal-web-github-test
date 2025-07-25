using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTITestClassAssignmentRepository : IRepository<QTITestClassAssignmentData>, IQTITestClassAssignmentRepository
    {
        private readonly Table<QTITestClassAssignmentEntity> table;
        private readonly TestDataContext _testDataContext;

        public QTITestClassAssignmentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<QTITestClassAssignmentEntity>();
            _testDataContext = TestDataContext.Get(connectionString);
            Mapper.CreateMap<QTITestClassAssignmentData, QTITestClassAssignmentEntity>();
        }

        public IQueryable<QTITestClassAssignmentData> Select()
        {
            return table.Select(x => new QTITestClassAssignmentData
            {
                QTITestClassAssignmentId = x.QTITestClassAssignmentID,
                VirtualTestId = x.VirtualTestID,
                ClassId = x.ClassID,
                AssignmentDate = x.AssignmentDate,
                Code = x.Code,
                CodeTimestamp = x.CodeTimestamp,
                AssignmentGuId = x.AssignmentGUID,
                TestSetting = x.TestSetting,
                Status = x.Status,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
                ModifiedUserId = x.ModifiedUserID,
                ComparisonPasscodeLength = x.ComparisonPasscodeLength,
                Type = x.Type,
                TutorialMode = x.Mode ?? 1,
                IsHide = x.IsHide ?? false,
                DistrictID = x.DistrictID ?? 0,
                SurveyAssignmentType = x.SurveyAssignmentType,
                ListOfDisplayQuestions = x.ListOfDisplayQuestions,
                AuthenticationCode = x.AuthenticationCode,
                AuthenticationCodeExpirationDate = x.AuthenticationCodeExpirationDate
            });
        }

        public void Save(QTITestClassAssignmentData item)
        {
            var entity = table.FirstOrDefault(x => x.QTITestClassAssignmentID.Equals(item.QTITestClassAssignmentId));

            if (entity.IsNull())
            {
                entity = new QTITestClassAssignmentEntity();
                table.InsertOnSubmit(entity);
            }

            Map(item, entity);
            table.Context.SubmitChanges();
            item.QTITestClassAssignmentId = entity.QTITestClassAssignmentID;
        }

        public void SaveMutipleRecord(List<QTITestClassAssignmentData> items)
        {
            var listID = items.Select(y => y.QTITestClassAssignmentId);
            var entities = table.Where(x => listID.Contains(x.QTITestClassAssignmentID)).ToList();

            if (entities.IsNotNull())
            {
                MapList(items, entities);
                table.Context.SubmitChanges();
            }
        }

        private void Map(QTITestClassAssignmentData item, QTITestClassAssignmentEntity entity)
        {
            entity.VirtualTestID = item.VirtualTestId;
            entity.ClassID = item.ClassId;
            entity.AssignmentDate = item.AssignmentDate;
            entity.Code = item.Code;
            entity.CodeTimestamp = item.CodeTimestamp;
            entity.AssignmentGUID = item.AssignmentGuId;
            entity.TestSetting = item.TestSetting;
            entity.Status = item.Status;
            entity.ModifiedBy = item.ModifiedBy;
            entity.ModifiedDate = item.ModifiedDate;
            entity.ModifiedUserID = item.ModifiedUserId;
            entity.ComparisonPasscodeLength = item.ComparisonPasscodeLength;
            entity.Type = item.Type;
            entity.Mode = item.TutorialMode;
            entity.IsHide = item.IsHide;
            entity.DistrictID = item.DistrictID;
            entity.SurveyAssignmentType = item.SurveyAssignmentType;
            entity.ListOfDisplayQuestions = item.ListOfDisplayQuestions;
            entity.AuthenticationCode = item.AuthenticationCode;
            entity.AuthenticationCodeExpirationDate = item.AuthenticationCodeExpirationDate;
        }

        private void MapList(List<QTITestClassAssignmentData> items, List<QTITestClassAssignmentEntity> entities)
        {
            foreach (var item in items)
            {
                var entity = entities.Where(x => x.QTITestClassAssignmentID == item.QTITestClassAssignmentId).FirstOrDefault();
                if (entity != null)
                {
                    entity.VirtualTestID = item.VirtualTestId;
                    entity.ClassID = item.ClassId;
                    entity.AssignmentDate = item.AssignmentDate;
                    entity.Code = item.Code;
                    entity.CodeTimestamp = item.CodeTimestamp;
                    entity.AssignmentGUID = item.AssignmentGuId;
                    entity.TestSetting = item.TestSetting;
                    entity.Status = item.Status;
                    entity.ModifiedBy = item.ModifiedBy;
                    entity.ModifiedDate = item.ModifiedDate;
                    entity.ModifiedUserID = item.ModifiedUserId;
                    entity.ComparisonPasscodeLength = item.ComparisonPasscodeLength;
                    entity.Type = item.Type;
                    entity.Mode = item.TutorialMode;
                    entity.IsHide = item.IsHide;
                    entity.DistrictID = item.DistrictID;
                    entity.SurveyAssignmentType = item.SurveyAssignmentType;
                    entity.ListOfDisplayQuestions = item.ListOfDisplayQuestions;
                    entity.AuthenticationCode = item.AuthenticationCode;
                    entity.AuthenticationCodeExpirationDate = item.AuthenticationCodeExpirationDate;
                }

            }
        }

        public void Delete(QTITestClassAssignmentData item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.QTITestClassAssignmentID.Equals(item.QTITestClassAssignmentId));
                if (entity != null)
                {
                    entity.Status = 0;
                    table.Context.SubmitChanges();
                }
            }
        }

        public CheckMatchEmailDto CheckMatchEmail(string email, int districtId, int virtualTestId, int termId, int assignmentType)
        {
            var emailStatus = _testDataContext.CheckMatchEmail(email, districtId, virtualTestId, termId, assignmentType).FirstOrDefault().Column1;
            var matchEmailDto = new CheckMatchEmailDto
            {
                Email = email,
                Status = emailStatus
            };

            return matchEmailDto;
        }

        public bool CanActiveForRetake(int qtiTestClassAssignmentID)
        {
            var res = _testDataContext.CanActiveTestAssignmentForRetake(qtiTestClassAssignmentID).FirstOrDefault();

            return res != null && res.CanActive.GetValueOrDefault();
        }

        public void InsertMultipleRecord(List<QTITestClassAssignmentData> items)
        {
            var entities = MapListInsert(items);

            foreach(var entity in entities)
            {
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
            SetIDGenerated(items, entities);
        }

        private void SetIDGenerated(List<QTITestClassAssignmentData> items, List<QTITestClassAssignmentEntity> entities)
        {
            foreach (var item in items)
            {
                var entity = entities.Where(x => x.Code == item.Code && x.AssignmentGUID == item.AssignmentGuId).FirstOrDefault();

                if (entity != null)
                {
                    item.QTITestClassAssignmentId = entity.QTITestClassAssignmentID;
                }
            }
        }

        private List<QTITestClassAssignmentEntity> MapListInsert(List<QTITestClassAssignmentData> items)
        {
            return items.Select(item => new QTITestClassAssignmentEntity
            {
                VirtualTestID = item.VirtualTestId,
                ClassID = item.ClassId,
                AssignmentDate = item.AssignmentDate,
                Code = item.Code,
                CodeTimestamp = item.CodeTimestamp,
                AssignmentGUID = item.AssignmentGuId,
                TestSetting = item.TestSetting,
                Status = item.Status,
                ModifiedBy = item.ModifiedBy,
                ModifiedDate = item.ModifiedDate,
                ModifiedUserID = item.ModifiedUserId,
                ComparisonPasscodeLength = item.ComparisonPasscodeLength,
                Type = item.Type,
                Mode = item.TutorialMode,
                IsHide = item.IsHide,
                DistrictID = item.DistrictID,
                SurveyAssignmentType = item.SurveyAssignmentType,
                ListOfDisplayQuestions = item.ListOfDisplayQuestions,
                AuthenticationCode = item.AuthenticationCode,
                AuthenticationCodeExpirationDate = item.AuthenticationCodeExpirationDate,
            }).ToList();
        }
    }
}
