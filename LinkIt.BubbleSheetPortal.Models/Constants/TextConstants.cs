namespace LinkIt.BubbleSheetPortal.Models.Constants
{
    public class TextConstants
    {
        public const string ACTIVE = "Active";
        public const string DEACTIVE = "Deactive";
        public const string INACTIVE = "Inactive";

        // TLDS Text constants
        public const string DUPLICATED_GROUP_NAME = "The name is already in use. Please use a different name.";

        public const string ALL = "All";
        public const string SELECT_GROUP = "Select Group";
        public const string DEFAULT_DATE_FORMAT_AU = "dd-MMM-yy";
        public const string TLDS_DATE_FORMAT_LOGIN = "dd/MM/yyyy";

        // Navigator report
        public const string REPORT_STATUS_NEW = "New";

        public const string REPORT_STATUS_ERROR = "Error";
        public const string REPORT_STATUS_PROCESSING = "Processing";
        public const string REPORT_STATUS_SPLITING = "Splitting";
        public const string REPORT_STATUS_UPLOADING = "Uploading to S3";
        public const string REPORT_STATUS_VALIDATING = "Validating";
        internal static readonly string DISTRICT_ADMIN = "District Admin";
        internal static readonly string SCHOOL_ADMIN = "School admin";
        internal static readonly string TEACHER = "Teacher";
        internal static readonly string STUDENT = "Student";
        public static string REPORT_STATUS_SUCCESSED = "Succeeded";
        public static string REPORT_STATUS_DELETED = "Deleted";
        public static string REPORT_STATUS_NOTFOUND = "Not found";
        public static string SELECT_REPORT_FIRST = "You must select reports first.";
        public static string CONTENT_TYPE_PDF = "application/pdf";
        public static string CONTENT_TYPE_ZIP = "application/zip";
        public static string CANNOT_DOWNLOAD_FILE_FROM_S3 = "Cannot download file from server";
        public static string NOTIFICATION_TYPE_NAVIGATOR_REPORT = "NavigatorReport";
        public static string NAVIGATOR_CATEGORY_TEACHER_SLIDES = "Benchmark Teacher Slides";
        public static string NAVIGATOR_CATEGORY_FINGERTIP = "Fingertips";

        // Notification
        public static string NOTIFICATION_PUBLISHED_STATUS = "Published";

        public static string NOTIFICATION_UNPUBLISHED_STATUS = "Unpublished";

        // Configurations
        public static string TESTDESIGN_CONFIG_NAVIGATION_METHODS = "TESTDESIGN_CONFIG_NAVIGATION_METHODS";

        // Master Standard
        public static string STATE_CODE_AC = "ac";

        public static string STATE_CODE_CC = "cc";
        public static string STATE_CODE_AP = "ap";

        public static string META_FORMAT_DATE = "MetaFormatDate";
        public static string META_TYPE_DATE = "Date";

        public static string STUDENT_RACE = "Student.Race";

        //Login
        public static string LOGIN_ROLE_PARENT = "parent";

        public static string LOGIN_ROLE_STUDENT = "student";

        public static string IS_OVERWRITE_RESULTS = "IsOverwriteResults";

        public static string MODIFIED_BY_DEFAULT = "SSO";

        public static string GRADE_OTHER = "Other";
        public static string SURVEY_PREFERENCES_LABEL = "survey";
        public static string TEST_PREFERENCES_LABEL = "test";
        public static string SECURITY_PREFERENCES_LABEL = "security";
        public const string PASSWORD_RESET_MESSAGE = "If the LinkIt! username is associated with your email address, you will receive a password reset link. The link will expire in {0} minutes. If you do not receive the email, check your Spam folder and contact your administrator for support before making another reset request.";
        public const string NOT_FOUND_CLEVER_DISTRICT_ID_ON_VAULT = "NOT_FOUND_CLEVER_DISTRICT_ID_ON_VAULT";

        public const string EXIST_TEST_IN_PROGRESS = "This question cannot be added because there is already a test In Progress.";
        public const string IMPORT_INVALID_MULTIPART = "One or more multi-part items are currently unsupported. Please choose another item from the item bank.";

        public const string ACCOUNT_NO_PASSWORD = "This account has no password. Please use the registration code to update a new password.";

        // Security Settings
        public const string ENABLE_MFA_EMAIL_PUBLISHER = "enableMFAEmail_publisher";
        public const string ENABLE_MFA_EMAIL_NETWORKADMIN = "enableMFAEmail_networkAdmin";
        public const string ENABLE_MFA_EMAIL_DISTRICTADMIN = "enableMFAEmail_districtAdmin";
        public const string ENABLE_MFA_EMAIL_SCHOOLADMIN = "enableMFAEmail_schoolAdmin";
        public const string ENABLE_MFA_EMAIL_TEACHER = "enableMFAEmail_teacher";
        public const string ENABLE_MFA_EMAIL_STUDENT = "enableMFAEmail_student";
        public const string ENABLE_MFA_EMAIL_PARENT = "enableMFAEmail_parent";
        public const string ENABLE_MFA_EMAIL_USER = "enableMFAEmail_user";
    }
}
