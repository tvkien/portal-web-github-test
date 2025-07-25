using System;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AblesReportJobRepository : IRepository<AblesReportJob>
    {
        private readonly Table<AblesReportJobEntity> table;

        public AblesReportJobRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<AblesReportJobEntity>();
        }

        public IQueryable<AblesReportJob> Select()
        {
            return table.Select(x => new AblesReportJob
            {
                AblesReportJobId = x.AblesReportJobID,
                CreatedDate = x.CreatedDate,
                DistrictId = x.DistrictID,
                DownloadUrl = x.DownloadUrl,
                ReportTypeId = x.ReportTypeID,
                Status = x.Status,
                UserId = x.UserID,
                ErrorMessage = x.ErrorMessage,
                JsonAblesDataPost = x.JsonAblesDataPost,
                LearningArea = x.LearningArea,
                SchoolId = x.SchoolID.Value,
                ClassId = x.ClassID.Value,
                DistrictTermId = x.DistrictTermID.Value,
                TeacherId = x.TeacherID.Value
            });
        }

        public void Save(AblesReportJob item)
        {
            var entity = table.FirstOrDefault(x => x.AblesReportJobID.Equals(item.AblesReportJobId));

            if (entity == null)
            {
                entity = new AblesReportJobEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.AblesReportJobId = entity.AblesReportJobID;
        }

        public void Delete(AblesReportJob item)
        {
            var entity = table.FirstOrDefault(x => x.AblesReportJobID.Equals(item.AblesReportJobId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(AblesReportJob item, AblesReportJobEntity entity)
        {
            entity.CreatedDate = item.CreatedDate;
            entity.DistrictID = item.DistrictId;
            entity.DownloadUrl = item.DownloadUrl;
            entity.ReportTypeID = item.ReportTypeId;
            entity.Status = item.Status;
            entity.UserID = item.UserId;
            entity.ErrorMessage = item.ErrorMessage;
            entity.JsonAblesDataPost = item.JsonAblesDataPost;
            entity.LearningArea = item.LearningArea;
            entity.SchoolID = item.SchoolId;
            entity.TeacherID = item.TeacherId;
            entity.ClassID = item.ClassId;
            entity.DistrictTermID = item.DistrictTermId;
        }
    }
}
