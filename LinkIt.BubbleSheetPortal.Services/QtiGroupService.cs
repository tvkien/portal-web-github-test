using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QtiGroupService
    {
        private readonly IQTIGroupRepository _iQtiGroup;
        private readonly IReadOnlyRepository<QTIItemData> _qtiItemReadOnlyRepository;
        private readonly IQtiBankRepository _qtiBankRepository;
        private readonly IReadOnlyRepository<User> _userRepository; 

        public QtiGroupService(IQTIGroupRepository iQtiGroup, IReadOnlyRepository<QTIItemData> qtiItemReadOnlyRepository, IQtiBankRepository qtiBankRepository,
            IReadOnlyRepository<User> userRepository)
        {
            this._iQtiGroup = iQtiGroup;
            this._qtiItemReadOnlyRepository = qtiItemReadOnlyRepository;
            this._qtiBankRepository = qtiBankRepository;
            this._userRepository = userRepository;
        }

        public QtiGroup GetById(int qtiGroupId)
        {
            return _iQtiGroup.Select().FirstOrDefault(o => o.QtiGroupId == qtiGroupId);
        }

        public IQueryable<QtiGroup> GetByIdList(List<int> qtiGroupIdList)
        {
            return _iQtiGroup.Select().Where(o => qtiGroupIdList.Contains(o.QtiGroupId));
        }

        public DateTime? GetQTIGroupModifiedDate(int qtiGroupId, DateTime? qtiGroupModifiedDate)
        {
            var modifiedDate = DateTime.MinValue;

            if (qtiGroupModifiedDate.HasValue && qtiGroupModifiedDate.Value > modifiedDate)
                modifiedDate = qtiGroupModifiedDate.Value;

            var qtiItems = _qtiItemReadOnlyRepository.Select().Where(en => en.QTIGroupID == qtiGroupId);
            if(qtiItems.Any())
            {
                var qtiItemModifiedDate = qtiItems.Max(en => en.Updated);

                if (qtiItemModifiedDate.HasValue && qtiItemModifiedDate.Value > modifiedDate)
                    modifiedDate = qtiItemModifiedDate.Value;
            }

            if (modifiedDate == DateTime.MinValue)
                return null;

            return modifiedDate;
        }

        public void Save(QtiGroup qtiGroup)
        {
            if (qtiGroup != null)
            {
                if (qtiGroup.QtiGroupId == 0)
                    qtiGroup.CreatedDate = DateTime.Now;
                qtiGroup.ModifiedDate = DateTime.Now;

                _iQtiGroup.Save(qtiGroup);
            }
        }

        public List<QtiGroup> GetListQtiGroupByQtiBankId(int qtiBankId, int userid, int roleId, int districtId)
        {
            return _iQtiGroup.GetlistQtiGroups(qtiBankId, userid, roleId, districtId);
        }

        public List<QtiGroup> GetOwnerListQtiGroupByQtiBankId(int qtiBankId)
        {
            return _iQtiGroup.GetOwnerListQtiGroupByQtiBankId(qtiBankId);
        }

        public bool ExistItemSet(int userId, int itembankId, string strName)
        {
            if (userId > 0 && itembankId > 0 && !string.IsNullOrEmpty(strName))
            {
               return _iQtiGroup.Select()
                    .Any(o => o.UserId == userId && o.Name.Equals(strName) && o.QtiBankId == itembankId);
            }
            return false;
        }

        public bool ExistItemSetList(int userId, int itembankId,  List<string> strNames)
        {
            if (itembankId > 0 && strNames.Count > 0)
            {
                var temp = strNames.Select(x=>x.ToLower());
                return _iQtiGroup.Select()
                     .Any(o => o.UserId == userId && temp.Contains(o.Name.ToLower()) && o.QtiBankId == itembankId);
            }
            return false;
        }

        public string DeleteItemSetAndItems(int qtiGroupId, int userId)
        {
            return _iQtiGroup.DeleteItemSetAndItems(qtiGroupId, userId);
        }

        public void UpdateAuthorGroupId(int qtiGroupId, int authorGroupId)
        {
            var obj = _iQtiGroup.Select().FirstOrDefault(o => o.QtiGroupId == qtiGroupId);

            if (obj != null && authorGroupId > 0)
            {
                obj.AuthorGroupId = authorGroupId;
                obj.ModifiedDate = DateTime.Now;
                _iQtiGroup.Save(obj);
            }
        }


        public bool CheckExistSetName(string groupName, int userId, int qtiBankId, int? qtiGroupId)
        {
            if (qtiGroupId.HasValue)
                return _iQtiGroup.Select().Any(o => o.UserId == userId && o.Name.ToLower() == groupName.ToLower() && o.QtiBankId == qtiBankId && o.QtiGroupId != qtiGroupId.Value);

            return _iQtiGroup.Select().Any(o => o.UserId == userId && o.Name == groupName && o.QtiBankId == qtiBankId);
        }
        public void MoveToOtherItemBank(List<QtiGroup> qtiGroupList, int qtiBankId, int userId)
        {
            foreach (var qtiGroup in qtiGroupList)
            {
                qtiGroup.QtiBankId = qtiBankId;
                qtiGroup.UserId = userId;
                _iQtiGroup.Save(qtiGroup);
            }
        }
        public bool MoveToOtherItemBank(int qtiGroupId, int qtiBankId, string newItemSetName)
        {
            var result = true;

            var qtiGroup = _iQtiGroup.Select().SingleOrDefault(en => en.QtiGroupId == qtiGroupId);
            if(qtiGroup.QtiBankId == qtiBankId)
            {
                result = false; 
            }

            qtiGroup.QtiBankId = qtiBankId;
            qtiGroup.Name = newItemSetName;
            _iQtiGroup.Save(qtiGroup);
            
            return result;
        }
        public List<QtiGroup> CloneToItemBank(List<QtiGroup> qtiGroupList, int qtiBankId, int userId, out Dictionary<int, int> oldNewQtiGroupIdMap)
        {
            oldNewQtiGroupIdMap = new Dictionary<int, int>();

            List<QtiGroup> rs = new List<QtiGroup>();
            foreach (var qtiGroup in qtiGroupList)
            {
                var newQtiGroup = new QtiGroup
                {
                    AccessId = qtiGroup.AccessId,
                    AuthorFirstName = qtiGroup.AuthorFirstName,
                    AuthorGroupId = qtiGroup.AuthorGroupId,
                    AuthorGroupName = qtiGroup.AuthorGroupName,
                    AuthorLastName = qtiGroup.AuthorLastName,
                    CreatedDate = DateTime.Now,
                    GroupId = qtiGroup.GroupId,
                    ModifiedDate = DateTime.Now,
                    Name = qtiGroup.Name,
                    OldMasterCode = qtiGroup.OldMasterCode,
                    OwnershipType = qtiGroup.OwnershipType,
                    QtiBankId = qtiBankId,
                    Source = qtiGroup.Source,
                    SourceId = qtiGroup.SourceId,
                    Type = qtiGroup.Type,
                    UserId = userId,
                    VirtualTestId = qtiGroup.VirtualTestId
                };
                _iQtiGroup.Save(newQtiGroup);

                rs.Add(newQtiGroup);

                oldNewQtiGroupIdMap.Add(qtiGroup.QtiGroupId, newQtiGroup.QtiGroupId);
            }

            return rs;
        }

        public QtiGroup CloneToItemBank(int qtiGroupId, int qtiBankId, int userId, string newItemSetName)
        {
            var result = true;

            var qtiGroup = _iQtiGroup.Select().SingleOrDefault(en => en.QtiGroupId == qtiGroupId);
            if (qtiGroup == null)
                result = false;

            if(result)
            {
                var newQtiGroup = new QtiGroup
                                        {
                                            AccessId = qtiGroup.AccessId,
                                            AuthorFirstName = qtiGroup.AuthorFirstName,
                                            AuthorGroupId = qtiGroup.AuthorGroupId,
                                            AuthorGroupName = qtiGroup.AuthorGroupName,
                                            AuthorLastName = qtiGroup.AuthorLastName,
                                            CreatedDate = DateTime.Now,
                                            GroupId = qtiGroup.GroupId,
                                            ModifiedDate = DateTime.Now,
                                            Name = newItemSetName,
                                            OldMasterCode = qtiGroup.OldMasterCode,
                                            OwnershipType = qtiGroup.OwnershipType,
                                            QtiBankId = qtiBankId,
                                            Source = qtiGroup.Source,
                                            SourceId = qtiGroup.SourceId,
                                            Type = qtiGroup.Type,
                                            UserId = userId,
                                            VirtualTestId = qtiGroup.VirtualTestId
                                        };
                _iQtiGroup.Save(newQtiGroup);
                return newQtiGroup;
            }


            return null;
        }
        public   IQueryable<QtiGroup> GetQtiGroupsByBank(int qtiBankId)
        {
            return _iQtiGroup.Select().Where(x => x.QtiBankId == qtiBankId);
        }
        public QtiGroup GetItemSetByName(int userId, int itembankId, string strName)
        {
            if (userId > 0 && itembankId > 0 && !string.IsNullOrEmpty(strName))
            {
                return _iQtiGroup.Select().FirstOrDefault(o => o.UserId == userId && o.Name.Equals(strName) && o.QtiBankId == itembankId);
            }
            return null;
        }
        

        public QtiGroup CreateItemSetByUserId(int userId, int? qtiiBankId, string testName)
        {
            var vQTIGroup = GetItemSetByName(userId, qtiiBankId.GetValueOrDefault(), testName);
            if (vQTIGroup == null)
            {
                vQTIGroup = new QtiGroup()
                {
                    Name = testName,
                    Source = "itemset_temp",
                    Type = "normal",
                    AccessId = 1,
                    UserId = userId,
                    QtiBankId = qtiiBankId,
                    OwnershipType = 1,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };
                Save(vQTIGroup);
            }
            return vQTIGroup;
        }
        public QtiGroup GetDefaultQTIGroup(int userID, int qtiBankID, string virtualTestName, bool isSurvey = false)
        {
            var result = this.GetItemSetByName(userID, qtiBankID, virtualTestName);
            if (result != null) return result;

            result = new QtiGroup
            {
                Name = virtualTestName,
                Source = "itemset_0", //as creating new on Test Design -> Assessment Item New
                Type = "normal",
                AccessId = 1,
                UserId = userID,
                QtiBankId = qtiBankID,
                OwnershipType = 1
            };

            if (isSurvey)
                result.QtiBankId = null;

            _iQtiGroup.Save(result);

            return result;
        }

        public void ReassignQuestionOrder(int qtiGroupId)
        {
            _iQtiGroup.ReassignQuestionOrder(qtiGroupId);
        }

        public IQueryable<QtiGroup> GetAll()
        {
            return _iQtiGroup.Select();
        }
        public bool HasRightToEditQtiGroup(int qtiGroupId,User currentUser)
        {
            //Check permission
            //User might enter a qtiGroupId in url so it must check right permission of user
            var itemSet = _iQtiGroup.Select().FirstOrDefault(o => o.QtiGroupId == qtiGroupId);
            if (itemSet == null)
            {
                //There's no item set
                return false; // no permission
            }

            var itemBank = _qtiBankRepository.Select().FirstOrDefault(x => x.QtiBankId == itemSet.QtiBankId);
            if (itemBank == null)
            {
                return true;//sometime there is no data in column QtiGroup.QtiBankID 
            }

            var districtId = 0;
            if (itemBank != null && itemBank.DistrictId.HasValue)
            {
                districtId = itemBank.DistrictId.Value;
            }
            else
            {
                var itemBankCreator = _userRepository.Select().FirstOrDefault(x => x.Id == itemBank.UserId);
                districtId = itemBankCreator != null ? itemBankCreator.DistrictId.GetValueOrDefault() : currentUser.DistrictId.GetValueOrDefault();
            }

            var authorizedQtiBankList = _qtiBankRepository.LoadQTIBanks(currentUser.Id, currentUser.RoleId, districtId).Select(
                    x => x.QTIBankId).ToList();


            if (!authorizedQtiBankList.Contains(itemSet.QtiBankId ?? 0))
            {
                return false; // no permission
            }
            else
            {
                //Check at qti group level
                var authorizedQtiGroupIdList = _iQtiGroup.GetlistQtiGroups(
                    itemSet.QtiBankId ?? 0, currentUser.Id,
                    currentUser.RoleId,
                    districtId).Select(x => x.QtiGroupId);
                if (!authorizedQtiGroupIdList.Contains(qtiGroupId))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
