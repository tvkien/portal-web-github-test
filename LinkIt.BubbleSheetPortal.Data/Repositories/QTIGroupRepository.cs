using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIGroupRepository : IQTIGroupRepository
    {
        private readonly AssessmentDataContext _assessmentDataContext;
        private readonly Table<QTIGroupEntity> _tableQtiGroupEntities;

        public QTIGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
            _tableQtiGroupEntities = _assessmentDataContext.GetTable<QTIGroupEntity>();
        }

        public IQueryable<QtiGroup> Select()
        {
            return _tableQtiGroupEntities.Select(
                x => new QtiGroup
                {
                    AccessId = x.AccessID,
                    AuthorGroupId = x.AuthorGroupID,
                    GroupId = x.GroupID,
                    Name = x.Name,
                    OldMasterCode = x.OldMasterCode,
                    OwnershipType = x.OwnershipType,
                    QtiBankId = x.QTIBankID,
                    QtiGroupId = x.QTIGroupID,
                    Source = x.Source,
                    //SourceId = x.SourceID,
                    Type = x.Type,
                    UserId = x.UserID,
                    VirtualTestId = x.VirtualTestID,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate
                });
        }

        public void Save(QtiGroup item)
        {
            var entity = _tableQtiGroupEntities.FirstOrDefault(x => x.QTIGroupID.Equals(item.QtiGroupId));

            if (entity.IsNull())
            {
                entity = new QTIGroupEntity();
                _tableQtiGroupEntities.InsertOnSubmit(entity);
            }

            MappingQTIGroup(item, entity);
            if (string.IsNullOrEmpty(entity.Source) || entity.Source.Equals("itemset_temp"))
            {
                entity.Source = string.Format("itemset_{0}", entity.QTIGroupID);
            }
            _tableQtiGroupEntities.Context.SubmitChanges();
            item.QtiGroupId = entity.QTIGroupID;
        }

        private void MappingQTIGroup(QtiGroup item, QTIGroupEntity entity)
        {
            entity.AccessID = item.AccessId;
            entity.AuthorGroupID = item.AuthorGroupId;
            entity.GroupID = item.GroupId;
            entity.Name = item.Name;
            entity.OldMasterCode = item.OldMasterCode;
            entity.OwnershipType = item.OwnershipType;
            entity.QTIBankID = item.QtiBankId;
            entity.QTIGroupID = item.QtiGroupId;
            entity.Source = item.Source;
            //item.SourceId = entity.SourceID;
            entity.Type = item.Type;
            entity.UserID = item.UserId;
            entity.VirtualTestID = item.VirtualTestId;
            entity.CreatedDate = item.CreatedDate;
            entity.ModifiedDate = item.ModifiedDate;
        }

        public void Delete(QtiGroup item)
        {
            var entity = _tableQtiGroupEntities.FirstOrDefault(x => x.QTIBankID.Equals(item.QtiBankId));
            if (entity != null)
            {
                _tableQtiGroupEntities.DeleteOnSubmit(entity);
                _tableQtiGroupEntities.Context.SubmitChanges();
            }
        }

        public List<QtiGroup> GetlistQtiGroups(int qtiBankId, int userId, int roleId, int districtId)
        {
            return _assessmentDataContext.ListQTIGroup(userId, roleId, qtiBankId, districtId)
                .Select(o => new QtiGroup()
                {
                    QtiGroupId = o.QTIGroupID,
                    Name = o.Name,
                    Source = o.Source,
                    Type = o.Type,
                    UserId = o.UserID,
                    GroupId = o.GroupID,
                    VirtualTestId = o.VirtualTestID,
                    QtiBankId = o.QTIBankID,
                    OldMasterCode = o.OldMasterCode,
                    AccessId = o.AccessID,
                    OwnershipType = o.OwnershipType,
                    AuthorGroupId = o.AuthorGroupID,
                    AuthorGroupName = o.AuthorGroupName??"",
                    AuthorFirstName = o.AuthorFirstName??"",
                    AuthorLastName = o.AuthorLastName??""
                }).ToList();
        }

        public string DeleteItemSetAndItems(int qtiItemSetId,int userId)
        {
            if (qtiItemSetId > 0)
            {
                var obj =_assessmentDataContext.DeleteQTIGroupAndSubItems(qtiItemSetId, userId).FirstOrDefault();
                if (obj != null)
                    return obj.returnMessage;
            }
            return string.Empty;
        }

        public void ReassignQuestionOrder(int qtiGroupId)
        {
            _assessmentDataContext.ReassignQuestionOrder(qtiGroupId);
        }


        public List<QtiGroup> GetOwnerListQtiGroupByQtiBankId(int qtiBankId)
        {
            return _tableQtiGroupEntities.Where(x => x.QTIBankID == qtiBankId).Select(x => new QtiGroup() {
                AccessId = x.AccessID,
                AuthorGroupId = x.AuthorGroupID,
                GroupId = x.GroupID,
                Name = x.Name,
                OldMasterCode = x.OldMasterCode,
                OwnershipType = x.OwnershipType,
                QtiBankId = x.QTIBankID,
                QtiGroupId = x.QTIGroupID,
                Source = x.Source,
                //SourceId = x.SourceID,
                Type = x.Type,
                UserId = x.UserID,
                VirtualTestId = x.VirtualTestID,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate
            }).ToList();
        }
    }
}
