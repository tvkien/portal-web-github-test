using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIRefObjectRepository : IRepository<QtiRefObject>, IQTIRefObjectRepository
    {
        private readonly Table<QTIRefObjectView> view;
        private readonly Table<QTIRefObjectEntity> table;
        private readonly AssessmentDataContext _assessmentDataContext;

        public QTIRefObjectRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            view = dataContext.GetTable<QTIRefObjectView>();
            table = dataContext.GetTable<QTIRefObjectEntity>();

            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
        }
        public bool GetHasRightToEditPassage(int userId, int districtId, int qtiRefObjectId)
        {
            var data = _assessmentDataContext.GetHasRightToEditPassage(userId, districtId, qtiRefObjectId).ToList();
            if (data != null && data.Count > 0)
            {
                return data[0].Value == 1 ? true : false;
            }
            return false;
        }

        public IQueryable<QtiRefObject> GetQtiRefObject(GetQtiRefObjectFilter filter)
        {
            return _assessmentDataContext
                .GetQTIRefObject(filter.UserId, filter.GradeId, filter.Subject, filter.TextTypeId, filter.TextSubTypeId, filter.FleschKincaidId, filter.Name, filter.PassageNumber,
                filter.DistrictId, filter.QTIRefObjectIDs, filter.ExcludeQTIRefObjectIDs, filter.StartRow, filter.PageSize, filter.GeneralSearch, filter.SortColumn, filter.SortDirection)
                .Select(x => new QtiRefObject
                {
                    QTIRefObjectID = x.QTIRefObjectID,
                    Name = x.Name,
                    OldMasterCode = x.OldMasterCode,
                    TypeID = x.TypeID,
                    UserID = x.UserID,
                    QTIRefObjectFileRef = x.QTIRefObjectFileRef,
                    Subject = x.Subject,
                    GradeID = x.GradeID,
                    TextTypeID = x.TextTypeID,
                    TextSubTypeID = x.TextSubTypeID,
                    FleschKincaidID = x.FleschKincaidID,
                    DistrictId = x.DistrictID ?? 0,
                    StateId = x.StateID ?? 0,
                    TextSubType = x.TextSubType,
                    TextType = x.TextType,
                    GradeName = x.GradeName,
                    FleschKinkaidName = x.FleschKinkaidName,
                    GradeOrder = x.GradeOrder,
                    TotalCount = x.TotalCount,
                }).AsQueryable();
        }

        public IQueryable<QtiRefObject> Select()
        {
            return view.Select(x => new QtiRefObject
            {
                QTIRefObjectID = x.QTIRefObjectID,
                Name = x.Name,
                OldMasterCode = x.OldMasterCode,
                TypeID = x.TypeID,
                UserID = x.UserID,
                QTIRefObjectFileRef = x.QTIRefObjectFileRef,
                Subject = x.Subject,
                GradeID = x.GradeID,
                TextTypeID = x.TextTypeID,
                TextSubTypeID = x.TextSubTypeID,
                FleschKincaidID = x.FleschKincaidID,
                DistrictId = x.DistrictID,
                StateId = x.StateID,
                CreatedDate = x.CreatedDate,
            });
        }

        public void Save(QtiRefObject item)
        {
            var entity = table.FirstOrDefault(x => x.QTIRefObjectID == item.QTIRefObjectID);

            if (entity == null)
            {
                entity = new QTIRefObjectEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();

            if (entity.QTIRefObjectFileRef == 0)
            {
                entity.QTIRefObjectFileRef = entity.QTIRefObjectID;
                table.Context.SubmitChanges();
            }

            item.QTIRefObjectID = entity.QTIRefObjectID;
        }

        public void Delete(QtiRefObject item)
        {
            var entity = table.FirstOrDefault(x => x.QTIRefObjectID.Equals(item.QTIRefObjectID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(QTIRefObjectEntity entity, QtiRefObject item)
        {
            entity.FleschKincaidID = item.FleschKincaidID;
            entity.GradeID = item.GradeID;
            entity.Name = item.Name;
            entity.OldMasterCode = item.OldMasterCode;
            entity.QTIRefObjectFileRef = item.QTIRefObjectFileRef;
            entity.QTIRefObjectID = item.QTIRefObjectID;
            entity.Subject = item.Subject;
            entity.TextSubTypeID = item.TextSubTypeID;
            entity.TextTypeID = item.TextTypeID;
            entity.CreatedDate = item.CreatedDate;
            entity.UpdatedByUserID = item.UpdatedByUserID;
            entity.UpdatedDate = item.UpdatedDate;
            entity.RevertedFromQTIRefObjectHistoryID = item.RevertedFromQTIRefObjectHistoryID;

            if (entity.QTIRefObjectID == 0)
            {
                entity.TypeID = item.TypeID;
                entity.UserID = item.UserID;
            }
        }

        public IQueryable<QtiRefObject> GetAllQtiRefObjects()
        {
            return table.Select(x => new QtiRefObject
            {
                QTIRefObjectID = x.QTIRefObjectID,
                Name = x.Name,
                OldMasterCode = x.OldMasterCode,
                TypeID = x.TypeID,
                UserID = x.UserID,
                QTIRefObjectFileRef = x.QTIRefObjectFileRef,
                Subject = x.Subject,
                GradeID = x.GradeID,
                TextTypeID = x.TextTypeID,
                TextSubTypeID = x.TextSubTypeID,
                FleschKincaidID = x.FleschKincaidID,
                UpdatedByUserID = x.UpdatedByUserID,
                RevertedFromQTIRefObjectHistoryID = x.RevertedFromQTIRefObjectHistoryID,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
            });
        }

        public IEnumerable<QtiRefObjectVirtualTestDto> GetQtiRefObjectVirtualTests(int qtiRefObjectId)
        {
            return _assessmentDataContext.GetVirtualTestByQtiItemPassage(qtiRefObjectId)
                            .Select(o => new QtiRefObjectVirtualTestDto
                            {
                                VirtualTestId = o.VirtualTestID.GetValueOrDefault(),
                                QTIItemId = o.QTIItemID.GetValueOrDefault(),
                                QtiRefObjectId = o.QtiRefObjectId.GetValueOrDefault()
                            });
        }
    }
}
