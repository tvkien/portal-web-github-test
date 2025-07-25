using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Data.Repositories.PerformanceBandAutomations;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class PerformanceBandAutomationService
    {
        private readonly IPerformanceBandAutomationRepository _performanceBandAutomationRepository;
        private readonly IPerformanceBandVirtualTestRepository _performanceBandVirtualTestRepository;
        private readonly Dictionary<int?, string> _performanceBandVirtualTestLevelMap
            = new Dictionary<int?, string>
            {
                { (int)PerformanceBandVirtualTestLevel.Enterprise, Constanst.PERFORMANCE_BAND_VIRTUAL_TEST_ENTERPRISE },
                { (int)PerformanceBandVirtualTestLevel.District, Constanst.PERFORMANCE_BAND_VIRTUAL_TEST_DISTRICT },
            };

        public PerformanceBandAutomationService(
            IPerformanceBandAutomationRepository performanceBandAutomationRepository,
            IPerformanceBandVirtualTestRepository performanceBandVirtualTestRepository)
        {
            _performanceBandAutomationRepository = performanceBandAutomationRepository;
            _performanceBandVirtualTestRepository = performanceBandVirtualTestRepository;
        }

        public IEnumerable<TestTypeGradeAndSubjectForPBSDto> GetTestTypeGradeAndSubject(TestTypeGradeAndSubjectForPBSFilter criteria)
        {
            return _performanceBandAutomationRepository.GetTestTypeGradeAndSubject(criteria)
                .ToArray()
                .Select(c => new TestTypeGradeAndSubjectForPBSDto()
                {
                    Kind = c.Kind,
                    Id = c.ID,
                    Name = c.Name,
                    Order = c.Order,
                }).ToArray();
        }

        public IEnumerable<GetTestForPBSResult> GetTestForPBS(TestForPBSFilter criteria)
        {
            return _performanceBandAutomationRepository.GetTestForPBS(criteria);
        }

        public ApplySettingForPBSDto ApplySetting(ApplySettingForPBSPayload payload)
        {
            var result = _performanceBandAutomationRepository.ApplySettingForPBS(payload);
            return ProcessAfterApplyRemoveSetting(result.ToList(), payload.DistrictID);
        }

        public IEnumerable<SelectListItemDTO> GetPBSInEffect()
        {
            return new List<SelectListItemDTO>
            {
                new SelectListItemDTO
                {
                    Id = (int)PerformanceBandVirtualTestLevel.Enterprise,
                    Name = Constanst.PERFORMANCE_BAND_VIRTUAL_TEST_ENTERPRISE
                },
                new SelectListItemDTO
                {
                    Id = (int)PerformanceBandVirtualTestLevel.District,
                    Name = Constanst.PERFORMANCE_BAND_VIRTUAL_TEST_DISTRICT
                },
                new SelectListItemDTO
                {
                    Id = (int)PerformanceBandVirtualTestLevel.None,
                    Name = Constanst.PERFORMANCE_BAND_VIRTUAL_TEST_NONE
                }
            };
        }

        public IEnumerable<SelectListItemDTO> GetPBSGroup(int districtId)
        {
            var list = _performanceBandAutomationRepository.GetPerformanceBandGroups(districtId, 0);
            return list.Select(x => new SelectListItemDTO
            {
                Id = x.PerformanceBandGroupID,
                Name = x.PerformanceBandGroupName
            });
        }

        public ApplySettingForPBSDto RemoveSetting(ApplySettingForPBSPayload payload)
        {
            var result = _performanceBandAutomationRepository.RemoveSettingForPBS(payload);
            return ProcessAfterApplyRemoveSetting(result.ToList(), payload.DistrictID);
        }

        private ApplySettingForPBSDto ProcessAfterApplyRemoveSetting(List<ApplySettingForPBSItemDto> result, int districtID)
        {
            var virtualTestIDs = result.Select(x => x.VirtualTestID).Distinct();
            var performanceBandVirtualTests = _performanceBandVirtualTestRepository.Get(virtualTestIDs);

            foreach (var item in result)
            {
                var performanceBandVirtualTest = performanceBandVirtualTests
                    .Where(x => x.VirtualTestID == item.VirtualTestID && (x.ID == 0 || x.ID == districtID))
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();
                if (performanceBandVirtualTest != null &&
                    _performanceBandVirtualTestLevelMap.TryGetValue(performanceBandVirtualTest.Level, out string pbsInEffect))
                {
                    item.PBSInEffect = pbsInEffect;
                }
                else
                {
                    item.PBSInEffect = Constanst.PERFORMANCE_BAND_VIRTUAL_TEST_NONE;
                }
            }

            return new ApplySettingForPBSDto
            {
                VirtualTests = result.Select(x => new ApplySettingForPBSItemDto
                {
                    VirtualTestID = x.VirtualTestID,
                    PBSInEffect = x.PBSInEffect,
                    IsChange = x.IsChange
                })
            };
        }

    }
}
