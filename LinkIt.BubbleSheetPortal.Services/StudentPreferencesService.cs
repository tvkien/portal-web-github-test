using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs;
using LinkIt.BubbleSheetPortal.Data.Repositories.StudentPreferences;
using LinkIt.BubbleSheetPortal.Data.Entities;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.Enum;
using System.Linq;
using System;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;
using LinkIt.BubbleSheetPortal.Models.Old.StudentPreferenceDTOs;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Services.BusinessObjects;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentPreferencesService
    {
        private readonly IStudentPreferenceRepository _studentPreferenceRepository;
        private readonly VirtualTestService _virtualTestService;
        private readonly ITestResultRepository _testResultRepository;

        private readonly string[] _dependShowItemDetailChart = new string[] { "showCorrectAnswers", "showStudentAnswers", "showPointsPossible" };
        private readonly string[] _dependLaunchItemAnalysis = new string[] {
            "showQuestions",
            "showTimeSpent",
            "showTimeSpentClass",
            "showTimeSpentSchool",
            "showTimeSpentDistrict" ,
            "showItemDetailChart",
            "showCorrectAnswers",
            "showStudentAnswers",
            "showPointsPossible"
        };
        private readonly string[] _levelList = Enum.GetNames(typeof(StudentPreferencePriorityEnum)).ToArray();

        private readonly string[] _dataLockerTestOptions = new string[] {
                "showTest", "visibilityInTestSpecific", "showNotes", "showArtifacts"
            };

        private Dictionary<string, int> columnTypeDictionary = new Dictionary<string, int>()
        {
            { "default", 1 },
            { "testtype", 2 },
            { "specific", 3 }
        };

        public OptionMatrix GetFullOptionIncludeDependency(string level, int districtID, int schoolID, int userID, int dataSetCategoryID, int virtualTestID, string tabActive = "", int[] classIds = null)
        {
            if (virtualTestID > 0 && dataSetCategoryID == 0)
            {
                dataSetCategoryID = _virtualTestService.GetDataSetCategoryIDByVirtualTestID(virtualTestID);
            }

            var optionMatrixType = typeof(OptionMatrix);
            var currentLevel = DetectCurrentLevel(level, virtualTestID, dataSetCategoryID);
            var position = Array.IndexOf(_levelList, currentLevel);
            var matrix = new OptionMatrix(level, districtID, schoolID, userID, dataSetCategoryID, virtualTestID);
            matrix.Position = position;

            var columnType = 1;
            if (columnTypeDictionary.TryGetValue(currentLevel.Split('_')[0], out columnType))
            {
                matrix.ColumnType = columnType;
            }

            // default_enterprise
            SetDataDefaultEnterprise(position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.Default_Enterprise)));

            // specific_enterprise
            SetDataSpecificEnterprise(virtualTestID, position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.Specific_Enterprise)));

            // default_district
            SetDataDefaultDistrict(districtID, position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.Default_District)));

            // testtype_district
            SetDataTesttypeDistrict(districtID, dataSetCategoryID, position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.TestType_District)));

            // specific_district
            SetDataSpecificDistrict(districtID, virtualTestID, position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.Specific_District)));


            var schoolIds = new int[] { schoolID };
            if (string.Compare(level, "class", true) == 0)
            {

                schoolIds = _testResultRepository
                     .Select()
                     .Where(c => c.VirtualTestId == virtualTestID && classIds.Contains(c.ClassId))
                     .Select(c => c.SchoolId)
                     .Distinct()
                     .ToArray();
            }

            // default_school
            SetDataDefaultSchool(schoolIds, position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.Default_School)));

            // testtype_school
            SetDataTesttypeSchool(schoolIds, dataSetCategoryID, position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.TestType_School)));

            // specific_school
            SetDataSpecificSchool(schoolIds, virtualTestID, position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.Specific_School)));

            // specific_classes
            SetDataSpecificClasses(classIds, virtualTestID, position, matrix, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.Specific_Class)));

            var isInherictView = "1";
            matrix.Final_Option = CalcualteFinalOption(matrix.Final_Option, position, isInherictView == "1", tabActive);

            matrix.Final_Option = CheckDependenceByPref(matrix.Final_Option);

            AddToMatrix(matrix.Final_Option, matrix.StudentPreferenceMatrix, optionMatrixType.GetDisplayName(nameof(matrix.Final_Option)));
            matrix.StudentPreferenceMatrix.RemoveAll(x => x.Name == StudentPreferenceDto.VisibilityInTestSpecific);
            return matrix;
        }

    

        private void SetDataDefaultEnterprise(int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > -1)
            {
                var pref = _studentPreferenceRepository.GetByLevel(PreferenceLevel.ENTERPRISE, 0, null, null);
                if (pref != null)
                {
                    matrix.Default_Enterprise = pref.StudentPreferenceDetailEntities
                                                    .Where(x => StudentPreferenceDto.ListName
                                                    .Contains(x.Name))
                                                    .Select(m =>
                                                    {
                                                        var item = Mapper.Map<StudentPreferenceDetailDto>(m);
                                                        if (item.Name == StudentPreferenceDto.ShowTest)
                                                            item.IsNotShow = true;
                                                        return item;
                                                    })
                                                    .ToList();
                    matrix.Default_Enterprise = SetPriorityPref(matrix.Default_Enterprise, (int)StudentPreferencePriorityEnum.default_enterprise);
                    matrix.Final_Option.AddRange(matrix.Default_Enterprise);
                }
                else
                {
                    matrix.Default_Enterprise = new List<StudentPreferenceDetailDto>();
                }

                matrix.Default_Enterprise = InitOffIfMissing(matrix.Default_Enterprise);
                AddToMatrix(matrix.Default_Enterprise, tableMatrix, displayName);
            }
        }

        public IEnumerable<ClassDetailDto> GetAssociatedClassesThatHasTestResult(int userId, int? districtId, int roleId)
        {
            return this._studentPreferenceRepository.GetAssociatedClassesThatHasTestResult(userId: userId, districtId: districtId, schoolId: 0, roleId: roleId)
                .Select(c => new ClassDetailDto()
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName
                }).ToArray();
        }

        private void SetDataSpecificEnterprise(int? virtualTestID, int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > (int)StudentPreferencePriorityEnum.default_enterprise && matrix.ColumnType > 2)
            {
                var pref = _studentPreferenceRepository.GetByLevel(PreferenceLevel.ENTERPRISE, 0, virtualTestID, null);
                if (pref != null)
                {
                    matrix.Specific_Enterprise = pref.StudentPreferenceDetailEntities.Where(x => StudentPreferenceDto.ListName.Contains(x.Name)).Select(m => Mapper.Map<StudentPreferenceDetailDto>(m))
                        .ToList();
                    matrix.Specific_Enterprise = SetPriorityPref(matrix.Specific_Enterprise, (int)StudentPreferencePriorityEnum.specific_enterprise);
                    matrix.Final_Option.AddRange(matrix.Specific_Enterprise);
                }
                else
                {
                    matrix.Specific_Enterprise = new List<StudentPreferenceDetailDto>();
                }

                matrix.Specific_Enterprise = InitOffIfMissing(matrix.Specific_Enterprise);
                AddToMatrix(matrix.Specific_Enterprise, tableMatrix, displayName);
            }
        }

        private void SetDataDefaultDistrict(int districtID, int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > (int)StudentPreferencePriorityEnum.specific_enterprise)
            {
                var pref = _studentPreferenceRepository.GetByLevel(PreferenceLevel.DISTRICT, districtID, null, null);
                if (pref != null)
                {
                    matrix.Default_District = pref.StudentPreferenceDetailEntities
                                                    .Where(x => StudentPreferenceDto.ListName
                                                    .Contains(x.Name))
                                                    .Select(m =>
                                                    {
                                                        var item = Mapper.Map<StudentPreferenceDetailDto>(m);
                                                        if (item.Name == StudentPreferenceDto.ShowTest)
                                                            item.IsNotShow = true;
                                                        return item;
                                                    })
                                                    .ToList();
                    matrix.Default_District = SetPriorityPref(matrix.Default_District, (int)StudentPreferencePriorityEnum.default_district);
                    matrix.Final_Option.AddRange(matrix.Default_District);
                }
                else
                {
                    matrix.Default_District = new List<StudentPreferenceDetailDto>()
                    {
                        new StudentPreferenceDetailDto { Name = StudentPreferenceDto.ShowTest, IsNotShow = true}
                    };
                }

                matrix.Default_District = InitOffIfMissing(matrix.Default_District, true);
                AddToMatrix(matrix.Default_District, tableMatrix, displayName);
            }
        }

        private void SetDataTesttypeDistrict(int districtID, int? testTypeID, int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > (int)StudentPreferencePriorityEnum.default_district && matrix.ColumnType > 1)
            {
                var pref = _studentPreferenceRepository.GetByLevel(PreferenceLevel.DISTRICT, districtID, null, testTypeID);
                if (pref != null)
                {
                    matrix.TestType_District = pref.StudentPreferenceDetailEntities
                                                    .Where(x => StudentPreferenceDto.ListName
                                                    .Contains(x.Name))
                                                    .Select(m =>
                                                    {
                                                        var item = Mapper.Map<StudentPreferenceDetailDto>(m);
                                                        if (item.Name == StudentPreferenceDto.ShowTest)
                                                            item.IsNotShow = true;
                                                        return item;
                                                    })
                                                    .ToList();

                    matrix.TestType_District = SetPriorityPref(matrix.TestType_District, (int)StudentPreferencePriorityEnum.testtype_district);
                    matrix.Final_Option.AddRange(matrix.TestType_District);
                }
                else
                {
                    matrix.TestType_District = new List<StudentPreferenceDetailDto>();
                }

                matrix.TestType_District = InitOffIfMissing(matrix.TestType_District, true);
                AddToMatrix(matrix.TestType_District, tableMatrix, displayName);
            }
        }

        private void SetDataSpecificDistrict(int districtID, int? virtualTestID, int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > (int)StudentPreferencePriorityEnum.testtype_district && matrix.ColumnType > 2)
            {
                var pref = _studentPreferenceRepository.GetByLevel(PreferenceLevel.DISTRICT, districtID, virtualTestID, null);
                if (pref != null)
                {
                    matrix.Specific_District = pref.StudentPreferenceDetailEntities.Where(x => StudentPreferenceDto.ListName.Contains(x.Name)).Select(m => Mapper.Map<StudentPreferenceDetailDto>(m))
                        .ToList();
                    matrix.Specific_District = SetPriorityPref(matrix.Specific_District, (int)StudentPreferencePriorityEnum.specific_district);
                    matrix.Final_Option.AddRange(matrix.Specific_District);
                }
                else
                {
                    matrix.Specific_District = new List<StudentPreferenceDetailDto>();
                }

                matrix.Specific_District = InitOffIfMissing(matrix.Specific_District);
                AddToMatrix(matrix.Specific_District, tableMatrix, displayName);
            }
        }

        private void SetDataDefaultSchool(int[] schoolIds, int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > (int)StudentPreferencePriorityEnum.specific_district)
            {
                var preferences = _studentPreferenceRepository.GetByLevelIds(PreferenceLevel.SCHOOL, schoolIds, 0, 0)
                    .ToArray()
                    ;
                if (preferences != null)
                {
                    var concatenatedDetail = preferences
                       .SelectMany(c => c.StudentPreferenceDetailEntities.Where(x => StudentPreferenceDto.ListName.Contains(x.Name)).ToArray())
                       .ToArray()
                       .GroupBy(c => c.Name)
                       .Select(groupedItems =>
                       {
                           var item = Mapper.Map<StudentPreferenceDetailDto>(groupedItems.First());
                           if (IsPreferencesConflict(groupedItems))
                           {
                               item.IsConflict = true;
                           }
                           if (item.Name == StudentPreferenceDto.ShowTest)
                               item.IsNotShow = true;
                           return item;
                       }).ToList();

                    matrix.Default_School = concatenatedDetail;
                    matrix.Default_School = SetPriorityPref(matrix.Default_School, (int)StudentPreferencePriorityEnum.default_school);
                    matrix.Final_Option.AddRange(matrix.Default_School);
                }
                else
                {
                    matrix.Default_School = new List<StudentPreferenceDetailDto>()
                    {
                        new StudentPreferenceDetailDto { Name = StudentPreferenceDto.ShowTest, IsNotShow = true}
                    };
                }

                matrix.Default_School = InitOffIfMissing(matrix.Default_School, true);
                AddToMatrix(matrix.Default_School, tableMatrix, displayName);
            }
        }

        private void SetDataTesttypeSchool(int[] schoolIds, int testTypeID, int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > (int)StudentPreferencePriorityEnum.default_school && matrix.ColumnType > 1)
            {
                var preferences = _studentPreferenceRepository.GetByLevelIds(PreferenceLevel.SCHOOL, schoolIds, 0, testTypeID);
                if (preferences != null)
                {
                    var concatenatedDetail = preferences
                     .SelectMany(c => c.StudentPreferenceDetailEntities.Where(x => StudentPreferenceDto.ListName.Contains(x.Name)).ToArray())
                     .ToArray()
                     .GroupBy(c => c.Name)
                     .Select(groupedItems =>
                     {
                         var item = Mapper.Map<StudentPreferenceDetailDto>(groupedItems.First());
                         if (IsPreferencesConflict(groupedItems))
                         {
                             item.IsConflict = true;
                         }
                         if (item.Name == StudentPreferenceDto.ShowTest)
                             item.IsNotShow = true;
                         return item;
                     }).ToList();

                    matrix.TestType_School = concatenatedDetail;
                    matrix.TestType_School = SetPriorityPref(matrix.TestType_School, (int)StudentPreferencePriorityEnum.testtype_school);
                    matrix.Final_Option.AddRange(matrix.TestType_School);
                }
                else
                {
                    matrix.TestType_School = new List<StudentPreferenceDetailDto>();
                }

                matrix.TestType_School = InitOffIfMissing(matrix.TestType_School, true);
                AddToMatrix(matrix.TestType_School, tableMatrix, displayName);
            }
        }

        private void SetDataSpecificSchool(int[] schoolIds, int virtualTestID, int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > (int)StudentPreferencePriorityEnum.testtype_school && matrix.ColumnType > 2)
            {
                var preferences = _studentPreferenceRepository.GetByLevelIds(PreferenceLevel.SCHOOL, schoolIds, virtualTestID, 0);
                if (preferences != null)
                {
                    var concatenatedDetail = preferences
                     .SelectMany(c => c.StudentPreferenceDetailEntities.Where(x => StudentPreferenceDto.ListName.Contains(x.Name)).ToArray())
                     .ToArray()
                     .GroupBy(c => c.Name)
                     .Select(groupedItems =>
                     {
                         var item = Mapper.Map<StudentPreferenceDetailDto>(groupedItems.First());
                         if (IsPreferencesConflict(groupedItems))
                         {
                             item.IsConflict = true;
                         }
                         return item;
                     }).ToList();

                    matrix.Specific_School = concatenatedDetail;
                    matrix.Specific_School = SetPriorityPref(matrix.Specific_School, (int)StudentPreferencePriorityEnum.specific_school);
                    matrix.Final_Option.AddRange(matrix.Specific_School);
                }
                else
                {
                    matrix.Specific_School = new List<StudentPreferenceDetailDto>();
                }

                matrix.Specific_School = InitOffIfMissing(matrix.Specific_School);
                AddToMatrix(matrix.Specific_School, tableMatrix, displayName);
            }
        }

        private void SetDataSpecificClasses(int[] classIds, int virtualTestID, int position, OptionMatrix matrix, List<StudentPreferenceMatrix> tableMatrix, string displayName)
        {
            if (position > (int)StudentPreferencePriorityEnum.specific_school && matrix.ColumnType > 2 && classIds?.Length > 0)
            {
                var preferences = _studentPreferenceRepository.GetByLevelIds(PreferenceLevel.CLASS, classIds, virtualTestID, 0).ToArray();
                if (preferences != null)
                {
                    var concatenatedDetail = preferences
                        .SelectMany(c => c.StudentPreferenceDetailEntities.Where(x => StudentPreferenceDto.ListName.Contains(x.Name)).ToArray())
                        .ToArray()
                        .GroupBy(c => c.Name)
                        .Select(groupedItems =>
                        {
                            var item = Mapper.Map<StudentPreferenceDetailDto>(groupedItems.First());
                            item.Locked = false;
                            if (IsPreferencesConflict(groupedItems))
                            {
                                item.IsConflict = true;
                            }
                            return item;
                        }).ToList();
                    matrix.Specific_Class = concatenatedDetail;
                    matrix.Specific_Class = SetPriorityPref(matrix.Specific_Class, (int)StudentPreferencePriorityEnum.specific_class);
                    matrix.Final_Option.AddRange(matrix.Specific_Class);
                }
                else
                {
                    matrix.Specific_Class = new List<StudentPreferenceDetailDto>();
                }

                matrix.Specific_Class = InitOffIfMissing(matrix.Specific_Class);
                AddToMatrix(matrix.Specific_Class, tableMatrix, displayName);
            }
        }

        private bool IsPreferencesConflict(IGrouping<string, StudentPreferenceDetailEntity> items)
        {
            return items.GroupBy(c => $"{c.Value}_{c.Locked}").Count() > 1;
        }

        private void AddToMatrix(List<StudentPreferenceDetailDto> studentPreferences, List<StudentPreferenceMatrix> tableMatrix, string levelName)
        {
            studentPreferences.ForEach(x =>
            {
                var studentOption = StudentPreferenceDto.StudentOptions.FirstOrDefault(y => y.Name == x.Name);
                tableMatrix.Add(new StudentPreferenceMatrix
                {
                    LevelName = levelName,
                    Name = x.Name,
                    DisplayName = studentOption.DisplayName,
                    ClassStyle = studentOption.ClassStyle,
                    Value = x.Value,
                    Locked = x.Locked,
                    IsDisabled = x.IsDisabled,
                    IsDisabledByType = x.IsDisabledByType,
                    Priority = x.Priority,
                    IsMissing = x.IsMissing,
                    IsNotShow = x.IsNotShow,
                    IsConflict = x.IsConflict,
                    Order = studentOption.Order
                });
            });
        }

        public IEnumerable<AvailableTestTypeGradeAndSubject> GetAvailableTestTypeGradeAndSubjectForStudentPreference(SearchBankCriteria criteria)
        {
            return _studentPreferenceRepository.GetAvailableTestTypeGradeAndSubjectForStudentPreference(criteria)
                .ToArray()
                .Select(c => new AvailableTestTypeGradeAndSubject()
                {
                    Kind = c.Kind,
                    Id = c.Id,
                    Name = c.Name,
                    Order = c.Order,
                    IsShow = c.IsShow.GetValueOrDefault(),
                    Tooltip = c.Tooltip
                }).ToArray();
        }

        private List<StudentPreferenceDetailDto> SetPriorityPref(List<StudentPreferenceDetailDto> details, int priority)
        {
            foreach (var detail in details)
            {
                detail.Priority = priority;
            }
            return details;
        }

        private List<StudentPreferenceDetailDto> InitOffIfMissing(List<StudentPreferenceDetailDto> details, bool hiddenShowTest = false)
        {
            foreach (var name in StudentPreferenceDto.ListName)
            {
                if (!details.Exists(m => m.Name == name))
                {
                    details.Add(new StudentPreferenceDetailDto
                    {
                        Name = name,
                        Value = false,
                        Locked = false,
                        IsMissing = !hiddenShowTest,
                        IsNotShow = (name == StudentPreferenceDto.ShowTest && hiddenShowTest)
                    });
                }
            }

            details = details.OrderBy(d => StudentPreferenceDto.ListName.IndexOf(d.Name)).ToList();
            return details;
        }

        private List<StudentPreferenceDetailDto> CalcualteFinalOption(List<StudentPreferenceDetailDto> details, int position, bool isInheric, string tabActive)
        {
            var final = new List<StudentPreferenceDetailDto>();
            var allPrefs = details.Where(m => !m.IsMissing).ToList();

            if (isInheric)
            {
                var subfixLevel = _levelList[position].Split('_')[1];
                final = GetInhericPrefSameLevel(allPrefs, subfixLevel);

                foreach (var item in final)
                {

                    if (item.Priority < position && !_levelList[item.Priority].Contains(subfixLevel))
                    {
                        item.IsDisabled = true;
                        item.IsDisabledByType = true;
                    }
                }

                var existedNames = final.Select(m => m.Name).ToList();
                var missingPref = StudentPreferenceDto.ListName.Where(m => !existedNames.Contains(m)).ToList();

                // get inheric
                var inhericObjs = allPrefs.Where(m => !m.Locked && missingPref.Contains(m.Name) && m.Priority == position && !m.IsNotShow)
                    .OrderByDescending(m => m.Priority)
                    .GroupBy(m => m.Name)
                    .Select(m => m.First())
                    .ToList();

                if (inhericObjs.Count == 0)
                {
                    inhericObjs = allPrefs.Where(m => !m.Locked && missingPref.Contains(m.Name) && !m.IsNotShow)
                    .OrderByDescending(m => m.Priority)
                    .GroupBy(m => m.Name)
                    .Select(m => m.First())
                    .ToList();
                }

                final.AddRange(inhericObjs);
            }
            else
            {
                final = details.Where(m => m.Priority == position).ToList();
            }

            final = InitOffIfMissing(final);
            if (tabActive == ((int)StudentPreferenceTabEnum.DefaultOption).ToString())
                final.FirstOrDefault(x => x.Name == StudentPreferenceDto.ShowTest).IsMissing = true;
            return final;
        }

        private static List<StudentPreferenceDetailDto> GetInhericPrefSameLevel(List<StudentPreferenceDetailDto> allPrefs, string level)
        {
            var groupEnterprise = allPrefs.Where(m => (m.Priority == (int)StudentPreferencePriorityEnum.default_enterprise
                                                    || m.Priority == (int)StudentPreferencePriorityEnum.specific_enterprise)
                                                    && (level == PreferenceLevel.ENTERPRISE || m.Locked) && !m.IsNotShow)
                                            .OrderByDescending(m => m.Priority)
                                            .GroupBy(m => m.Name)
                                            .Select(m => m.First())
                                            .ToList();
            var groupDistrict = allPrefs.Where(m => (m.Priority == (int)StudentPreferencePriorityEnum.default_district
                                                || m.Priority == (int)StudentPreferencePriorityEnum.testtype_district
                                                || m.Priority == (int)StudentPreferencePriorityEnum.specific_district)
                                                && (level == PreferenceLevel.DISTRICT || m.Locked) && !m.IsNotShow)
                                        .OrderByDescending(m => m.Priority)
                                        .GroupBy(m => m.Name)
                                        .Select(m => m.First())
                                        .ToList();
            var groupSchool = allPrefs.Where(m => (m.Priority == (int)StudentPreferencePriorityEnum.default_school
                                                || m.Priority == (int)StudentPreferencePriorityEnum.testtype_school
                                                || m.Priority == (int)StudentPreferencePriorityEnum.specific_school)
                                                && (level == PreferenceLevel.SCHOOL || m.Locked) && !m.IsNotShow)
                                        .OrderByDescending(m => m.Priority)
                                        .GroupBy(m => m.Name)
                                        .Select(m => m.First())
                                        .ToList();
            var groupClasses = allPrefs.Where(m => (m.Priority == (int)StudentPreferencePriorityEnum.specific_class)
                                            && (level == PreferenceLevel.CLASS || m.Locked) && !m.IsNotShow)
                                    .OrderByDescending(m => m.Priority)
                                    .GroupBy(m => m.Name)
                                    .Select(m => m.First())
                                    .ToList();
            var final = groupEnterprise.Concat(groupDistrict)
                                        .Concat(groupSchool)
                                        .Concat(groupClasses)
                                        .OrderBy(m => m.Priority)
                                        .GroupBy(m => m.Name)
                                        .Select(m => m.First())
                                        .ToList();

            return final;
        }

        public StudentPreferencesService(IStudentPreferenceRepository repository, VirtualTestService virtualTestService, ITestResultRepository testResultRepository)
        {
            _studentPreferenceRepository = repository;
            _virtualTestService = virtualTestService;
            _testResultRepository = testResultRepository;
        }

        public TestForStudentPreferenceResponseDto GetTestForStudentPreferences(StudentPreferenceRequestDto criteria)
        {
            return _studentPreferenceRepository.GetTestForStudentPreferences(criteria);
        }

        public IList<TestTypeDto> GetListTestType(int districtID, int userID, int roleID, int schoolID = 0)
        {
            return _studentPreferenceRepository.GetListTestType(districtID, userID, roleID, schoolID);
        }

        public void SetDefaultOption(StudentPreferenceDto model, int userId, int? districtId, int roleId)
        {
            var virtualTestIDs = !string.IsNullOrEmpty(model.VirtualTestIDs) ?
                 model.VirtualTestIDs.Split(',').Select(c =>
                 {
                     int.TryParse(c, out int _val); return _val;
                 }).ToArray() : new int[] { model.VirtualTestID.GetValueOrDefault() };
            List<StudentPreferenceDto> models = new List<StudentPreferenceDto>();
            foreach (var virtualTestId in virtualTestIDs)
            {
                if (virtualTestId == 0)
                    model.VirtualTestID = null;
                else
                    model.VirtualTestID = virtualTestId;

                if (model.Level == PreferenceLevel.CLASS)
                {
                    var classIdsThatHasTestResult = _studentPreferenceRepository.GetAssociatedClassesThatHasTestResult
                        (userId, districtId.GetValueOrDefault(), schoolId: 0, roleId, model.VirtualTestID).Select(c => c.ClassId).ToArray();

                    classIdsThatHasTestResult = classIdsThatHasTestResult.Where(c => model.ClassIds.Contains(c)).ToArray();

                    foreach (var classId in classIdsThatHasTestResult)
                    {
                        model.LevelID = classId;
                        models.Add(model.Clone());
                    }
                }
                else
                {
                    models.Add(model.Clone());
                }
            }

            _studentPreferenceRepository.SetStudentsPreference(models);
        }
        public StudentPreferenceDto GetStudentPreferences(string level, int levelID, int dataSetCategoryID, int schoolID, int districtID, string virtualTestIds, int userId, string classIds = "")
        {
            var virtualTestID = 0;
            var isGeneric = false;
            StudentPreferenceDto result = null;
            List<StudentPreferenceDetailDto> details = null;

            if (!string.IsNullOrEmpty(virtualTestIds))
            {
                var testIds = virtualTestIds.Split(',').ToList();

                if (testIds.Count == 1)
                {
                    virtualTestID = int.Parse(testIds[0]);
                    dataSetCategoryID = _virtualTestService.GetDataSetCategoryIDByVirtualTestID(virtualTestID);
                }
                else
                {
                    isGeneric = true;
                }
            }

            if (!isGeneric)
            {
                var preference = _studentPreferenceRepository.GetByLevel(level, levelID, virtualTestID, dataSetCategoryID);

                var classIdsAsIntArray = (classIds ?? "").ToIntArray(",");

                if (preference == null)
                {
                    preference = new StudentPreferencesEntity();
                    preference.Level = level;
                    preference.LevelID = levelID;
                    preference.VirtualTestID = virtualTestID;
                    preference.TestTypeID = dataSetCategoryID;
                }

                result = Mapper.Map<StudentPreferenceDto>(preference);
                result.ClassIds = classIdsAsIntArray;

                var matrix = GetFullOptionIncludeDependency(level, districtID, schoolID, userId, dataSetCategoryID, virtualTestID, string.Empty, classIdsAsIntArray);
                details = matrix.Final_Option;

            }
            else
            {
                var preference = new StudentPreferencesEntity
                {
                    Level = level,
                    LevelID = levelID,
                    VirtualTestID = virtualTestID,
                    TestTypeID = dataSetCategoryID
                };
                result = Mapper.Map<StudentPreferenceDto>(preference);

                InitDefaultPreferenceForMultipleTest(out details);
                details = CheckDependenceByPref(details);
                details = CheckDependenceByTestType(details, virtualTestID);
            }

            result.Details = details;

            result.GroupDetails = PushPreferenceItemToGroup(details);

            return result;
        }
        public List<ListSubjectItem> GetSubjectByUserId(int districtId, int userId, int roleId, int schoolId)
        {
            var subjects = _studentPreferenceRepository.GetSubjectGradeByUserID(districtId, userId, roleId, schoolId)
                .Select(m => new ListSubjectItem
                {
                    Id = m.SubjectName,
                    Name = m.SubjectName
                })
                .GroupBy(p => p.Id).Select(m => m.First())
                .ToList();

            return subjects;
        }

        public List<ListItem> GetGradeByUserId(int districtId, int userId, int roleId, int schoolId)
        {
            var grades = _studentPreferenceRepository.GetSubjectGradeByUserID(districtId, userId, roleId, schoolId)
                .OrderBy(m => m.GradeOrder)
                .Select(m => new ListItem
                {
                    Id = m.GradeID,
                    Name = m.GradeName
                })
                .GroupBy(p => p.Id).Select(m => m.First())
                .ToList();

            return grades;
        }

        private static void InitDefaultPreferenceForMultipleTest(out List<StudentPreferenceDetailDto> details)
        {
            details = new List<StudentPreferenceDetailDto>();
            foreach (var name in StudentPreferenceDto.ListName)
            {
                details.Add(new StudentPreferenceDetailDto
                {
                    Name = name,
                    Value = false,
                    Locked = false
                });
            }
        }

        private List<StudentPreferenceDetailDto> CheckDependenceByPref(List<StudentPreferenceDetailDto> details)
        {
            if (details == null)
                return new List<StudentPreferenceDetailDto>();
            var deserializeSettings = new Newtonsoft.Json.JsonSerializerSettings { ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace };
            details = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StudentPreferenceDetailDto>>(Newtonsoft.Json.JsonConvert.SerializeObject(details), deserializeSettings);

            if (details.Any(m => m.Name == StudentPreferenceDto.ShowItemDetailChart && !m.Value))
            {
                details.Where(o => _dependShowItemDetailChart.Contains(o.Name))
                    .ToList()
                    .ForEach(x =>
                {
                    if (!x.Locked)
                    {
                        x.Value = false;
                    }
                    x.IsDisabled = true;
                });
            }
            if (details.Any(m => m.Name == StudentPreferenceDto.LaunchItemAnalysis && !m.Value))
            {
                details.Where(o => _dependLaunchItemAnalysis.Contains(o.Name))
                    .ToList()
                    .ForEach(x =>
                    {
                        if (!x.Locked)
                        {
                            x.Value = false;
                        }
                        x.IsDisabled = true;
                    });
            }
            return details;
        }

        private List<StudentPreferenceDetailDto> CheckDependenceByTestType(List<StudentPreferenceDetailDto> details, int virtualTestID, int testTypeId = -1)
        {
            if (details == null)
                return new List<StudentPreferenceDetailDto>();

            var deserializeSettings = new Newtonsoft.Json.JsonSerializerSettings { ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace };
            details = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StudentPreferenceDetailDto>>(Newtonsoft.Json.JsonConvert.SerializeObject(details), deserializeSettings);

            var type = _virtualTestService.GetVirtualTestTypeById(virtualTestID);

            if (type == VirtualTestTypeEnum.DataLocker)
            {
                // non-release details.RemoveRange(3, details.Count - 3);
                details.RemoveAll(m => !_dataLockerTestOptions.Contains(m.Name));
            }
            else if (type == VirtualTestTypeEnum.NonLinkit || testTypeId > 0)
            {
                details.RemoveRange(2, details.Count - 2);
            }
            return details;
        }

        private static string DetectCurrentLevel(string level, int virtualTestID, int testTypeID)
        {
            string currentLevel;
            if (virtualTestID > 0)
            {
                currentLevel = $"specific_{level}";
            }
            else if (testTypeID > 0 || testTypeID == -1)
            {
                currentLevel = $"testtype_{level}";
            }
            else
            {
                currentLevel = $"default_{level}";
            }

            return currentLevel;
        }

        private List<StudentPreferenceGroupDetailDto> PushPreferenceItemToGroup(List<StudentPreferenceDetailDto> studentPreferenceDetails)
        {
            studentPreferenceDetails = studentPreferenceDetails.OrderBy(d => StudentPreferenceDto.ListName.IndexOf(d.Name)).ToList();
            var group = new List<StudentPreferenceGroupDetailDto>();

            var skip = 0;
            var index = 0;
            foreach (var count in StudentPreferenceDto.TotalItemInGroups)
            {
                var itemsInGroup = studentPreferenceDetails.Skip(skip).Take(count).ToList();
                group.Add(new StudentPreferenceGroupDetailDto
                {
                    GroupName = StudentPreferenceDto.GroupNames[index],
                    List = itemsInGroup
                });

                skip += count;
                index++;
            }

            return group;
        }
    }
}
