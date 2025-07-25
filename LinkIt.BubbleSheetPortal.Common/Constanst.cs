using System.Collections.Generic;
using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class Constanst
    {
        public static string TemplateACT = "ACT";
        public static string TempateACTNoEssay = "ACT-NoEssay";

        public static string TemplateNewACT = "NewACT";
        public static string TempateNewACTNoEssay = "NewACT-NoEssay";

        public static string TemplateSAT = "SAT";
        public static string TemplateSATNoEssay = "SAT-NoEssay";

        public static string TemplateNewSAT = "NewSAT-Writing";
        public static string TemplateNewSATWritingNoEssay = "NewSAT-Writing-NoEssay";
        public static string TemplateNewSATNoWriting = "NewSAT-NoWriting";
        public static string EmailRegex = "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$";
        public static Dictionary<string, string> EXTENSION_MAP_TO_FILE_TYPE = new Dictionary<string, string>() {
                    { ".xls","Excel"},
                    { ".xlsx","Excel"},
                    { ".ppt","PPT"},
                    { ".pptx","PPT"},
                    { ".pdf","PDF"},
                };
        public const char SYMBOL_BAR = '|';
        public const char NAVIGATOR_RATIONALIZEFILENAME_REPLACE_BY = '_';
        public const string NAVIGATOR_ATTRIBUTE_KEYWORD = "Keyword";
        public const string NAVIGATOR_ATTRIBUTE_REPORTINGPERIOD = "Reporting Period";
        public const string NAVIGATOR_ATTRIBUTE_CATEGORY = "Category";

        public static class NumberFormats
        {
            public const int DECIMAL_DEFAULT_DIGIT = 0;
        }

        public static int ACTStudentReportMinAmountOfTestForChart
        {
            get
            {
                int testAmount;
                if (int.TryParse(ConfigurationManager.AppSettings["ACTReportMinAmountOfTestForChart"], out testAmount))
                {
                    return testAmount;
                }
                return 3; //default value is 3, if no setting is found in web.config
            }
        }

        public static int SATStudentReportMinAmountOfTestForChart
        {
            get
            {
                int testAmount;
                if (int.TryParse(ConfigurationManager.AppSettings["SATReportMinAmountOfTestForChart"], out testAmount))
                {
                    return testAmount;
                }
                return 3; //default value is 3, if no setting is found in web.config
            }
        }


        public const string Configuration_SGOBankID = "SGOBankID";
        public const string Configuration_SGODistrictID = "SGODistrictID";
        public const string Configuration_SGOTeacherID = "SGOTeacherID";
        public const string Configuration_SGOSchoolID = "SGOSchoolID";
        public const string Configuration_SGOTermID = "";
        public const string Configuration_SGOClassID = "SGOClassID";
        public const string Configuration_SGODistrictTermID = "SGODistrictTermID";
        public const string Configuration_SGOUserID = "SGOUserID";

        public const string Configuration_SGOEmailTemplate = "SGOEmailTemplate";
        public const string Configuration_SGOEmailTemplatePreApprove = "SGOEmailTemplatePreApprove";
        public const string Configuration_SGOEmailTemplatePreDeny = "SGOEmailTemplatePreDeny";
        public const string Configuration_SGOEmailTemplateSubmitForApproval = "SGOEmailTemplateSubmitForApproval";
        public const string Configuration_SGOEmailTemplateAdminApproval = "SGOEmailTemplateAdminApproval";
        public const string Configuration_SGOEmailTemplateAdminDeny = "SGOEmailTemplateAdminDeny";
        public const string Configuration_SGOEmailTemplateTeacherAcknowledged = "SGOEmailTemplateTeacherAcknowledged";
        public const string Configuration_SGOEmailTemplateAuthorizeRevision = "SGOEmailTemplateAuthorizeRevision";

        //---------SGO---------------//
        public const string SGOStudentPopulateInstrodution = "SGOStudentPopulateInstruction";
        public const string SGOStudentsInterval = "SGOStudentsInterval";
        public const string SGOHomeDirection = "SGOHomeDirection";
        public const string SGOPreparednessGroupDirection = "SGOPreparednessGroupDirection";
        public const string SGOAdminReviewDirection = "SGOAdminReviewDirection";
        public const string SGOFinalSignoffDirection = "SGOFinalSignoffDirection";

        public const int ToBePlacedGroupOrder = 98;
        public const string ToBePlacedGroupName = "To Be Placed";
        public const int ExcludedGroupOrder = 99;
        public const string ExcludedGroupName = "Excluded";
        public const string SGODefaultWeek = "SGODefaultWeek";
        //--------------------------//\

        public const string Configuration_PreviewTestTeacherID = "PreviewTestTeacherID";

        public const string VaultKey = "CurrentVault";
        public const string LinkitSettingsKey = "LinkitSettingsKey";
        public const string LoginLimit = "LoginLimit";
        public const int LoginLimitDefault = 2;

        public const string ASPNETSessionId = "cksession";
        public const string ResetPasswordLimit = "ResetPasswordLimit";
        public const int ResetPasswordLimitDefault = 2;

        public const string StudentLoginFlag = "chyten";
        public const string DefaultCookieTimeOut = "DefaultCookieTimeOutMinutes";
        public const string LKARCookie = "LKARCookie";
        public const string DistrictDecode_NotUseSecureQuestion = "NotUseSecureQuestion";
        public const string DefaultDateFormat = "DefaultDateFormat";
        public const string DefaultTimeFormat = "DefaultTimeFormat";

        public const string DefaultJqueryDateFormat = "DefaultJqueryDateFormat";
        public const string IgnoreSingleLoginWord = "IgnoreSingleLoginWord";
        public const string PortalWarningTimeOutMinute = "PortalWarningTimeOutMinute";
        public const string WarningExpire = "WarningExpire";

        public const string DateFormat = "DateFormat";
        public const string TimeFormat = "TimeFormat";
        public const string JQueryDateFormat = "jQueryDateFormat";
        public const string HandsonTableDateFormat = "HandsonTableDateFormat";



        public const string DefaultDateFormatValue = "MM/dd/yyyy";//based on US style
        public const string DefaultTimeFormatValue = "h:mm tt";
        public const string DefaultJqueryDateFormatValue = "mm/dd/yy";
        public const string DefaultHandsonTableDateFormat = "MMM/DD/YY";
        public struct XLIFunction
        {
            public const string DistrictLibrary = "District Library";
            public const string CerticaLibrary = "NWEA Library";
            public const string ProgressLibrary = "Progress Library";
        }
        public const string ACTReportShowTableBorder = "ACTReport-ShowTableBorder";
        public const string SATReportShowTableBorder = "SATReport-ShowTableBorder";
        public const string NewACTReportShowTableBorder = "NewACTReport-ShowTableBorder";
        public const string NewSATReportShowTableBorder = "NewSATReport-ShowTableBorder";

        public const string ACTReportBoldZeroPercentScore = "ACTReport-BoldZeroPercentScore";
        public const string SATReportBoldZeroPercentScore = "SATReport-BoldZeroPercentScore";
        public const string NewACTReportBoldZeroPercentScore = "NewACTReport-BoldZeroPercentScore";
        public const string NewSATReportBoldZeroPercentScore = "NewSATReport-BoldZeroPercentScore";
        public const string Configuration_TLDSEmailTemplateSubmitToSchool = "TLDSEmailTemplateSubmitToSchool";
        public const string Configuration_TLDSEmailTemplateRejectedProfile = "TLDSEmailTemplateRejectedProfile";

        public const string IsSupportQuestionGroup = "IsSupportQuestionGroup";

        public const string IsLaunchTeacherLedTest = "IsLaunchTeacherLedTest";

        public const string IsCustomItemNaming = "ar_CustomItemNaming";

        public const string StudentOnlineTesting = "StudentOnlineTesting";

        public const string PreferenceTypeTestAssignment = "testassignment";
        public const string PreferenceTypeQTITestClassAssignment = "dlpublishing";

        private const string jQueryDateFormat = "jQueryDateFormat";

        public const string UserRoleIdCookie = "UserRoleIdCookie";
        public const string UserLogoutCookie = "UserLogoutCookie";
        public const string Deleted = "Deleted";

        ////---------TLDSProfileLink---------------//
        public const string TLDSProfileLink_Open = "Open";
        public const string TLDSProfileLink_InProgress = "In-Progress";
        public const string TLDSProfileLink_Completed = "Completed";
        public const string TLDSProfileLink_Expired = "Expired";
        public const string TLDSProfileLink_Deactivated = "Deactivated";
        public const string TLDSForm_SaveButtonText = "(Saved) Continue";
        public const string TLDSForm_OpenButtonText = "Open";
        public const string TLDSForm_SubmitButtonText = "Submitted";
        public const string TLDSSendMailSubmitedSectionTemplate = "TLDSSendMailSubmitedSectionTemplate";


        public const string SHOW_GOOGLE_LOGIN_BUTTON = "Show_Google_Login_Button";
        public const string SHOW_MICROSOFT_LOGIN_BUTTON = "Show_Microsoft_Login_Button";
        public const string SHOW_CLEVER_LOGIN_BUTTON = "Show_Clever_Login_Button";
        public const string SHOW_NYC_LOGIN_BUTTON = "Show_NYC_Login_Button";
        public const string SHOW_CLASSLINK_LOGIN_BUTTON = "Show_ClassLink_Login_Button";

        public const string UseMultiDateTemplate = "UseMulti-DateTemplate";

        public const string USER_CODE_VALIDATION = "UserCodeValidation";
        public const string STUDENT_CODE_VALIDATION = "StudentCodeValidation";
        public const string USERNAME_VALIDATION = "UserNameValidation";
        public const string SCHOOL_CODE_VALIDATION = "SchoolCodeValidation";
        public const string SCHOOL_STATE_CODE_VALIDATION = "SchoolStateCodeValidation";
        public const string STUDENT_ALT_CODE_VALIDATION = "StudentAltCodeValidation";
        public const string UserCodeValidation = "UserCodeValidation";
        public const string StudentCodeValidation = "StudentCodeValidation";

        public const string TemplateStudentParentRegistrationCode = "TemplateStudentParentRegistrationCode";
        public const string TemplateStudentRegistrationCode = "TemplateStudentRegistrationCode";

        public const string ALLOW_STUDENT_USER_GENERATION = "allowStudentUserGeneration";
        public const string STUDENT_SECRET_LETTERS = "StudentSecretLetters";
        public const string STUDENT_SECRET_NUMBERS = "StudentSecretNumbers";
        public const string STUDENT_SECRET_USERNAME_PATTERN = "StudentSecretUserNamePattern";
        public const string STUDENT_SECRET_PASSWORD_PATTERN = "StudentSecretPasswordPattern";

        public static string BOOL_STRING_TRUE = "true";
        public static string STUDENT_LOGIN_REQUIRE_KEY = "StudentPortal_Kiosk_Mode_Restriction";

        public static string ASSIGNMENT_TYPE_BUBBLESHEET = "BubbleSheet";
        public static string ASSIGNMENT_TYPE_ONLINE = "OnlineTest";


        public const string NAVIGATOR_SUBJECT_EMAIL_KEY = "NAVIGATOR_SUBJECT_EMAIL_KEY";
        public const string NAVIGATOR_TEMPLATE_EMAIL_BODY_ALL_ROLE = "NAVIGATOR_TEMPLATE_EMAIL_BODY_ALL_ROLE";
        public const string NAVIGATOR_SUBJECT_EMAIL_SUMMARY = "New LinkIt! Navigator Reports Available for [DistrictName]";


        public static string NAVIGATOR_TEMPLATE_EMAIL_BODY_SCHOOL_ADMIN = "NAVIGATOR_TEMPLATE_EMAIL_BODY_SCHOOL_ADMIN";
        public static string NAVIGATOR_TEMPLATE_EMAIL_BODY_TEACHER = "NAVIGATOR_TEMPLATE_EMAIL_BODY_TEACHER";
        public static string NAVIGATOR_TEMPLATE_EMAIL_BODY_STUDENT = "NAVIGATOR_TEMPLATE_EMAIL_BODY_STUDENT";
        public static string NAVIGATOR_TEMPLATE_EMAIL_BODY_DISTRICT_ADMIN = "NAVIGATOR_TEMPLATE_EMAIL_BODY_DISTRICT_ADMIN";
        public const int ALGORITHMIC_BRANCHING_NAVIGATION_METHOD = 14;
        public const int APAK_SCORING_METHOD = 9;
        public static string NAVIGATOR_DISTRICT_CODE_SUB_DOMAIN = "[districtcode]";

        public static string DISTRICTDATAPARM_IMPORTTYPE = "Attendance";
        public static string EnableAbilityToChangeTestCategory = "EnableAbilityToChangeTestCategory";
        public static string SURVEYTEMPLATENAME = "Survey Template";

        public const string ClassTestCodeLength = "ClassTestCodeLength";
        public const string StudentTestCodeLength = "StudentTestCodeLength";
        public const string ComparisonPasscodeLength = "ComparisonPasscodeLength";
        public const string PortalContain = "Portal";

        public const string SURVEY_EMAIL_TEMPLATE_DISTRIBUTE = "SURVEY_EMAIL_TEMPLATE_DISTRIBUTE";
        public const string DEFAULTSURVEYURL = "DefaultPortalURL";

        public const int ENTERPRISE_DISTRICT = 6006;
        public const string ENTERPRISE_FOLDER = "0";
        public const string REQUIRE_TEST_TAKER_AUTHENTICATION = "requireTestTakerAuthentication";
        public const string HIDE_LOGIN_CREDENTIALS = "Hide_Login_Credentials";

        public struct EDM
        {
            public const string API_ACCOUNT_PUBLIC_KEY = "EdmSignature";
            public const string PATH = "Path";

            public struct Configuration
            {
                public const string EDM_URL = "EDMUrl";
            }

            public struct ContentType
            {
                public const string JSON = "application/json";
            }

            public struct Endpoints
            {
                public const string GET_DOCUMENT_INFO = "api/document/{0}/info";
                public const string DOWNLOAD_DOCUMENT = "api/document/{0}/download";
                public const string DELETE_DOCUMENT = "api/document";
                public const string CREATE_DOCUMENT_INFO = "api/document/createInfo";
                public const string CREATE_DOCUMENT_META = "api/documentMeta/document/{0}";

                public const string CREATE_UPLOAD_LINK = "api/document/create-upload-link";
                public const string GET_DOWNLOAD_LINK = "api/document/{0}/download-link";
                public const string CANCEL_UPLOAD_MULTI_PART = "api/document/cancel-upload-multi-part";

                public const string ALIVE_CONFIRM_DOCUMENT = "api/document/{0}/alive-confirm";
                public const string UPDATE_PATH_ETAGS = "api/document/update-path-etags";
                public const string GET_DOCUMENT_INFO_LIST = "api/document/info/list";
            }

            public struct QueryString
            {
                public const string CLIENT_ID = "clientId";
                public const string TIMESTAMP = "timestamp";
                public const string SIGNATURE = "signature";
            }

            public struct Signature
            {
                public const string SIGNATURE_EXPIRED_TIME_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";
            }
        }

        public struct AdminReporting
        {
            public struct Configuration
            {
                public const string REPORTING_URL = "ReportingUrl";
            }

            public struct Endpoints
            {
                public const string VIEW_ARTIFACT = "document/{0}/viewer";

                // PC
                public const string DATALOCKER_HAS_ASSIGNED_DATA_POINT = "api/intervention-manager/performance-criteria/dl-has-assigned-data-point";
                public const string CLEAR_CACHE_MANAGER = "api/cache-manager/clear-redis-cache";
            }
        }

        public struct Artifact
        {
            public const string VIEW_ON_DS_SERVER_URL = "Artifact/View?documentGuid={0}";

            public struct Configuration
            {
                public const string RecordingOptions = "AssessmentArtifactRecordingOptions";
                public const string DocumentTypeId = "DocumentTypeId";
            }
        }


        public const string ORIGINAL_TEST = "ORIGINAL";
        public const string MODIFIEDBY_ROSTERLOADER = "RosterLoader";
        public const string MODIFIEDBY_FOCUS_GROUP_AUTOMATION = "Focus Group Automation";
        public const string PERFORMANCE_BAND_VIRTUAL_TEST_DISTRICT = "District";
        public const string PERFORMANCE_BAND_VIRTUAL_TEST_ENTERPRISE = "Enterprise";
        public const string PERFORMANCE_BAND_VIRTUAL_TEST_NONE = "None";
        public const string FULLACCESS = "Full Access";
        public const string NOACCESS = "No Access";
        public const string PARTIALACCESS = "Partial Access";
        public const string CATEGORY = "category";
        public const string TEST = "test";

        public const string LINKIT_URL_KEY = "LinkItUrl";
        public const string CLASS_META_DATA = "ClassMetaData";

        public const string CORS_SETTING_KEY = "CorsSettings";

        #region ScopyTypes
        public const string ScoreRaw = "ScoreRaw";
        public const string ScoreScaled = "ScoreScaled";
        public const string ScorePercent = "ScorePercent";
        public const string ScorePercentage = "ScorePercentage";
        public const string ScoreCustomN_1 = "ScoreCustomN_1";
        public const string ScoreCustomN_2 = "ScoreCustomN_2";
        public const string ScoreCustomN_3 = "ScoreCustomN_3";
        public const string ScoreCustomN_4 = "ScoreCustomN_4";
        #endregion
    }
}
