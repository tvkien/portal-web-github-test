using ICSharpCode.SharpZipLib.Zip;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestMaker.S3VirtualTest;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTSummaryReport;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SATSummaryReport;
using Microsoft.IdentityModel.Tokens;
using S3Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TestMaker;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.Old.Configugration;
using LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public static class Util
    {
        #region Contain

        public const string AssessmentItem = "ASSESSMENTITEM";
        public const string AssessmentAchievedDetail = "ASSESSMENTACHIEVEDDETAIL";
        public const string AssessmentItemResponse = "ASSESSMENTITEMRESPONSE";
        public const string ASSMNT_ITEMR_ACADEMIC_STDS = "ASSMNT_ITEMR_ACADEMIC_STDS";
        public const string ASSMNT_SUBTEST_ACADEMIC_STDS = "ASSMNT_SUBTEST_ACADEMIC_STDS";
        public const string ASSESSMENT_FACT = "ASSESSMENT_FACT";
        public const string ASSESSMENT_RESPONSE = "ASSESSMENT_RESPONSE";
        public const string ASSESSMENT_ACC_MOD_FACT = "ASSESSMENT_ACC_MOD_FACT";
        public const string TEST_TEMPLATE = "TEST";
        public const string QUESTION_TEMPLATE = "QUESTION";
        public const string TESTRESULT_TEMPLATE = "TEST_RESULT";
        public const string POINTSEARNED_TEMPLATE = "POINTS_EARNED";
        public const string STUDENTRESPONSE_TEMPLATE = "STUDENT_RESPONSE";
        public const string CLASSTESTASSIGNMENT_TEMPLATE = "CLASS_TEST_ASSIGNMENT";
        public const string ROSTER_TEMPLATE = "ROSTER";
        public const string USER_TEMPLATE = "USER";
        public const string RefreshMenu = "RefreshMenu";

        public const string FromNumberError =
            "Can not send sms right now. Unknow which number is used for sending sms service!";

        public const string ItemBankConstant = "itembank";
        public const int LessonFileTypeUrlId = 22; //table LessonFileType where Name='url''
        public const string ItemSetConstant = "itemset";
        public const int CCStateID = 78;
        public static readonly string[] GradeNames = { "7", "8", "9", "10", "11", "12" };
        public const string PublishedToDistrictDistrictAdminOnly = "PublishedToDistrictDistrictAdminOnly";
        public const string Parent = "parent";
        public const string Student = "student";
        public const string HomeZip = "homezip";
        public const string HasAccessToReport = "HasAccessToReport";

        public const string DistrictDecode_TestScoreExtract = "test_score_extract";
        public const string DistrictDecode_DefaultTemplates = "TestExtract_DefaultTemplates";
        public const string DistrictDecode_HideStudentName = "TestExtract_HideStudentName";
        public const string DistrictDecode_EmailTemplate = "TestExtract_EmailTemplate";
        public const string DistrictDecode_EmailSubject = "TestExtract_EmailSubject";
        public const string DistrictDecode_EmailFrom = "TestExtract_EmailFrom";
        public const string DistrictDecode_SGOStartingPoints = "SGOStartingPoints";
        public const string DistrictDecode_SGOScoringPlan = "SGOScoringPlan";
        public const string DistrictDecode_SGOUnstructuredScoringPlan = "SGOUnstructuredScoringPlan";
        public const string DistrictDecode_SGORationale = "SGORationale";
        public const string DistrictDecode_SendTestResultToGenesis = "gradebookSIS";

        public const string DistrictDecode_SGOHomeDirection = "SGOHomeDirection";
        public const string DistrictDecode_SGODataPointDirection = "SGODataPointDirection";
        public const string DistrictDecode_SGORationaleAndPostAssessmentGuidanceDirection = "SGORationaleAndPostAssessmentGuidanceDirection";
        public const string DistrictDecode_SGOPreparednessGroupDirection = "SGOPreparednessGroupDirection";
        public const string DistrictDecode_SGOScoringPlansDirection = "SGOScoringPlansDirection";
        public const string DistrictDecode_SGOAdminReviewDirection = "SGOAdminReviewDirection";
        public const string DistrictDecode_SGOProgressMonitorAndScoreDirection = "SGOProgressMonitorAndScoreDirection";
        public const string DistrictDecode_SGOFinalSignoffDirection = "SGOFinalSignoffDirection";

        public const string DistrictDecode_OpenAllApplicationInSameTab = "OpenAllApplicationInSameTab";
        public const string DistrictDecode_Portal_UseNewDesign = "Portal_UseNewDesign";

        public const string QTIOnlineSessionStatus_Created = "Created";
        public const string QTIOnlineSessionStatus_Started = "Started";
        public const string QTIOnlineSessionStatus_Paused = "Paused";
        public const string QTIOnlineSessionStatus_Completed = "Completed";
        public const string SchoolZip = "schoolzip";
        public const string SchoolId = "schoolid";
        public const string UserId = "userid";
        public const string CreatedBy = "createdby";
        public const string SchoolStudentTest = "SchoolStudentTest";
        public const string CHYTEN_EmailSubject = "CHYTEN_StudentRegistrationEmailSubject";
        public const string CHYTEN_EmailContent = "CHYTEN_StudentRegistrationEmailContent";
        public const string CHYTEN_CredentialInformation = "CHYTEN_CredentialInformation";
        public const string CHYTEN_EnableStudentLogin = "EnableStudentLogin";
        public const string CHYTEN_BankStudentTest = "BankStudentTest";
        public const string CEE_LICode = "CEE";
        public const int CHYTEN_DistrictID = 2754;

        public const string QTIOnlineSessionStatus_WaitingForReview = "WaitingForReview";

        public const int ACTSATReportContentOption_ScoreOnly = 1;
        public const int ACTSATReportContentOption_ScoreAndEssay = 2;

        public const int ACTSATReportContentOption_EssayOnly = 3;

        public const int BubbleSheetFileEssayPageType = 3;

        public const string KNOWSYS_EssayComment_ScoreKey = "KNOWSYS_EssayComment_Score_{0}";
        public const string KNOWSYS_SATEssayComment_ScoreKey = "KNOWSYS_SATEssayComment_Score_{0}";
        public const string KNOWSYS_EssayComment_Title = "KNOWSYS_EssayComment_Title";
        public const string KNOWSYS_SATEssayComment_Title = "KNOWSYS_SATEssayComment_Title";
        public const string KNOWSYS_SATScoreRange = "KNOWSYS_SATScoreRange";

        public const string KNOWSYS_SATReport_SectionPageBreak = "KNOWSYS_SATReport_SectionPageBreak";
        public const string KNOWSYS_SATReport_ShowScoreRange = "KNOWSYS_SATReport_ShowScoreRange";
        public const string KNOWSYS_SATReport_ShowSectionScoreScaled = "KNOWSYS_SATReport_ShowSectionScoreScaled";
        public const string KNOWSYS_SATReport_ShowAssociatedTagName = "KNOWSYS_SATReport_ShowAssociatedTagName";
        public const string KNOWSYS_SATReport_ShowEssay = "KNOWSYS_SATReport_ShowEssay";
        public const string KNOWSYS_SATReport_ShowComment = "KNOWSYS_SATReport_ShowComment";

        public const string NewACTReport_ShowComment = "NewACTReport_ShowComment";
        public const string NewACTEssayComment_Title = "NewACTEssayComment_Title";
        public const string NewACTEssayComment_ScoreKey = "NewACTEssayComment_Tag_{0}_Score_{1}";

        public const string NewSATReport_ShowComment = "NewSATReport_ShowComment";
        public const string NewSATEssayComment_Title = "NewSATEssayComment_Title";
        public const string NewSATEssayComment_ScoreKey = "NewSATEssayComment_Tag_{0}_Score_{1}";

        public const string NewACTEssayComment_ScoreRangeKey = "NewACTEssayComment_Tag_{0}_ScoreRange";
        public const string NewSATEssayComment_ScoreRangeKey = "NewSATEssayComment_Tag_{0}_ScoreRange";

        public const string PreviewTestDistrictID = "PreviewTestDistrictID";
        public const string PreviewTestClassID = "PreviewTestClassID";
        public const string PreviewTestSchoolID = "PreviewTestSchoolID";
        public const string PreviewTestTermID = "PreviewTestTermID";
        public const string PreviewTestTeacherID = "PreviewTestTeacherID";
        public const string PreviewTestStudentID = "PreviewTestStudentID";
        public const string PreviewQTIItemTestCode = "PreviewQTIItemTestCode";
        public const string KNOWSYS_SATReport_IncludeStateInformation = "KNOWSYS_SATReport_IncludeStateInformation";
        public const string KNOWSYS_SATReport_StateInformationImage = "KNOWSYS_SATReport_StateInformationImage";

        public const string ACTReport_TagTable_UseAlternativeStyle = "ACTReport_TagTable_UseAlternativeStyle";
        public const string SATReport_TagTable_UseAlternativeStyle = "SATReport_TagTable_UseAlternativeStyle";

        public const string KnowsysSATCustomFooter = "KnowsysSATCustomFooter";
        public const string Config_Resource_MimeType = "RESOURCE_MIMETYPE";
        public const string IsShow_AddNewStudentButton = "IsShow_AddNewStudentButton";
        public const string IsShowIconHelpTextInfo = "IsShowIconHelpTextInfo";
        public const string Ables_UoM_Bank = "Ables/UoM_Bank";
        public const string GoogleAnalyticsTrackingScript = "GoogleAnalyticsTrackingScript";
        public const string IsShowTutorialMode = "IsShowTutorialMode";
        public const string IsStudentInformationSystem = "IsStudentInformationSystem";
        public const string BubbleSheetIntervalAutoSave = "BubbleSheetIntervalAutoSave";

        public const string HideSupportHighlightText = "HideSupportHighlightText";

        public const string AssignOnlineTest_DefaultTestFilterMode = "AssignOnlineTest_DefaultTestFilterMode";

        //PassThrough V-DET
        public const string PassThroughVDETStatusSuccess = "Success";

        public const string PassThroughVDETStatusFail = "Error";

        public const string PassThroughVDETCode101 = "101";
        public const string PassThroughVDETMessage101 = "Invalid API public key";

        public const string PassThroughVDETCode102 = "102";
        public const string PassThroughVDETMessage102 = "API Account not authorized to use this function";

        public const string PassThroughVDETCode103 = "103";
        public const string PassThroughVDETMessage103 = "SAML Token expired";

        public const string PassThroughVDETCode104 = "104";
        public const string PassThroughVDETMessage104 = "Invalid SAML signature";

        public const string PassThroughVDETCode105 = "105";
        public const string PassThroughVDETMessage105 = "Invalid SAML format";

        public const string PassThroughVDETCode106 = "106";
        public const string PassThroughVDETMessage106 = "Invalid Action Type";

        public const string PassThroughVDETCode107 = "107";
        public const string PassThroughVDETMessage107 = "Invalid Return URL";

        public const string PassThroughVDETCode108 = "108";
        public const string PassThroughVDETMessage108 = "Invalid Sector Code";

        public const string PassThroughVDETMessage114 = "Invalid SAML Email address";

        public const string PassThroughVDETCode109 = "109";
        public const string PassThroughVDETMessage109 = "User ID not found";

        public const string PassThroughVDETCode110 = "110";
        public const string PassThroughVDETMessage110 = "Failed to process SAML token";

        public const string PassThroughVDETCode111 = "111";
        public const string PassThroughVDETMessage111 = "User ID required";

        public const string PassThroughVDETCode112 = "112";
        public const string PassThroughVDETMessage112 = "Sector Code Required";

        public const string PassThroughVDETCode113 = "113";
        public const string PassThroughVDETMessage113 = "Your login credentials cannot be associated with a user in our system. Please contact your system administrator if you believe you have received this message in error.";

        public const string PassThroughVDETCode200 = "200";
        public const string PassThroughVDETMessage200 = "Logoff Successful";

        public const string PassThroughVDETCode201 = "201";
        public const string PassThroughVDETMessage201 = "Login Successful";

        public const string ACTReportFolder = "ACTReports";

        public const string PreBBSLevel = "bbsassignment";
        public const string PreBBSLable = "bbstest";
        //\

        #endregion Contain

        #region "Token"

        public const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        #endregion "Token"

        public static string[] SplitString(string str, char ch)
        {
            if (string.IsNullOrEmpty(str))
                return new string[0];
            return str.Split(ch);
        }

        public static string GenerateRandonStringKey(int length)
        {
            string key = "";
            while (key.Length < length)
            {
                key += Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[^0-9a-zA-Z]", "");
            }

            return key.Substring(0, length);
        }

        public static string FormatRubricFileName(string inputFileName)
        {
            const int fileLength = 30;

            if (inputFileName.Length > fileLength)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
                var fileExtension = Path.GetExtension(inputFileName);

                if (!string.IsNullOrEmpty(fileExtension))
                {
                    return fileNameWithoutExtension.Substring(0, fileLength - 3 - fileExtension.Length) + "..." +
                           fileExtension;
                }
                else
                {
                    return fileNameWithoutExtension.Substring(0, fileLength - 3) + "...";
                }
            }
            else
            {
                return inputFileName;
            }
        }

        public static string WriteFileTemp(string strContent, string strFileName, string strfolderPath)
        {
            if (!Directory.Exists(strfolderPath))
            {
                Directory.CreateDirectory(strfolderPath);
            }
            var strFileNamePath = Path.Combine(strfolderPath, strFileName + "_" + DateTime.Now.Ticks + ".txt");
            if (File.Exists(strFileNamePath) == false)
            {
                try
                {
                    var fileTest = new StreamWriter(strFileNamePath);
                    fileTest.Write(strContent);
                    fileTest.Close();
                    fileTest.Dispose();
                }
                catch (Exception ex1)
                {
                    PortalAuditManager.LogException(ex1);
                    return ex1.ToString();
                }
            }
            return string.Empty;
        }

        public static string WriteFileTemp(IEnumerable<string> strContent, string strFileName, string strfolderPath)
        {
            if (!Directory.Exists(strfolderPath))
            {
                Directory.CreateDirectory(strfolderPath);
            }
            var strFileNamePath = Path.Combine(strfolderPath, strFileName + "_" + DateTime.Now.Ticks + ".txt");
            if (!File.Exists(strFileNamePath))
            {
                try
                {
                    File.WriteAllLines(strFileNamePath, strContent);
                }
                catch (Exception ex1)
                {
                    PortalAuditManager.LogException(ex1);
                    return ex1.ToString();
                }
            }
            return string.Empty;
        }

        public static string ZipfolderExport(string strfolderpath)
        {
            var zip = new FastZip();
            FileInfo fInfo = new FileInfo(strfolderpath);
            string dest = fInfo.Directory + "\\" + fInfo.Name + ".zip";
            try
            {
                zip.CreateZip(dest, strfolderpath, true, null);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return dest;
            }
            return dest;
        }

        public static string WriteTracker(string strContent, string strFolder)
        {
            var strFileNamePath = Path.Combine(strFolder, "StopWatch_" + DateTime.Now.Ticks + ".txt");
            if (File.Exists(strFileNamePath) == false)
            {
                try
                {
                    var fileTest = new StreamWriter(strFileNamePath);
                    fileTest.Write(strContent);
                    fileTest.Close();
                    fileTest.Dispose();
                }
                catch (Exception ex1)
                {
                    PortalAuditManager.LogException(ex1);
                    return ex1.ToString();
                }
            }
            return string.Empty;
        }

        public static string ConvertHtmlToClearText(string html, int maxLength)
        {
            // Convert line break to whitespace
            html = Regex.Replace(html, "<br>|<br/>|<br />|<p>", " ", RegexOptions.IgnoreCase);

            // Remove html tag
            var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            string cleatText = reg.Replace(html, "");

            if (maxLength > 0 && cleatText.Length > maxLength)
                cleatText = cleatText.Substring(0, maxLength);

            return cleatText;
        }

        public static string EmailDisplayCreatedDateTime(DateTime createdDateTime)
        {
            if (createdDateTime.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                return createdDateTime.ToString("h:mm tt");
            }
            else
            {
                return createdDateTime.ToString("MMM d");
            }
        }

        public static string EmailDisplayFullCreatedDateTime(DateTime createdDateTime)
        {
            return createdDateTime.ToString("MMM d, yyyy") + " at " + createdDateTime.ToString("h:mm tt");
        }

        public static QtiItemXmlContentData BindQtiItemXmlContentFromXml(string xmlContent)
        {
            var qtiItemXmlContent = new QtiItemXmlContentData() { MaxChoices = 0, Cardinality = string.Empty };
            try
            {
                if (string.IsNullOrWhiteSpace(xmlContent)) return qtiItemXmlContent;
                var doc = ServiceUtil.LoadXmlDocument(xmlContent);

                var elemList = doc.GetElementsByTagName("choiceInteraction");
                if (elemList.Count == 0) return qtiItemXmlContent;
                var xmlAttributeCollection = elemList[0].Attributes;
                if (xmlAttributeCollection == null) return qtiItemXmlContent;

                if (xmlAttributeCollection["maxChoices"] != null)
                {
                    var attrVal = xmlAttributeCollection["maxChoices"].Value.ToLower();
                    qtiItemXmlContent.MaxChoices = CommonUtils.ConverStringToInt(attrVal, 0);
                }

                // Get Cardinality
                elemList = doc.GetElementsByTagName("responseDeclaration");
                if (elemList.Count == 0) return qtiItemXmlContent;
                xmlAttributeCollection = elemList[0].Attributes;
                if (xmlAttributeCollection == null) return qtiItemXmlContent;

                if (xmlAttributeCollection["cardinality"] != null)
                {
                    var attrVal = xmlAttributeCollection["cardinality"].Value;
                    qtiItemXmlContent.Cardinality = attrVal;
                }

                return qtiItemXmlContent;
            }
            catch
            {
                //TODO: Should Log exception
                return qtiItemXmlContent;
            }
        }

        public static string ProcessWildCharacters(string s)
        {
            if (s == null)
            {
                return null;
            }
            return s.Replace("'", "''");
        }

        public static string ReplaceWeirdCharactersCommon(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            var result = str.ReplaceWeirdCharacters();
            result =
                result.Replace("<p><span><list listStylePosition=\"outside\" listStyleType=\"decimal\" paragraphSpaceAfter=\"12\" styleName=\"passageNumbering\"><listMarkerFormat><ListMarkerFormat color=\"#aaaaaa\" paragraphEndIndent=\"20\"/></listMarkerFormat>", "<ol>")
                .Replace("<list ", "<ol ")
                .Replace("</list></span></p>", "</ol>")
                .Replace("</list>", "</ol>");
            result = ReplaceVideoTag(result);

            return result;
        }

        public static string DecodeHtmlCharacter(string str)
        {
            MatchCollection mc = Regex.Matches(str, @"&#\s*;");
            foreach (Match m in mc)
            {
                str = str.Replace(m.ToString(), System.Web.HttpUtility.HtmlDecode(m.ToString()));
            }
            return str;
        }

        public static string ConvertByteArrayStringToUtf8String(string byteArrayString)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(byteArrayString))
            {
                //split tagToSearch
                var temp = byteArrayString.Split(',');
                //convert temp to byte
                List<byte> bytes = new List<byte>();

                foreach (var s in temp)
                {
                    bytes.Add(byte.Parse(s));
                }

                result = System.Text.Encoding.UTF8.GetString(bytes.ToArray());
            }
            return result;
        }

        public static string ConvertUtf8StringToUtf8ByteString(string str)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            string result = string.Empty;
            foreach (var b in bytes)
            {
                result += b.ToString() + ",";
            }
            //remove the last ','
            if (result.Length > 0)
            {
                result = result.Remove(result.Length - 1);
            }
            return result;
        }

        public static string ReplaceTagListByTagOl(string xmlContent, bool generagePDF = false)
        {
            string listTag1 =
                @"<list listStylePosition=""outside"" listStyleType=""decimal"" paragraphSpaceAfter=""12"" styleName=""passageNumbering""><listMarkerFormat><ListMarkerFormat color=""#aaaaaa"" paragraphEndIndent=""20""/></listMarkerFormat>";
            string listTag2 =
                @"<list listStylePosition=""outside"" listStyleType=""decimal"" paragraphSpaceAfter=""12"" styleName=""passageNumbering""><listMarkerFormat><ListMarkerFormat color=""#aaaaaa"" paragraphEndIndent=""20""></ListMarkerFormat></listMarkerFormat>";
            //string olTag = @"<ol style=""list-style-type:decimal;padding-left:50px"">";
            string olTag = @"<ol style=""list-style-type:decimal;"">";
            //sometime there is a close </ListMarkerFormat>
            if (generagePDF)
            {
                //<p><span>&nbsp;</span> was added to adapt PDF Generator. Without <p><span>&nbsp;</span> PDF Generator will generate identifier wich has only number list outside the border ( see LNKT-6976)
                olTag = @"<p><span>&nbsp;</span></p><ol style=""list-style-type:decimal;padding-left:3px !important"">";
                //<p><span>&nbsp;</span> was added to adapt PDF Generator. Without <p><span>&nbsp;</span> PDF Generator will generate identifier wich has only number list outside the border ( see LNKT-6976)
            }
            //if XmlContent contains list tag, it must be replaced by ol
            //Unable to replace on client be cause tml.Raw(ViewBag.HtmlContent) will render tag list unsuccessfully
            xmlContent = xmlContent.Replace(listTag1, olTag);
            xmlContent = xmlContent.Replace(listTag2, olTag);
            xmlContent = xmlContent.Replace("<list ", "<ol ");
            xmlContent = xmlContent.Replace("</list>", "</ol>");
            return xmlContent;
        }

        public static string ReplaceXmlNamespace(string xmlContent)
        {
            //some xmlcontent contains "<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />" that will lead to error when LoadXml
            //just replace to "<!--?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" /-->"
            string xmlNamespace =
                "<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = \"http://www.w3.org/1998/Math/MathML\" />";
            string replacedXmlNamespace =
                "<!--?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = \"http://www.w3.org/1998/Math/MathML\" /-->";
            xmlContent = xmlContent.Replace(xmlNamespace, replacedXmlNamespace);
            return xmlContent;
        }

        public static string ReplaceTagListByTagOlForPassage(string xmlContent, bool generagePDF = false)
        {
            //remove tag listMarkerFormat
            var doc = new XmlContentProcessing(xmlContent);
            doc.RemoveSingleTag("listMarkerFormat");
            doc.RemoveSingleTag("listmarkerformat");
            xmlContent = doc.GetXmlContent();

            string listTag1 =
                "<list listStylePosition=\"outside\" listStyleType=\"decimal\" paragraphSpaceAfter=\"12\" styleName=\"passageNumbering\"><listMarkerFormat><ListMarkerFormat color=\"#aaaaaa\" paragraphEndIndent=\"20\" /></listMarkerFormat>";
            string listTag2 = "<list style=\"margin-left: 0px;\">";
            string listTag3 =
                "<list listStylePosition=\"outside\" listStyleType=\"decimal\" paragraphSpaceAfter=\"12\" styleName=\"passageNumbering\"><listMarkerFormat><ListMarkerFormat color=\"#aaaaaa\" paragraphEndIndent=\"20\"/></listMarkerFormat>";
            string olTag = @"<ol style=""list-style-type:decimal;"">";
            if (generagePDF)
            {
                olTag = @"<p><span>&nbsp;</span></p><ol style=""list-style-type:decimal;padding-left:3px !important"">";
            }
            //if XmlContent contains list tag, it must be replaced by ol
            //Unable to replace on client be cause tml.Raw(ViewBag.HtmlContent) will render tag list unsuccessfully
            xmlContent = xmlContent.Replace(listTag1, olTag);
            xmlContent = xmlContent.Replace(listTag3, olTag);
            xmlContent = xmlContent.Replace(listTag2, olTag);
            //remove ListMarkerFormat
            //xmlContent =
            //    xmlContent.Replace(
            //        "<listMarkerFormat><ListMarkerFormat color=\"#aaaaaa\" paragraphEndIndent=\"20\"/></listMarkerFormat>",
            //        "");
            xmlContent = xmlContent.Replace("<list", "<ol");
            xmlContent = xmlContent.Replace("</list>", "</ol>");
            return xmlContent;
        }

        public static string FormatFullname(string fullname)
        {
            if (!string.IsNullOrEmpty(fullname))
            {
                fullname = fullname.Trim();
                if (fullname[0] == ',')
                    return fullname.Substring(1);
                if (fullname[fullname.Length - 1] == ',')
                    return fullname.Substring(0, fullname.Length - 1);

                return fullname;
            }

            return fullname;
        }

        public static string ReadValue(string key, string defaultValue)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Any(k => k.Equals(key)))
            {
                return ConfigurationManager.AppSettings[key];
            }
            return defaultValue;
        }

        public static int ReadValue(string key, int defaultValue)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Any(k => k.Equals(key)))
            {
                int i = 0;
                if (int.TryParse(ConfigurationManager.AppSettings[key], out i))
                    return i;
            }
            return defaultValue;
        }

        public static string FormatACTSummaryScore(decimal score, ActSummaryReportType type)
        {
            if (score == -1)
                return "-";

            var scoreString = Math.Round(score, MidpointRounding.AwayFromZero).ToString();

            return scoreString;
        }

        public static string FormatSATSummaryScore(decimal score, SATSummaryReportType type)
        {
            if (score == -1)
                return "-";

            var roundedScore = Math.Round(score, MidpointRounding.AwayFromZero);
            if (roundedScore < 0) roundedScore = 0;
            var scoreString = roundedScore.ToString();

            return scoreString;
        }

        public static List<PassageViewModel> GetPassageList(string xmlContent, bool qti3p, string urlPath = null, bool from3pUpload = false, bool addNumberFirst = false, Qti3pPassageService qti3pPassageService = null, QTIRefObjectService qtiRefObjectService = null, DataFileUploadPassageService dataFileUploadPassageService = null)
        {
            List<PassageViewModel> result = new List<PassageViewModel>();

            var doc = ItemSetPrinting.LoadXmlReferenceObjects(xmlContent);

            var objectNodes = doc.GetElementsByTagName("object");
            if (objectNodes != null)
            {
                for (int i = 0; i < objectNodes.Count; i++)
                {
                    var node = objectNodes[i];
                    PassageViewModel passage = new PassageViewModel();
                    try
                    {
                        if (node.Attributes["data"] != null)
                        {
                            passage.Data = node.Attributes["data"].Value;
                        }
                        else
                        {
                            passage.Data = string.Empty;
                        }

                        if (node.Attributes["dataFileUploadPassageID"] != null)
                        {
                            //Progressive DataFile Upload
                            passage.DataFileUploadPassageID = Int32.Parse(node.Attributes["dataFileUploadPassageID"].Value);
                        }
                        if (node.Attributes["dataFileUploadTypeID"] != null)
                        {
                            //Progressive DataFile Upload
                            passage.DataFileUploadTypeID = Int32.Parse(node.Attributes["dataFileUploadTypeID"].Value);
                        }

                        if (node.Attributes["Qti3pPassageID"] != null)
                        {
                            //Progressive DataFile Upload in ThirdPartyItemBank
                            passage.Qti3pPassageID = Int32.Parse(node.Attributes["Qti3pPassageID"].Value);
                        }
                        if (node.Attributes["Qti3pSourceID"] != null)
                        {
                            //Progressive DataFile Upload in ThirdPartyItemBank
                            passage.Qti3pSourceID = Int32.Parse(node.Attributes["Qti3pSourceID"].Value);
                        }
                        if (passage.Qti3pSourceID > 0)
                        {
                            switch (passage.Qti3pSourceID)
                            {
                                case (int)QTI3pSourceEnum.Progress:
                                    passage.Qti3pSource = QTI3pSourceEnum.Progress.ToString();
                                    break;
                            }
                        }
                        if (qti3p)
                        {
                            //If item is qti3p then node.Attributes["data"] will looks like "passages/3035.htm"
                            //Assign the number 3035 as RefObjectID
                            if (!string.IsNullOrEmpty(passage.Data))
                            {
                                var numer = Path.GetFileNameWithoutExtension(passage.Data);
                                if (!string.IsNullOrEmpty(numer))
                                {
                                    int iNumber = 0;
                                    if (!int.TryParse(numer, out iNumber))
                                    {
                                        numer = numer.Replace("RSC-LOGIC--", "");
                                    }
                                    passage.RefNumber = Int32.Parse(numer);
                                }
                            }
                        }
                        //need to add Qti3pItem.UrlPath to data for Qti3pItem
                        if (urlPath != null)
                        {
                            if (!string.IsNullOrEmpty(passage.Data) && !passage.Data.ToLower().StartsWith("http") && !passage.Data.ToLower().StartsWith("https"))
                            {
                                if (!urlPath.ToLower().StartsWith("http"))
                                {
                                    urlPath = "http://" + urlPath;
                                }
                                passage.Data = urlPath + "/" + passage.Data;
                            }
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (!qti3p)
                        {
                            if (string.IsNullOrEmpty(passage.Data))
                            {
                                if (node.Attributes["refObjectID"] != null)
                                {
                                    var refObjectId = Int32.Parse(node.Attributes["refObjectID"].Value); //only linkit passage has refObjectID
                                    passage.QtiRefObjectID = refObjectId;
                                    if (qtiRefObjectService != null)
                                    {
                                        //get the passage name
                                        var refObject = qtiRefObjectService.GetById(refObjectId);
                                        if (refObject != null)
                                        {
                                            var refObjectName = string.IsNullOrEmpty(refObject.Name) ? "[unnamed]" : refObject.Name;
                                            if (addNumberFirst)
                                                passage.Name = $"{refObjectId}: {refObjectName.ReplaceWeirdCharacters()}";
                                            else
                                                passage.Name = refObjectName.ReplaceWeirdCharacters();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //sometime data is a link likes http://www.linkit.com/NWEA13-2/01QTI 2.0/01FullItemBank/05 96 DPI JPG and MathML/LanguageArtsGrade 01-0/passages/3035.htm
                                //extract the number 3035 of 3035.html to show in button view passage
                                passage.RefNumber = 0;
                                var numer = Path.GetFileNameWithoutExtension(passage.Data);
                                var passageName = Path.GetFileName(passage.Data);
                                if (!string.IsNullOrEmpty(numer))
                                {
                                    if (qti3pPassageService != null && passage.DataFileUploadTypeID != (int)DataFileUploadTypeEnum.DataFileUpload)
                                    {
                                        var qti3pPassage = qti3pPassageService.GetQti3PassageByName(passageName);
                                        if (qti3pPassage != null)
                                        {
                                            if (addNumberFirst)
                                                passage.Name = $"{qti3pPassage.Number}: {qti3pPassage.PassageTitle}";
                                            else
                                                passage.Name = qti3pPassage.PassageTitle;
                                        }
                                    }
                                    if(dataFileUploadPassageService != null && passage.DataFileUploadTypeID == (int)DataFileUploadTypeEnum.DataFileUpload)
                                    {
                                        var dataFilePassage = dataFileUploadPassageService.GetDataFilePassageByUploadLogId(passage.DataFileUploadPassageID);
                                        if (dataFilePassage != null)
                                        {
                                            numer = Regex.Replace(numer, @"\D", "");
                                            if (addNumberFirst)
                                                passage.Name = $"{numer}: {dataFilePassage.PassageTitle}";
                                            else
                                                passage.Name = dataFilePassage.PassageTitle;
                                            passage.FileName = dataFilePassage.FileName;
                                        }
                                    }

                                    int iNumber = 0;
                                    if (!int.TryParse(numer, out iNumber))
                                    {
                                        numer = numer.Replace("RSC-LOGIC--", "");
                                    }
                                    passage.RefNumber = Int32.Parse(numer);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }

                    if (passage.QtiRefObjectID > 0 || passage.Data.Length > 0)
                    {
                        result.Add(passage);
                    }
                }
            }

            return result;
        }

        public static List<string> GetPassageNameList(string xmlContent, QTIRefObjectService passageService,
                                                      Qti3pPassageService qti3pPassageService,
                                                      bool addNumberFirst = false)
        {
            var result = new List<string>();

            // Reduce LoadXml times to increase performance
            if (!xmlContent.Contains("<object"))
                return result;

            string link = string.Empty;

            var doc = ItemSetPrinting.LoadXmlReferenceObjects(xmlContent);

            var objectNodes = doc.GetElementsByTagName("object");

            if (objectNodes != null)
            {
                for (int i = 0; i < objectNodes.Count; i++)
                {
                    link = string.Empty;
                    var node = objectNodes[i];

                    try
                    {
                        if (node.Attributes["data"] != null)
                        {
                            link = node.Attributes["data"].Value;
                            if (!string.IsNullOrWhiteSpace(link))
                            {
                                string[] tmp = link.Split('/');
                                if (tmp != null)
                                {
                                    var passageName = tmp[tmp.Length - 1];
                                    var qti3pPassage = qti3pPassageService.GetQti3PassageByName(passageName);
                                    if (qti3pPassage != null)
                                    {
                                        if (addNumberFirst)
                                        {
                                            result.Add(string.Format("{0}: {1}", qti3pPassage.Number,
                                                                     qti3pPassage.PassageTitle));
                                        }
                                        else
                                        {
                                            result.Add(qti3pPassage.PassageTitle);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (string.IsNullOrEmpty(link) && node.Attributes["refObjectID"] != null)
                        {
                            var refObjectId = int.Parse(node.Attributes["refObjectID"].Value);

                            //get the passage name
                            var passage = passageService.GetById(refObjectId);
                            if (passage != null)
                            {
                                var name = string.IsNullOrEmpty(passage.Name) ? "[unnamed]" : passage.Name;
                                if (addNumberFirst)
                                {
                                    result.Add(string.Format("{0}: {1}", refObjectId,
                                                             name.ReplaceWeirdCharacters()));
                                }
                                else
                                {
                                    result.Add(name.ReplaceWeirdCharacters());
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return result;
        }

        public static List<string> GetPassageNameListOptimize(string xmlContent, Qti3pPassageService qti3pPassageService,
            List<QtiRefObject> refObjects,
            bool addNumberFirst = false)
        {
            var result = new List<string>();

            // Reduce LoadXml times to increase performance
            if (!xmlContent.Contains("<object"))
                return result;

            string link = string.Empty;

            var doc = ItemSetPrinting.LoadXmlReferenceObjects(xmlContent);

            var objectNodes = doc.GetElementsByTagName("object");

            if (objectNodes != null)
            {
                for (int i = 0; i < objectNodes.Count; i++)
                {
                    link = string.Empty;
                    var node = objectNodes[i];

                    try
                    {
                        if (node.Attributes["data"] != null)
                        {
                            link = node.Attributes["data"].Value;
                            if (!string.IsNullOrWhiteSpace(link))
                            {
                                string[] tmp = link.Split('/');
                                if (tmp != null)
                                {
                                    var passageName = tmp[tmp.Length - 1];
                                    var qti3pPassage = qti3pPassageService.GetQti3PassageByName(passageName);
                                    if (qti3pPassage != null)
                                    {
                                        if (addNumberFirst)
                                        {
                                            result.Add(string.Format("{0}: {1}", qti3pPassage.Number,
                                                qti3pPassage.PassageTitle));
                                        }
                                        else
                                        {
                                            result.Add(qti3pPassage.PassageTitle);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (string.IsNullOrEmpty(link) && node.Attributes["refObjectID"] != null)
                        {
                            var refObjectId = Int32.Parse(node.Attributes["refObjectID"].Value);
                            //get the passage name
                            var passage =
                                refObjects.FirstOrDefault(x => x.QTIRefObjectID == refObjectId);
                            if (passage != null)
                            {
                                if (addNumberFirst)
                                {
                                    result.Add(string.Format("{0}: {1}", refObjectId,
                                        passage.Name.ReplaceWeirdCharacters()));
                                }
                                else
                                {
                                    result.Add(passage.Name.ReplaceWeirdCharacters());
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return result;
        }

        public static List<string> GetPassageIdList(string xmlContent, ref List<int> refObjectIds)
        {
            var result = new List<string>();

            // Reduce LoadXml times to increase performance
            if (!xmlContent.Contains("<object"))
                return result;

            var doc = ItemSetPrinting.LoadXmlReferenceObjects(xmlContent);

            var objectNodes = doc.GetElementsByTagName("object");

            for (int i = 0; i < objectNodes.Count; i++)
            {
                var node = objectNodes[i];

                try
                {
                    if (node.Attributes != null
                        && (node.Attributes["data"] == null || string.IsNullOrEmpty(node.Attributes["data"].Value))
                        && node.Attributes["refObjectID"] != null)
                    {
                        var refObjectId = Int32.Parse(node.Attributes["refObjectID"].Value);
                        refObjectIds.Add(refObjectId);
                    }
                }
                catch
                {
                }
            }

            return result;
        }

        public static List<PassageViewModel> GetPassageNoshuffleList(string xmlContent, out string xmlContentUpdated)
        {
            List<PassageViewModel> result = new List<PassageViewModel>();

            var doc = ItemSetPrinting.LoadXmlReferenceObjects(xmlContent);

            var objectNodes = doc.GetElementsByTagName("object");

            foreach (XmlNode node in objectNodes)
            {
                //var node = objectNodes[i];
                PassageViewModel passage = new PassageViewModel();

                if (node.Attributes["noshuffle"] == null)
                {
                    continue;
                }

                node.Attributes.RemoveNamedItem("noshuffle");
                if (node.Attributes["dataFileUploadPassageID"] != null)
                {
                    //Progressive DataFile Upload
                    passage.DataFileUploadPassageID = Int32.Parse(node.Attributes["dataFileUploadPassageID"].Value);
                }

                if (node.Attributes["Qti3pPassageID"] != null)
                {
                    //Progressive DataFile Upload in ThirdPartyItemBank
                    passage.Qti3pPassageID = Int32.Parse(node.Attributes["Qti3pPassageID"].Value);
                }

                if (node.Attributes["refObjectID"] != null)
                {
                    passage.QtiRefObjectID = Int32.Parse(node.Attributes["refObjectID"].Value); //only linkit passage has refObjectID
                }

                if (passage.QtiRefObjectID <= 0 && passage.DataFileUploadPassageID <= 0 && passage.Qti3pPassageID <= 0)
                {
                    if (node.Attributes["data"] != null)
                    {
                        passage.Data = node.Attributes["data"].Value;
                    }
                }
                if (passage.QtiRefObjectID > 0 || passage.DataFileUploadPassageID > 0 || passage.Qti3pPassageID > 0 || !string.IsNullOrEmpty(passage.Data))
                {
                    result.Add(passage);
                }
            }

            xmlContentUpdated = doc.OuterXml;
            return result;
        }

        public static string AddNoshuffleAttrForPassage(string xmlContent, List<VirtualQuestionPassageNoShuffle> passageNoShuffles)
        {
            //add attr 'noshuffle=true' to object
            var xmlContentProcessing = new XmlContentProcessing(xmlContent);
            var dataFileUploadPassageIds = passageNoShuffles.Select(x => x.DataFileUploadPassageID).ToList();
            var qti3pPassageIds = passageNoShuffles.Select(x => x.QTI3pPassageID).ToList();
            var qtiRefObjectIds = passageNoShuffles.Select(x => x.QTIRefObjectID).ToList();
            var dataList = passageNoShuffles.Select(x => x.PassageURL).ToList();
            var result = xmlContentProcessing.AddNoshuffleAttrForPassage(dataFileUploadPassageIds,
                qti3pPassageIds, qtiRefObjectIds, dataList);
            return result;
        }

        public static List<string> ParseStandardNumber(string xml, int roleId, List<int> userStateIdList)
        {
            if (string.IsNullOrWhiteSpace(xml)) return new List<string>();
            var xdoc = XDocument.Parse(xml);
            var result = new List<string>();
            int stateId = 0;
            foreach (var node in xdoc.Element("StandardNumberList").Elements("StandardNumber"))
            {
                if (roleId == (int)Permissions.Publisher)
                {
                    result.Add(GetStringValue(node.Element("Number")));
                }
                else
                {
                    stateId = GetIntValue(node.Element("StateID"));
                    if (stateId == Util.CCStateID) //Common Core State Standards
                    {
                        result.Add(GetStringValue(node.Element("Number")));
                    }
                    else
                    {
                        if (userStateIdList.Contains(stateId))
                        {
                            result.Add(GetStringValue(node.Element("Number")));
                        }
                    }
                }
            }

            return result.Distinct().ToList();
        }

        //public static List<string> ParseStandardNumber(string xml)
        //{
        //    if (string.IsNullOrWhiteSpace(xml)) return new List<string>();
        //    var xdoc = XDocument.Parse(xml);
        //    var result = new List<string>();

        //    foreach (var node in xdoc.Element("StandardNumberList").Elements("StandardNumber"))
        //    {
        //        result.Add(GetStringValue(node.Element("Number")));
        //    }
        //    return result;
        //}

        public static List<string> ParseTopic(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return new List<string>();
            var xdoc = XDocument.Parse(xml);
            var result = new List<string>();

            foreach (var node in xdoc.Element("TopicList").Elements("Topic"))
            {
                result.Add(GetStringValue(node.Element("Name")));
            }
            return result;
        }

        public static List<string> ParseSkill(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return new List<string>();
            var xdoc = XDocument.Parse(xml);
            var result = new List<string>();

            foreach (var node in xdoc.Element("SkillList").Elements("Skill"))
            {
                result.Add(GetStringValue(node.Element("Name")));
            }
            return result;
        }

        public static List<string> ParseOther(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return new List<string>();
            var xdoc = XDocument.Parse(xml);
            var result = new List<string>();

            foreach (var node in xdoc.Element("OtherList").Elements("Other"))
            {
                result.Add(GetStringValue(node.Element("Name")));
            }
            return result;
        }

        public static Dictionary<string, List<string>> ParseDistrictTag(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return new Dictionary<string, List<string>>();
            var xdoc = XDocument.Parse(xml);
            Dictionary<string, List<string>> districtTagDic = new Dictionary<string, List<string>>();
            string category;
            string tag;
            foreach (var node in xdoc.Element("ItemTagList").Elements("ItemTag"))
            {
                category = GetStringValue(node.Element("Category"));
                tag = GetStringValue(node.Element("Tag"));
                if (!districtTagDic.ContainsKey(category))
                {
                    districtTagDic.Add(category, new List<string> { tag });
                }
                else
                {
                    districtTagDic[category].Add(tag);
                }
            }
            return districtTagDic;
        }

        public static AttachmentSettingViewModel ParseAttachmentSetting(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return null;

            try
            {
                var xmlDocument = ServiceUtil.LoadXmlDocument(xml);

                if (xmlDocument.GetElementsByTagName("attachmentSetting").Count == 0)
                    return null;

                var attachmentSettingElement = xmlDocument.GetElementsByTagName("attachmentSetting")[0];

                var result = new AttachmentSettingViewModel();

                result.AllowStudentAttachment =
                    attachmentSettingElement.Attributes["allowStudentAttachment"].Value == "true";

                result.RequireAttachment = attachmentSettingElement.Attributes["requireAttachment"].Value == "true";

                return result;
            }
            // when parse fail // property
            catch
            {
                return null;
            }
        }

        public static IEnumerable<AssessmentArtifactRecordingOptionViewModel> GetAssessmentArtifactRecordingOptions()
        {
            try
            {
                return Util.GetConfigByKey(Constanst.Artifact.Configuration.RecordingOptions, "[]")
                    .DeserializeObject<List<AssessmentArtifactRecordingOptionViewModel>>();
            }
            // when user misconfigured json, return empty
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Enumerable.Empty<AssessmentArtifactRecordingOptionViewModel>();
            }
        }

        public static string GetStringValue(XElement element)
        {
            if (element == null) return string.Empty;
            return element.Value;
        }

        public static int GetIntValue(XElement element)
        {
            if (element == null) return 0;
            return Convert.ToInt32(element.Value);
        }

        public static void ReplaceWeirdCharacters(S3VirtualTest s3VirtualTest)
        {
            if (s3VirtualTest == null || s3VirtualTest.sections == null) return;
            foreach (var section in s3VirtualTest.sections)
            {
                if (section.sectionData == null) continue;
                section.sectionData.sectionInstruction =
                    section.sectionData.sectionInstruction.ReplaceWeirdCharacters();
                if (section.items == null) continue;
                foreach (var item in section.items)
                {
                    item.xmlContent = item.xmlContent.ReplaceWeirdCharacters();
                    item.xmlContent = RemoveCorrectResponseData(item.xmlContent);
                }
            }

            if (!string.IsNullOrWhiteSpace(s3VirtualTest.testData.testInstruction))
                s3VirtualTest.testData.testInstruction =
                    s3VirtualTest.testData.testInstruction.ReplaceWeirdCharacters();
        }

        public static S3Result UploadVirtualTestJsonFileToS3(S3VirtualTest s3VirtualTest, IS3Service s3Service)
        {
            ReplaceWeirdCharacters(s3VirtualTest);

            var s3json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(s3VirtualTest);
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(s3json ?? ""));

            var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
            var keyName = string.Empty;
            if (string.IsNullOrEmpty(folder))
            {
                keyName = string.Format("VirtualTest/VT_{0}.json", s3VirtualTest.virtualTestID);
            }
            else
            {
                keyName = string.Format("{0}/VirtualTest/VT_{1}.json", folder.RemoveEndSlash(), s3VirtualTest.virtualTestID);
            }
            return s3Service.UploadRubricFile(bucketName,
                                              keyName,
                                              fileStream, false);
        }

        public static bool SendMail(string strBody, string strSubject, string strEmailTo, string strEmailCC = null)
        {
            try
            {
                var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.EmailUtilUseEmailCredentialKey);

                MailMessage objMessage = new MailMessage();
                objMessage.From = new MailAddress(EmailSetting.EmailUtilSender);
                objMessage.To.Add(strEmailTo);
                if (strEmailCC != null)
                    objMessage.CC.Add(strEmailCC);
                objMessage.Subject = strSubject;
                objMessage.Body = strBody;
                objMessage.IsBodyHtml = true;

                var objClient = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Host = emailCredentialSetting.Host,
                    Port = emailCredentialSetting.Port,
                    UseDefaultCredentials = true,
                    EnableSsl = true,
                    Timeout = 1000000
                };

                if (EmailSetting.EmailUtilUsingCredential)
                {
                    objClient.Credentials = new NetworkCredential(emailCredentialSetting.UserName, emailCredentialSetting.Password);
                }

                objClient.Send(objMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendMailSGO(string strBody, string strSubject, string strEmailTo)
        {
            try
            {
                var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.SGOUseEmailCredentialKey);

                MailMessage objMessage = new MailMessage();
                objMessage.From = new MailAddress(EmailSetting.SGOEmailSender);
                objMessage.To.Add(strEmailTo);
                objMessage.Subject = strSubject;
                objMessage.Body = strBody;
                objMessage.IsBodyHtml = true;

                SmtpClient objClient = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Host = emailCredentialSetting.Host,
                    Port = emailCredentialSetting.Port,
                    EnableSsl = true,
                    Timeout = 1000000
                };

                if (EmailSetting.EmailSGOUsingCredential)
                {
                    objClient.Credentials = new NetworkCredential(emailCredentialSetting.UserName, emailCredentialSetting.Password);
                }

                objClient.Send(objMessage);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendMailTLDS1(string strBody, string strSubject, string strEmailTo)
        {
            try
            {
                var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.TLDSUseEmailCredentialKey);
                MailMessage objMessage = new MailMessage
                {
                    From = new MailAddress(EmailSetting.TLDSEmailSender),
                    Subject = strSubject,
                    Body = strBody,
                    IsBodyHtml = true
                };
                objMessage.To.Add(strEmailTo);

                SmtpClient objClient = new SmtpClient
                {
                    Host = emailCredentialSetting.Host,
                    Port = emailCredentialSetting.Port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Timeout = 1000000
                };

                if (EmailSetting.EmailTLDSUsingCredential)
                {
                    objClient.Credentials = new NetworkCredential(emailCredentialSetting.UserName, emailCredentialSetting.Password);
                }

               
                objClient.Send(objMessage);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendMailTLDSV2(string strBody, string strSubject, string strEmailTo, EmailCredentialSetting emailCredentialSetting)
        {
            try
            {
                MailMessage objMessage = new MailMessage
                {
                    From = new MailAddress(EmailSetting.TLDSEmailSender),
                    Subject = strSubject,
                    Body = strBody,
                    IsBodyHtml = true
                };
                objMessage.To.Add(strEmailTo);

                SmtpClient objClient = new SmtpClient
                {
                    Host = emailCredentialSetting.Host,
                    Port = emailCredentialSetting.Port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Timeout = 1000000
                };

                if (EmailSetting.EmailTLDSUsingCredential)
                {
                    objClient.Credentials = new NetworkCredential(emailCredentialSetting.UserName, emailCredentialSetting.Password);
                }


                objClient.Send(objMessage);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendMailWithCredentialInformation(string credentialInformation, string strBody, string strSubject, string strEmailTo, string strEmailCC = null)
        {
            try
            {
                var smtpClient = new SmtpClient();
                var mailMessage = new MailMessage();
                CreateCredentialInformation(credentialInformation, smtpClient, mailMessage);

                mailMessage.To.Add(strEmailTo);
                if (strEmailCC != null)
                    mailMessage.CC.Add(strEmailCC);
                mailMessage.Subject = strSubject;
                mailMessage.Body = strBody;
                mailMessage.IsBodyHtml = true;

                //The default value is 100,000 (100 seconds).
                smtpClient.Timeout = 1000000;
                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return false;
            }
        }

        public static void CreateCredentialInformation(string credentialInformation, SmtpClient smtpClient, MailMessage mailMessage)
        {
            //Credential example: email:Notification@linkit.com;alias:Notification@linkit.com;username:Notification@linkit.com;password:xxx;smtpserver:smtp.gmail.com;smtpport:0;enablessl:true

            try
            {
                var email = "";
                var alias = "";
                var username = "";
                var password = "";
                var smtpServer = "";
                var smtpPort = 0;
                var enableSsl = false;

                var credentialKeys = credentialInformation.Split(';');
                foreach (var credentialKey in credentialKeys)
                {
                    if (credentialKey.ToLower().StartsWith("email:"))
                    {
                        email = credentialKey.Split(':')[1];
                    }

                    if (credentialKey.ToLower().StartsWith("alias:"))
                    {
                        alias = credentialKey.Split(':')[1];
                    }

                    if (credentialKey.ToLower().StartsWith("username:"))
                    {
                        username = credentialKey.Split(':')[1];
                    }

                    if (credentialKey.ToLower().StartsWith("password:"))
                    {
                        password = credentialKey.Split(':')[1];
                    }

                    if (credentialKey.ToLower().StartsWith("smtpserver:"))
                    {
                        smtpServer = credentialKey.Split(':')[1];
                    }

                    if (credentialKey.ToLower().StartsWith("smtpport:"))
                    {
                        if (!string.IsNullOrEmpty(credentialKey.Split(':')[1]))
                        {
                            smtpPort = Convert.ToInt32(credentialKey.Split(':')[1]);
                        }
                    }

                    if (credentialKey.ToLower().StartsWith("enablessl:"))
                    {
                        enableSsl = credentialKey.Split(':')[1] == "true";
                    }
                }

                smtpClient.Host = smtpServer;
                smtpClient.Credentials = new NetworkCredential(username, password);
                smtpClient.EnableSsl = enableSsl;
                if (smtpPort != 0)
                    smtpClient.Port = smtpPort;

                mailMessage.From = new MailAddress(email, alias);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
            }
        }

        public static void UploadMultiVirtualTestJsonFileToS3(int qtiItemId, VirtualQuestionService virtualQuestionService, VirtualTestService virtualTestService, IS3Service s3Service)
        {
            var virtualQuestionIds =
                virtualQuestionService.Select().Where(en => en.QTIItemID == qtiItemId).Select(en => en.VirtualQuestionID)
                    .ToList();
            var virtualTestIds =
                virtualQuestionService.Select().Where(x => virtualQuestionIds.Contains(x.VirtualQuestionID)).Select(
                    x => x.VirtualTestID).Distinct();

            foreach (var virtualTestId in virtualTestIds)
            {
                var s3VirtualTest = virtualTestService.CreateS3Object(virtualTestId);
                Util.ReplaceWeirdCharacters(s3VirtualTest);

                var s3json = new JavaScriptSerializer();
                s3json.MaxJsonLength = int.MaxValue;
                var virtualTestJson = s3json.Serialize(s3VirtualTest);
                var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(virtualTestJson ?? ""));

                var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;
                var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                var keyName = string.Empty;
                if (string.IsNullOrEmpty(folder))
                {
                    keyName = string.Format("VirtualTest/VT_{0}.json", s3VirtualTest.virtualTestID);
                }
                else
                {
                    keyName = string.Format("{0}/VirtualTest/VT_{1}.json", folder.RemoveEndSlash(), s3VirtualTest.virtualTestID);
                }

                var s3Result = s3Service.UploadRubricFile(bucketName,
                                                         keyName,
                                                         fileStream, false);
            }
        }

        public static bool AddQtiItemsToVirtualSection(int virtualTestId, string qtiItemIdString, int virtualSectionId,
                                                bool isCloned, string currentUserName, int currentUserId, int currentUserStateId,
                                                VirtualTestService virtualTestService, QtiBankService qtiBankService,
                                                QtiGroupService qtiGroupService, QTIITemService qtiItemService, out string errorMessage, int? questionGroupId, IS3Service s3Service)
        {
            errorMessage = string.Empty;

            var virtualTest = virtualTestService.GetTestById(virtualTestId);

            if (virtualTest == null)
            {
                errorMessage = "Virtual Test can not found";
                return false;
            }

            if (isCloned)
            {
                int? qtiGroupID = null;
                var qtiBank = qtiBankService.GetDefaultQTIBank(currentUserName, currentUserId);
                var itemSet = qtiGroupService.GetDefaultQTIGroup(currentUserId, qtiBank.QtiBankId, virtualTest.Name);
                qtiGroupID = itemSet.QtiGroupId;

                var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;
                qtiItemIdString = qtiItemService.DuplicateListQTIItem(currentUserId, qtiItemIdString, qtiGroupID,
                true, bucketName, folder,
                LinkitConfigurationManager.GetS3Settings().S3Domain);

                if (qtiItemIdString.EndsWith(","))
                    qtiItemIdString = qtiItemIdString.Substring(0, qtiItemIdString.Length - 1);

                UpdateItemPassage(qtiItemIdString, qtiItemService);
            }

            virtualTestService.AddQtiItemToVirtualSection(virtualTestId, virtualSectionId, currentUserStateId, qtiItemIdString, questionGroupId);

            //if (UploadTestItemMediaToS3)//it's now alway upload to S3, no web server more
            {
                var s3VirtualTest = virtualTestService.CreateS3Object(virtualTestId);
                var s3Result = UploadVirtualTestJsonFileToS3(s3VirtualTest, s3Service);

                if (s3Result.IsSuccess)
                {
                    return true;
                }
                else
                {
                    errorMessage = "Virtual Test has been imported successfully but uploading json file to S3 fail: " +
                                   s3Result.ErrorMessage;
                    return false;
                }
            }
            return true;
        }

        private static void UpdateItemPassage(string qtiItemIdString, QTIITemService qtiItemService)
        {
            if (!string.IsNullOrWhiteSpace(qtiItemIdString))
            {
                var newQTIItemIds = qtiItemIdString.Split(',');
                if (newQTIItemIds != null && newQTIItemIds.Length > 0)
                {
                    foreach (var stringId in newQTIItemIds)
                    {
                        int qtiItemId = 0;
                        int.TryParse(stringId, out qtiItemId);
                        if (qtiItemId > 0)
                        {
                            var qtiItem = qtiItemService.GetQtiItemById(qtiItemId);
                            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
                            qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
                            qtiItem.XmlContent = qtiItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
                            List<PassageViewModel> passageList = GetPassageList(qtiItem.XmlContent, false);
                            if (passageList != null)
                            {
                                qtiItemService.UpdateItemPassage(qtiItemId, passageList.Select(x => x.QtiRefObjectID).ToList(),
                                    passageList.Select(x => x.RefNumber).ToList());
                            }
                        }
                    }
                }
            }
        }

        public static string CorrectImgSrc(string imgPath)
        {
            //LNKT-9065:Sometime imgPath starts with / such as /ItemSet_16304/take after-201408260349314008.png,
            //then when Path.Combine(rootPath, imgPath); will return c:\ItemSet_16304/take after-201408260349314008.png -> Wrong
            //so it's necessary to remove the first / of /ItemSet_16304/take after-201408260349314008.png
            if (imgPath.StartsWith("/"))
            {
                if (imgPath.Length >= 2)
                {
                    imgPath = imgPath.Substring(1);
                }
                else
                {
                    imgPath = string.Empty;
                }
            }
            return imgPath;
        }

        public static string ReplaceVideoTag(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            var xmlContentProcessing = new XmlContentProcessing(html);
            xmlContentProcessing.ReplaceTags(new List<string> { "video" }, "videolinkit");
            xmlContentProcessing.ReplaceTags(new List<string> { "source" }, "sourcelinkit");
            var result = xmlContentProcessing.GetXmlContent();

            return result;
        }

        public static string ConvertTags(string data, List<string> tagNames, string destTageName, bool convertToLowerKey = false)
        {
            var xmlContentProcessing = new XmlContentProcessing(data);
            xmlContentProcessing.ReplaceTags(tagNames, destTageName, convertToLowerKey);
            var result = xmlContentProcessing.GetXmlContent();
            return result;
        }

        /// <summary>
        /// Get Config by Key
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetConfigByKey(string strKey, string defaultValue)
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[strKey]))
            {
                return ConfigurationManager.AppSettings[strKey];
            }

            return defaultValue;
        }

        public static bool CheckQtiRefObjectEditPermission(int qtiRefObjectId, QTIRefObjectService qtiRefObjectService, int currentUserId, int districtId)
        {
            var authorizedPassageList =
                   qtiRefObjectService.GetQtiRefObject(new GetQtiRefObjectFilter
                   {
                       UserId = currentUserId,
                       DistrictId= districtId,
                       StartRow = 0,
                       PageSize = int.MaxValue
                   }).Select(x => x.QTIRefObjectID).ToList();

            if (!authorizedPassageList.Contains(qtiRefObjectId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Function support SSO
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="timems"></param>
        /// <returns></returns>
        public static string GetHash(string secret, string name, string email, string timems)
        {
            string input = name + secret + email + timems;
            var keybytes = Encoding.Default.GetBytes(secret);
            var inputBytes = Encoding.Default.GetBytes(input);

            var crypto = new HMACMD5(keybytes);
            byte[] hash = crypto.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                string hexValue = b.ToString("X").ToLower(); // Lowercase for compatibility on case-sensitive systems
                sb.Append((hexValue.Length == 1 ? "0" : "") + hexValue);
            }
            return sb.ToString();
        }

        public static Dictionary<int, List<string>> ParseStandardNumberQti3pItem(
           List<Qti3pItemStandardXml> masterStandardXML, int roleId, List<int> userStateIdList)
        {
            if (masterStandardXML == null) return new Dictionary<int, List<string>>();
            var result = new Dictionary<int, List<string>>();
            foreach (var standard in masterStandardXML)
            {
                if (!result.ContainsKey(standard.Qti3pItemId))
                {
                    result[standard.Qti3pItemId] = new List<string>();
                }

                if (!string.IsNullOrWhiteSpace(standard.MasterStandardXml))
                {
                    var xdoc = XDocument.Parse(standard.MasterStandardXml);
                    int stateId = 0;
                    foreach (var node in xdoc.Element("StandardNumberList").Elements("StandardNumber"))
                    {
                        if (roleId == (int)Permissions.Publisher)
                        {
                            result[standard.Qti3pItemId].Add(GetStringValue(node.Element("Number")));
                        }
                        else
                        {
                            stateId = GetIntValue(node.Element("StateID"));
                            if (stateId == Util.CCStateID) //Common Core State Standards
                            {
                                result[standard.Qti3pItemId].Add(GetStringValue(node.Element("Number")));
                            }
                            else
                            {
                                if (userStateIdList.Contains(stateId))
                                {
                                    result[standard.Qti3pItemId].Add(GetStringValue(node.Element("Number")));
                                }
                            }
                        }
                        result[standard.Qti3pItemId] = result[standard.Qti3pItemId].Distinct().ToList();
                    }
                }
            }

            return result;
        }

        public static string GetContentType(string extension)
        {
            var mimeType = "image/jpeg";
            switch (extension)
            {
                case ".svg":
                    {
                        mimeType = "image/svg+xml";
                        break;
                    }
                case ".tif":
                    {
                        mimeType = "image/tiff";
                        break;
                    }
                default:
                    {
                        mimeType = "image/jpeg";
                        break;
                    }
            }
            return mimeType;
        }

        public static string EncryptString(string message, string passphrase)
        {
            byte[] Results;
            var UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            var HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }

        public static string DecryptString(string message, string passphrase)
        {
            byte[] Results;
            var UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            var HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        private static bool IsValidXmlString(string text)
        {
            try
            {
                XmlConvert.VerifyXmlChars(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string UpdateS3LinkForItemMedia(string xmlContent)
        {
            //change source="http://portal.linkit.com/Content/FlashModules/TestMaker/proc/linkitTestMaker.php/getImage/ItemSet_8752/images.jpg"/>
            // or src="/ItemSet_16485/ad-152962277-9190-1417056016-201502270920282340.jpg"
            //to S3 direct linkt such as  https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_8752/eagle.png
            try
            {
                var xmlContentProcessing = new XmlContentProcessing(xmlContent);
                var mediaModel = new MediaModel();
                var idx = mediaModel.S3Domain.LastIndexOf("/");
                var s3Domain = mediaModel.S3Domain;
                if (idx == mediaModel.S3Domain.Length - 1) //the last character
                {
                    s3Domain = mediaModel.S3Domain.Substring(0, mediaModel.S3Domain.Length - 1);//remove the last "/" if any
                }

                xmlContent = xmlContentProcessing.UpdateS3LinkForItemMedia(s3Domain, mediaModel.UpLoadBucketName,
                mediaModel.AUVirtualTestFolder);
                return xmlContent;
            }
            catch
            {
                return xmlContent;
            }
        }

        public static string GetSGOStatusByStatusId(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return "Draft";

                case 2:
                    return "Preparation Submitted For Approval";

                case 3:
                    return "Preparation Approved";

                case 4:
                    return "Preparation Denied";

                case 5:
                    return "Unlocked";

                case 6:
                    return "Evaluation Submitted For Approval";

                case 7:
                    return "SGO Approved";

                case 8:
                    return "SGO Denied";

                case 9:
                    return "Teacher Acknowledged";

                default:
                    return string.Empty;
            }
        }

        public static bool HasRightOnDistrict(User user, int districId)
        {
            if (user.IsPublisher)
            {
                return true;
            }
            else if (user.IsNetworkAdmin)
            {
                return user.GetMemberListDistrictId().Contains(districId);
            }
            else
            {
                return user.DistrictId.GetValueOrDefault() == districId;
            }
        }
        public static bool HasRightOnLevel(User user, int settingLevel)
        {
            if (!user.IsPublisher && settingLevel == (int)DataLockerPreferencesLevel.Enterprise)
            {
                return false;
            }

            if (user.IsSchoolAdmin && settingLevel == (int)DataLockerPreferencesLevel.District)
            {
                return false;
            }

            if (user.IsTeacher && (settingLevel == (int)DataLockerPreferencesLevel.District
                                   || settingLevel == (int)DataLockerPreferencesLevel.School))
            {
                return false;
            }

            return true;
        }

        public static bool HasRightOnSchoolLevel(User user, int settingLevel)
        {
            if (user.IsPublisher)
            {
                return true;
            }
            if ((user.IsSchoolAdmin || user.IsNetworkAdmin || user.IsDistrictAdmin) && settingLevel == (int)DataLockerPreferencesLevel.School)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Conver List String to List Int by character.
        /// </summary>
        /// <param name="strList"></param>
        /// <param name="keyParse"></param>
        /// <returns></returns>
        public static List<int> ParseListStringtoListInt(string strList, char keyParse)
        {
            var lst = new List<int>();
            if (!string.IsNullOrEmpty(strList))
            {
                string[] arr = strList.Split(keyParse);
                if (arr.Length > 0)
                {
                    foreach (var s in arr)
                    {
                        int iValue = 0;
                        if (int.TryParse(s, out iValue) && iValue > 0)
                        {
                            lst.Add(iValue);
                        }
                    }
                }
            }
            return lst;
        }

        public static bool IsSingleCardinality(string xmlContent)
        {
            var doc = new XmlContentProcessing(xmlContent);
            if (doc != null)
            {
                string cardinality = doc.GetCardinality();
                if (cardinality.ToLower() == "single")
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDrawable(string xmlContent)
        {
            var doc = new XmlContentProcessing(xmlContent);
            if (doc != null)
            {
                string drawable = doc.GetDrawable();
                if (drawable.ToLower() == "true")
                {
                    return true;
                }
            }
            return false;
        }

        public static string RemoveCorrectResponseData(string xmlContent)
        {
            XmlContentProcessing doc = new XmlContentProcessing(xmlContent);
            var correctResponseNodes = doc.GetElementsByTagName("correctResponse");
            for (int i = correctResponseNodes.Count - 1; i >= 0; i--)
            {
                XmlNode correctResponseNode = correctResponseNodes[i];
                var parentNode = correctResponseNode.ParentNode;
                parentNode.RemoveChild(correctResponseNode);
            }

            return doc.GetXmlContent();
        }

        public static string ChangeS3BucketNameAsSubdomain(MediaModel model, string oldS3MediaUrl)
        {
            //now change bucketname as the subdomain https://testitemmedia-au.s3.amazonaws.com/ItemSet_51643/love-201510120324338737.jpg instead of https://s3.amazonaws.com/testitemmedia-au/ItemSet_51643/love-201510120324338737.jpg

            var mediaPath = oldS3MediaUrl ?? string.Empty;
            mediaPath = mediaPath.Replace(model.UpLoadBucketName + "/", "");
            if (mediaPath.StartsWith("https://"))
            {
                mediaPath = mediaPath.Replace("https://", "https://" + model.UpLoadBucketName + ".");
            }
            if (mediaPath.StartsWith("http://"))
            {
                mediaPath = mediaPath.Replace("http://", "http://" + model.UpLoadBucketName + ".");
            }
            return mediaPath;
        }

        public static string GetS3PassageJsonUrl(int qtiRefObjectId)
        {
            var result = string.Empty;
            MediaModel mediaModel = new MediaModel();
            var s3domain = mediaModel.S3Domain;
            if (s3domain.EndsWith("/"))
            {
                s3domain = s3domain.Remove(s3domain.Length - 1);//remove the last /
            }
            var bucket = mediaModel.UpLoadBucketName;
            if (bucket.EndsWith("/"))
            {
                bucket = bucket.Remove(bucket.Length - 1);//remove the last /
            }
            var folder = mediaModel.AUVirtualTestROFolder;
            if (folder.EndsWith("/"))
            {
                folder = folder.Remove(folder.Length - 1);//remove the last /
            }
            result = string.Format("{0}/{1}/{2}/RO/RO_{3}.json", s3domain, bucket, folder, qtiRefObjectId);
            result = ChangeS3BucketNameAsSubdomain(mediaModel, result);
            return result;
        }

        //public static bool CheckSectionAudioExist(string sectionAudio, int sectionId)
        //{
        //    var sectionAudioFileName = Path.GetFileName(sectionAudio);
        //    //SectionMedia/Section_16902/Square.mp3
        //    var mediaModel = new MediaModel();
        //    mediaModel.ID = sectionId;
        //    string path = string.Format("{0}/{1}", mediaModel.FileSystemSectionMedia.RemoveEndSlash(),
        //        sectionAudioFileName.RemoveStartSlash());

        //    return File.Exists(path);
        //}
        ///// <summary>
        ///// Move audio file from Section_0 to folder Section_[VirtualTestSectionID]
        ///// </summary>
        ///// <param name="sectionAudioRelativePath">Audio in Section_0 folder</param>
        ///// <param name="sectionId">VirtualSectionID</param>
        ///// <returns></returns>
        //public static void MoveSectionAudioFromDefaultToSectionFolder(string sectionAudioRelativePath, int sectionId)
        //{
        //    //sectionAudioRelativePath such as /SectionMedia/Section_0/Circle-201511040717020304.mp3
        //    //get the current audio file
        //    var mediaModel = new MediaModel();
        //    var currentPath = string.Format("{0}/{1}", mediaModel.FileSystemTestItemMediaPath.RemoveEndSlash(),
        //        sectionAudioRelativePath.RemoveStartSlash());
        //    if (File.Exists(currentPath))
        //    {
        //        mediaModel.ID = sectionId;
        //        if (!Directory.Exists( mediaModel.FileSystemSectionMedia))
        //        {
        //            Directory.CreateDirectory(mediaModel.FileSystemSectionMedia);
        //        }
        //         var sectionAudioFileName = Path.GetFileName(sectionAudioRelativePath);//get only file name, such as Circle-201511040717020304.mp3

        //        string newPath = string.Format("{0}/{1}", mediaModel.FileSystemSectionMedia.RemoveEndSlash(),
        //        sectionAudioFileName.RemoveStartSlash());

        //        //move file to new location
        //        File.Copy(currentPath,newPath);
        //    }
        //    else
        //    {
        //        throw new Exception("Can not find audio file");
        //    }

        //}

        public static List<FileType> ParseFileTypesFromXmlConfig(string xmlConfig)
        {
            /* Parse list of FileType from string in table Configuraiont (Name=RESOURCE_MIMETYPE)
             * xmlConfig Value looks like
             * <fileTypes all="false">
	                <fileType ext="jpg" mimetype="image\/jpg"/>
             * </fileTypes>
             */

            List<FileType> fileTypeList = new List<FileType>();
            if (string.IsNullOrEmpty(xmlConfig))
            {
                return fileTypeList;
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlConfig);
            var fileTypeNodeList = doc.GetElementsByTagName("fileType");
            foreach (XmlNode fileTypeNode in fileTypeNodeList)
            {
                var fileType = new FileType();
                fileType.Extension = XmlUtils.GetNodeAttribute(fileTypeNode, "ext");
                fileType.MimeType = XmlUtils.GetNodeAttribute(fileTypeNode, "mimetype");
                fileTypeList.Add(fileType);
            }
            return fileTypeList;
        }

        public static void LoadDateFormatToCookies(int userDistrictId, DistrictDecodeService districtDecodeService)
        {
            // write to DefaultDateFormat cookie
            var dateFormatModel = districtDecodeService.GetDateFormat(userDistrictId);
            AddOrSetCookie(Constanst.DefaultDateFormat, dateFormatModel.DateFormat);
            AddOrSetCookie(Constanst.DefaultTimeFormat, dateFormatModel.TimeFormat);
            AddOrSetCookie(Constanst.DefaultJqueryDateFormat, dateFormatModel.JQueryDateFormat);
        }

        public static string UpdateS3LinkForPassageLink(string xmlContent)
        {
            try
            {
                var xmlContentProcessing = new XmlContentProcessing(xmlContent);
                var mediaModel = new MediaModel();

                var idx = mediaModel.S3Domain.LastIndexOf("/");
                var s3Domain = mediaModel.S3Domain.RemoveEndSlash();

                xmlContent = xmlContentProcessing.UpdateS3LinkForPassageLink(s3Domain, mediaModel.UpLoadBucketName);
                return xmlContent;
            }
            catch
            {
                return xmlContent;
            }
        }

        public static System.Web.HttpCookie AddOrSetCookie(string cookieName, string cookieValue)
        {
            var config = ConfigurationManager.AppSettings["DefaultDateFormatCookieTimeout"];
            var configValueInDay = 0;
            if (!Int32.TryParse(config, out configValueInDay))
            {
                configValueInDay = 30;//use 30 (days) if there's no config
            }
            var cookie = System.Web.HttpContext.Current.Response.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new System.Web.HttpCookie(cookieName, cookieValue) //make this cookie like cookie FRMAUTH
                {
                    HttpOnly = true,
                    Path = FormsAuthentication.FormsCookiePath,
                    Secure = FormsAuthentication.RequireSSL
                };
                cookie.Domain = FormsAuthentication.CookieDomain;
                cookie.Expires = DateTime.Now.AddDays(configValueInDay);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
            {
                cookie.Value = cookieValue;
                cookie.Expires = DateTime.Now.AddDays(configValueInDay);
                System.Web.HttpContext.Current.Response.Cookies.Set(cookie);
            }
            return cookie;
        }

        public static long ToEpochTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static string UpdateS3LinkForItemMediaQuestionGroup(string xmlContent)
        {
            //change source="http://portal.linkit.com/Content/FlashModules/TestMaker/proc/linkitTestMaker.php/getImage/ItemSet_8752/images.jpg"/>
            // or src="/ItemSet_16485/ad-152962277-9190-1417056016-201502270920282340.jpg"
            //to S3 direct linkt such as  https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_8752/eagle.png
            try
            {
                var xmlContentProcessing = new XmlContentProcessing(xmlContent);
                var mediaModel = new MediaModel();
                var idx = mediaModel.S3Domain.LastIndexOf("/");
                var s3Domain = mediaModel.S3Domain;
                if (idx == mediaModel.S3Domain.Length - 1) //the last character
                {
                    s3Domain = mediaModel.S3Domain.Substring(0, mediaModel.S3Domain.Length - 1);//remove the last "/" if any
                }

                xmlContent = xmlContentProcessing.UpdateS3LinkForItemMediaQuestionGroup(s3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestFolder);
                return xmlContent;
            }
            catch
            {
                return xmlContent;
            }
        }

        public static string UpdateS3LinkForItemMediaQuestionGroupNew(string xmlContent)
        {
            //change source="http://portal.linkit.com/Content/FlashModules/TestMaker/proc/linkitTestMaker.php/getImage/ItemSet_8752/images.jpg"/>
            // or src="/ItemSet_16485/ad-152962277-9190-1417056016-201502270920282340.jpg"
            //to S3 direct linkt such as  https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_8752/eagle.png
            try
            {
                var mediaModel = new MediaModel();
                var idx = mediaModel.S3Domain.LastIndexOf("/");
                var s3Domain = mediaModel.S3Domain;
                if (idx == mediaModel.S3Domain.Length - 1) //the last character
                {
                    s3Domain = mediaModel.S3Domain.Substring(0, mediaModel.S3Domain.Length - 1);//remove the last "/" if any
                }
                string folderName = mediaModel.AUVirtualTestFolder;
                string result = string.Empty;
                int idxMedia = xmlContent.LastIndexOf("RO/RO_0_media");
                if (idxMedia >= 0)
                {
                    var subDomain = UrlUtil.GenerateS3Subdomain(s3Domain, mediaModel.UpLoadBucketName);
                    if (string.IsNullOrEmpty(folderName))
                    {
                        //result = string.Format("{0}/{1}", subDomain.RemoveEndSlash(), result.Replace(" ", "%20").RemoveStartSlash());
                        xmlContent = xmlContent.Replace("/RO/RO_0_media", string.Format("{0}/RO/RO_0_media", subDomain.RemoveEndSlash()));
                    }
                    else
                    {
                        //result = string.Format("{0}/{1}/{2}", subDomain.RemoveEndSlash(), folderName.RemoveStartSlash().RemoveEndSlash(), result.Replace(" ", "%20").RemoveStartSlash());
                        xmlContent = xmlContent.Replace("/RO/RO_0_media", string.Format("{0}/{1}/RO/RO_0_media", subDomain.RemoveEndSlash(), folderName.RemoveEndSlash()));
                    }
                }
                return xmlContent;
            }
            catch
            {
                return xmlContent;
            }
        }

        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        public static string GenerateToken(string username, int districtId, int expireMinutes = 20, Dictionary<string, string> aditional = null)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var claims = new List<Claim>()
            {
                new Claim("UserName", username),
                new Claim("DistrictID", districtId.ToString())
            };

            if (aditional != null)
            {
                foreach (var item in aditional)
                {
                    claims.Add(new Claim(item.Key, item.Value));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static string GenerateToken(Dictionary<string, string> information, int expireMinutes = 1)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var claims = new List<Claim>();

            foreach (var item in information)
            {
                claims.Add(new Claim(item.Key, item.Value));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static User InitFakeUser(string passwordQuestion)
        {
            return new User
            {
                PasswordQuestion = passwordQuestion,
                EmailAddress = DateTime.MaxValue.Ticks.ToString(),
                Id = 0 - DateTime.Now.Second
            };
        }

        public static ResponseObject SuccessFormat(object obj)
        {
            return new ResponseObject
            {
                Status = "success",
                Message = string.Empty,
                Data = obj
            };
        }

        public static ResponseObject SuccessFormat(object obj, string message)
        {
            return new ResponseObject
            {
                Status = "success",
                Message = message,
                Data = obj
            };
        }

        public static ResponseObject ErrorFormat(string message, object obj)
        {
            return new ResponseObject
            {
                Status = "error",
                Message = message,
                Data = obj
            };
        }

        public static string GetIPAddressClient(System.Web.HttpRequestBase request)
        {
            string szRemoteAddr = request.UserHostAddress;
            string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];
            string szIP = "";

            if (szXForwardedFor == null)
            {
                szIP = szRemoteAddr;
            }
            else
            {
                szIP = szXForwardedFor;
                if (szIP.IndexOf(",") > -1 && szIP[0] != ',')
                {
                    string[] arIPs = szIP.Split(',');

                    foreach (string item in arIPs)
                    {
                        if (!IsPrivateIpAddress(item))
                        {
                            return item;
                        }
                    }
                }
            }
            return szIP;
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are:
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        /// <summary>
        /// Check permission access to roleID
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public static bool HasPermissionImpersonateRole(int currentUserRoleID, int accessRoleID)
        {
            var listRoleId = new List<int>()
                        {
                            (int)Permissions.Student,
                            (int)Permissions.Parent,
                            (int)Permissions.Teacher,
                            (int)Permissions.SchoolAdmin,
                            (int)Permissions.DistrictAdmin,
                            (int)Permissions.NetworkAdmin,
                            (int)Permissions.Publisher,
                        };

            return listRoleId.IndexOf(currentUserRoleID) > listRoleId.IndexOf(accessRoleID);
        }

        public static List<SelectListItem> GetListAvailableByRoleID(int currentUserRoleID)
        {
            var listRoleDefault = new List<SelectListItem>
                                {
                                    new SelectListItem {Text = "Student", Value = "28"},
                                    new SelectListItem {Text = "Parent", Value = "26"},
                                    new SelectListItem {Text = "Teacher", Value = "2"},
                                    new SelectListItem {Text = "School Admin", Value = "8"},
                                    new SelectListItem {Text = LabelHelper.DistrictLabel + " Admin", Value = "3"},
                                    new SelectListItem {Text = "Network Admin", Value = "27"},
                                    new SelectListItem {Text = "Publisher", Value = "5"}
                                };
            var listRoleValueDefault = listRoleDefault.Select(x => x.Value).ToList();

            return listRoleDefault.Take(listRoleValueDefault.IndexOf(currentUserRoleID.ToString())).ToList();
        }

        public static string RandomCharsAndNums(int length)
        {
            Random _rdmChar = new Random();
            return new string(Enumerable.Repeat("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", length).Select(s => s[_rdmChar.Next(s.Length)]).ToArray());
        }

        public static MemoryStream RotateImage(byte[] byteArray, int rotate)
        {
            var value = rotate % 360;

            if (value < 0) value = 360 + value;
            var rotateValue = RotateFlipType.Rotate90FlipNone;

            switch (value)
            {
                case 90:
                    rotateValue = RotateFlipType.Rotate90FlipNone;
                    break;

                case 180:
                    rotateValue = RotateFlipType.Rotate180FlipNone;
                    break;

                case 270:
                    rotateValue = RotateFlipType.Rotate270FlipNone;
                    break;

                default:
                    break;
            }

            using (var memoryStream = new MemoryStream(byteArray))
            {
                var rotateImage = Image.FromStream(memoryStream);
                rotateImage.RotateFlip(rotateValue);
                var newStream = new MemoryStream();
                rotateImage.Save(newStream, ImageFormat.Png);
                return newStream;
            }
        }

        public static bool IsValidXmlContent(string xmlContent)
        {
            var xmlContentProcessing = new XmlContentProcessing(xmlContent);
            return xmlContentProcessing.IsXmlLoadedSuccess;
        }

        public static bool IsValidMultiPartXmlContent(string xmlContent)
        {
            if (!IsValidXmlContent(xmlContent))
                return false;
            var supportedTypes = new string[] {
                "choiceinteraction",
                "extendedtextinteraction",
                "inlinechoiceinteraction",
                "textentryinteraction"
            };
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            XmlNodeList nodes = xmlDoc.SelectNodes("//*[@responseIdentifier]");

            foreach (XmlNode node in nodes)
            {
                if (!supportedTypes.Contains(node.Name.ToLower()))
                    return false;
            }
            return true;
        }

        static void TransformInlineStyles(XmlElement element)
        {

            if (element.HasAttribute("style"))
            {
                string styleValue = element.GetAttribute("style");
                string[] styleDeclarations = styleValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string declaration in styleDeclarations)
                {
                    string[] parts = declaration.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 2)
                    {
                        string property = parts[0].Trim();
                        string value = parts[1].Trim();
                        if (property == "transform")
                            value = value.Replace("px", "");
                        element.SetAttribute(property, value);
                    }
                }
                element.RemoveAttribute("style");
            }
            foreach (XmlNode child in element.ChildNodes)
            {
                if (child is XmlElement childElement)
                {
                    TransformInlineStyles(childElement);
                }
            }
        }

        public static string UpdateMathImageForPrint(string xmlContent)
        {
            var imgs = xmlContent.Split(new string[] { "<img " }, StringSplitOptions.None)
                .Skip(1)
                .Select(str => {
                    try
                    {
                        return str.Split(new string[] { "class=\"imageupload\"" }, StringSplitOptions.None)[1]
                            .Split(new string[] { "data-latex=\"" }, StringSplitOptions.None)[1]
                            .Split(new string[] { "src=\"" }, StringSplitOptions.None)[1]
                            .Split('\"')[0];
                    } catch { return ""; }
                })
                .Where(str => !string.IsNullOrEmpty(str));

            foreach (var src in imgs)
            {
                if (src.StartsWith("data:image/svg+xml;base64,"))
                {
                    byte[] svgBytes = Convert.FromBase64String(src.Replace("data:image/svg+xml;base64,", ""));
                    string svgString = Encoding.UTF8.GetString(svgBytes);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(svgString);
                    var doc = xmlDoc.DocumentElement;
                    TransformInlineStyles(doc);
                    byte[] modifiedSvgBytes = Encoding.UTF8.GetBytes(doc.OuterXml);
                    string modifiedBase64Svg = Convert.ToBase64String(modifiedSvgBytes);
                    xmlContent = xmlContent.Replace(src, "data:image/svg+xml;base64," + modifiedBase64Svg);
                }
            }
            return xmlContent;
        }

        public static string RepairUncloseXmlTags(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return xml;
            var tags = new string[] { "input", "img", "hr" };
            foreach (var tag in tags)
            {
                xml = xml.Replace($"</{tag}>", "");
                string pattern = $"<{tag}(?<attributes>.+?)>";
                xml = Regex.Replace(xml, pattern, match =>
                {
                    if (!match.Value.EndsWith("/>"))
                        return match.Value.Substring(0, match.Value.Length - 1) + " />";
                    return match.Value;
                });
            }
            return xml;
        }
    }


    public enum EditStudentSource
    {
        FromManageClasses = 1,
        FromManageSchools = 2
    }

    public enum ExtractLocalTestStatusEnum
    {
        NotProcess = 0,
        ProcessSuccess = 1,
        ProcessFail = -1
    }

    public enum ExtractTypeEnum
    {
        TestResults = 1,
        Users = 2,
        Tests = 3,
        TestAssignments = 4,
        Rosters = 5
    }

    public enum StudentStatus
    {
        Active = 1,
        Inactive = 2
    }
}
