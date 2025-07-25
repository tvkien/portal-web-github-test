using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.PerformanceBandAutomations
{
    public class PerformanceBandAutomationRepository : IPerformanceBandAutomationRepository
    {
        private readonly TestDataContext _context;
        private readonly string _connectionString;
        private readonly IReadOnlyRepository<District> _districtRepository;

        public PerformanceBandAutomationRepository(IConnectionString conn, IReadOnlyRepository<District> districtRepository)
        {
            _connectionString = conn.GetLinkItConnectionString();
            _context = TestDataContext.Get(_connectionString);
            _districtRepository = districtRepository;
        }

        public IEnumerable<GetTestTypeGradeAndSubjectForPBSResult> GetTestTypeGradeAndSubject(TestTypeGradeAndSubjectForPBSFilter criteria)
        {
            return _context.GetTestTypeGradeAndSubjectForPBS(criteria.DistrictId, (int)(criteria.DataSetOriginID ?? 0), criteria.SchoolIds).ToArray();
        }

        public IEnumerable<GetTestForPBSResult> GetTestForPBS(TestForPBSFilter criteria)
        {
            return _context.GetTestForPBS(
                criteria.DistrictID,
                criteria.VirtualTestTypeIds,
                criteria.GradeIDs,
                criteria.SubjectNames,
                criteria.PBSInEffect,
                criteria.PBSGroup,
                criteria.StartRow,
                criteria.PageSize,
                criteria.GeneralSearch,
                criteria.SortColumn,
                criteria.SortDirection
                ).ToArray();
        }

        public IEnumerable<GetPerformanceBandGroupsResult> GetPerformanceBandGroups(int districtId, int dataSetOriginID)
        {
            return _context.GetPerformanceBandGroups(districtId, dataSetOriginID).ToList();
        }

        public IEnumerable<ApplySettingForPBSItemDto> ApplySettingForPBS(ApplySettingForPBSPayload payload)
        {
            var virtualTestIDs = payload.VirtualTestIDs.ToEnumerable().ToList();

            List<ApplySettingForPBSItemDto> result = virtualTestIDs.Select(x=> new ApplySettingForPBSItemDto
            {
                IsChange = false,
                VirtualTestID = x,
                PBSInEffect = string.Empty
            }).ToList();
            
            var stateID = _districtRepository.Select().Where(x => x.Id == payload.DistrictID).Select(x=>x.StateId).FirstOrDefault();

            var virtualTestMappingPBCs = GetVirtualTestMappingPBC(payload.DistrictID, virtualTestIDs.ToIntCommaSeparatedString());

            virtualTestIDs = virtualTestMappingPBCs.Select(x => x.VirtualTestID).Distinct().ToList();

            if (virtualTestIDs.Count() == 0)
            {
                return result;
            }

            var performanceBandConfigurationDtos = GetPerformanceBandConfigurations(payload.DistrictID, virtualTestIDs.ToIntCommaSeparatedString());

            if (performanceBandConfigurationDtos.Count() == 0)
            {
                return result;
            }

            var performanceBandSettingDtos = MappingPerformanceBandSetting(
                virtualTestIDs,
                virtualTestMappingPBCs,
                performanceBandConfigurationDtos,
                payload.DistrictID,
                stateID);

            if (performanceBandSettingDtos.Count == 0)
            {
                return result;
            }

            var virtualTestIDsMapped = AutoMapPerformanceBandVirtualTest(performanceBandSettingDtos);

            result.ForEach(x => x.IsChange = virtualTestIDsMapped.Contains(x.VirtualTestID));

            return result;
        }

        private List<VirtualTestMappingPBC> GetVirtualTestMappingPBC(int districtID, string virtualTestIDs)
        {
            var virtualTestMappingPBCs = _connectionString.Query<VirtualTestMappingPBC>(
                "[dbo].[LAC_GetVirtualTestMappingPBC]",
                new
                {
                    DistrictID = districtID,
                    VirtualTestIDs = virtualTestIDs
                },
                timeout: 120);

            return virtualTestMappingPBCs.ToList();
        }

        private List<PerformanceBandConfigurationDto> GetPerformanceBandConfigurations(int districtID, string virtualTestIDs)
        {
            var performanceBandConfigurationDtos = _connectionString.Query<PerformanceBandConfigurationDto>(
                "[dbo].[LAC_GetPBCMultipleVirtualTestIDs]",
                new
                {
                    DistrictID = districtID,
                    VirtualTestIDs = virtualTestIDs
                },
                timeout: 120);

            return performanceBandConfigurationDtos.ToList();
        }

        private IEnumerable<int> AutoMapPerformanceBandVirtualTest(List<PerformanceBandSettingDto> performanceBandSettingDtos)
        {
            var result = new List<PbsVirtualTestDto>();
            var performanceBandSettingEnterpriseLevel = performanceBandSettingDtos.Where(x => x.DistrictID == 0);

            if (performanceBandSettingEnterpriseLevel.Any())
            {
                var pbsEnterpriseLevel = _connectionString.BulkCopy<PbsVirtualTestDto>(
                    new List<TempTableCreation>()
                    {
                            new TempTableCreation
                            {
                                TempTableName = "#PerformanceBandSetting",
                                TempTableNameCreateScript = "CREATE TABLE #PerformanceBandSetting (VirtualTestID INT, PerformanceBandGroupID INT, Cutoffs VARCHAR(500), ScoreName VARCHAR(64), IsPrincipalLevel BIT, ScoreTier INT, SubScoreName VARCHAR (200))",
                                TempTableData = performanceBandSettingEnterpriseLevel.Select(x=> new
                                {
                                    VirtualTestID = x.VirtualTestID,
                                    PerformanceBandGroupID = x.PerformanceBandGroupID,
                                    Cutoffs = x.Cutoffs,
                                    ScoreName = x.ScoreName,
                                    IsPrincipalLevel = x.IsPrincipalLevel,
                                    ScoreTier = x.ScoreTier,
                                    SubScoreName = x.SubScoreName
                                })
                            }
                    },
                    "[dbo].[LAC_AutoMapPerformanceBandVirtualTest]",
                    new
                    {
                        ConfigLevel = 0,
                        ConfigDistrictID = 0
                    },
                    timeout: 120);

                result.AddRange(pbsEnterpriseLevel);
            }

            var performanceBandSettingDistrictLevel = performanceBandSettingDtos.Where(x => x.DistrictID > 0);

            if (performanceBandSettingDistrictLevel.Any())
            {
                var pbsDistrictLevel = _connectionString.BulkCopy<PbsVirtualTestDto>(
                    new List<TempTableCreation>()
                    {
                            new TempTableCreation
                            {
                                TempTableName = "#PerformanceBandSetting",
                                TempTableNameCreateScript = "CREATE TABLE #PerformanceBandSetting (VirtualTestID INT, PerformanceBandGroupID INT, Cutoffs VARCHAR(500), ScoreName VARCHAR(64), IsPrincipalLevel BIT, ScoreTier INT, SubScoreName VARCHAR (200))",
                                TempTableData = performanceBandSettingDistrictLevel.Select(x=> new
                                {
                                    VirtualTestID = x.VirtualTestID,
                                    PerformanceBandGroupID = x.PerformanceBandGroupID,
                                    Cutoffs = x.Cutoffs,
                                    ScoreName = x.ScoreName,
                                    IsPrincipalLevel = x.IsPrincipalLevel,
                                    ScoreTier = x.ScoreTier,
                                    SubScoreName = x.SubScoreName
                                })
                            }
                    },
                    "[dbo].[LAC_AutoMapPerformanceBandVirtualTest]",
                    new
                    {
                        ConfigLevel = 2,
                        ConfigDistrictID = performanceBandSettingDistrictLevel.FirstOrDefault().DistrictID
                    },
                    timeout: 120);

                result.AddRange(pbsDistrictLevel);
            }

            return result.Select(x=>x.VirtualTestID).Distinct();
        }

        private List<PerformanceBandSettingDto> MappingPerformanceBandSetting(
            List<int> virtualTestIDs,
            IEnumerable<VirtualTestMappingPBC> virtualTestMappingPBCs,
            IEnumerable<PerformanceBandConfigurationDto> performanceBandConfigurationDtos,
            int districtID,
            int stateID)
        {
            var performanceBandSettingDtos = new List<PerformanceBandSettingDto>();

            foreach (var virtualTestID in virtualTestIDs)
            {
                var filter = new PerformanceBandConfigurationFilter
                {
                    TestName = virtualTestMappingPBCs.FirstOrDefault(x => x.VirtualTestID == virtualTestID)?.TestName.ToLower() ?? string.Empty,
                    SubjectName = virtualTestMappingPBCs.FirstOrDefault(x => x.VirtualTestID == virtualTestID)?.SubjectName.ToLower() ?? string.Empty,
                    GradeName = virtualTestMappingPBCs.FirstOrDefault(x => x.VirtualTestID == virtualTestID)?.GradeName.ToLower() ?? string.Empty,
                    DataSetCategoryID = virtualTestMappingPBCs.FirstOrDefault(x => x.VirtualTestID == virtualTestID)?.DataSetCategoryID ?? 0,
                    DistrictID = districtID,
                    StateID = stateID,
                    SubscoreNames = virtualTestMappingPBCs.Where(x => x.VirtualTestID == virtualTestID && !string.IsNullOrEmpty(x.SubScoreName)).Select(x => x.SubScoreName).Distinct().ToList()
                };

                var performanceBandConfigurations = performanceBandConfigurationDtos
                    .Where(x => (x.DataSetCategoryID == filter.DataSetCategoryID)
                            && (string.IsNullOrEmpty(x.Year) || filter.TestName.Contains(x.Year.ToLower()))
                            && (string.IsNullOrEmpty(x.Season) || filter.TestName.Contains(x.Season.ToLower()))
                            && (string.IsNullOrEmpty(x.Subject) || x.Subject.ToLower() == filter.SubjectName)
                            && (string.IsNullOrEmpty(x.Grade) || x.Grade.ToLower() == filter.GradeName)
                            && (string.IsNullOrEmpty(x.Keyword) || Regex.IsMatch(filter.TestName, $@"\b{x.Keyword.ToLower()}\b", RegexOptions.IgnoreCase))
                            && (x.ScoreTier == 1 || (x.ScoreTier == 2 && (string.IsNullOrEmpty(x.SubscoreName) || filter.SubscoreNames.Any(y => y.ToLower() == x.SubscoreName.ToLower()))))).ToList();

                if (!performanceBandConfigurations.Any())
                {
                    continue;
                }

                var pbsScoreTier1 = performanceBandConfigurations.Where(x => x.ScoreTier == 1);

                var pbsScoreTier1GroupByScoreName = pbsScoreTier1.GroupBy(x => x.ScoreName);

                foreach(var group in pbsScoreTier1GroupByScoreName)
                {
                    var listOfPBSScoreTier1ByGroup = group.ToList();

                    var pbsValid = FindClosestMatching(listOfPBSScoreTier1ByGroup, filter);

                    if (pbsValid != null)
                    {
                        performanceBandSettingDtos.Add(new PerformanceBandSettingDto
                        {
                            VirtualTestID = virtualTestID,
                            Cutoffs = pbsValid.Cutoffs,
                            IsPrincipalLevel = pbsValid.IsPrincipalLevel,
                            PerformanceBandGroupID = pbsValid.PerformanceBandGroupID,
                            ScoreName = pbsValid.ScoreName,
                            ScoreTier = pbsValid.ScoreTier,
                            SubScoreName = pbsValid.SubscoreName,
                            DistrictID = pbsValid.DistrictID
                        });
                    }
                }

                var pbsScoreTier2 = performanceBandConfigurations.Where(x => x.ScoreTier == 2);

                var pbsValidScoreTier2 = MappingScoreTier2(pbsScoreTier2, filter);

                if (pbsValidScoreTier2.Any())
                {
                    performanceBandSettingDtos.AddRange(pbsValidScoreTier2.Select(x => new PerformanceBandSettingDto
                    {
                        VirtualTestID = virtualTestID,
                        Cutoffs = x.Cutoffs,
                        IsPrincipalLevel = x.IsPrincipalLevel,
                        PerformanceBandGroupID = x.PerformanceBandGroupID,
                        ScoreName = x.ScoreName,
                        ScoreTier = x.ScoreTier,
                        SubScoreName = x.SubscoreName,
                        DistrictID = x.DistrictID
                    }));
                }
            }

            return performanceBandSettingDtos;
        }

        private PerformanceBandConfigurationDto FindClosestMatching(
            IEnumerable<PerformanceBandConfigurationDto> pbs,
            PerformanceBandConfigurationFilter filter)
        {
            if (!pbs.Any())
            {
                return null;
            }

            if (pbs.Any(x => x.DistrictID == filter.DistrictID))
            {
                pbs = pbs.Where(x => x.DistrictID == filter.DistrictID);
            }

            if (pbs.Any(x => x.StateID == filter.StateID))
            {
                pbs = pbs.Where(x => x.StateID == filter.StateID);
            }

            if (pbs.Any(x => !string.IsNullOrEmpty(x.Year) && filter.TestName.Contains(x.Year.ToLower())))
            {
                pbs = pbs.Where(x => !string.IsNullOrEmpty(x.Year) && filter.TestName.Contains(x.Year.ToLower()));
            }

            if (pbs.Any(x => !string.IsNullOrEmpty(x.Season) && filter.TestName.Contains(x.Season.ToLower())))
            {
                pbs = pbs.Where(x => !string.IsNullOrEmpty(x.Season) && filter.TestName.Contains(x.Season.ToLower()));
            }

            if (pbs.Any(x => !string.IsNullOrEmpty(x.Subject) && x.Subject.ToLower() == filter.SubjectName))
            {
                pbs = pbs.Where(x => !string.IsNullOrEmpty(x.Subject) && x.Subject.ToLower() == filter.SubjectName);
            }

            if (pbs.Any(x => !string.IsNullOrEmpty(x.Grade) && x.Grade.ToLower() == filter.GradeName))
            {
                pbs = pbs.Where(x => !string.IsNullOrEmpty(x.Grade) && x.Grade.ToLower() == filter.GradeName);
            }

            if (pbs.Any(x => !string.IsNullOrEmpty(x.Keyword) && Regex.IsMatch(filter.TestName, $@"\b{x.Keyword.ToLower()}\b", RegexOptions.IgnoreCase)))
            {
                pbs = pbs.Where(x => !string.IsNullOrEmpty(x.Keyword) && Regex.IsMatch(filter.TestName, $@"\b{x.Keyword.ToLower()}\b", RegexOptions.IgnoreCase));
            }

            return pbs.FirstOrDefault();
        }

        private IEnumerable<PerformanceBandConfigurationDto> MappingScoreTier2(
            IEnumerable<PerformanceBandConfigurationDto> pbs,
            PerformanceBandConfigurationFilter filter)
        {
            var newPbsScoreTier2 = new List<PerformanceBandConfigurationDto>();

            if (!pbs.Any())
            {
                return newPbsScoreTier2;
            }

            var pbsScoreTier2GroupByScoreName = pbs.GroupBy(x => x.ScoreName);

            foreach(var group in pbsScoreTier2GroupByScoreName)
            {
                var listOfpbsScoreTier2ByGroup = group.ToList();
                var pbsSubscoreNulls = listOfpbsScoreTier2ByGroup.Where(x => x.SubscoreName == null);
                var pbsSubscoreNull = FindClosestMatching(pbsSubscoreNulls, filter);

                foreach (var subscoreName in filter.SubscoreNames)
                {
                    var pbsSubscores = listOfpbsScoreTier2ByGroup.Where(x => !string.IsNullOrEmpty(x.SubscoreName) && x.SubscoreName.ToLower() == subscoreName.ToLower());
                    var pbsValid = FindClosestMatching(pbsSubscores, filter);

                    var pbsAfterCompare = ComparePbsSubScoreName(pbsValid, pbsSubscoreNull, filter);

                    if (pbsAfterCompare != null)
                    {
                        newPbsScoreTier2.Add(new PerformanceBandConfigurationDto
                        {
                            CustomYearSeason = pbsAfterCompare.CustomYearSeason,
                            Cutoffs = pbsAfterCompare.Cutoffs,
                            DataSetCategoryID = pbsAfterCompare.DataSetCategoryID,
                            DistrictID = pbsAfterCompare.DistrictID,
                            Grade = pbsAfterCompare.Grade,
                            IsPrincipalLevel = pbsAfterCompare.IsPrincipalLevel,
                            Keyword = pbsAfterCompare.Keyword,
                            PerformanceBandConfigurationID = pbsAfterCompare.PerformanceBandConfigurationID,
                            PerformanceBandGroupID = pbsAfterCompare.PerformanceBandGroupID,
                            ScoreName = pbsAfterCompare.ScoreName,
                            ScoreTier = pbsAfterCompare.ScoreTier,
                            Season = pbsAfterCompare.Season,
                            Subject = pbsAfterCompare.Subject,
                            Year = pbsAfterCompare.Year,
                            SubscoreName = subscoreName
                        });
                    }
                }
            }

            return newPbsScoreTier2;
        }

        private PerformanceBandConfigurationDto ComparePbsSubScoreName(
            PerformanceBandConfigurationDto source,
            PerformanceBandConfigurationDto target,
            PerformanceBandConfigurationFilter filter)
        {
            if (source == null)
            {
                return target;
            }

            if (target == null)
            {
                return source;
            }

            if ((source.DistrictID != 0 && target.DistrictID == 0)
                || (source.DistrictID != 0 && target.DistrictID != 0 && source.DistrictID == filter.DistrictID && target.DistrictID != filter.DistrictID))
            {
                return source;
            }

            if ((source.DistrictID == 0 && target.DistrictID != 0)
                || (source.DistrictID != 0 && target.DistrictID != 0 && source.DistrictID != filter.DistrictID && target.DistrictID == filter.DistrictID))
            {
                return target;
            }

            if ((source.StateID != 0 && target.StateID == 0)
                || (source.StateID != 0 && target.StateID != 0 && source.StateID == filter.StateID && target.StateID != filter.StateID))
            {
                return source;
            }

            if ((source.StateID == 0 && target.StateID != 0)
                || (source.StateID != 0 && target.StateID != 0 && source.StateID != filter.StateID && target.StateID == filter.StateID))
            {
                return target;
            }

            if ((!string.IsNullOrEmpty(source.Year) && string.IsNullOrEmpty(target.Year))
                || (!string.IsNullOrEmpty(source.Year) && !string.IsNullOrEmpty(target.Year) && filter.TestName.Contains(source.Year) && !filter.TestName.Contains(target.Year)))
            {
                return source;
            }

            if ((string.IsNullOrEmpty(source.Year) && !string.IsNullOrEmpty(target.Year))
                || (!string.IsNullOrEmpty(source.Year) && !string.IsNullOrEmpty(target.Year) && filter.TestName.Contains(target.Year) && !filter.TestName.Contains(source.Year)))
            {
                return target;
            }

            if ((!string.IsNullOrEmpty(source.Season) && string.IsNullOrEmpty(target.Season))
                ||(!string.IsNullOrEmpty(source.Season) && !string.IsNullOrEmpty(target.Season) && filter.TestName.Contains(source.Season.ToLower()) && !filter.TestName.Contains(target.Season.ToLower())))
            {
                return source;
            }

            if ((string.IsNullOrEmpty(source.Season) && !string.IsNullOrEmpty(target.Season))
                ||(!string.IsNullOrEmpty(source.Season) && !string.IsNullOrEmpty(target.Season) && filter.TestName.Contains(target.Season.ToLower()) && !filter.TestName.Contains(source.Season.ToLower())))
            {
                return target;
            }

            if (source?.Subject?.ToLower() == filter.SubjectName && target?.Subject?.ToLower() != filter.SubjectName)
            {
                return source;
            }

            if (target?.Subject?.ToLower() == filter.SubjectName && source?.Subject?.ToLower() != filter.SubjectName)
            {
                return target;
            }

            if (source?.Grade?.ToLower() == filter.GradeName && target?.Grade?.ToLower() != filter.GradeName)
            {
                return source;
            }

            if (target?.Grade?.ToLower() == filter.GradeName && source?.Grade?.ToLower() != filter.GradeName)
            {
                return target;
            }

            if ((!string.IsNullOrEmpty(source.Keyword) && string.IsNullOrEmpty(target.Keyword))
                || (!string.IsNullOrEmpty(source.Keyword) && !string.IsNullOrEmpty(target.Keyword) && Regex.IsMatch(filter.TestName, $@"\b{source?.Keyword?.ToLower()}\b", RegexOptions.IgnoreCase) && !Regex.IsMatch(filter.TestName, $@"\b{target?.Keyword?.ToLower()}\b", RegexOptions.IgnoreCase)))
            {
                return source;
            }

            if ((string.IsNullOrEmpty(source.Keyword) && !string.IsNullOrEmpty(target.Keyword))
                || (!string.IsNullOrEmpty(source.Keyword) && !string.IsNullOrEmpty(target.Keyword) && Regex.IsMatch(filter.TestName, $@"\b{target?.Keyword?.ToLower()}\b", RegexOptions.IgnoreCase) && !Regex.IsMatch(filter.TestName, $@"\b{source?.Keyword?.ToLower()}\b", RegexOptions.IgnoreCase)))
            {
                return target;
            }

            return source;
        }

        public IEnumerable<ApplySettingForPBSItemDto> RemoveSettingForPBS(ApplySettingForPBSPayload payload)
        {
            var virtualTestIDs = payload.VirtualTestIDs.Split(',')
                .Select(x => int.TryParse(x, out int virtualTestID) ? virtualTestID : default)
                .Where(x => x != default);

            var result = _context.RemovePerformanceBands(payload.DistrictID, virtualTestIDs.ToIntCommaSeparatedString());
            if (result != null)
            {
                var ids = result.ToList();
                return virtualTestIDs.Select(vt => new ApplySettingForPBSItemDto
                {
                    VirtualTestID = vt,
                    IsChange = ids.Any(x => x.VirtualTestID == vt),
                    PBSInEffect = string.Empty
                }).ToList();
            }
            return new List<ApplySettingForPBSItemDto>();
        }
    }
}
