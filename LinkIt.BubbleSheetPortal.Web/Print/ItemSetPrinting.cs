using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using S3Library;
using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Web.Print
{
    public class ItemSetPrinting
    {
        public static string GenerateHtml(Controller controller, ItemSetPrintingModel model,bool useS3Content, IS3Service s3Service)
        {
            useS3Content = true;
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            var lineFeedReplace = string.Format("<![CDATA[{0}]]>", Guid.NewGuid());
            model.Items = model.Items.OrderBy(o => o.QuestionOrder).ToList();
            foreach (var item in model.Items)
            {
                item.XmlContent = item.XmlContent.ReplaceWeirdCharacters();//should be done first ( decode xmlcontent to unicode inside )

                var urlPath =  item.UrlPath;
                item.PassageTexts = GetReferenceContents(model, item.XmlContent, urlPath, s3Service);
                item.XmlContent = item.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);

                item.XmlContent = item.XmlContent.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", lineFeedReplace);
                item.XmlContent = Util.UpdateS3LinkForItemMedia(item.XmlContent);
                item.XmlContent = Util.UpdateS3LinkForPassageLink(item.XmlContent);
                item.XmlContent = FormatXmlContent(item.XmlContent, urlPath,false);
                item.XmlContent = item.XmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken).Replace(lineFeedReplace, "\n");;

                item.XmlContent = item.XmlContent.Replace(
                    "<list listStylePosition=\"outside\" listStyleType=\"decimal\" paragraphSpaceAfter=\"12\" styleName=\"passageNumbering\"><listMarkerFormat><ListMarkerFormat color=\"#aaaaaa\" paragraphEndIndent=\"20\"/></listMarkerFormat>",
                    "<ol>")
                    .Replace("</list>", "</ol>");
                item.XmlContent = item.XmlContent.Replace("<p><span><ol>", "<ol>").Replace("</ol></span></p>", "</ol>");
                item.XmlContent = Util.ReplaceTagListByTagOl(item.XmlContent, false);
            }

            var content = controller.RenderRazorViewToString("ItemSet", model);

            return content;
        }

        public static ItemModel TransformXmlContentToHtml(string xmlContent, string urlPath, bool isQti3pItem, IS3Service s3Service)
        {
            if (string.IsNullOrEmpty(xmlContent)) return null;
            var item = new ItemModel();
            item.PassageTexts = GetReferenceContents(xmlContent, s3Service);
            item.XmlContent = FormatXmlContent(xmlContent, urlPath, isQti3pItem);

            var xmlContentProcessing = new XmlContentProcessing(item.XmlContent);
            xmlContentProcessing.FormatPartialCreditXmlContent();

            item.XmlContent = xmlContentProcessing.GetXmlContent();

            return item;
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

        private static string FormatXmlContent(string xmlContent, string urlPath,bool isQti3pItem)
        {
            if (string.IsNullOrWhiteSpace(xmlContent)) return string.Empty;
            if (urlPath == null)
            {
                urlPath = string.Empty;
            }
            if (isQti3pItem)
            {
                xmlContent = HtmlUtils.UpdateImageUrls(urlPath, xmlContent);
            }
            else
            {
                if (!urlPath.ToLower().StartsWith("nwea.s3.amazonaws.com"))
                {
                    xmlContent = HtmlUtils.UpdateImageUrls(urlPath, xmlContent);
                }
            }
            
            var tagsWillBeRemoved = new List<string> { "outcomeDeclaration", "stylesheet", "object" };
            var tagsContentRemoved = new List<string> { "responseDeclaration" };
            xmlContent = DeleteNodes(xmlContent, tagsWillBeRemoved);
            xmlContent = DeleteChildNodes(xmlContent, tagsContentRemoved);

            var replaceTokens = GetDivReplaceTokens();
            xmlContent = ConvertTags(xmlContent, replaceTokens, "div");
            xmlContent = ConvertBoxedText(xmlContent);
            xmlContent = ConvertTags(xmlContent, new List<string> { "choiceInteraction", "inlineChoiceInteraction" }, "ol");
            xmlContent = ConvertTags(xmlContent, new List<string> { "simpleChoice", "inlineChoice" }, "li");

            replaceTokens = GetInputReplaceTokens();
            xmlContent = ConvertTagsToInput(xmlContent, replaceTokens, "span");

            xmlContent = XmlUtils.RemoveSelfClosingTags(xmlContent);
            xmlContent = AdjustXmlContentFloatImg(xmlContent);
            return xmlContent;
        }

        public static string ConvertBoxedText(string data)
        {
            var xmlContentProcessing = new XmlContentProcessing(data);
            xmlContentProcessing.ConvertBoxedTextTag();
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

        private static string ConvertTagsToInput(string data, List<string> tagNames, string destTageName)
        {
            foreach (var token in tagNames)
            {
                var open = new KeyValuePair<string, string>(string.Format("<{0}", token),
                    string.Format("<{0} class=\"{1}\"", destTageName, token));
                var close = new KeyValuePair<string, string>(string.Format("</{0}>", token), string.Format("</{0}>", destTageName));

                // Remove class before converting to prevent dulicate attribute
                string pattern = $@"(<{token}[^>]*?)\s+class\s*=\s*['""][^'""]*['""]";
                data = Regex.Replace(data, pattern, "$1");

                data = data.Replace(open.Key, open.Value);
                data = data.Replace(close.Key, close.Value);
            }

            return data;
        }

        private static List<string> GetReferenceContents(string xmlContent, IS3Service s3Service)
        {
            return GetReferenceContents(null, xmlContent, null, s3Service);
        }

        private static List<string> GetReferenceContents(ItemSetPrintingModel model, string xmlContent, string testItemMediaPath, IS3Service s3Service)
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

                var content = GetReferenceContent(refObj.Key, refObj.Value, testItemMediaPath, s3Service);
                var mediaModel = new MediaModel();

                content = PassageUtil.UpdateS3LinkForPassageMedia(content, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestFolder);

                if(!string.IsNullOrEmpty(content))
                {
                    content = content.Replace("class=\"passage\"", "class=\"passage1\"");//New Flash version contains <div class="passage" styleName="passage"> that will lead to error in background ,temporary replace it to passage1 to avoid error
                    content = ItemSetPrinting.AdjustXmlContentFloatImg(content);
                    content = ConvertTags(content, new List<string> { "passage" }, "div");

                    var passageProcessing = new XmlContentProcessing(content);
                    passageProcessing.ScaleTable(600);

                    content = passageProcessing.GetXmlContent();

                    result.Add(content);
                }
            }

            return result;
        }

        public static string GetReferenceHtml(string refObjectID, string data, IS3Service s3Service)
        {
            var xmlContent = GetReferenceContent(refObjectID, data, null, s3Service);
            xmlContent = xmlContent.Replace("class=\"passage\"", "class=\"passage1\"");//New Flash version contains <div class="passage" styleName="passage"> that will lead to error in background ,temporary replace it to passage1 to avoid error
            xmlContent = XmlUtils.SanitizeAmpersands(xmlContent);
            var htmlContent = ConvertTags(xmlContent, new List<string> { "passage" }, "div");
            return htmlContent;
        }
        public static string GetReferenceContent(string refObjectID, string data, string rootPath, IS3Service s3Service)
        {
            var result = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(refObjectID) && !string.IsNullOrWhiteSpace(data))
                {
                    using (var client = new HttpClient())
                    {
                        //data = System.Web.HttpUtility.UrlEncode(data);//no need to encode, but make sure url start with http
                        HttpResponseMessage response = client.GetAsync(data).Result;
                        result = response.Content.ReadAsStringAsync().Result;

                        var lastIndex = data.LastIndexOf("/", System.StringComparison.Ordinal);
                        rootPath = data.Substring(0, lastIndex);

                        result = HtmlUtils.UpdateImageUrls(rootPath, result);
                        return result;
                    }
                }
                
                //now get passage from S3 always
                var model = new MediaModel();
                bool notFound;
                result = PassageUtil.GetS3PassageContent(s3Service, Int32.Parse(refObjectID), model.UpLoadBucketName, model.AUVirtualTestROFolder, out notFound);

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        private static List<string> GetDivReplaceTokens()
        {
            var result = new List<string>();

            result.Add("assessmentItem");
            result.Add("itemBody");
            result.Add("extendedTextInteraction");
			result.Add("destinationObject");
            result.Add("destinationItem");
            result.Add("imageHotSpot");
            result.Add("sourceItem");
            result.Add("numberLine");
            result.Add("numberLineItem");

            return result;
        }

        private static List<string> GetInputReplaceTokens()
        {
            var result = new List<string>();

            result.Add("textEntryInteraction");

            return result;
        }

        public static XmlDocument LoadXmlReferenceObjects(string xmlContent)
        {
            
            var xdoc = new XmlDocument();
            try
            {
                xdoc = ServiceUtil.LoadXmlDocument(xmlContent);
            }
            catch
            {
                //sometime xmlContent is not well format and lead to error,so that we should manually extract object(s)
                var str = xmlContent.ToLower();
                int idx = str.IndexOf("<itembody>");
                if (idx < 0)
                {
                    idx = str.IndexOf("<itembody >");//sometime it's <itembody >
                }
                if (idx > 0)
                {
                    int idx1 = str.IndexOf("<div stylename=\"mainbody\"");
                    if (idx1 < 0)
                    {
                        idx1 = str.IndexOf("<div class=\"mainbody\"");//somtime class="mainbody" next to "<div " not stylename="mainbody"
                    }
                    if (idx1 > 0)
                    {
                        //get object between <itembody> and <div stylename="mainbody"
                        string strObject = str.Substring(idx, idx1 - idx);
                        //create a temp xml contains only object xml
                        strObject = strObject + "</itembody>";
                        try
                        {
                            xdoc.LoadXml(strObject);
                        }
                        catch
                        {

                        }

                    }
                }
            }
            return xdoc;
        }
        public static string AdjustXmlContentFloatImg(string xmlContent)
        {
            // Just process item having img tag to improve performance
            if (xmlContent.Contains("<img"))
            {
                var xmlContentProcessing = new XmlContentProcessing(xmlContent);
                xmlContentProcessing.AdjustXmlContentFloatImg();
                var result = xmlContentProcessing.GetXmlContent();

                return result;
            }

            return xmlContent;
        }

        /// <summary>
        /// Try return passage invalid xml. But display OK on browser.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static string GetOriginalReferenceHtml(string requestUri)
        {
            var xmlContent = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(requestUri))
                {
                    using (var client = new HttpClient())
                    {                        
                        HttpResponseMessage response = client.GetAsync(requestUri).Result;
                        xmlContent = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception)
            {
                xmlContent = string.Empty;
            }
            var result = xmlContent.Replace("class=\"passage\"", "class=\"passage1\"");//New Flash version contains <div class="passage" styleName="passage"> that will lead to error in background ,temporary replace it to passage1 to avoid error
            result = result.Replace("<passage", "<div");
            result = result.Replace("passage>", "div>");
            return result;
        }
    }
}
