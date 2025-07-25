using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class VirtualTestPrinting
    {
        public static string GenerateHtml(Controller controller, VirtualTestPrintingModel model, string templateName, IS3Service s3Service)
        {
            FormatXmlContent(model, s3Service);
            foreach (var section in model.Sections)
            {
                section.SectionInstruction = section.SectionInstruction.ReplaceWeirdCharacters();
                foreach (var item in section.Items)
                {
                    item.XmlContent = item.XmlContent.Replace("<list listStylePosition=\"outside\" listStyleType=\"decimal\" paragraphSpaceAfter=\"12\" styleName=\"passageNumbering\"><listMarkerFormat><ListMarkerFormat color=\"#aaaaaa\" paragraphEndIndent=\"20\"/></listMarkerFormat>", "<ol>")
                    .Replace("<list ","<ol ")
                    .Replace("</list>", "</ol>");
                    item.XmlContent =
                    item.XmlContent.Replace("<p><span><ol>", "<ol>")
                    .Replace("</ol></span></p>", "</ol>");
                   
                }
            }

            model.TestItemMediaPath = string.Empty;
            var content = controller.RenderRazorViewToString(templateName, model);

            return content;
        }

        private static string DeleteNodes(string xml, List<string> tagNames)
        {
            var xmlContentProcessing = new XmlContentProcessing(xml);
            xmlContentProcessing.DeleteNodes(tagNames);
            var result = xmlContentProcessing.GetXmlContent();
            return result;
        }

        private static string DeleteChildNodes(string xml, List<string> tagNames)
        {
            var xmlContentProcessing = new XmlContentProcessing(xml);
            tagNames.ForEach(tag =>
            {
                var nodes = xmlContentProcessing.XmlDocument.DocumentElement.GetElementsByTagName(tag);
                for (var index = 0; index < nodes.Count; index++)
                {
                    var node = nodes[index];
                    if (node != null)
                    {
                        node.InnerXml = "";
                    }
                }
            });
            var result = xmlContentProcessing.GetXmlContent();
            return result;
        }

        private static void FormatXmlContent(VirtualTestPrintingModel model, IS3Service s3Service)
        {
            if (model == null) return;
            foreach (var section in model.Sections)
            {
                foreach (var item in section.Items)
                {
                    var urlPath = item.UrlPath;
                    item.PassageTexts = GetReferenceContents(model, item.XmlContent, model.S3Domain, model.UpLoadBucketName, model.AUVirtualTestROFolder, s3Service);
                    var xmlSpecialCharToken = new XmlSpecialCharToken();
                    item.XmlContent = item.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);

                    var tempSpan = string.Format("<![CDATA[{0}]]>", Guid.NewGuid().ToString());  // Store <span> </span> to revert it; otherwise it will be converted to <span></span>
                    item.XmlContent = item.XmlContent.Replace("<span> </span>", tempSpan);
                    item.XmlContent = Util.UpdateS3LinkForItemMedia(item.XmlContent);
                    item.XmlContent = Util.UpdateS3LinkForPassageLink(item.XmlContent);
                    item.XmlContent = FormatXmlContent(item.XmlContent);
                    item.XmlContent = item.XmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken);
                    item.XmlContent = item.XmlContent.Replace(tempSpan, "<span> </span>");

                    var xmlContentProcessing = new XmlContentProcessing(item.XmlContent);
                    xmlContentProcessing.ScaleTable(model.ColumnCount == "1" ? 580 : 260);
                    item.XmlContent = xmlContentProcessing.GetXmlContent();
                }
            }
        }

        private static string FormatXmlContent(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent)) return string.Empty;
           
            var tagsWillBeRemoved = new List<string> { "outcomeDeclaration", "stylesheet", "object" };
            var tagsContentRemoved = new List<string> { "responseDeclaration" };
            xmlContent = DeleteNodes(xmlContent, tagsWillBeRemoved);
            xmlContent = DeleteChildNodes(xmlContent, tagsContentRemoved);

            var replaceTokens = GetDivReplaceTokens();
            xmlContent = ConvertTags(xmlContent, replaceTokens, "div");
            xmlContent = ConvertBoxedText(xmlContent);
            xmlContent = ConvertTags(xmlContent, new List<string> { "choiceInteraction", "inlineChoiceInteraction" }, "ol");
            xmlContent = ConvertTags(xmlContent, new List<string> { "simpleChoice", "inlineChoice" }, "li");
            xmlContent = ConvertTags(xmlContent, new List<string> { "imageHotSpot", "numberLine", "numberLineItem" }, "div");

            replaceTokens = GetInputReplaceTokens();
            xmlContent = ConvertTagsToInput(xmlContent, replaceTokens, "span");

            xmlContent = XmlUtils.RemoveSelfClosingTags(xmlContent);
            xmlContent = AdjustXmlContentFloatImg(xmlContent);
            return xmlContent;
        }

        public static string ConvertTags(string data, List<string> tagNames, string destTageName, bool convertToLowerKey = false)
        {
            var xmlContentProcessing = new XmlContentProcessing(data);
            xmlContentProcessing.ReplaceTags(tagNames, destTageName, convertToLowerKey);
            var result = xmlContentProcessing.GetXmlContent();
            return result;
        }
        public static string ConvertBoxedText(string data)
        {
            var xmlContentProcessing = new XmlContentProcessing(data);
            xmlContentProcessing.ConvertBoxedTextTag();
            var result = xmlContentProcessing.GetXmlContent();
            return result;
        }

        private static string ConvertTagsToInput(string data, List<string> tagNames, string destTageName)
        {
            foreach (var token in tagNames)
            {
                var open = new KeyValuePair<string, string>(string.Format("<{0}", token),
                    string.Format("<{0} class=\"{1}\"", destTageName, token));
                var close = new KeyValuePair<string, string>(string.Format("</{0}>", token), string.Format("</{0}>", destTageName));
                data = data.Replace(open.Key, open.Value);
                data = data.Replace(close.Key, close.Value);
            }

            return data;
        }

        private static List<string> GetReferenceContents(VirtualTestPrintingModel model, string xmlContent, string s3Domain, string uploadBucketName, string AUVirtualTestROFolder, IS3Service s3Service)
        {
            var result = new List<string>();
            var xmlContentProcessing = new XmlContentProcessing(xmlContent);
            var refObjs = xmlContentProcessing.GetReferenceObjects();
            foreach (var refObj in refObjs)
            {
                if (model != null)
                {
                    var isExisted =
                    model.PreferenceObjects.Any(
                        o =>
                            String.CompareOrdinal(o.Key, refObj.Key) == 0 &&
                            String.CompareOrdinal(o.Value, refObj.Value) == 0);
                    if (isExisted) continue;
                    model.PreferenceObjects.Add(refObj);
                }
                var content = PassageUtil.GetReferenceContent(s3Service, refObj.Key, refObj.Value, s3Domain, uploadBucketName, AUVirtualTestROFolder);

                if(!string.IsNullOrEmpty(content))
                {
                    string originalContent = content; //Support case Parse xmlContent error.
                    content = content.Replace("class=\"passage\"", "class=\"passage1\"");//New Flash version contains <div class="passage" styleName="passage"> that will lead to error in background ,temporary replace it to passage1 to avoid error
                    content = ItemSetPrinting.AdjustXmlContentFloatImg(content);
                    content = ConvertTags(content, new List<string> { "passage" }, "div");

                    var mediaModel = new MediaModel();
                    content = PassageUtil.UpdateS3LinkForPassageMedia(content, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestFolder);
                    var passageProcessing = new XmlContentProcessing(content);
                    passageProcessing.ScaleTable(600);
                    content = passageProcessing.GetXmlContent();

                    content = string.IsNullOrEmpty(content) ? originalContent : content;
                    result.Add(content);
                }
            }

            return result;
        }
        public static string GetReferenceHtml(string refObjectID, string data, string s3Domain, string uploadBucketName, string AUVirtualTestROFolder, IS3Service s3Service)
        {
            var xmlContent = PassageUtil.GetReferenceContent(s3Service, refObjectID, data, s3Domain, uploadBucketName, AUVirtualTestROFolder);
            xmlContent = xmlContent.Replace("class=\"passage\"", "class=\"passage1\"");//New Flash version contains <div class="passage" styleName="passage"> that will lead to error in background ,temporary replace it to passage1 to avoid error
            var htmlContent = ConvertTags(xmlContent, new List<string> { "passage" }, "div");
            return htmlContent;
        }

        private static List<string> GetDivReplaceTokens()
        {
            var result = new List<string>();

            result.Add("assessmentItem");
            result.Add("itemBody");
            result.Add("extendedTextInteraction");
            result.Add("destinationObject");
            result.Add("destinationItem");
            result.Add("sourceItem");

            return result;
        }

        private static List<string> GetInputReplaceTokens()
        {
            var result = new List<string>();

            result.Add("textEntryInteraction");

            return result;
        }

        public static string AdjustXmlContentFloatImg(string xmlContent)
        {
            var xmlContentProcessing = new XmlContentProcessing(xmlContent);
            xmlContentProcessing.AdjustXmlContentFloatImg();
            var result = xmlContentProcessing.GetXmlContent();

            return result;
        }

    }
}
