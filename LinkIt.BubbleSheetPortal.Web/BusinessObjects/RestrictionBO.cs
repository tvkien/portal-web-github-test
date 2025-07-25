using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.RestrictionDTO;
using LinkIt.BubbleSheetPortal.Web.Models.RestrictionDTO;

namespace LinkIt.BubbleSheetPortal.Web.BusinessObjects
{
    public class RestrictionBO : IRestriction
    {
        readonly AuthorGroupService _authorGroupService;
        readonly TestRestrictionModuleService _testRestrictionModuleService;
        readonly VirtualTestService _testService;


        public RestrictionBO(AuthorGroupService authorGroupService, TestRestrictionModuleService testRestrictionModuleService, VirtualTestService testService)
        {
            _authorGroupService = authorGroupService;
            _testRestrictionModuleService = testRestrictionModuleService;
            _testService = testService;
        }

        /// <summary>
        /// Filter List Banks by Restriction data
        /// </summary>
        /// <param name="banks"></param>
        /// <param name="module"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ListItem> FilterBanks(FilterBankQueryDTO query, List<RestrictionDTO> restrictions = null)
        {
            if(restrictions == null)
            {
                restrictions = GetRestrictionList(query.ModuleCode, query.UserId, query.RoleId, PublishLevelTypeEnum.District, query.DistrictId);
            }

            var restrictBankIds = restrictions
                .Where(m=>m.RestrictionObjectType == RestrictionObjectType.Bank)
                .Select(m => m.RestrictionObjectId).ToList();

            query.Banks.RemoveAll(m => restrictions.Any(h => h.RestrictionObjectType == RestrictionObjectType.Bank 
                                                            && h.RestrictionObjectId == m.Id));
            return query.Banks;
        }

        /// <summary>
        /// Filter List Tests by Restriction data
        /// </summary>
        /// <param name="tests"></param>
        /// <param name="module"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ListItem> FilterTests(FilterTestQueryDTO query, List<RestrictionDTO> restrictions = null)
        {
            if(restrictions == null)
            {
                restrictions = GetRestrictionList(query.ModuleCode, query.UserId, query.RoleId, PublishLevelTypeEnum.District, query.DistrictId);
            }

            var restrictionBank = restrictions.FirstOrDefault(m => m.RestrictionObjectType == RestrictionObjectType.Bank && m.RestrictionObjectId == query.BankId);

            if (restrictionBank != null)
            {
                query.Tests = new List<ListItem>();
            }
            else
            {
                query.Tests.RemoveAll(m => restrictions.Any(h => h.RestrictionObjectType == RestrictionObjectType.Test
                                                        && h.RestrictionObjectId == m.Id));
                //exclude testIds
            }

            return query.Tests;
        }

        /// <summary>
        /// Check restriction status of a bank or test
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool IsAllowTo(IsCheckRestrictionObjectDTO query)
        {
            var result = true;
            var listRestrictions = GetRestrictionList(query.UserId, query.RoleId, query.DistrictId, query.IsExport);

            if (query.ObjectType == RestrictionObjectType.Bank)
            {
                var restictionObject = listRestrictions.FirstOrDefault(m => m.RestrictionObjectType == RestrictionObjectType.Bank
                                                                                && m.RestrictionObjectId == query.BankId);

                if (restictionObject != null)
                {
                    result = false;
                }
            }
            else if (query.ObjectType == RestrictionObjectType.Test)
            {
                var restrictionObject = listRestrictions
                    .FirstOrDefault(m => ((m.RestrictionObjectType == RestrictionObjectType.Test && m.RestrictionObjectId == query.TestId)
                                        || (m.RestrictionObjectType == RestrictionObjectType.Bank && m.RestrictionObjectId == query.BankId))
                                                && m.TestRestrictionModuleCode == query.ModuleCode);

                if (restrictionObject != null)
                {
                    return false;
                }
            }

            return result;
        }

        public List<RestrictionDTO> GetRestrictionList(string moduleCode, int userId, int roleId, PublishLevelTypeEnum publishLevelType, int publishLevelId)
        {
            // get restrictions
            var result = _testRestrictionModuleService.GetListRestriction(moduleCode, userId, roleId, publishLevelType, publishLevelId);

            // get author bank
            var authorBankIds = GetAuthorBank(userId);

            // remove bank of author list
            result.RemoveAll(m => m.RestrictionObjectType == RestrictionObjectType.Bank && authorBankIds.Contains(m.RestrictionObjectId));

            // get test restricted id
            var testIds = result.Where(m => m.RestrictionObjectType == RestrictionObjectType.Test)
                .Select(m => m.RestrictionObjectId)
                .Distinct()
                .ToList();

            // query bank of test restricted
            var bankIdTestIds = _testService.GetBankIds(testIds);

            // remove test if that has bank in authorgroup
            bankIdTestIds.RemoveAll(m => authorBankIds.Contains(m.Item1));
            testIds = bankIdTestIds.Select(m => m.Item2).ToList();

            result.RemoveAll(m => m.RestrictionObjectType == RestrictionObjectType.Test && !testIds.Contains(m.RestrictionObjectId));

            return result;
        }

        public List<RestrictionDTO> GetRestrictionList(int userId, int roleId, int districtId, bool isExport)
        {
            var result = _testRestrictionModuleService.GetListRestriction(userId, roleId, districtId);

            if (!isExport)
            {
                // get author bank
                var authorBankIds = GetAuthorBank(userId);

                // remove bank of author list
                result.RemoveAll(m => m.RestrictionObjectType == RestrictionObjectType.Bank && authorBankIds.Contains(m.RestrictionObjectId));

                // get test restricted id
                var testIds = result.Where(m => m.RestrictionObjectType == RestrictionObjectType.Test).Select(m => m.RestrictionObjectId).ToList();

                // query bank of test restricted
                var bankIdTestIds = _testService.GetBankIds(testIds);

                // remove test if that has bank in authorgroup
                bankIdTestIds.RemoveAll(m => authorBankIds.Contains(m.Item1));
                testIds = bankIdTestIds.Select(m => m.Item2).ToList();

                result.RemoveAll(m => m.RestrictionObjectType == RestrictionObjectType.Test && !testIds.Contains(m.RestrictionObjectId));
            }

            return result;
        }
        #region Private Functions
        private List<int> GetAuthorBank(int userId)
        {
            var bankIds = _authorGroupService.GetBanksOfUsers(userId).ToList();
            return bankIds;
        }

        #endregion
    }
}
