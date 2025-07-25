using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using Newtonsoft.Json;
using RequestSheet = LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator.RequestSheet;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetPrintingService
    {
        private readonly QuestionOptionsService questionOptionsService;
        private readonly ClassStudentService classStudentService;
        private readonly TestService testService;
        private readonly BubbleSheetService bubbleSheetService;
        //private readonly IValidator<RequestSheet> sheetRequestValidator;
        private readonly IValidator<BubbleSheet> bubbleSheetValidator;

        private readonly VirtualSectionService virtualSectionService;
        private readonly VirtualSectionQuestionService virtualSectionQuestionService;
        private readonly VirtualTestService virtualTestService;
        private readonly DistrictDecodeService districtDecodeService;
        private readonly SubjectService subjectService;
        private readonly ClassService classService;
        private readonly UserService userService;
        private readonly ClassUserService classUserService;
        private readonly VirtualQuestionItemTagService virtualQuestionItemTagService;

        private const string DomainTagCategoryLabel = "DomainTagCategoryID";
        private const string SATDomainTagCategoryLabel = "SATDomainTagCategoryID";

        public BubbleSheetPrintingService(QuestionOptionsService questionOptionsService, 
            ClassStudentService classStudentService, 
            TestService testService,
            BubbleSheetService bubbleSheetService,
            //IValidator<RequestSheet> sheetRequestValidator,
            IValidator<BubbleSheet> bubbleSheetValidator,
            VirtualSectionService virtualSectionService,
            VirtualSectionQuestionService virtualSectionQuestionService,
            VirtualTestService virtualTestService, 
            DistrictDecodeService districtDecodeService, 
            SubjectService subjectService,
            ClassService classService,
            UserService userService,
            ClassUserService classUserService,
            VirtualQuestionItemTagService virtualQuestionItemTagService)
        {
            this.questionOptionsService = questionOptionsService;
            this.classStudentService = classStudentService;
            this.testService = testService;
            this.bubbleSheetService = bubbleSheetService;
            //this.sheetRequestValidator = sheetRequestValidator;
            this.bubbleSheetValidator = bubbleSheetValidator;
            this.virtualSectionService = virtualSectionService;
            this.virtualSectionQuestionService = virtualSectionQuestionService;
            this.virtualTestService = virtualTestService;
            this.districtDecodeService = districtDecodeService;
            this.subjectService = subjectService;
            this.classService = classService;
            this.userService = userService;
            this.classUserService = classUserService;
            this.virtualQuestionItemTagService = virtualQuestionItemTagService;
        }

        public RequestSheet InitializeRequestSheet(int count, string apiKey)
        {
            return new RequestSheet
            {
                ApiKey = apiKey,
                OutputFormat = "PDF",
                SheetCount = count,
                PostToOnCompleted = null
            };
        }

        public int GetBubbleSize(int bubbleSize)
        {
            switch (bubbleSize)
            {
                case 0:
                    return 25;
                case 1:
                    return 30;
                case 3:
                    return 40;
                case 4:
                    return 45;
                default:
                    return 35;
            }
        }

        public void AssignStudentsToTest(BubbleSheetData model, RequestSheet test, string s3CssKey)
        {
            var virtualTest = virtualTestService.Select().Single(en => en.VirtualTestID == model.TestId);

            test.DistrictId = model.DistrictId;
            var rosterStyle = model.SheetStyleId == (int)SheetStyle.Roster;

            if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.ACT || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT)
            {
                SetACTTestDetails(test, model, virtualTest.VirtualTestSubTypeID.GetValueOrDefault(), virtualTest, s3CssKey);
            }
            else if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT)
            {
                SetSATTestDetails(test, model, virtualTest, s3CssKey);
            }
            else
            {
                SetTestDetails(test, model, rosterStyle);    
            }
            
            if (rosterStyle)
            {
                AddStudentsToRoster(test, model);
            }
            else if (model.IsGenericBubbleSheet)
            {
                AddStudentsToGenericSheet(test, model, virtualTest);
            }
            else
            {
                AddStudentsToTest(test, model);
            }
        }

        private void AddStudentsToGenericSheet(RequestSheet test, BubbleSheetData model, VirtualTestData virtualTest)
        {
            test.SheetCount = 1;
            var studentDictionary = InitializeStudentDictionary(test, model, "Student Name: _______________________________");
            test.PerSheetTemplateText.Add(studentDictionary);
            test.TemplateText["IsGeneric"] = "true";
            test.TemplateText["NumberOfSheet"] = model.NumberOfGenericSheet.ToString();
            test.TemplateText["PrintStudentIDs"] = model.PrintStudentIDs.ToString();
            if (model.PrintStudentIDs)
            {
                var listStudentWithCode = new List<KeyValuePair<string, string>>();
                //get all students from class
                var studentList =
                    classStudentService.GetClassStudentsByClassId(model.ClassId)
                        .ToList()
                        .OrderBy(x => x.FullName)
                        .ToList();
                foreach (var student in studentList)
                {
                    listStudentWithCode.Add(new KeyValuePair<string, string>(student.Code, student.FullName));
                }
                test.TemplateText["ListStudentIDs"] = JsonConvert.SerializeObject(listStudentWithCode);
            }
        }

        public void AddStudentsToTest(RequestSheet test, BubbleSheetData model)
        {
            var studentList = classStudentService.GetClassStudentsByClassId(model.ClassId).ToList();
            for (var i = 0; i < model.StudentIdList.Count; i++)
            {
                int studentId;
                int.TryParse(model.StudentIdList[i], out studentId);
                var student = studentList.FirstOrDefault(x => x.StudentId.Equals(studentId));
                if (student.IsNotNull())
                {
                    var studentDictionary = InitializeStudentDictionary(test, model, student.FullName + " (" + student.Code + ")");
                    test.PerSheetTemplateText.Add(studentDictionary);
                }
            }
        }

        public void AddStudentsToRoster(RequestSheet test, BubbleSheetData model)
        {
            AddFooter(test, model);
            var studentList = classStudentService.GetClassStudentsByClassId(model.ClassId).ToList();
            foreach (var id in model.StudentIdList)
            {
                int studentId;
                int.TryParse(id, out studentId);
                var student = studentList.FirstOrDefault(x => x.StudentId.Equals(studentId));
                if (student.IsNotNull())
                {
                    test.Roster.Add(student.FullName);
                }
            }
        }

        public void SetTestDetails(RequestSheet test, BubbleSheetData model, bool rosterStyle)
        {
            //test.SetValidator(sheetRequestValidator);

            test.Template = rosterStyle ? "Roster" : "CustomizedName";

            if (rosterStyle)
            {
                test.SheetCount = 1;
                test.TemplateText["Name"] = "Student Name: _______________________________";
            }

            test.BubbleSize = GetBubbleSize(model.BubbleSizeId);
            test.TemplateText["Header"] = model.TeacherName + " - " + model.ClassName;
            test.TemplateText["NameHeader"] = model.SchoolName;
            test.TemplateText["TestDetail1"] = model.SubjectName;
            test.TemplateText["TestDetail2"] = model.TestName;
            test.TemplateText["TestDetail5"] = model.DistrictName;
            test.TemplateText["PaginationOption"] = model.PaginationOption.ToString();
            test.QuestionCount = testService.GetTestById(model.TestId).QuestionCount;
            SetQuestions(test, model.TestId, model.BubbleFormat, model.PaginationQuestionIds);
            SetDefaultSectionQuestions(test, model.TestId, model.BubbleFormat, model.PaginationSectionIds);
            test.Barcode2 = "0";
        }

        public void SetACTTestDetails(RequestSheet test, BubbleSheetData model, int virtualTestSubTypeId, VirtualTestData virtualTest, string s3CSSKey)
        {
            if (virtualTestSubTypeId == (int) VirtualTestSubType.NewACT)
            {
                test.Template = model.IsIncludeEssayPage ? Constanst.TemplateNewACT : Constanst.TempateNewACTNoEssay;
            }
            else
            {
                test.Template = model.IsIncludeEssayPage ? Constanst.TemplateACT : Constanst.TempateACTNoEssay;
            }
            
            test.SheetCount = model.StudentIdList.Count;
            test.BubbleSize = GetBubbleSize(0); // Size: 25

            test.TemplateText["DistrictLogoLink"] = GetDistrictLogo(model.DistrictId, s3CSSKey);
            test.TemplateText["HeaderLeft1"] = model.DistrictName;
            test.TemplateText["HeaderRight1"] = model.SchoolName;            
            test.TemplateText["HeaderRight3"] = model.TestName;
            test.TemplateText["FooterLeft1"] = ConfigurationManager.AppSettings["BubbleSheetFooterVersion"];
            test.TemplateText["FooterLeft2"] = model.TeacherName + " - " + model.ClassName;
            test.Barcode2 = "0";
            
            test.QuestionCount = testService.GetTestById(model.TestId).QuestionCount;
            SetSectionQuestions(test, model.TestId, model.BubbleFormat, virtualTest, virtualTestSubTypeId == (int)VirtualTestSubType.NewACT);
        }

        public void SetSATTestDetails(RequestSheet test, BubbleSheetData model, VirtualTestData virtualTest, string s3CssKey)
        {
            bool hasEssayPage = CheckSATIncludeEssayPage(virtualTest, model.DistrictId);
            if (virtualTest.VirtualTestSubTypeID == (int) VirtualTestSubType.SAT)
            {
                test.Template = hasEssayPage ? Constanst.TemplateSAT : Constanst.TemplateSATNoEssay;
            }
            else
            {
                test.Template = hasEssayPage
                    ? model.IsIncludeEssayPage ? Constanst.TemplateNewSAT : Constanst.TemplateNewSATWritingNoEssay
                    : Constanst.TemplateNewSATNoWriting;
            }
                

            test.SheetCount = model.StudentIdList.Count;
            test.BubbleSize = GetBubbleSize(0); // Size: 25

            test.TemplateText["DistrictLogoLink"] = GetDistrictLogo(model.DistrictId, s3CssKey);
            test.TemplateText["HeaderLeft1"] = model.DistrictName;
            test.TemplateText["HeaderRight1"] = model.SchoolName;
            test.TemplateText["HeaderRight3"] = model.TestName;
            test.TemplateText["FooterLeft1"] = ConfigurationManager.AppSettings["BubbleSheetFooterVersion"];
            test.TemplateText["FooterLeft2"] = model.TeacherName + " - " + model.ClassName;
            test.TemplateText["IsIncludeShading"] = model.IsIncludeShading.ToString();
            test.Barcode2 = "0";

            test.QuestionCount = testService.GetTestById(model.TestId).QuestionCount;
            SetSectionQuestions(test, model.TestId, model.BubbleFormat, virtualTest);
        }

        /// <summary>
        /// If there is no Writing subject, 'Official Use Only' section and essay pages would not be generated.
        /// If Writing subject has no open ended section, 
        /// 'Official Use Only' section and essay pages would not be generated
        /// </summary>
        /// <param name="virtualTest"></param>
        /// <returns></returns>
        public bool CheckSATIncludeEssayPage(VirtualTestData virtualTest, int districtID)
        {
            var essaySection = GetSATEssaySection(virtualTest, districtID);
            return essaySection != null;
        }

        public VirtualSection GetSATEssaySection(VirtualTestData virtualTest, int districtID)
        {
            var listSection = virtualSectionService.GetVirtualSectionByVirtualTest(virtualTest.VirtualTestID);
            var listWritingSubjectID = districtDecodeService.GetDistrictDecodesByLabel("SATWritingSubject")
                .Where(x => x.DistrictID == districtID).ToList()
                .Select(x => ConvertValue.ToInt(x.Value))
                .Where(x => x > 0)
                .ToList();

            foreach (var virtualSection in listSection)
            {
                if (virtualSection.SubjectId.HasValue
                    && listWritingSubjectID.Contains(virtualSection.SubjectId.Value))
                {
                    var listSectionQuestion =
                        virtualSectionQuestionService.GetVirtualSectionQuestionBySection(virtualSection.VirtualTestId,virtualSection.VirtualSectionId)
                        .Select(x => x.VirtualQuestionId).ToList();
                    
                    var questions =
                        questionOptionsService.GetQuestionOptionsByTestId(virtualTest.VirtualTestID)
                            .Where(x => listSectionQuestion.Contains(x.VirtualQuestionId)).ToList();

                    if (questions.Any(x => x.QtiSchemaId != (int)QtiSchemaEnum.ExtendedText) == false)
                    {
                        return virtualSection;
                    }
                }
            }

            return null;
        }

        private string GetDistrictLogo(int districtId, string s3CSSKey)
        {
            var logoUrl = string.Format("{0}{1}-logo.png", s3CSSKey, districtId);

            if (UrlUtil.CheckUrlStatus(logoUrl))
            {
                return logoUrl;
            }

            return "";
        }

        private Dictionary<string, string> InitializeStudentDictionary(RequestSheet test, BubbleSheetData model, string studentNameText)
        {
            if(test.Template == Constanst.TemplateACT 
                || test.Template == Constanst.TempateACTNoEssay
                || test.Template == Constanst.TemplateNewACT
                || test.Template == Constanst.TempateNewACTNoEssay 
                || test.Template == Constanst.TemplateSAT
                || test.Template == Constanst.TemplateSATNoEssay
                || test.Template == Constanst.TemplateNewSAT
                || test.Template == Constanst.TemplateNewSATNoWriting
                || test.Template == Constanst.TemplateNewSATWritingNoEssay)
            {
                return new Dictionary<string, string>
                {
                    {"Name", studentNameText},
                    {"FooterLeft1", ConfigurationManager.AppSettings["BubbleSheetFooterVersion"] },
                    {"TestDetail4", "Created " + DateTime.UtcNow.AddMinutes(model.TimezoneOffset*(-1)).DisplayDateWithFormat(true)},
                    {"FooterLeft2", model.TeacherName + " - " + model.ClassName},
                    {"HeaderRight1", model.SchoolName},
                    {"HeaderRight3", model.TestName},
                    {"HeaderLeft1", model.DistrictName}
                };
            }
            else
            {
                return new Dictionary<string, string>
                {
                    {"Name", studentNameText},
                    {"TestDetail3", ConfigurationManager.AppSettings["BubbleSheetFooterVersion"] },
                    {"TestDetail4", "Created " + DateTime.UtcNow.AddMinutes(model.TimezoneOffset*(-1)).DisplayDateWithFormat(true)},
                    {"Header", model.TeacherName + " - " + model.ClassName},
                    {"NameHeader", model.SchoolName},
                    {"TestDetail1", model.SubjectName},
                    {"TestDetail2", model.TestName},
                    {"TestDetail5", model.DistrictName}
                };
            }
            
        }

        private void AddFooter(RequestSheet test, BubbleSheetData model)
        {
            var footer = new Dictionary<string, string>
            {
                {"TestDetail3", ConfigurationManager.AppSettings["BubbleSheetFooterVersion"] },
                {"TestDetail4", "Created " + DateTime.UtcNow.AddMinutes(model.TimezoneOffset*(-1)).DisplayDateWithFormat(true)},
                {"TestDetail2", model.TestName},
                {"TestDetail1", model.SubjectName},
                {"TestDetail5", model.DistrictName}
            };

            test.PerSheetTemplateText.Add(footer);
        }

        private char SetOffsetCharacters(int problemNumber)
        {
            switch (problemNumber % 5)
            {
                case 1:
                    return 'A';
                case 2:
                    return 'E';
                case 3:
                    return 'I';
                case 4:
                    return 'M';
                default:
                    return 'Q';
            }
        }

        public void SetQuestions(RequestSheet test, int testId, int bubbleFormat, string strPaginationQuestionIds)
        {
            var paginationQuestionIds = new List<int>();

            if (!string.IsNullOrEmpty(strPaginationQuestionIds))
                paginationQuestionIds = strPaginationQuestionIds.Split(';').Select(x => Convert.ToInt32(x) + 1).ToList();

            var questions = questionOptionsService.GetQuestionOptionsByTestId(testId).ToList();
            
            
            if (bubbleFormat.Equals((int)BubbleFormat.Default))
            {
                // Comment this line of code to always send QuestionOption (IsOpenEnded and IsGhost data) to PDF Generator
                //questions.RemoveAll(RemoveStandardOptions());
            }


            foreach (var option in questions)
            {
                test.Questions.Add(new Question
                                   {
                                        Index = option.ProblemNumber - 1,
                                        Characters = DetermineCharacterOptionMethod(option, bubbleFormat),
                                        IsPageBreak = paginationQuestionIds.Any(x => x == option.ProblemNumber),
                                        IsOpenEndedQuestion = option.IsOpenEndedQuestion,
                                        IsGhostQuestion = option.IsGhostQuestion,
                                        PointsPossible = option.PointsPossible,
                                        QuestionGroupID = option.QuestionGroupID,
                                        VirtualSectionID = option.VirtualSectionID,
                                   });
            }
        }

        public void SetShadingOption(RequestSheet test, BubbleSheetGroupData modelForGroup)
        {
            test.TemplateText["IsIncludeShading"] = modelForGroup.IsIncludeShading.ToString();
        }

        public void SetPrintStudentIDsListOption(RequestSheet test, BubbleSheetGroupData modelForGroup)
        {
            test.TemplateText["PrintStudentIDs"] = modelForGroup.PrintStudentIDs.ToString();
            if (modelForGroup.PrintStudentIDs)
            {
                var listStudentWithCodeForGroup = new List<string>();
                foreach (var classId in modelForGroup.ClassIdList)
                {
                    var listStudentWithCode = new List<KeyValuePair<string, string>>();
                    //get all students from class
                    var studentList =
                        classStudentService.GetClassStudentsByClassId(classId)
                            .ToList()
                            .OrderBy(x => x.FullName)
                            .ToList();
                    foreach (var student in studentList)
                    {
                        listStudentWithCode.Add(new KeyValuePair<string, string>(student.Code, student.FullName));
                    }
                    //test.TemplateText[string.Format("ListStudentIDs-{0}", classId)] = JsonConvert.SerializeObject(listStudentWithCode);
                    listStudentWithCodeForGroup.Add(JsonConvert.SerializeObject(listStudentWithCode));
                }
                test.TemplateText["ListStudentIDsGroup"] = JsonConvert.SerializeObject(listStudentWithCodeForGroup);
            }
        }

        public void SetPrintStudentIDsListOptionLargeClass(RequestSheet test, BubbleSheetGroupData modelForGroup)
        {
            test.TemplateText["PrintStudentIDs"] = modelForGroup.PrintStudentIDs.ToString();
            if (modelForGroup.PrintStudentIDs)
            {
                var listStudentWithCodeForGroup = new List<string>();
                var listStudentWithCode = new List<KeyValuePair<string, string>>();

                var classTeacherAndClassNames =
                    modelForGroup.ClassIdList.Select(
                        x => new {classId = x, teacherAndClassName = GetTeacherAndClassName(x)});

                var classInfos = classTeacherAndClassNames.Select(x => new
                {
                    x.classId,
                    className = x.teacherAndClassName.Split('|')[0],
                    lastName = x.teacherAndClassName.Split('|')[1],
                    fistName = x.teacherAndClassName.Split('|')[2],
                    fullName =
                        !string.IsNullOrEmpty(x.teacherAndClassName.Split('|')[1]) &&
                        !string.IsNullOrEmpty(x.teacherAndClassName.Split('|')[2])
                            ? x.teacherAndClassName.Split('|')[1] + ", " + x.teacherAndClassName.Split('|')[2]
                            : (x.teacherAndClassName.Split('|')[1] + " " + x.teacherAndClassName.Split('|')[2]).Trim()
                });

                classInfos = classInfos.OrderBy(x => x.lastName).ThenBy(x => x.className);


                foreach (var classInfo in classInfos)
                {
                    listStudentWithCode.Add(new KeyValuePair<string, string>("ClassName",
                        classInfo.className + " (" + classInfo.fullName + ")"));
                    
                    //get all students from class
                    var studentList =
                        classStudentService.GetClassStudentsByClassId(classInfo.classId)
                            .ToList()
                            .OrderBy(x => x.FullName)
                            .ToList();
                    foreach (var student in studentList)
                    {
                        listStudentWithCode.Add(new KeyValuePair<string, string>(student.Code, student.FullName));
                    }
                }

                listStudentWithCodeForGroup.Add(JsonConvert.SerializeObject(listStudentWithCode)); // For large class add all student information into 1st class
                test.TemplateText["ListStudentIDsGroup"] = JsonConvert.SerializeObject(listStudentWithCodeForGroup);
            }
        }

        private string GetTeacherAndClassName(int classId)
        {
            var className = "";
            var teacherLastName = "";
            var teacherFirstName = "";

            var cls =
            classService.GetClassById(classId);
            if (cls != null)
            {
                className = cls.Name;

                // Detect primary teacher
                var classUser =
                    classUserService.GetClassUsersByClassId(classId).FirstOrDefault(x => x.ClassUserLOEId == 1);

                if (classUser != null)
                {
                    var user = userService.GetUserById(classUser.UserId);
                    if (user != null)
                    {
                        teacherFirstName = user.FirstName;
                        teacherLastName = user.LastName;
                    }
                }
            }

            return className + "|" + teacherLastName + "|" + teacherFirstName;
        }

        public List<Question> GetDomainTagForEssaySection(int testId)
        {
            var virtualTest = virtualTestService.GetTestById(testId);
            int districtID = GetDistrictIDFromTestOwner(virtualTest);
            int tagCategoryForWriting = GetTagCategoryForWriting(virtualTest, districtID);
            int essaySectionOrder = GetEssaySectionOrder(virtualTest, districtID);
            var sections = virtualSectionService.GetVirtualSectionByVirtualTest(testId).OrderBy(en => en.Order);
            var testQuestions = questionOptionsService.GetQuestionOptionsByTestId(testId).ToList();
            var listQuestion = new List<Question>();

            foreach (var virtualSection in sections)
            {
                var problemNumberCounter = 1;
                if (virtualSection.Order != essaySectionOrder) continue;
                var sectionQuestions =
                    virtualSectionQuestionService.GetVirtualSectionQuestionBySection(virtualSection.VirtualTestId,
                        virtualSection.VirtualSectionId);
                var questions =
                    testQuestions.Where(en => sectionQuestions.Any(x => x.VirtualQuestionId == en.VirtualQuestionId))
                        .OrderBy(en => en.ProblemNumber)
                        .ToList();
                foreach (var option in questions)
                {
                    option.ProblemNumber = problemNumberCounter;

                    var question = new Question
                                   {
                                       Index = option.ProblemNumber - 1,
                                       TagName =
                                           GetFirstVirtualQuestionTagName(option.VirtualQuestionId,
                                               tagCategoryForWriting),
                                       SectionID = virtualSection.Order
                                   };
                    listQuestion.Add(question);
                    problemNumberCounter++;
                }
            }

            return listQuestion;
        }

        public void SetSectionQuestions(RequestSheet test, int testId, int bubbleFormat, VirtualTestData virtualTest, bool removeZeroPointForEssay = false)
        {
            var sections = virtualSectionService.GetVirtualSectionByVirtualTest(testId).OrderBy(en => en.Order);
            var testQuestions = questionOptionsService.GetQuestionOptionsByTestId(testId).ToList().AsQueryable();
            test.QuestionSections = new List<QuestionSection>();

            //get tagCategory from DistrictDecode, to filter TagName for Writing Section
            int districtID = GetDistrictIDFromTestOwner(virtualTest);
            int tagCategoryForWriting = GetTagCategoryForWriting(virtualTest, districtID);
            int essaySectionOrder = GetEssaySectionOrder(virtualTest, districtID);

            foreach (var virtualSection in sections)
            {
                // Reset ProblemNumber for each section
                var problemNumberCounter = 1;

                var sectionQuestions =
                    virtualSectionQuestionService.GetVirtualSectionQuestionBySection(virtualSection.VirtualTestId, virtualSection.VirtualSectionId);

                var questions =
                    testQuestions.Where(en => sectionQuestions.Any(x => x.VirtualQuestionId == en.VirtualQuestionId)).OrderBy(en => en.ProblemNumber).ToList();

                var questionSection = new QuestionSection
                {
                    Index = virtualSection.Order,
                    SectionName = virtualSection.Title,
                    ListQuestions = new List<Question>()
                };

                foreach (var option in questions)
                {
                    option.ProblemNumber = problemNumberCounter;

                    var question = new Question
                    {
                        Index = option.ProblemNumber - 1,
                        Characters = DetermineCharacterOptionMethod(option, bubbleFormat),
                        IsTextEntry = Equals(option.QtiSchemaId, (int)QtiSchemaEnum.TextEntry),
                    };

                    if (virtualSection.Order == essaySectionOrder)
                    {
                        question.TagName = GetFirstVirtualQuestionTagName(option.VirtualQuestionId, tagCategoryForWriting);
                        if (removeZeroPointForEssay)
                            question.Characters = question.Characters.Where(x => x != "0").ToList();
                    }

                    questionSection.ListQuestions.Add(question);

                    problemNumberCounter++;
                }

                test.QuestionSections.Add(questionSection);
            }
        }

        private int GetEssaySectionOrder(VirtualTestData virtualTest, int districtID)
        {
            int essaySectionOrder = 0;

            if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.ACT ||
                virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT)
            {
                essaySectionOrder = 5;
            }
            else if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT)
            {
                var essaySection = GetSATEssaySection(virtualTest, districtID);
                if (essaySection != null) essaySectionOrder = essaySection.Order;
            }

            return essaySectionOrder;
        }

        private int GetTagCategoryForWriting(VirtualTestData virtualTest, int districtID)
        {
            var districtDecodeLabel = virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT
                            ? DomainTagCategoryLabel
                            : virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT
                                ? SATDomainTagCategoryLabel
                                : string.Empty;
            var districtDecodeValue =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtID, districtDecodeLabel)
                    .FirstOrDefault();
            var tagCategoryForWriting = 0;
            if (districtDecodeValue != null)
            {
                int.TryParse(districtDecodeValue.Value, out tagCategoryForWriting);
            }

            return tagCategoryForWriting;
        }

        private int GetDistrictIDFromTestOwner(VirtualTestData virtualTest)
        {
            //get districtID of test creator
            var testCreatorUser = userService.GetUserById(virtualTest.AuthorUserID.GetValueOrDefault());
            var districtID = testCreatorUser != null ? testCreatorUser.DistrictId.GetValueOrDefault() : 0;
            return districtID;
        }

        private string GetFirstVirtualQuestionTagName(int virtualQuestionId, int tagCategoryID)
        {
            var itemTag =
                virtualQuestionItemTagService.GetVirtualQuestionItemTagByTagCategoryID(tagCategoryID)
                    .FirstOrDefault(x => x.VirtualQuestionId == virtualQuestionId);
            if (itemTag != null)
            {
                return itemTag.Name;
            }

            return "    ";
        }

        public void SetDefaultSectionQuestions(RequestSheet test, int testId, int bubbleFormat, string strPaginationSectionIds)
        {
            var paginationSectionIds = new List<int>();
            if (!string.IsNullOrEmpty(strPaginationSectionIds))
                paginationSectionIds = strPaginationSectionIds.Split(';').Select(x => Convert.ToInt32(x) + 1).ToList();

            var sections = virtualSectionService.GetVirtualSectionByVirtualTest(testId).OrderBy(en => en.Order); 
            var questions = questionOptionsService.GetQuestionOptionsByTestId(testId).ToList();
            test.QuestionSections = new List<QuestionSection>();
            foreach (var virtualSection in sections)
            {
                var sectionQuestions =
                    virtualSectionQuestionService.GetVirtualSectionQuestionBySection(testId, virtualSection.VirtualSectionId);
                var questionSection = new QuestionSection
                                      {
                                          Index = virtualSection.Order,
                                          SectionName = virtualSection.Title,
                                          IsPageBreak = paginationSectionIds.Any(x => x == virtualSection.Order),
                                          ListQuestions = questions
                                              .Where(
                                                  x =>
                                              sectionQuestions.Any(y => y.VirtualQuestionId == x.VirtualQuestionId))
                                              .Select(x => new Question
                                                           {
                                                               Characters =
                                                                   DetermineCharacterOptionMethod(x, bubbleFormat),
                                                               Index = x.ProblemNumber - 1,
                                                               IsPageBreak = false
                                                           }).ToList()
                                      };
                
                test.QuestionSections.Add(questionSection);
            }
        }

        private List<string> DetermineCharacterOptionMethod(QuestionOptions question, int bubbleFormat)
        {
            switch (bubbleFormat)
            {
                case ((int)BubbleFormat.Alternating):
                    return GetAlternatingOptionsForQuestion(question).ToList();
                case ((int)BubbleFormat.Numbered):
                    return GetNumberedOptionsForQuestion(question).ToList();
                case ((int)BubbleFormat.Gates):
                    return !question.Options.Count().Equals(4) ? GetOptionsForQuestion(question).ToList() : GetGatesOptionsForQuestion(question).ToList();
                case((int)BubbleFormat.Alternating2):
                    return GetAlternatingOptions2ForQuestion(question).ToList();
                default:
                    return GetOptionsForQuestion(question).ToList();
            }
        }

        private IEnumerable<string> GetGatesOptionsForQuestion(QuestionOptions option)
        {
            var character = SetOffsetCharacters(option.ProblemNumber);
            if (option.IsOpenEndedQuestion)
            {
                for (int i = 0; i < option.PointsPossible + 1; i++)
                {
                    yield return i.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                for (int i = 0; i < option.Options.Count(); i++)
                {
                    yield return ((char)(character + i)).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private IEnumerable<string> GetNumberedOptionsForQuestion(QuestionOptions option)
        {
            if (option.IsOpenEndedQuestion)
            {
                for (int i = 0; i < option.PointsPossible + 1; i++)
                {
                    yield return i.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                for (int i = 0; i < option.Options.Count(); i++)
                {
                    yield return ((char)('1' + i)).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private Predicate<QuestionOptions> RemoveStandardOptions()
        {
            return x => x.Options.Count() == 4 && !x.IsOpenEndedQuestion;
        }

        private IEnumerable<string> GetAlternatingOptionsForQuestion(QuestionOptions option)
        {
            if (option.IsOpenEndedQuestion)
            {
                for (int i = 0; i < option.PointsPossible + 1; i++)
                {
                    yield return i.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                char character = (option.ProblemNumber - 1) % 2 == 0 ? 'A' : 'F';
                for (int i = 0; i < option.Options.Count(); i++)
                {
                    int characterOffset = i;
                    if (character == 'F')
                    {
                        if (i >= 3)
                        {
                            characterOffset += 1;
                        }
                    }
                    yield return ((char)(character + characterOffset)).ToString(CultureInfo.InvariantCulture);
                }
            }
        }


        private IEnumerable<string> GetAlternatingOptions2ForQuestion(QuestionOptions option)
        {
            if (option.IsOpenEndedQuestion)
            {
                for (int i = 0; i < option.PointsPossible + 1; i++)
                {
                    yield return i.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                char character = (option.ProblemNumber - 1) % 2 == 0 ? 'A' : 'J';
                for (int i = 0; i < option.Options.Count(); i++)
                {
                    int characterOffset = i;
                    /*if (character == 'F')
                    {
                        if (i >= 3)
                        {
                            characterOffset += 1;
                        }
                    }*/
                    yield return ((char)(character + characterOffset)).ToString(CultureInfo.InvariantCulture);
                }
            }
        }


        private IEnumerable<string> GetOptionsForQuestion(QuestionOptions option)
        {
            if (option.IsOpenEndedQuestion)
            {
                for (int i = 0; i < option.PointsPossible + 1; i++)
                {
                    yield return i.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                for (int i = 0; i < option.Options.Count(); i++)
                {
                    var index = i >= 20 ? i + 1 : i;
                    int letterIndex = index % 26;
                    int prefixIndex = index / 26;
                    char? prefixChar = null;

                    if (prefixIndex > 0)
                        prefixChar = (char)('A' + prefixIndex - 1);

                    yield return prefixChar + ((char)('A' + letterIndex)).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public IEnumerable<BubbleSheet> CreateBubbleSheets(BubbleSheetData model, RequestSheet test)
        {
            var bubbleSheets = new List<BubbleSheet>();

            if (model.SheetStyleId == (int)SheetStyle.Roster)
            {
                bubbleSheets.Add(InsertRosterBubbleSheet(model, model.StudentIdList));
            }
            else if (model.IsGenericBubbleSheet)
            {
                bubbleSheets.Add(InsertGenericBubbleSheet(model));
            }
            else
            {
                bubbleSheets.AddRange(model.StudentIdList.Select(studentId => InsertClassBubbleSheet(model, studentId)));
            }

            var barcodes = bubbleSheets.Select(x => x.Id.ToString(CultureInfo.InvariantCulture));
            test.Barcode1 = barcodes.ToList();
            return bubbleSheets;
        }

        private BubbleSheet InsertClassBubbleSheet(BubbleSheetData model, string studentId)
        {
            var bubbleSheet = InitializeBubbleSheet(model);
            bubbleSheet.BubbleSheetCode = model.TestId + "." + studentId;
            bubbleSheet.StudentId = Convert.ToInt32(studentId);
            bubbleSheet.IsManualEntry = model.IsManualEntry;
            bubbleSheet.IsGenericSheet = false;
            if (model.TestExtract >= 0)
                bubbleSheet.TestExtract = model.TestExtract;

            bubbleSheet.SetValidator(bubbleSheetValidator);
            bubbleSheetService.Save(bubbleSheet);
            return bubbleSheet;
        }

        private BubbleSheet InsertRosterBubbleSheet(BubbleSheetData model, IEnumerable<string> studentIds)
        {
            var bubbleSheet = InitializeBubbleSheet(model);
            bubbleSheet.BubbleSheetCode = "na";
            bubbleSheet.StudentId = 0;
            bubbleSheet.StudentIds = studentIds.Aggregate(string.Empty, (current, studentId) => current + "\n" + studentId);
            bubbleSheet.IsGenericSheet = false;
            if (model.TestExtract >= 0)
                bubbleSheet.TestExtract = model.TestExtract;

            bubbleSheet.SetValidator(bubbleSheetValidator);
            bubbleSheetService.Save(bubbleSheet);
            return bubbleSheet;
        }

        private BubbleSheet InsertGenericBubbleSheet(BubbleSheetData model)
        {
            var bubbleSheet = InitializeBubbleSheet(model);
            bubbleSheet.BubbleSheetCode = "na";
            bubbleSheet.StudentId = 0;
            bubbleSheet.IsGenericSheet = true;
            if (model.TestExtract >= 0)
                bubbleSheet.TestExtract = model.TestExtract;

            bubbleSheet.SetValidator(bubbleSheetValidator);
            bubbleSheetService.Save(bubbleSheet);
            return bubbleSheet;
        }

        private BubbleSheet InitializeBubbleSheet(BubbleSheetData model)
        {
            return new BubbleSheet
            {
                ClassId = model.ClassId,
                SchoolId = model.SchoolId,
                TestId = model.TestId,
                BubbleSize = GetBubbleSize(model.BubbleSizeId).ToString(CultureInfo.InvariantCulture),
                UserId = model.UserId,
                DistrictTermId = model.DistrictTermId,
                SubmittedDate = DateTime.UtcNow,
                CreatedByUserId = model.CreatedByUserId
            };
        }
    }
}
