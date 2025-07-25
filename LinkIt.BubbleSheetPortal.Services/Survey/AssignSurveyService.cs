using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.MessageQueue;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using LinkIt.BubbleSheetPortal.Services.CodeGen;
using LinkIt.BubbleSheetPortal.Services.CommonServices;
using LinkIt.BubbleSheetPortal.SimpleQueueService.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Services.Survey
{
    public class AssignSurveyService
    {
        private readonly ShortLinkService _shortLinkService;
        private readonly ConfigurationService _configurationService;
        private readonly SchoolService _schoolService;
        private readonly ClassService _classService;
        private readonly QTITestClassAssignmentService _qTITestClassAssignmentService;
        private readonly TestCodeGenerator _testCodeGenerator;
        private readonly PreferencesService _preferencesService;
        private readonly StudentService _studentService;
        private readonly TestResultService _testResultService;
        private readonly DistrictService _districtService;
        private readonly UserService _userService;

        private readonly LookupStudentRepository _lookupStudentRepository;
        private readonly IManageTestRepository _manageTestRepository;
        private readonly IMessageQueueService _messageQueueService;

        public AssignSurveyService(
            ShortLinkService shortLinkService,
            ConfigurationService configurationService,
            SchoolService schoolService,
            ClassService classService,
            QTITestClassAssignmentService qTITestClassAssignmentService,
            TestCodeGenerator testCodeGenerator,
            PreferencesService preferencesService,
            StudentService studentService,
            TestResultService testResultService,
            DistrictService districtService,
            UserService userService,
            LookupStudentRepository lookupStudentRepository,
            IManageTestRepository manageTestRepository,
            IMessageQueueService messageQueueService)
        {
            _shortLinkService = shortLinkService;
            _configurationService = configurationService;
            _schoolService = schoolService;
            _classService = classService;
            _qTITestClassAssignmentService = qTITestClassAssignmentService;
            _testCodeGenerator = testCodeGenerator;
            _preferencesService = preferencesService;
            _studentService = studentService;
            _testResultService = testResultService;
            _districtService = districtService;
            _userService = userService;
            _lookupStudentRepository = lookupStudentRepository;
            _manageTestRepository = manageTestRepository;
            _messageQueueService = messageQueueService;
        }

        public void AssignPublicAnonymous(SurveyAssignmentData data, string testCodePrefix)
        {
            var allowAssign = _qTITestClassAssignmentService.CheckAllowAssignSurvey(data.DistrictId, data.DistrictTermId, data.TestId, (int)SurveyAssignmentTypeEnum.PublicAnonymous);
            if (allowAssign)
            {
                DateTime currentDateTime = DateTime.UtcNow;
                var schoolClass = _schoolService.GetSurveySchoolClass(data.DistrictId, data.DistrictTermId, data.TestName, data.UserId);
                var codeLength = GetTestCodeLength();
                var code = _testCodeGenerator.GenerateTestCode(codeLength, testCodePrefix);
                var qtiTest = new QTITestClassAssignmentData()
                {
                    VirtualTestId = data.TestId,
                    AssignmentDate = currentDateTime,
                    Code = code,
                    CodeTimestamp = currentDateTime,
                    AssignmentGuId = Guid.NewGuid().ToString(),
                    TestSetting = string.Empty,
                    Status = 1,
                    ModifiedDate = currentDateTime,
                    ModifiedUserId = data.UserId,
                    Type = (int)AssignmentType.Roster,
                    ModifiedBy = Constanst.PortalContain,
                    DistrictID = data.DistrictId,
                    ClassId = schoolClass.Id2,
                    SurveyAssignmentType = (int)SurveyAssignmentTypeEnum.PublicAnonymous
                };
                _qTITestClassAssignmentService.Save(qtiTest);

                _shortLinkService.GenerateSurveyTestTakerURL(qtiTest.QTITestClassAssignmentId, qtiTest.VirtualTestId, schoolClass.Id1, qtiTest.Code);

                var preferences = GetPreference(data.DistrictId);
                if (!string.IsNullOrEmpty(preferences) && qtiTest.QTITestClassAssignmentId > 0)
                {
                    _preferencesService.Save(new Preferences()
                    {
                        Id = qtiTest.QTITestClassAssignmentId,
                        Level = ContaintUtil.TestPreferenceLevelTestAssignment,
                        Label = ContaintUtil.TestPreferenceLabelTest,
                        Value = preferences
                    });
                }
            }
            else
            {
                throw new ArgumentException("There should only be one active Public Anonymous Assignment link for each Survey on a specific Term.");
            }
        }

        public List<CheckMatchEmailDto> CheckMatchEmails(string emails, int districtId, int surveyId, int termId, int assignmentType)
        {
            var result = new List<CheckMatchEmailDto>();
            if (!string.IsNullOrEmpty(emails))
            {
                foreach (var email in emails.Split(','))
                {
                    var matchEmailDto = _qTITestClassAssignmentService.CheckMatchEmail(email, districtId, surveyId, termId, assignmentType);
                    result.Add(matchEmailDto);
                }
            }

            return result;
        }

        public void AssignExtraCodePublicIndividualized(SurveyAssignmentData data, string testCodePrefix)
        {
            if (data.NumberOfCode > 0)
            {
                var schoolClass = _schoolService.GetSurveySchoolClass(data.DistrictId, data.DistrictTermId, data.TestName, data.UserId);
                var codeLength = GetTestCodeLength();
                var assignCodeFullLinks = new List<AssignCodeFullLink>();
                for (int i = 0; i < data.NumberOfCode; i++)
                {
                    var code = GenerateClassTestCode(testCodePrefix, codeLength, assignCodeFullLinks);
                    var shortLinkDto = _shortLinkService.GenerateSurveyKeyAndFullLink(data.TestId, schoolClass.Id1, code);
                    var assignCodeFullLink = new AssignCodeFullLink()
                    {
                        Code = code,
                        FullLink = shortLinkDto.FullLink,
                        ShortLinkKey = shortLinkDto.Code
                    };
                    assignCodeFullLinks.Add(assignCodeFullLink);
                }

                var preferences = GetPreference(data.DistrictId);
                var param = new SurveyAssignParameter()
                {
                    ClassId = schoolClass.Id2,
                    UserId = data.UserId,
                    SurveyId = data.TestId,
                    DistrictId = data.DistrictId,
                    AssignCodeFullLinks = assignCodeFullLinks,
                    Preferences = preferences
                };

                _qTITestClassAssignmentService.BatchSaveAssignmentPublicIndividualized(param);
            }
        }
        public void AssignAndDistributePublicIndividualized(SurveyAssignmentData data, DistributeSetting distributeSetting)
        {
            if (data.Emails.Count > 0)
            {
                var schoolClass = _schoolService.GetSurveySchoolClass(data.DistrictId, data.DistrictTermId, data.TestName, data.UserId);
                var portalUrl = _configurationService.GetConfigurationByKey(Constanst.DEFAULTSURVEYURL);
                var codeLength = GetTestCodeLength();
                var assignCodeFullLinks = new List<AssignCodeFullLink>();
                var distributeInfos = new List<DistributeInformation>();
                foreach (var email in data.Emails)
                {
                    var code = GenerateClassTestCode(distributeSetting.TestCodePrefix, codeLength, assignCodeFullLinks);
                    var shortLinkDto = _shortLinkService.GenerateSurveyKeyAndFullLink(data.TestId, schoolClass.Id1, code);
                    var assignCodeFullLink = new AssignCodeFullLink()
                    {
                        Code = code,
                        FullLink = shortLinkDto.FullLink,
                        ShortLinkKey = shortLinkDto.Code,
                        Email = email.ToLower()
                    };
                    assignCodeFullLinks.Add(assignCodeFullLink);

                    distributeInfos.Add(new DistributeInformation()
                    {
                        Email = email,
                        SurveyLink = $"{portalUrl.Value}?{assignCodeFullLink.ShortLinkKey}"
                    });
                }

                var preferences = GetPreference(data.DistrictId);
                var param = new SurveyAssignParameter()
                {
                    ClassId = schoolClass.Id2,
                    UserId = data.UserId,
                    SurveyId = data.TestId,
                    DistrictId = data.DistrictId,
                    AssignCodeFullLinks = assignCodeFullLinks,
                    Preferences = preferences
                };
                _qTITestClassAssignmentService.BatchSaveAssignmentPublicIndividualized(param);

                var surveyDistribute = new SurveyDistributeNotifyDto()
                {
                    DistributeInformations = distributeInfos,
                    DistrictId = data.DistrictId,
                    SurveyId = data.TestId,
                    SurveyName = data.TestName,
                    PublishedBy = distributeSetting.DistributedBy,
                    SurveyAssignmentType = data.SurveyAssignmentType
                };
                DistributeAssignment(surveyDistribute);
            }
        }
        

        public List<AssignResult> AssignPrivate(SurveyAssignmentData data, string testCodePrefix)
        {
            if (data.AssignUserIds.Count > 0)
            {
                var schoolClass = _schoolService.GetSurveySchoolClass(data.DistrictId, data.DistrictTermId, data.TestName, data.UserId);
                var codeLength = GetTestCodeLength();
                var assignCodeFullLinks = new List<AssignCodeFullLink>();
                foreach (var assUserId in data.AssignUserIds)
                {
                    var code = GenerateClassTestCode(testCodePrefix, codeLength, assignCodeFullLinks);
                    var shortLinkDto = _shortLinkService.GenerateSurveyKeyAndFullLink(data.TestId, schoolClass.Id1, code);
                    var assignCodeFullLink = new AssignCodeFullLink()
                    {
                        Code = code,
                        FullLink = shortLinkDto.FullLink,
                        ShortLinkKey = shortLinkDto.Code,
                        UserId = assUserId
                    };
                    assignCodeFullLinks.Add(assignCodeFullLink);
                }

                var preferences = GetPreference(data.DistrictId);
                var param = new SurveyAssignParameter()
                {
                    ClassId = schoolClass.Id2,
                    UserId = data.UserId,
                    SurveyId = data.TestId,
                    DistrictId = data.DistrictId,
                    SurveyAssignmentType = data.SurveyAssignmentType,
                    AssignCodeFullLinks = assignCodeFullLinks,
                    Preferences = preferences
                };

                return _qTITestClassAssignmentService.BatchSaveAssignmentPrivate(param);
            }
            return new List<AssignResult>();
        }
        public void AssignAndDistributePrivate(SurveyAssignmentData data, DistributeSetting distributeSetting)
        {
            var emails = AssignPrivate(data, distributeSetting.TestCodePrefix);
            emails = emails.Where(x => !string.IsNullOrEmpty(x.Email)).ToList();
            if (emails.Any())
            {
                var district = _districtService.GetDistrictById(data.DistrictId);
                var portalUrl = $"{distributeSetting.HTTPProtocol}://{district.LICode?.ToLower()}.{ConfigurationManager.AppSettings["LinkItUrl"]}";
                var distributeInfos = emails.Select(x => new DistributeInformation()
                {
                    Email = x.Email,
                    SurveyLink = x.RoleId == (int)Permissions.Student ? $"{portalUrl}/student?ReturnUrl=/TakeSurvey"
                    : (x.RoleId == (int)Permissions.Parent ? $"{portalUrl}/parent?ReturnUrl=/TakeSurvey" : $"{portalUrl}/TakeSurvey")
                }).ToList();
                var surveyDistribute = new SurveyDistributeNotifyDto()
                {
                    DistributeInformations = distributeInfos,
                    DistrictId = data.DistrictId,
                    SurveyId = data.TestId,
                    SurveyName = data.TestName,
                    PublishedBy = distributeSetting.DistributedBy,
                    SurveyAssignmentType = data.SurveyAssignmentType
                };
                DistributeAssignment(surveyDistribute);
            }
        }
        public void Distribute(SurveyDistributeNotifyDto surveyDistribute, string httpProtocol)
        {
            if (surveyDistribute.SurveyAssignmentType == (int)SurveyAssignmentTypeEnum.PrivateAnonymous || surveyDistribute.SurveyAssignmentType == (int)SurveyAssignmentTypeEnum.PrivateIndividualized)
            {
                var qtiTestClassAssignmentIds = surveyDistribute.DistributeInformations.Select(x => x.QTITestClassAssignmentId).ToList();
                var studentParentAssignments = _qTITestClassAssignmentService.GetSurveyPrivateAssignmentOfStudentAndParent(string.Join(",", qtiTestClassAssignmentIds));
                var district = _districtService.GetDistrictById(surveyDistribute.DistrictId ?? 0);
                var portalUrl = $"{httpProtocol}://{district.LICode?.ToLower()}.{ConfigurationManager.AppSettings["LinkItUrl"]}";

                foreach (var info in surveyDistribute.DistributeInformations)
                {
                    if (studentParentAssignments.Any(x => x.QTITestClassAssignmentId == info.QTITestClassAssignmentId && x.RoleId == (int)Permissions.Student))
                        info.SurveyLink = $"{portalUrl}/student?ReturnUrl=/TakeSurvey";
                    else if (studentParentAssignments.Any(x => x.QTITestClassAssignmentId == info.QTITestClassAssignmentId && x.RoleId == (int)Permissions.Parent))
                        info.SurveyLink = $"{portalUrl}/parent?ReturnUrl=/TakeSurvey";
                    else
                        info.SurveyLink = $"{portalUrl}/TakeSurvey";
                }
            }

            DistributeAssignment(surveyDistribute);
        }
        private int GetTestCodeLength()
        {
            var objCon = _configurationService.GetConfigurationByKey(Constanst.ClassTestCodeLength);
            int iTestCodeLength = 5;
            if (objCon != null)
            {
                iTestCodeLength = CommonUtils.ConverStringToInt(objCon.Value, 5);
            }
            return iTestCodeLength;
        }
        private string GenerateClassTestCode(string testCodePrefix, int iTestCodeLength, List<AssignCodeFullLink> currentTestCodes)
        {
            var code = string.Empty;
            do
            {
                code = _testCodeGenerator.GenerateTestCode(iTestCodeLength, testCodePrefix);
            } while (currentTestCodes.Any(x => x.Code == code));
            return code;
        }

        private void DistributeAssignment(SurveyDistributeNotifyDto surveyDistribute)
        {
            if (surveyDistribute != null)
            {
                var messages = BuildMessageQueue(surveyDistribute);
                var queueUrl = ConfigurationManager.AppSettings["SendmaiQueueUrl"];
                if (messages != null && messages.Count > 0)
                {
                    var maxItemCount = 10;
                    var batchCount = messages.Count / maxItemCount;
                    if (messages.Count % maxItemCount > 0)
                        batchCount++;

                    for (int i = 0; i < batchCount; i++)
                    {
                        var batchMessages = messages.Skip(i * maxItemCount).Take(maxItemCount).ToList();
                        _messageQueueService.SendMessageBatch(queueUrl, batchMessages);
                    }
                }
            }
        }

        private List<MessageQueueDto> BuildMessageQueue(SurveyDistributeNotifyDto surveyDistribute)
        {
            var results = new List<MessageQueueDto>();
            var subjects = new Dictionary<string, string>()
                {
                    { "SurveyName", surveyDistribute.SurveyName}
                };

            foreach (var info in surveyDistribute.DistributeInformations)
            {
                var contents = new Dictionary<string, string>()
                {
                    { "PublishedBy", surveyDistribute.PublishedBy},
                    { "SurveyLink", info.SurveyLink}
                };
                var messageQueue = new MessageQueueDto()
                {
                    ServiceType = MessageQueueServiceTypeEnums.EMAIL.DescriptionAttr(),
                    Key = "SURVEY_DISTRIBUTE",
                    DistrictId = surveyDistribute.DistrictId ?? 0,
                    EmailTo = info.Email,
                    Data = new EmailData()
                    {
                        Subjects = subjects,
                        Contents = contents
                    }
                };
                results.Add(messageQueue);
            }

            return results;
        }
        public IQueryable<SurveyGetGridPopulationUserDto> GetSurveyListUsersByRoles(GetUserResultsRequest request)
        {
            return _lookupStudentRepository.SurveyGetGridPopulationUser(request);
        }

        public IList<ListItem> GetAssignSurveyBanksByUserID(int? districtId, int roleId, int userId)
        {
            return _manageTestRepository.GetAssignSurveyBanksByUserID(districtId, roleId, userId).OrderBy(x => x.Name).ToList();
        }

        private string GetPreference(int districtId)
        {
            var preferences = _preferencesService.GetPreferenceInCurrentLevel(new GetPreferencesParams
            {
                CurrrentLevelId = (int)TestPreferenceLevel.SurveyDistrict,
                DistrictId = districtId,
                IsSurvey = true
            });

            if(preferences != null)
                return _preferencesService.ConvertTestPreferenceModelToString(preferences);

            return string.Empty;
        }
    }
}
