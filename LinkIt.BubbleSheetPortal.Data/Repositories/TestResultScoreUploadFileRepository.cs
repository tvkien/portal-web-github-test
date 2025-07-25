using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestResultScoreUploadFileRepository : IReadOnlyRepository<TestResultScoreUploadFile>
    {
        private readonly Table<TestResultScoreUploadFileEntity> table;
        private readonly DataLockerContextDataContext dbContext;

        public TestResultScoreUploadFileRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DataLockerContextDataContext.Get(connectionString).GetTable<TestResultScoreUploadFileEntity>();
        }

        public IQueryable<TestResultScoreUploadFile> Select()
        {
            return table.Select(x => new TestResultScoreUploadFile
            {
                TestResultScoreID = x.TestResultScoreID,
                TestResultSubScoreID = x.TestResultSubScoreID,
                FileName = x.FileName,
                IsUrl = x.IsUrl ?? false,
                UploadDate = x.UploadedDate ?? x.UpdatedDate.Value,
                Tag = x.Tag,
                CreatedBy = x.CreatedBy,
                DocumentGuid = x.DocumentGUID,
                TestResultScoreUploadFileID = x.TestResultScoreUploadFileID
            });
        }
    }
}
