using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QtiBankRepository : IQtiBankRepository, IRepository<QtiBank>
    {
        private readonly TestDataContext _testDataContext;

        private readonly Table<QTIBankPublishedDistrictView> _tableQtiBankPublishedDistrictView;
        private readonly Table<QTIBankPublishedSchoolView> _tableQtiBankPublishedSchoolView;
        private readonly Table<QTIBankEntity> _tableQtiBankEntity;
        private readonly Table<QtiBankPersonalView> _qtiBankPersonalView;

        public QtiBankRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);

            _tableQtiBankPublishedDistrictView = _testDataContext.GetTable<QTIBankPublishedDistrictView>();
            _tableQtiBankPublishedSchoolView = _testDataContext.GetTable<QTIBankPublishedSchoolView>();
            _tableQtiBankEntity = _testDataContext.GetTable<QTIBankEntity>();
            _qtiBankPersonalView = _testDataContext.GetTable<QtiBankPersonalView>();
        }

        public IQueryable<GetQTIBankListResult> GetQtiBankList(int userId, int districtId, string bankName,
            string author, string publishedTo)
        {
            return
                _testDataContext.GetQTIBankList(userId, districtId, bankName ?? "", author ?? "", publishedTo ?? "")
                    .AsQueryable();
        }

        public IQueryable<QtiBankPublishedDistrict> GetPublishedDistrict()
        {
            return _tableQtiBankPublishedDistrictView.Select(
                x => new QtiBankPublishedDistrict
                {
                    DistrictId = x.DistrictID,
                    Name = x.Name,
                    QtiBankDistrictId = x.QTIBankDistrictID,
                    QtiBankId = x.QTIBankID
                }
                );
        }

        public IQueryable<QtiBankPublishedSchool> GetPublishedSchool()
        {
            return _tableQtiBankPublishedSchoolView.Select(
                x => new QtiBankPublishedSchool
                {
                    SchoolId = x.SchoolID,
                    Name = x.Name,
                    DistrictName = x.DistrictName,
                    QtiBankId = x.QTIBankID,
                    QtiBankSchoolId = x.QTIBankSchoolID
                }
                );
        }

        public IQueryable<QtiBank> Select()
        {
            return _tableQtiBankEntity.Select(
                x => new QtiBank
                {
                    AccessId = x.AccessID,
                    AuthorGroupId = x.AuthorGroupID,
                    DistrictId = x.DistrictID,
                    Name = x.Name,
                    QtiBankId = x.QTIBankID,
                    SchoolId = x.SchoolID,
                    StateId = x.StateID,
                    UserId = x.UserID,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate
                });
        }

        public void Save(QtiBank item)
        {
            var entity = _tableQtiBankEntity.FirstOrDefault(x => x.QTIBankID.Equals(item.QtiBankId));

            if (entity.IsNull())
            {
                entity = new QTIBankEntity();
                _tableQtiBankEntity.InsertOnSubmit(entity);
            }

            MappingQTIBank(item, entity);
            _tableQtiBankEntity.Context.SubmitChanges();
            item.QtiBankId = entity.QTIBankID;
        }

        public void Delete(QtiBank item)
        {
            var entity = _tableQtiBankEntity.FirstOrDefault(x => x.QTIBankID.Equals(item.QtiBankId));
            if (entity != null)
            {
                _tableQtiBankEntity.DeleteOnSubmit(entity);
                _tableQtiBankEntity.Context.SubmitChanges();
            }
        }

        public List<QtiBankCustom> LoadQTIBanks(int currentUserId, int roleId, int districtId, bool? hideTeacherBanks=null, bool? hideOtherPeopleBanks=null)
        {
            return _testDataContext.ListQTIBanks_HideTeacherBank(currentUserId, roleId, districtId,hideTeacherBanks,hideOtherPeopleBanks).Select(o => new QtiBankCustom()
            {
                QTIBankId = o.QTIBankID,
                Name = o.Name,
                AuthorGroupId = o.AuthorGroupID,
                AuthorGroupName = o.AuthoGroupName??"",
                //AuthorFirstName = o.AuthorFirstName??"",
                //AuthorLastName = o.AuthorLastName??""
                ModifiedDate =  o.ModifiedDate
            }).ToList();
        }

        public DateTime? GetQTIBankModifiedDate(int qtiBankId)
        {
            var modifiedDateData = _testDataContext.GetQTIBankModifiedDate(qtiBankId).FirstOrDefault();
            var modifiedDate = DateTime.MinValue;

            if (modifiedDateData.QTIBankModifiedDate.HasValue && modifiedDateData.QTIBankModifiedDate > modifiedDate)
                modifiedDate = modifiedDateData.QTIBankModifiedDate.Value;

            if (modifiedDateData.QTIGroupModifiedDate.HasValue && modifiedDateData.QTIGroupModifiedDate > modifiedDate)
                modifiedDate = modifiedDateData.QTIGroupModifiedDate.Value;

            if (modifiedDateData.QTIItemModifiedDate.HasValue && modifiedDateData.QTIItemModifiedDate > modifiedDate)
                modifiedDate = modifiedDateData.QTIItemModifiedDate.Value;

            if (modifiedDate == DateTime.MinValue)
                return null;

            return modifiedDate;
        }

        private void MappingQTIBank(QtiBank item, QTIBankEntity entity)
        {
            entity.AccessID = item.AccessId;
            entity.AuthorGroupID = item.AuthorGroupId;
            entity.DistrictID = item.DistrictId;
            entity.Name = item.Name;
            entity.SchoolID = item.SchoolId;
            entity.StateID = item.StateId;
            entity.UserID = item.UserId;
            entity.CreatedDate = item.CreatedDate;
            entity.ModifiedDate = item.ModifiedDate;
        }
        public List<QtiBank> LoadQTIBanksPersonal(int userId, int districtId)
        {
            return _testDataContext.GetQtiItemBanksPersonal(userId,districtId).Select(x => new QtiBank()
                                                        {
                                                            QtiBankId = x.QTIBankID,
                                                            Name = x.Name
                                                        }).ToList();
        }




        public List<QtiBank> GetOwnerItemBanks(int userId)
        {
            return _tableQtiBankEntity.Where(x => x.UserID == userId).Select(x => new QtiBank()
            {
                AccessId = x.AccessID,
                AuthorGroupId = x.AuthorGroupID,
                CreatedDate = x.CreatedDate,
                DistrictId = x.DistrictID,
                ModifiedDate = x.ModifiedDate,
                Name = x.Name,
                QtiBankId = x.QTIBankID,
                SchoolId = x.SchoolID,
                StateId = x.StateID,
                UserId = x.UserID,
            }).ToList();
        }
        public List<QtiBankSchool> GetQtiBankSchoolOfSchools(string schoolIdString)
        {
            return _testDataContext.GetQtiBankSchoolOfSchools(schoolIdString)
            .Select(x => new QtiBankSchool
            {
                SchoolId = x.SchoolID,
                EditedByUserId = x.EditedByUserID,
                QtiBankSchoolId = x.QTIBankSchoolID,
                QtiBankId = x.QTIBankID
            }).ToList();
        }

        public List<QtiBankSchool> GetQtiBankSchoolOfSchools(List<int> schoolIdList)
        {
            var schoolIdString = string.Empty;
            if (schoolIdList != null)
            {
                schoolIdString = string.Join(",", schoolIdList.ToArray());
            }

            return _testDataContext.GetQtiBankSchoolOfSchools(schoolIdString)
          .Select(x => new QtiBankSchool
          {
              SchoolId = x.SchoolID,
              EditedByUserId = x.EditedByUserID,
              QtiBankSchoolId = x.QTIBankSchoolID,
              QtiBankId = x.QTIBankID
          }).ToList();
        }
    }
}