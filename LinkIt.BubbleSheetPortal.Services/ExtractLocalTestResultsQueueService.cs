using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ExtractLocalTestResultsQueueService
    {
        private readonly IRepository<ExtractLocalTestResultsQueue> repository;
        private readonly IRepository<ExtractTestResultParam> paramRepository;

        public ExtractLocalTestResultsQueueService(IRepository<ExtractLocalTestResultsQueue> repository, IRepository<ExtractTestResultParam> paramRepository)
        {
            this.repository = repository;
            this.paramRepository = paramRepository;
        }

        public IQueryable<ExtractLocalTestResultsQueue> Select()
        {
            return repository.Select();
        }

        public void Save(ExtractLocalTestResultsQueue extractQueue)
        {
            repository.Save(extractQueue);
        }
        public IQueryable<ExtractTestResultParam> SelectAll()
        {
            return paramRepository.Select();
        }

        public int AddExtractionToQueueTestResult(ExtractTestResultParamCustom param)
        {
            string strListResult = string.Empty;
            int? paramId = null;
            if (param.IsCheckAll)
            {
                DateTime dtStartDate, dtEndDate;
                param.ExtractLocalCustom.StartDate = DateTime.TryParse(param.ExtractLocalCustom.StrStartDate, out dtStartDate) ? dtStartDate : DateTime.UtcNow;
                param.ExtractLocalCustom.EndDate = DateTime.TryParse(param.ExtractLocalCustom.StrEndDate, out dtEndDate) ? dtEndDate : DateTime.UtcNow;
                
                if (!string.IsNullOrEmpty(param.ListIdsUncheck))
                    param.ListIdsUncheck = param.ListIdsUncheck.Replace("_", "");               

                var extractParam = new ExtractTestResultParam()
                {
                    FromDate = param.ExtractLocalCustom.StartDate,
                    ToDate = param.ExtractLocalCustom.EndDate.AddDays(1),
                    GradeID = param.ExtractLocalCustom.GradeId,
                    SubjectID = param.ExtractLocalCustom.SubjectId,
                    BankID = param.ExtractLocalCustom.BankdId,
                    SchoolID = param.ExtractLocalCustom.SchoolId,
                    TeacherID = param.ExtractLocalCustom.TeacherId,
                    ClassID = param.ExtractLocalCustom.ClassId,
                    StudentID = param.ExtractLocalCustom.StudentId,
                    ListTestIDs = param.ExtractLocalCustom.ListTestIDs ?? string.Empty,
                    ListIdsUncheck = param.ListIdsUncheck ?? string.Empty,
                    UserID = param.ExtractLocalCustom.UserId,
                    UserRoleID = param.ExtractLocalCustom.UserRoleId,
                    SubjectName = param.ExtractLocalCustom.SubjectName,
                    GeneralSearch = param.ExtractLocalCustom.GeneralSearch,
                };
                paramRepository.Save(extractParam);
                if (extractParam.ExtractTestResultParamID > 0)
                {
                    paramId = extractParam.ExtractTestResultParamID;
                }
            }
            else
            {
                strListResult = param.ListId.Replace("_", "");
            }


            var queueEntity = new ExtractLocalTestResultsQueue()
            {
                DistrictId = param.ExtractLocalCustom.DistrictId,
                UserId = param.ExtractLocalCustom.UserId,                
                UserTimeZoneOffset = param.TimeZoneOffset,
                ExportTemplates = param.ListTemplates,
                ListIDsInput = strListResult,
                ExtractType = (int)ExtractTypeEnum.TestResults,
                Status = (int)ExtractLocalTestStatusEnum.NotProcess,
                CreatedDate = DateTime.UtcNow,
                BaseHostURL = param.BaseHostUrl,
                ExtractTestResultParamID = paramId
            };
            repository.Save(queueEntity);
            return queueEntity.ExtractLocalTestResultsQueueId;
        }
    }
}
