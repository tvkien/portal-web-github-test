using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Models.Constants
{
    public static class EmailSetting
    {
        public static bool SendMailUsingCredential { get; } = ConfigurationManager.AppSettings["SendMailUsingCredential"].ConvertToBool();
        public static string LinkItFromEmail { get; } = ConfigurationManager.AppSettings["LinkItFromEmail"];
        public static string LinkItUseEmailCredentialKey { get; } = ConfigurationManager.AppSettings["LinkItUseEmailCredentialKey"];


        public static bool EmailSGOUsingCredential { get; } = ConfigurationManager.AppSettings["EmailSGOUsingCredential"].ConvertToBool();
        public static string SGOEmailSender { get; } = ConfigurationManager.AppSettings["SGOEmailSender"];
        public static string SGOUseEmailCredentialKey { get; } = ConfigurationManager.AppSettings["SGOUseEmailCredentialKey"];

        public static bool EmailTLDSUsingCredential { get; } = ConfigurationManager.AppSettings["EmailTLDSUsingCredential"].ConvertToBool();
        public static string TLDSEmailSender { get; } = ConfigurationManager.AppSettings["TLDSEmailSender"];
        public static string TLDSUseEmailCredentialKey { get; } = ConfigurationManager.AppSettings["TLDSUseEmailCredentialKey"];

        public static bool EmailUtilUsingCredential { get; } = ConfigurationManager.AppSettings["EmailUtilUsingCredential"].ConvertToBool();
        public static string EmailUtilSender { get; } = ConfigurationManager.AppSettings["EmailUtilSender"];
        public static string EmailUtilUseEmailCredentialKey { get; } = ConfigurationManager.AppSettings["EmailUtilUseEmailCredentialKey"];

        public static bool SendMailUsingCredentialRegistration { get; } = ConfigurationManager.AppSettings["SendMailUsingCredentialRegistration"].ConvertToBool();
        public static string FromRegistration { get; } = ConfigurationManager.AppSettings["FromRegistration"];
        public static string SubjectRegistration { get; } = ConfigurationManager.AppSettings["SubjectRegistration"];
        public static string SubjectRegistrationStudent { get; } = ConfigurationManager.AppSettings["SubjectRegistrationStudent"];
        public static string RegistrationUseEmailCredentialKey { get; } = ConfigurationManager.AppSettings["RegistrationUseEmailCredentialKey"];

        public static bool SendMailUsingCredentialNavigator { get; } = ConfigurationManager.AppSettings["SendMailUsingCredential-Navigator"].ConvertToBool();
        public static string SmtpFromNavigator { get; } = ConfigurationManager.AppSettings["SmtpFrom-Navigator"];
        public static string NavigatorUseEmailCredentialKey { get; } = ConfigurationManager.AppSettings["NavigatorUseEmailCredentialKey"];
    }

    public static class StringExtention
    {
        public static bool ConvertToBool(this string stringToBool)
        {
            var isBool = bool.TryParse(stringToBool, out bool value);
            return isBool && value;
        }

        public static int ConvertToInt(this string stringToInt)
        {
            var isInt = int.TryParse(stringToInt, out int value);
            return isInt ? value : 0;
        }
    }
}
