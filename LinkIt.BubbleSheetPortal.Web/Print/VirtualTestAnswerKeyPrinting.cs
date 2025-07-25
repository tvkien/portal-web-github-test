using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class VirtualTestAnswerKeyPrinting
    {
        public static string GenerateHtml(Controller controller, VirtualTestPrintingModel model, string templateName)
        {

            if (model == null) return string.Empty;
            foreach (var section in model.Sections)
            {
                foreach (var item in section.Items)
                {
                    var urlPath =  item.UrlPath;
                    XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();

                    item.XmlContent = item.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
                    var tempSpan = string.Format("<![CDATA[{0}]]>", Guid.NewGuid().ToString());  // Store <span> </span> to revert it; otherwise it will be converted to <span></span>
                    item.XmlContent = item.XmlContent.Replace("<span> </span>", tempSpan);
                    item.XmlContent = Util.UpdateS3LinkForItemMedia(item.XmlContent);
                    item.XmlContent = Util.UpdateS3LinkForPassageLink(item.XmlContent);
                    item.XmlContent = FormatXmlContent(item.XmlContent, urlPath, true);
                    item.XmlContent = item.XmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken);
                    item.XmlContent = item.XmlContent.Replace(tempSpan, "<span> </span>");
                }
            }

            var content = controller.RenderRazorViewToString(templateName, model);
            return content;
        }

        private static string FormatXmlContent(string xmlContent, string urlPath, bool useS3Content)
        {
            useS3Content = true;
            if (string.IsNullOrWhiteSpace(xmlContent)) return string.Empty;
            if (!urlPath.ToLower().StartsWith("nwea.s3.amazonaws.com"))
            {
                xmlContent = HtmlUtils.UpdateImageUrls(urlPath, xmlContent);
            }
            var tagsWillBeRemoved = new List<string> { "outcomeDeclaration", "stylesheet", "object" };
            xmlContent = DeleteNodes(xmlContent, tagsWillBeRemoved);

            var replaceTokens = GetDivReplaceTokens();
            xmlContent = ConvertTags(xmlContent, replaceTokens, "div");

            xmlContent = XmlUtils.RemoveSelfClosingTags(xmlContent);
            xmlContent = AdjustXmlContentFloatImg(xmlContent);
            xmlContent = xmlContent.RemoveZeroWidthSpaceCharacterFromUnicodeString();

            return xmlContent;
        }

       private static List<string> GetDivReplaceTokens()
        {
            var result = new List<string>();

            result.Add("assessmentItem");
            result.Add("itemBody");
            result.Add("destinationObject");
            result.Add("destinationItem");
            result.Add("sourceObject");
            result.Add("partialCredit");
            result.Add("partialSequence");
            result.Add("imageHotSpot");
            result.Add("sourceItem");
            result.Add("responseDeclaration");
            result.Add("correctResponse");
            result.Add("value");
            result.Add("tableitem");
            result.Add("numberLine");
            result.Add("numberLineItem");

            return result;
        }

        public static string AdjustXmlContentFloatImg(string xmlContent)
        {
            var xmlContentProcessing = new XmlContentProcessing(xmlContent);
            xmlContentProcessing.AdjustXmlContentFloatImg();
            var result = xmlContentProcessing.GetXmlContent();

            return result;
        }

        private static string DeleteNodes(string xml, List<string> tagNames)
        {
            var xmlContentProcessing = new XmlContentProcessing(xml);
            xmlContentProcessing.DeleteNodes(tagNames);
            var result = xmlContentProcessing.GetXmlContent();
            return result;
        }

        public static string ConvertTags(string data, List<string> tagNames, string destTageName)
        {
            var xmlContentProcessing = new XmlContentProcessing(data);
            xmlContentProcessing.ReplaceTags(tagNames, destTageName);
            var result = xmlContentProcessing.GetXmlContent();
            return result;
        }
    }
}
