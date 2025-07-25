using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    [Serializable]
    public class MenuAccessItems
    {
        public List<string> DisplayedItems { get; set; }
        public Dictionary<string, MenuItemLabel> MainMenuItems { get; set; }
        public Dictionary<string, MenuItemLabel> SubMenuItems { get; set; }

        public bool IsDisplayHome
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Home);
            }
        }
        public bool IsDisplayHomeItem
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.HomeItem);
            }
        }

        public bool IsDisplayReportingItemVueJS
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportingItemVueJS);
            }
        }
        public bool IsDisplayDataExplorer
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DataExplorer);
            }
        }
        public bool IsDisplayReporting
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Reporting);
            }
        }         
        public bool IsDisplayReportingItemNew
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportingItemNew);
            }
        }
        public bool IsDisplayReportingItemCS
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportingItemCS);
            }
        }

        public bool IsDisplaySGOManager
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportItemSGOManager);
            }
        }
        public bool IsDisplayAblesReport
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.AblesReport);
            }
        }

        public bool IsDisplayTLDSManager
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportItemTLDSManager);
            }
        }

        public bool IsDisplayNavigatorUpload
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.NavigatorReportUpload);
            }
        }

        public bool IsDisplayNavigator
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.NavigatorReport);
            }
        }

        public bool IsDisplayTestDesign
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Testdesign);
            }
        }
        public bool IsDisplayTestdesignAssement
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignAssement);
            }
        }
        public bool IsDisplayTestdesignPassages
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignPassages);
            }
        }
        public bool IsDisplayTestdesignTests
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignTests);
            }
        }
        public bool IsDisplayTestdesignTags
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignTags);
            }
        }
        public bool IsDisplayTestdesignAssessmentItemNew
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignAssessmentItemHTML);
            }
        }
        public bool IsDisplayTestCloneItemBanks
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestCloneItemBanks);
            }
        }
        public bool IsDisplayTestdesignManageAuthorGroup
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignManageAuthorGroup);
            }
        }
        public bool IsDisplayTestdesignUpload3pItemBank
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignUpload3pItemBank);
            }
        }

        public bool IsDisplayTestdesignManageTest
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignManageTest);
            }
        }

        public bool IsDisplayTestdesignRubric
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignRubric);
            }
        }

        public bool IsDisplayTestdesignAssementOld
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignAssementOld);
            }
        }
        public bool IsDisplayTestdesignPassagesOld
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignPassagesOld);
            }
        }
        public bool IsDisplayTestdesignTestsOld
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignTestsOld);
            }
        }

        public bool IsDisplayMgmtBubbleSheets
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Managebubblesheets);
            }
        }
        public bool IsDisplayMgmtbbsCreate
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ManagebubblesheetsCreate);
            }
        }
        public bool IsDisplayMgmtbbsGrade
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ManagebubblesheetsGrade);
            }
        }
        public bool IsDisplayMgmtbbsReview
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ManagebubblesheetsReview);
            }
        }
        public bool IsDisplayMgmtbbsProcessError
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ManagebubblesheetsError);
            }
        }
        public bool IsDisplayMgmtbbsImportGroup
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ManagebubblesheetsGimport);
            }
        }

        public bool IsDisplayReportACTReport
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportItemACTReport);
            }
        }
        public bool IsDisplayReportingDownloadReport
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportingDownloadReport);
            }
        }

        public bool IsDisplayOnlineTest
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Onlinetesting);
            }
        }
        public bool IsDisplayOnlineTestItem
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlinetestingItem);
            }
        }
        public bool IsDisplayOnlineTestPreference
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlinetestPreference);
            }
        }

        public bool IsDisplayOnlineDefaultStudentPreference
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlineDefaultStudentPreference);
            }
        }

        public bool IsDisplayOnlineTestStudentPreference
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlinetestStudentPreference);
            }
        }
        public bool IsDisplayOnlineTestLockUnlockBank
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlinetestLockUnlockBank);
            }
        }

        public bool IsDisplayOnlineTestAssignRewrite
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlineTestAssignmentRewrite);
            }
        }
        public bool IsDisplayOnlineTestAssignReview
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlineTestAssignmentReview);
            }
        }

        public bool IsDisplayMonitorTestTaking
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlineTestMonitorTestTaking);
            }
        }

        public bool IsDisplayTestmanagement
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Testmanagement);
            }
        }
        public bool IsDisplayTmgmtUploadassessmentresults
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtUploadassessmentresults);
            }
        }
        //public bool IsDisplayTmgmtEinstructionimport
        //{
        //    get
        //    {
        //        return HasDisplayedItem(ContaintUtil.TmgmtEinstructionimport);
        //    }
        //}
        public bool IsDisplayTmgmtTestresultremover
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtTestresultremover);
            }
        }
        public bool IsDisplayTmgmtTestregrader
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtTestregrader);
            }
        }
        public bool IsDisplayTmgmtPurgetest
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtPurgetest);
            }
        }
        public bool IsDisplayTmgmtPrinttest
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtPrinttest);
            }
        }

        //public bool IsDisplayTmgmtCustomAssessments
        //{
        //    get
        //    {
        //        return HasDisplayedItem(ContaintUtil.TmgmtCustomAssessments);
        //    }
        //}

        public bool IsDisplayTmgmtExtractTestResult
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtExtractTestResult);
            }
        }
        public bool IsDisplayTmgmtTestResultTransfer
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtTestResultTransfer);
            }
        }
        public bool IsDisplayTmgmtTestResultExportGenesis
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtTestResultExportGenesis);
            }
        }

        public bool IsDisplayTmgmtTestLibraryExport
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtTestLibraryExport);
            }
        }

        public bool IsDisplayTmgmtDefineTemplate
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TmgmtDefineTemplates);
            }
        }

        public bool IsDisplayLessons
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Lessons);
            }
        }
        public bool IsDisplayLessonsItem
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.LessonsItem);
            }
        }

        public bool IsDisplayDataAdmin
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DataAdmin);
            }
        }
        public bool IsDisplayDadManageuser
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageuser);
            }
        }
        public bool IsDisplayDadManageUserGroup
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageUserGroup);
            }
        }
        public bool IsDisplayDadManageParent
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageParents);
            }
        }
        //public bool IsDisplayDadDistrictReferencedata
        //{
        //    get
        //    {
        //        return HasDisplayedItem(ContaintUtil.DadDistrictReferencedata);
        //    }
        //}
        public bool IsDisplayDadDistrictUsage
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadDistrictUsage);
            }
        }
        public bool IsDisplayDadManageSchools
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageSchools);
            }
        }
        public bool IsDisplayDadManageClasses
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageClasses);
            }
        }
        public bool IsDisplayStudentLookup
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.StudentLookup);
            }
        }
        public bool IsDisplayManageParent
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ManageParent);
            }
        }
        public bool IsDisplayDadRegisterClass
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageRegisterClasses);
            }
        }
        public bool IsDisplayDadManageRosters
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageRosters);
            }
        }

        public bool IsDisplayDadManageLAC
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageLAC);
            }
        }
        public bool IsDisplayDadLumos
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadLumos);
            }
        }

        public bool IsDisplayHelp
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Help);
            }
        }
        public bool IsDisplayHelpIntroduction
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.HelpIntroduction);
            }
        }
        public bool IsDisplayHelpGuide
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.HelpGuide);
            }
        }
        public bool IsDisplayHelpTechSupport
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.HelpTeachSupport);
            }
        }
        public bool IsDisplayHelpVideotutorials
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.HelpVideotutorials);
            }
        }
        public bool IsDisplayResultsEntryDataLocker
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ResultsEntryDataLocker);
            }
        }       
        public bool IsDisplayDefinTemplates
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Definetemplates);
            }
        }
        public bool IsDisplayBuildEntryForms
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.BuildEntryForms);
            }
        }
        public bool IsDisplayEntryResults
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.EnterResults);
            }
        }
        public bool IsDisplayDataLockerPreferences
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DataLockerPreferences);
            }
        }
        public bool IsDisplaySurveyModule
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.SurveyModule);
            }
        }
        public bool IsDisplayManageSurveys
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ManageSurveys);
            }
        }
        public bool IsDisplayAssignSurveys
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.AssignSurveys);
            }
        }
        public bool IsDisplayReviewSurveys
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReviewSurveys);
            }
        }
        public bool IsDisplayTakeSurveys
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TakeSurveys);
            }
        }
        public bool IsDisplaySettings
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Settings);
            }
        }

        public bool IsDisplayStudentProfile
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.StudentProfile);
            }
        }

        public bool IsDisplayStudentProfileEdit
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.StudentProfileEdit);
            }
        }

        public bool IsDisplayParentConnect
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ParentConnect);
            }
        }

        public bool IsDisplayMessageInbox
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.MessageInbox);
            }
        }

        public bool IsDisplayPContactInfo
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.PContactInfo);
            }
        }

        public bool IsDisplaySettingItem
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.SettingItem);
            }
        }

        public bool IsDisplayTechSupport
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.Techsupport);
            }
        }

        public bool IsDisplayTechMgmtReports
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TechMgmtReports);
            }
        }
        public bool IsDisplayTechETLtool
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TechETLtool);
            }
        }
        public bool IsDisplayTechUserImpersonation
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TechUserImpersonation);
            }
        }
        //public bool IsDisplayAPILog
        //{
        //    get
        //    {
        //        return HasDisplayedItem(ContaintUtil.TechAPILog);
        //    }
        //}



        public bool IsDisplayItemLibraryManagement
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ItemLibraryManagement);
            }
        }

        public bool IsDisplayLearningLibrarySearch
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.LearningLibrarySearch);
            }
        }
        public bool IsDisplayLearningLibraryResourceAdmin
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.LearningLibraryResourceAdmin);
            }
        }
        public bool IsDisplayTestdesignCreateTest
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignCreateTest);
            }
        }
        public bool IsDisplayManageProgram
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadManageProgram);
            }
        }

        public bool IsDisplayDadSecuritySetting
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DadSecuritySettings);
            }
        }

        public bool HasDisplayedItem(string item)
        {
            //if (RoleId == (int)Permissions.Publisher)
            //    return true;
            return DisplayedItems.Contains("|" + item + "|");
        }
        public bool IsDisplayDataAccessManagement
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.DataAccessManagement);
            }
        }

        public int DistrictId { get; set; }
        public int RoleId { get; set; }

        public List<int> ListSchoolId { get; set; }

        public MenuAccessItems()
        {
            DisplayedItems = new List<string>();
            ListSchoolId = new List<int>();
            MainMenuItems = new Dictionary<string, MenuItemLabel>();
            SubMenuItems = new Dictionary<string, MenuItemLabel>();
        }
        public bool IsDisplayTestdesignPassageNew
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestdesignPassageNew);
            }
        }

        public bool IsDisplayTestLaunch
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.TestLaunch);
            }
        }
        public bool IsDisplayOnlineTesting
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.OnlineTesting);
            }
        }
        public bool IsDisplayDataLockerAttachmentForStudent
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.AttachmentForStudent);
            }
        }
        public bool IsDisplayReportChytenReport
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportItemChytenReport);
            }
        }

        public bool IsDisplayCustomHelp
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.CustomHelp);
            }
        }
        public bool IsDisplayHelpResource
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.HelpResource);
            }
        }

        public bool IsDisplayReportingTestResult
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportingTestResult);
            }
        }

        public bool IsDisplayReportingTestResultCS
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.ReportingTestResultCS);
            }
        }

        public bool IsDisplayInterventionManager
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.InterventionManager);
            }
        }

        public bool IsIMDashboard
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.IMDashboard);
            }
        }

        public bool IsPerformanceCriteria
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.PerformanceCriteria);
            }
        }

        public bool IsGroupingModel
        {
            get
            {
                return HasDisplayedItem(ContaintUtil.GroupingModel);
            }
        }

        public string CustomHelpUrl { get; set; }
        public string TechSupportUrl { get; set; }
        public string RiseTechSupportUrl { get; set; }
    }

    [Serializable]
    public class MenuItemLabel
    {
        public MenuItemLabel(string code, string label, string tooltip, string helpText, string path = "")
        {
            this.Code = code;
            this.Label = label;
            this.Tooltip = tooltip;
            this.HelpText = helpText;
            this.Path = path;
        }

        public string Code { get; set; }
        public string Label { get; set; }
        public string Tooltip { get; set; }
        public string HelpText { get; set; }
        public string Path { get; set; }
        public List<MenuItemLabel> SubMenuItems { get; set; }
    }
}
