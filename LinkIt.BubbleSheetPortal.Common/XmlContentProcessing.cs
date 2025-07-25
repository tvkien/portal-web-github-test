using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using Microsoft.SqlServer.Server;
using System.Web;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class XmlContentProcessing
    {
        private readonly string _lineFeedReplace = string.Format("<![CDATA[{0}]]>", Guid.NewGuid());
        private readonly string _htmlEntityReplace = string.Format("<![CDATA[{0}]]>", Guid.NewGuid());
        private readonly XmlSpecialCharToken _xmlSpecialCharToken = new XmlSpecialCharToken();

        private XmlDocument _xDoc;
        private readonly string _xmlContent;
        private string _loadXmlContentException = string.Empty;
        public XmlContentProcessing(string xmlContent)
        {
            xmlContent = GetAssessmentXml(xmlContent);
            xmlContent = xmlContent.Replace("&nbsp;", "&#160;");
            

            xmlContent = EntityToUnicode(xmlContent);
            //xmlContent = xmlContent.Replace("&#", _htmlEntityReplace);
            xmlContent = xmlContent.ReplaceXmlSpecialChars(_xmlSpecialCharToken);

            xmlContent = xmlContent.Replace("\r\n", "\n");
            xmlContent = xmlContent.Replace("\r", "\n");
            xmlContent = xmlContent.Replace("\n", _lineFeedReplace);

            _xmlContent = xmlContent;

            _xDoc = new XmlDocument();
            _xDoc.PreserveWhitespace = true;
            try
            {                
                _xDoc.LoadXml(xmlContent);
            }
            catch (Exception ex)
            {                
                _xDoc = null;
                _loadXmlContentException = ex.Message;
            }
        }

        public string EntityToUnicode(string html)
        {
            var replacements = new Dictionary<string, string>();
            var regex = new Regex("(&[a-zA-Z0-9]{2,9};)");
            foreach (Match match in regex.Matches(html))
            {
                if (!replacements.ContainsKey(match.Value))
                {
                    var unicode = HttpUtility.HtmlDecode(match.Value);
                    if (unicode.Length == 1)
                    {
                        replacements.Add(match.Value, string.Concat("&#", Convert.ToInt32(unicode[0]), ";"));
                    }
                }
            }
            foreach (var replacement in replacements)
            {
                html = html.Replace(replacement.Key, replacement.Value);
            }
            return html;
        }

        public void DeleteNodes(List<string> tagNames)
        {
            if (_xDoc == null || tagNames == null) return;
            foreach (var tagName in tagNames)
            {
                while (true)
                {
                    var nodes = _xDoc.GetElementsByTagName(tagName);
                    if (nodes.Count == 0) break;
                    var node = nodes[0];
                    if (node.ParentNode == null) continue;
                    node.ParentNode.RemoveChild(node);
                }
            }
        }

        public void UpdateImageUrls(string imgPath)
        {
            if (_xDoc == null) return;
            var nodes = _xDoc.GetElementsByTagName("img");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null) continue;
                var src = GetNodeAttribute(node, "src");
                if (string.IsNullOrWhiteSpace(src))
                {
                    src = GetNodeAttribute(node, "source");
                }

                if (string.IsNullOrWhiteSpace(src)) continue;
                if (src.ToLower().StartsWith("http") || src.ToLower().StartsWith("www.") || src.ToLower().StartsWith("nwea.s3.amazonaws.com"))
                // check nwea.s3.amazonaws.com in case this file stays on Amazon server
                {
                    src = src.Replace(" ", "%20");
                }
                else
                {
                    src = GetFullImgPath(imgPath, src);
                }

                src = src.Replace("\\/", "\\");

                SetNodeAttribute(node, "src", src);
                SetNodeAttribute(node, "source", string.Empty);
            }
        }

        public void AdjustXmlContentFloatImg()
        {
            if (_xDoc == null) return;
            var nodes = _xDoc.GetElementsByTagName("img");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null) continue;
                var floatImg = GetNodeAttribute(node, "float");
                if (string.IsNullOrWhiteSpace(floatImg)) continue;
                //if there's no style,add new style attribute
                if (node.Attributes["style"] == null)
                {
                    var styleAtt = _xDoc.CreateAttribute("style");
                    styleAtt.Value = string.Format("float:{0}", floatImg);
                    node.Attributes.Append(styleAtt);
                }
                else
                {
                    //get attribute of style
                    var styleValue = GetNodeAttribute(node, "style");
                    styleValue = styleValue + string.Format(";float:{0}", floatImg);
                    SetNodeAttribute(node, "style", styleValue);
                }
            }
        }

        public string GetXmlContent()
        {
            if (_xDoc == null) return string.Empty;
            var result = _xDoc.OuterXml;
            //result = result.Replace(_htmlEntityReplace, "&#");
            result = result.RecoverXmlSpecialChars(_xmlSpecialCharToken);
            result = result.Replace(_lineFeedReplace, "\n");

            return result;
        }

        public List<KeyValuePair<string, string>> GetReferenceObjects()
        {
            var result = new List<KeyValuePair<string, string>>();

            XmlNodeList nodes;
            if (_xDoc == null)
            {
                var roXml = GetXmlBetweenItemBodyAndMainBody(_xmlContent);
                var roXDoc = new XmlDocument();
                roXDoc.PreserveWhitespace = true;
                roXDoc.LoadXml(roXml);
                nodes = roXDoc.GetElementsByTagName("object");
            }
            else
            {
                nodes = _xDoc.GetElementsByTagName("object");
            }

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null) continue;
                var stylename = GetNodeAttribute(node, "stylename");
                if (String.CompareOrdinal(stylename, "referenceObject") != 0) continue;
                var refObjID = GetNodeAttribute(node, "refObjectID");
                var refData = GetNodeAttribute(node, "data");
                result.Add(new KeyValuePair<string, string>(refObjID, refData));
            }

            return result;
        }

        public void FormatPartialCreditXmlContent()
        {
            if (_xDoc == null)
            {
                var roXml = GetXmlBetweenItemBodyAndMainBody(_xmlContent);
                _xDoc = new XmlDocument();
                _xDoc.PreserveWhitespace = true;
                _xDoc.LoadXml(roXml);
            }

            var nodes = _xDoc.GetElementsByTagName("destinationObject");
            for (var i = nodes.Count - 1; i >= 0; i--)
            {
                var destinationobjectNode = nodes[i];
                var parentNode = destinationobjectNode.ParentNode;

                if (destinationobjectNode.Attributes == null) continue;
                switch (GetNodeAttribute(destinationobjectNode, "type"))
                {
                    case "image":
                        parentNode.ReplaceChild(CreateDestinationImageDiv(_xDoc, destinationobjectNode),
                            destinationobjectNode);
                        break;
                    case "text":
                        parentNode.ReplaceChild(CreateDestinationTextDiv(_xDoc, destinationobjectNode),
                            destinationobjectNode);
                        break;
                }
            }
        }

        public void ConvertXmlContentFromMultipleChoiceToExtendedText(int newPoint)
        {
            //Made XmlContent change base on what Flash
            //Example: 
            //multiple choice before converting to extended text
            //<assessmentItem xsi:schemaLocation="http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd" adaptive="false" timeDependent="false" toolName="linkitTLF" toolVersion="1.0" qtiSchemeID="1" xmlns="http://www.imsglobal.org/xsd/imsqti_v2p0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><stylesheet href="stylesheet/linkitStyleSheet.css" type="text/css"/><responseDeclaration identifier="RESPONSE_1" baseType="identifier" cardinality="single" method="default" caseSensitive="false" type="string" pointsValue="1"><correctResponse><value>A</value></correctResponse></responseDeclaration><itemBody><div styleName="mainBody" class="mainBody"><p><span>Multiple Choice - Flash</span></p></div><choiceInteraction responseIdentifier="RESPONSE_1" shuffle="false" maxChoices="1"><simpleChoice identifier="A"><div class="answer" styleName="answer"><p><span>Answer A</span></p></div></simpleChoice><simpleChoice identifier="B"><div class="answer" styleName="answer"><p><span>Answer B</span></p></div></simpleChoice><simpleChoice identifier="C"><div class="answer" styleName="answer"><p><span>Answer C</span></p></div></simpleChoice><simpleChoice identifier="D"><div class="answer" styleName="answer"><p><span>Answer D</span></p></div></simpleChoice></choiceInteraction></itemBody></assessmentItem>

            //after converting to extended text
            //<assessmentItem xsi:schemaLocation="http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd" adaptive="false" timeDependent="false" toolName="linkitTLF" toolVersion="1.0" qtiSchemeID="10" xmlns="http://www.imsglobal.org/xsd/imsqti_v2p0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><stylesheet href="stylesheet/linkitStyleSheet.css" type="text/css"/><responseDeclaration identifier="RESPONSE_2" baseType="string" cardinality="single" method="default" caseSensitive="false" type="string" pointsValue="4"/><itemBody><div styleName="mainBody" class="mainBody"><p><span>Multiple Choice - Flash</span></p></div><extendedTextInteraction responseIdentifier="RESPONSE_2" expectedLength="200"/></itemBody></assessmentItem>

            //Base on that example (compare xmlcontent)=> What need to be done 
            //1. Update pointsValue in tag responseDeclaration
            //2. Remove tag correctResponse inside tag responseDeclaration
            //3. Remove tag choiceInteraction inside tag itemBody
            //4. Insert new tag <extendedTextInteraction responseIdentifier="RESPONSE_2" expectedLength="200"/> inside tag itemBody

            string result = string.Empty;
            if (_xDoc == null) return;
            //1. Update pointsValue in tag responseDeclaration
            var responseDeclaration = _xDoc.GetElementsByTagName("responseDeclaration")[0];
            try
            {
                responseDeclaration.Attributes["pointsValue"].Value = newPoint.ToString();
            }
            catch (Exception)
            {
                var pointsValueAttr = _xDoc.CreateAttribute("pointsValue");
                pointsValueAttr.Value = newPoint.ToString();
                responseDeclaration.Attributes.Append(pointsValueAttr);
            }
            //2. Remove tag correctResponse inside tag responseDeclaration
            var correctResponse = _xDoc.GetElementsByTagName("correctResponse")[0];
            var parent = correctResponse.ParentNode;
            parent.RemoveChild(correctResponse);

            //3. Remove tag choiceInteraction inside tag itemBody

            var choiceInteraction = _xDoc.GetElementsByTagName("choiceInteraction")[0];
            parent = choiceInteraction.ParentNode;
            parent.RemoveChild(choiceInteraction);

            var itemBodyNode = _xDoc.GetElementsByTagName("itemBody")[0];
            if (itemBodyNode.HasChildNodes)
            {
                //4. Insert new tag <extendedTextInteraction responseIdentifier="RESPONSE_2" expectedLength="200"/> inside tag itemBody
                //Note that responseIdentifier has the same value as identifier of responseDeclaration;
                XmlNode extendedTextInteraction = _xDoc.CreateNode(XmlNodeType.Element, "extendedTextInteraction",
                    _xDoc.DocumentElement.NamespaceURI);
                var responseIdentifierAttr = _xDoc.CreateAttribute("responseIdentifier");
                responseIdentifierAttr.Value = responseDeclaration.Attributes["identifier"].Value;
                extendedTextInteraction.Attributes.Append(responseIdentifierAttr);

                var expectedLengthAttr = _xDoc.CreateAttribute("expectedLength");
                expectedLengthAttr.Value = "50000"; //always 200
                extendedTextInteraction.Attributes.Append(expectedLengthAttr);
                itemBodyNode.ChildNodes[0].AppendChild(extendedTextInteraction);
            }
        }

        private string GetXmlBetweenItemBodyAndMainBody(string xmlContent)
        {
            var result = string.Empty;

            var str = CaseInsenstiveReplace(xmlContent, "<itembody", "<itembody");
            str = CaseInsenstiveReplace(str, "<div stylename=\"mainbody\"", "<div class=\"mainbody\"");
            str = CaseInsenstiveReplace(str, "<div class=\"mainbody\"", "<div class=\"mainbody\"");
            var idx = str.IndexOf("<itembody>", StringComparison.Ordinal);
            if (idx < 0) idx = str.IndexOf("<itembody >", StringComparison.Ordinal);
            if (idx <= 0) return result;

            var idx1 = str.IndexOf("<div class=\"mainbody\"", StringComparison.Ordinal);
            if (idx1 <= 0) return result;

            //get object between <itembody> and <div stylename="mainbody"
            result = str.Substring(idx, idx1 - idx);
            //create a temp xml contains only object xml
            result = result + "</itembody>";

            return result;
        }

        public static string CaseInsenstiveReplace(string originalString, string oldValue, string newValue)
        {
            var regEx = new Regex(oldValue, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(originalString, newValue);
        }

        private static string GetNodeAttribute(XmlNode node, string attributeName)
        {
            if (node == null || string.IsNullOrEmpty(attributeName)) return null;
            if (node.Attributes == null) return null;
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (string.Equals(attribute.Name, attributeName, StringComparison.OrdinalIgnoreCase))
                    return attribute.Value;
            }

            return null;
        }

        private static void SetNodeAttribute(XmlNode node, string attributeName, string value)
        {
            if (node == null || string.IsNullOrEmpty(attributeName)) return;
            if (node.Attributes == null) return;
            if (node.Attributes[attributeName] == null)
            {
                XmlUtils.AddAttribute(node, attributeName, value);
            }

            var xmlAttribute = node.Attributes[attributeName];
            if (xmlAttribute != null) xmlAttribute.Value = value;
        }

        private static string GetFullImgPath(string rootPath, string imgPath)
        {
            if (imgPath.StartsWith("data:"))
            {
                return imgPath;
            }
            if (!string.IsNullOrEmpty(imgPath))
            {
                if (imgPath.ToLower().StartsWith("http"))
                {
                    return imgPath;
                }
            }
            if (string.IsNullOrWhiteSpace(rootPath)) return string.Empty;
            if (rootPath.StartsWith("www.") || rootPath.StartsWith("nwea.s3.amazonaws.com"))
                rootPath = "http://" + rootPath;
            if (imgPath.Contains("data:image")) return imgPath;
            var fullPath = rootPath + "\\" + imgPath;
            if (fullPath.StartsWith("http"))
            {
                var uri = new Uri(fullPath);
                fullPath = uri.AbsoluteUri;
            }
            else
            {
                fullPath = Path.GetFullPath(fullPath);
            }

            return fullPath;
        }

        private static XmlNode CreateDestinationImageDiv(XmlDocument xdoc, XmlNode destinationobjectNode)
        {
            var objectWidth = GetNodeAttribute(destinationobjectNode, "width");
            var objectHeight = GetNodeAttribute(destinationobjectNode, "height");
            var imageSrc = GetNodeAttribute(destinationobjectNode, "src");

            var objectStyle = string.Format("width: {0}px; height: {1}px;", objectWidth, objectHeight);
            var imageStyle = string.Format("width: {0}px; max-width: {0}px; height: {1}px; max-height: {1}px;",
                objectWidth, objectHeight);

            var destinationObjectDiv = CreateElement(xdoc, "span");
            AssignAttribute(xdoc, destinationObjectDiv, "class", "partialDestinationObject partialAddDestinationImage");
            AssignAttribute(xdoc, destinationObjectDiv, "style", objectStyle);
            AssignAttribute(xdoc, destinationObjectDiv, "type", "image");

            if (imageSrc != null)
            {
                var destinationObjectImage = CreateElement(xdoc, "img");
                AssignAttribute(xdoc, destinationObjectImage, "class", "destinationImage");
                AssignAttribute(xdoc, destinationObjectImage, "src",
                    "/TestAssignmentRegrader/GetViewReferenceImg?imgPath=" + imageSrc);
                AssignAttribute(xdoc, destinationObjectImage, "style", imageStyle);
                destinationObjectDiv.AppendChild(destinationObjectImage);
            }

            foreach (XmlNode destinationItemNode in destinationobjectNode.ChildNodes)
            {
                var itemLeft = GetNodeAttribute(destinationItemNode, "left");
                var itemTop = GetNodeAttribute(destinationItemNode, "top");
                var itemWidth = GetNodeAttribute(destinationItemNode, "width");
                var itemHeight = GetNodeAttribute(destinationItemNode, "height");
                var itemOrder = GetNodeAttribute(destinationItemNode, "order");

                var itemStyle =
                    "display: block; top: {0}px; left: {1}px; width: {2}px; height: {3}px; position: absolute; border: solid 1px #000;";
                itemStyle = string.Format(itemStyle, new object[] {itemTop, itemLeft, itemWidth, itemHeight});

                var destinationItemtDiv = CreateElement(xdoc, "span");
                AssignAttribute(xdoc, destinationItemtDiv, "style", itemStyle);
                destinationItemtDiv.InnerText = " ";

                destinationObjectDiv.AppendChild(destinationItemtDiv);
            }

            return destinationObjectDiv;
        }

        private static void AssignAttribute(XmlDocument doc, XmlNode dest, string attributeName, string attributeValue)
        {
            var attribute = doc.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            dest.Attributes.Append(attribute);
        }

        private static XmlElement CreateElement(XmlDocument doc, string elementName)
        {
            return doc.CreateElement(elementName, doc.DocumentElement.NamespaceURI);
        }

        private static XmlNode CreateDestinationTextDiv(XmlDocument xdoc, XmlNode destinationobjectNode)
        {
            var destinationObjectDiv = CreateElement(xdoc, "div");
            AssignAttribute(xdoc, destinationObjectDiv, "class", "partialDestinationObject partialAddDestinationText");
            AssignAttribute(xdoc, destinationObjectDiv, "type", "text");
            foreach (XmlNode destinationItemNode in destinationobjectNode.ChildNodes)
            {
                var destinationItemtDiv = CreateElement(xdoc, "div");
                var objectWidth = GetNodeAttribute(destinationItemNode, "width");
                var objectHeight = GetNodeAttribute(destinationItemNode, "height");
                var objectStyle = string.Format("width: {0}px; height: {1}px;", objectWidth, objectHeight);
                AssignAttribute(xdoc, destinationItemtDiv, "style", objectStyle);
                AssignAttribute(xdoc, destinationItemtDiv, "class", "hotSpot");
                destinationItemtDiv.InnerText = destinationItemNode.InnerText;

                destinationObjectDiv.AppendChild(destinationItemtDiv);
            }

            return destinationObjectDiv;
        }

        public static string GetAssessmentXml(string xmlContent)
        {
            if (xmlContent == null) return null;
            var result = xmlContent.Trim();

            const string openAssessment = "<assessmentItem";
            const string closeAssessment = "</assessmentItem>";

            var indexOfStartAssessment = result.IndexOf(openAssessment, StringComparison.Ordinal);
            if (indexOfStartAssessment > 0)
            {
                var indexEndAssessment = result.IndexOf(closeAssessment, StringComparison.Ordinal) +
                                         closeAssessment.Length;
                result = result.Substring(indexOfStartAssessment, indexEndAssessment - indexOfStartAssessment);
            }

            return result;
        }

        public string UpdateS3LinkForItemMedia(string S3Domain, string bucketName, string folderName)
        {
            try
            {
                //update image ,such as <img height="196" width="258" source="http://portal.linkit.com/Content/FlashModules/TestMaker/proc/linkitTestMaker.php/getImage/ItemSet_8752/images.jpg"/>
                //or <img class="imageupload " drawable="false" percent="10" src="/ItemSet_16485/ad-152962277-9190-1417056016-201502270920282340.jpg" height="400" width="600"/>
                //then change src to S3 such as https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_8752/eagle.png
                var imgNodes = _xDoc.GetElementsByTagName("img");
                if (imgNodes != null)
                {
                    foreach (XmlNode imgNode in imgNodes)
                    {
                        //Get attribute src
                        if (imgNode.Attributes["src"] != null)
                        {
                            imgNode.Attributes["src"].Value = XmlContentProcessing.ReplaceByS3LinkForItemMedia(
                                S3Domain, bucketName, folderName, imgNode.Attributes["src"].Value);
                        }
                        //sometime use source instead of src
                        if (imgNode.Attributes["source"] != null)
                        {
                            imgNode.Attributes["source"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForItemMedia(S3Domain, bucketName, folderName,
                                    imgNode.Attributes["source"].Value);
                        }
                    }
                }
                
                //with item drag and drop, destination img is saved in destinationObject tag
                //example <destinationObject percent="8" imgorgw="505" imgorgh="395" partialID="Partial_1" src="/ItemSet_16503/All-201503120341046334.jpg" width="404" height="316" type="image">
                //    <destinationItem destIdentifier="DEST_1" left="123" top="9" width="60" height="78" order="1"/>
                //    <destinationItem destIdentifier="DEST_2" left="20" top="119" width="85" height="68" order="2"/>
                //    <destinationItem destIdentifier="DEST_3" left="331" top="9" width="62" height="81" order="3"/>
                //</destinationObject>
                var destinationObjectNodes = _xDoc.GetElementsByTagName("destinationObject");
                ReplaceByS3LinkForItemMediaDestinationObject(destinationObjectNodes, S3Domain, bucketName, folderName);
                var destinationobjectNodes = _xDoc.GetElementsByTagName("destinationobject");
                ReplaceByS3LinkForItemMediaDestinationObject(destinationobjectNodes, S3Domain, bucketName, folderName);
               
                //img hot spot, the img is saved in imageHotSpot tag
                //    <imageHotSpot responseIdentifier="RESPONSE_1" src="/ItemSet_16503/Sample Image-201503120354479882.JPG" percent="10" imgorgw="386" imgorgh="185" width="386" height="185" maxhotspot="2">
                //    <sourceItem identifier="IHS_1" pointValue="2" left="54" top="80.5" width="40" height="40" typeHotSpot="checkbox">&#160;</sourceItem>
                //    <sourceItem identifier="IHS_2" pointValue="0" left="175" top="80.5" width="40" height="40" typeHotSpot="checkbox">&#160;</sourceItem>
                //    <sourceItem identifier="IHS_3" pointValue="2" left="294" top="80.5" width="40" height="40" typeHotSpot="checkbox">&#160;</sourceItem>
                //    </imageHotSpot>
                var imageHotSpotNodes = _xDoc.GetElementsByTagName("imageHotSpot");
                if(imageHotSpotNodes == null || imageHotSpotNodes.Count == 0) imageHotSpotNodes = _xDoc.GetElementsByTagName("imagehotspot");
                if (imageHotSpotNodes != null)
                {
                    foreach (XmlNode imageHotSpotNode in imageHotSpotNodes)
                    {
                        //Get attribute src
                        if (imageHotSpotNode.Attributes["src"] != null)
                        {
                            imageHotSpotNode.Attributes["src"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForItemMedia(S3Domain, bucketName, folderName,
                                    imageHotSpotNode.Attributes["src"].Value);
                        }
                        //sometime use source instead of src
                        if (imageHotSpotNode.Attributes["source"] != null)
                        {
                            imageHotSpotNode.Attributes["source"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForItemMedia(S3Domain, bucketName, folderName,
                                    imageHotSpotNode.Attributes["source"].Value);
                        }
                    }
                }
                //update audio
                //First: audio of question,then audioref will be stored in itembody, such as <itembody audioref="/ItemSet_16485/English Sentences with Audio Using the Word _Ashamed_-201502270922512097.mp3">
                //then change to S3 such as  to S3 such as audioref="https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_16485/English Sentences with Audio Using the Word _Ashamed_-201502270922512097.mp3"
                var itembodyNodes = _xDoc.GetElementsByTagName("itembody");
                //sometime itembody will be changed to itemBody
                if (itembodyNodes == null || itembodyNodes.Count == 0)
                {
                    itembodyNodes = _xDoc.GetElementsByTagName("itemBody");
                }
                if (itembodyNodes != null)
                {
                    foreach (XmlNode itembodyNode in itembodyNodes)
                    {
                        //Get attribute src
                        if (itembodyNode.Attributes["audioRef"] != null)
                        {
                            itembodyNode.Attributes["audioRef"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForItemMedia(S3Domain, bucketName, folderName,
                                    itembodyNode.Attributes["audioRef"].Value);
                        }
                    }
                }

                //second : update for audio in answer ( of multiple question ), such as <simpleChoice audioRef="/ItemSet_16487/answer - a-201503020747214969.mp3" identifier="A">
                var simpleChoiceNodes = _xDoc.GetElementsByTagName("simpleChoice");
                if (simpleChoiceNodes != null)
                {
                    foreach (XmlNode simpleChoiceNode in simpleChoiceNodes)
                    {
                        //Get attribute src
                        if (simpleChoiceNode.Attributes["audioRef"] != null)
                        {
                            simpleChoiceNode.Attributes["audioRef"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForItemMedia(S3Domain, bucketName, folderName,
                                    simpleChoiceNode.Attributes["audioRef"].Value);

                        }
                    }
                }
                var result = this.GetXmlContent();
                return result;
            }
            catch
            {
                return this.GetXmlContent();
            }
        }

        public static string ReplaceByS3LinkForItemMedia(string S3Domain, string bucketName, string folderName,
            string mediaPath)
        {
            //change source="http://portal.linkit.com/Content/FlashModules/TestMaker/proc/linkitTestMaker.php/getImage/ItemSet_8752/images.jpg"/>
            // or src="/ItemSet_16485/ad-152962277-9190-1417056016-201502270920282340.jpg"
            //to S3 direct linkt such as  https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_8752/eagle.png

            //Update for AU
            //Now https://s3.amazonaws.com/testitemmedia-au/ItemSet_51643/love-201510120324338737.jpg
            //change to  https://testitemmedia-au.s3.amazonaws.com/ItemSet_51643/love-201510120324338737.jpg

            string result = string.Empty;
            if (string.IsNullOrEmpty(mediaPath))
            {
                return string.Empty;
            }
            result = mediaPath;
            var idx = mediaPath.LastIndexOf("ItemSet_");
            var qti3pIdx = mediaPath.LastIndexOf(DataFileUploadConstant.QTI3pSourceUploadSubFolder);
            if (idx >= 0 || qti3pIdx > 0)
            {
                if (idx >= 0)
                {
                    result = mediaPath.Substring(idx);
                }
                else
                {
                    result = mediaPath.Substring(qti3pIdx);
                }
                var subDomain = UrlUtil.GenerateS3Subdomain(S3Domain, bucketName);
                if (string.IsNullOrEmpty(folderName))
                {
                    result = string.Format("{0}/{1}", subDomain.RemoveEndSlash(), result.Replace(" ", "%20").RemoveStartSlash());
                }
                else
                {
                    result = string.Format("{0}/{1}/{2}", subDomain.RemoveEndSlash(), folderName.RemoveStartSlash().RemoveEndSlash(),
                        result.Replace(" ", "%20").RemoveStartSlash());
                }

            }
            return result;
        }

        public string UpdateS3LinkForPassageMedia(string S3Domain, string bucketName, string folderName)
        {
            try
            {
                //change audioRef="/RO/RO_6069_media/English Sentences with Audio Using the Word _Ashamed_-201503060435084447.mp3">
                // or <img class="imageupload " drawable="false" percent="10" src="..\RO\RO_6069_media\1-9706-1417056015-201503060434384817.jpg" height="330" width="600" source="" />
                //to S3 direct linkt such as  https://s3.amazonaws.com/testitemmedia/Vina/RO/RO_6069_media/1-9706-1417056015-201503060434384817.jpg
                var imgNodes = _xDoc.GetElementsByTagName("img");
                if (imgNodes != null)
                {
                    foreach (XmlNode imgNode in imgNodes)
                    {
                        //Get attribute src
                        if (imgNode.Attributes["src"] != null)
                        {
                            imgNode.Attributes["src"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForPassageMedia(S3Domain, bucketName, folderName,
                                    imgNode.Attributes["src"].Value);
                        }
                        //sometime use source instead of src
                        if (imgNode.Attributes["source"] != null)
                        {
                            imgNode.Attributes["source"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForPassageMedia(S3Domain, bucketName, folderName,
                                    imgNode.Attributes["source"].Value);
                        }
                    }
                }
                var result = this.GetXmlContent();
                //update audio
                //audio is stored as audioRef like this <div class="passage" toolName="linkitTLF" xmlUnicode="true" toolVersion="2.0" audioRef="/RO/RO_6069_media/English Sentences with Audio Using the Word _Ashamed_-201503060435084447.mp3">
                var divNodes = _xDoc.GetElementsByTagName("div");

                if (divNodes != null)
                {
                    foreach (XmlNode divNode in divNodes)
                    {
                        //Get attribute src
                        if (divNode.Attributes["audioRef"] != null)
                        {
                            divNode.Attributes["audioRef"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForPassageMedia(S3Domain, bucketName, folderName,
                                    divNode.Attributes["audioRef"].Value);
                        }
                    }
                }
                //sometime it is saved in <passage
                var passageNodes = _xDoc.GetElementsByTagName("passage");

                if (passageNodes != null)
                {
                    foreach (XmlNode passageNode in passageNodes)
                    {
                        //Get attribute src
                        if (passageNode.Attributes["audioRef"] != null)
                        {
                            passageNode.Attributes["audioRef"].Value =
                                XmlContentProcessing.ReplaceByS3LinkForPassageMedia(S3Domain, bucketName, folderName,
                                    passageNode.Attributes["audioRef"].Value);
                        }
                    }
                }
                result = this.GetXmlContent();

                return result;
            }
            catch
            {
                return this.GetXmlContent();
            }
        }

        public static string ReplaceByS3LinkForPassageMedia(string S3Domain, string bucketName, string folderName,
            string mediaPath)
        {
            //change audioRef="/RO/RO_6069_media/English Sentences with Audio Using the Word _Ashamed_-201503060435084447.mp3">
            // or <img class="imageupload " drawable="false" percent="10" src="..\RO\RO_6069_media\1-9706-1417056015-201503060434384817.jpg" height="330" width="600" source="" />
            //to S3 direct linkt such as  https://s3.amazonaws.com/testitemmedia/Vina/RO/RO_6069_media/1-9706-1417056015-201503060434384817.jpg
            string result = string.Empty;
            if (string.IsNullOrEmpty(mediaPath))
            {
                return string.Empty;
            }
            result = mediaPath;
            mediaPath = mediaPath.Replace("\\", "/");
            var idx = mediaPath.LastIndexOf("RO/RO_");

            if (idx >= 0)
            {
                result = mediaPath.Substring(idx);

                if (string.IsNullOrEmpty(folderName))
                {
                    //result = string.Format("{0}/{1}/{2}", S3Domain, bucketName, result.Replace(" ", "%20"));
                    result = string.Format("{0}/{1}", UrlUtil.GenerateS3Subdomain(S3Domain, bucketName), result.Replace(" ", "%20"));
                    
                }
                else
                {
                    //result = string.Format("{0}/{1}/{2}/{3}", S3Domain, bucketName, folderName, result.Replace(" ", "%20"));
                    result = string.Format("{0}/{1}/{2}", UrlUtil.GenerateS3Subdomain(S3Domain, bucketName), folderName.RemoveEndSlash().RemoveStartSlash(), result.Replace(" ", "%20"));
                }

            }
            idx = mediaPath.LastIndexOf("ItemSet_");//DataFileUpload Passage
            if (idx >= 0)
            {
                var subDomain = UrlUtil.GenerateS3Subdomain(S3Domain, bucketName);
                var relativeImgPath = mediaPath.Substring(idx, mediaPath.Length - idx);
                if (string.IsNullOrEmpty(folderName))
                {
                    result = string.Format("{0}/{1}", subDomain.RemoveEndSlash(), 
                    relativeImgPath.Replace(" ", "%20"));
                }
                else
                {
                    result = string.Format("{0}/{1}/{2}", subDomain.RemoveEndSlash(), folderName.RemoveStartSlash().RemoveEndSlash(),
                   relativeImgPath.Replace(" ", "%20"));
                }
                
            }
            return result;
        }

        public XmlNodeList GetElementsByTagName(string tagName)
        {
            return _xDoc.GetElementsByTagName(tagName);
        }

        public void ReplaceTags(List<string> oldTagNames, string newTagName, bool convertToLowerKey = false)
        {
            if (_xDoc == null || oldTagNames == null || newTagName == null) return;
            foreach (var oldTagName in oldTagNames)
            {
                if (convertToLowerKey)
                {
                    ReplaceTag(oldTagName.ToLower(), newTagName.ToLower());
                }
                else
                {
                    ReplaceTag(oldTagName, newTagName);
                }
            }
        }

        public void ReplaceTag(string oldTagName, string newTagName)
        {
            if (_xDoc == null) return;

            var root = _xDoc.SelectSingleNode("/*");
            if (root == null) return;

            var oldTags = _xDoc.GetElementsByTagName(oldTagName);
            if (oldTags.Count == 0) return;

            var oldTag = oldTags[0];
            while (oldTag != null)
            {
                var parrentNode = oldTag.ParentNode;
                if (parrentNode == null)
                {
                    oldTag = null;
                    continue;
                }

                var newTag = _xDoc.CreateElement(newTagName);


                while (oldTag.ChildNodes.Count != 0)
                {
                    newTag.AppendChild(oldTag.ChildNodes[0]);
                }

                var hasClassAttribute = false;
                while (oldTag.Attributes != null && oldTag.Attributes.Count != 0)
                {
                    var oldAttribute = oldTag.Attributes[0];
                    if (string.Equals(oldAttribute.Name, "class"))
                    {
                        hasClassAttribute = true;
                        var classValue = oldAttribute.Value;
                        if (classValue != null && !classValue.Contains(oldTagName))
                        {
                            classValue += " " + oldTagName;
                            oldAttribute.Value = classValue;
                        }
                    }
                    newTag.Attributes.Append(oldTag.Attributes[0]);
                }

                if (!hasClassAttribute)
                {
                    var classAttribute = _xDoc.CreateAttribute("class");
                    classAttribute.Value = oldTagName;
                    newTag.Attributes.Append(classAttribute);
                }

                parrentNode.ReplaceChild(newTag, oldTag);
                oldTag = oldTags[0];
            }
        }
        public string GetResponseRubric()
        {
            var result = string.Empty;

            if (_xDoc != null)
            {
                var responseRubrics = _xDoc.GetElementsByTagName("responseRubric");
                if (responseRubrics != null && responseRubrics.Count > 0)
                {
                    return responseRubrics[0].OuterXml;
                }
            }

            return result;
        }
        //Convert <p class="boxedtext" to <div class="boxedtext"
        public void ConvertBoxedTextTag()
        {
            if (_xDoc == null ) return;
            var pNodeList = _xDoc.GetElementsByTagName("p");
            List<XmlNode> boxedTextNodeList = new List<XmlNode>();
            foreach (XmlNode node in pNodeList)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name.ToLower().Equals("class"))
                    {
                        if (attr.Value.ToLower().Contains("boxedtext"))
                        {
                            boxedTextNodeList.Add(node);
                        }
                    }
                }
            }
            foreach (XmlNode node in boxedTextNodeList)
            {
                //replace this node
                var parrentNode = node.ParentNode;
                var newTag = _xDoc.CreateElement("div");
                while (node.ChildNodes.Count != 0)
                {
                    newTag.AppendChild(node.ChildNodes[0]);
                }
                foreach (XmlAttribute attr in node.Attributes)
                {
                    var newAttr = _xDoc.CreateAttribute(attr.Name);
                    newAttr.Value = attr.Value;
                    newTag.Attributes.Append(newAttr);
                }
                parrentNode.ReplaceChild(newTag, node);
            }
        }
        public string GetCardinality()
        {
            XmlNodeList  responseDeclaration = _xDoc.GetElementsByTagName("responseDeclaration");
            if (responseDeclaration == null)
            {
                return string.Empty;
            }
            else
            {
                if (responseDeclaration.Count == 0)
                {
                    return string.Empty;
                }
                foreach (XmlAttribute att in responseDeclaration[0].Attributes)
                {
                    if (att.Name.ToLower() == "cardinality")
                    {
                        return att.Value;
                    }
                }
            }
            return string.Empty;
        }
        public string GetDrawable()
        {
            if (_xDoc == null) return string.Empty;
            XmlNodeList extendedTextInteractions = _xDoc.GetElementsByTagName("extendedTextInteraction");
            if (extendedTextInteractions == null)
            {
                return string.Empty;
            }
            else
            {
                if (extendedTextInteractions.Count == 0)
                {
                    return string.Empty;
                }
                foreach (XmlAttribute att in extendedTextInteractions[0].Attributes)
                {
                    if (att.Name.ToLower() == "drawable")
                    {
                        return att.Value;
                    }
                }
            }
            return string.Empty;
        }
        public bool IsXmlLoadedSuccess
        {
            get { return _xDoc != null; }
        }


        public XmlDocument XmlDocument
        {
            get { return _xDoc; }
        }

        public string LoadXmlContentException
        {
            get { return _loadXmlContentException; }
        }
       
         public void RemoveSingleTag(string tagName)
        {
            if (_xDoc == null) return;

            var root = _xDoc.SelectSingleNode("/*");
            if (root == null) return;

            var tags = _xDoc.GetElementsByTagName(tagName);
            if (tags.Count == 0) return;

            var tag = tags[0];
            //get the parent of this tag
            var parentTag = tag.ParentNode;
            //remove this tag from parent
            parentTag.RemoveChild(tag);
        }

        public void ScaleTable(int maxWidth)
        {
            if (_xDoc == null) return;
            var nodes = _xDoc.GetElementsByTagName("table");
            foreach (XmlNode tableNode in nodes)
            {
                if (tableNode.Attributes == null) continue;
                if (!tableNode.HasChildNodes) continue;

                var clazz = GetNodeAttribute(tableNode, "class");
                if (string.IsNullOrWhiteSpace(clazz) || !clazz.Contains("linkit-table")) continue;

                var width = GetCssWidth(tableNode);
                if (!width.HasValue || width <= 0 || width <= maxWidth) continue;

                var reducePercent = ((float)width - maxWidth) / width;

                foreach (XmlNode tableChildNode in tableNode.ChildNodes)
                {
                    if (tableChildNode.Name == "tbody")
                    {
                        if (tableChildNode.HasChildNodes) ScaleTable(tableChildNode.ChildNodes, reducePercent.Value);
                    }
                    else if (tableChildNode.Name == "tr")
                    {
                        ScaleTable(tableChildNode, reducePercent.Value);
                    }
                }

                SetCssWidth(tableNode, maxWidth);
            }
        }

        private void ScaleTable(XmlNodeList trNodes, float reducePercent)
        {
            foreach (XmlNode trNode in trNodes)
            {
                ScaleTable(trNode, reducePercent);
            }
        }

        private void ScaleTable(XmlNode trNode, float reducePercent)
        {
            if (trNode.Name != "tr") return;
            if (!trNode.HasChildNodes) return;
            foreach (XmlNode tdNode in trNode.ChildNodes)
            {
                if (tdNode.Name != "td") continue;
                var tdWidth = GetCssWidth(tdNode);
                if (!tdWidth.HasValue) continue;
                var tdScaledWith = tdWidth.Value * (1 - reducePercent) - 10;
                SetCssWidth(tdNode, (int)tdScaledWith);
            }
        }

        private int? GetCssWidth(XmlNode node)
        {
            if (node == null) return null;

            var style = GetNodeAttribute(node, "style");
            if (string.IsNullOrWhiteSpace(style) || !style.Contains("width")) return null;

            var cssArr = style.Split(';');
            var cssWidth = cssArr.FirstOrDefault(o => o.Contains("width"));
            if (string.IsNullOrWhiteSpace(cssWidth)) return null;
           
            var strWidth = string.Join("", cssWidth.ToCharArray().Where(Char.IsDigit));
            
            int width = 0;
            if (!int.TryParse(strWidth, out width)) return null;

            if (cssWidth.Contains("%")) //case width table is %
            {
                width = width*817; //revert table width to px, 817 is the width of body in testeditor
            }
            return width;
        }

        private void SetCssWidth(XmlNode node, int width)
        {
            if (node == null) return;

            var style = GetNodeAttribute(node, "style");
            if (string.IsNullOrWhiteSpace(style) || !style.Contains("width")) return;

            var cssArr = style.Split(';');
            var cssWidth = cssArr.FirstOrDefault(o => o.Contains("width"));
            if (string.IsNullOrWhiteSpace(cssWidth)) return;

            var newCssWidth = string.Format("width:{0}px", width);
            var newStyle = style.Replace(cssWidth, newCssWidth);

            SetNodeAttribute(node, "style", newStyle);
            SetNodeAttribute(node, "width", string.Empty);
        }

        public string UpdateS3LinkForPassageLink(string S3Domain, string bucketName)
        {
            //change passage link like https://s3.amazonaws.com/testitemmedia/Vina/QTI3pSourceUpload/Progress/export_package_fl_socialstudies_world-geography-2015-08-19-201512020946017532/passages/3218.html
            //to https://testitemmedia.s3.amazonaws.com/Vina/QTI3pSourceUpload/Progress/export_package_fl_socialstudies_world-geography-2015-08-19-201512020946017532/passages/3218.html
            try
            {
                var objectNodes = _xDoc.GetElementsByTagName("object");
                if (objectNodes != null)
                {
                    foreach (XmlNode objectNode in objectNodes)
                    {
                        //Get attribute src
                        var data = objectNode.Attributes["data"];
                        if (data != null)
                        {
                            if (data.Value.ToLower().StartsWith(S3Domain.ToLower()))
                            {
                                bucketName = bucketName.ToLower();
                                var bucketIdx = data.Value.ToLower().IndexOf(bucketName);
                                if (bucketIdx > 0)
                                {
                                    var tmp = data.Value.Substring(bucketIdx);//return testitemmedia/Vina/QTI3pSourceUpl/Vina/......
                                    var newUrl = string.Empty;
                                    if (data.Value.ToLower().StartsWith("https"))
                                    {
                                        newUrl = S3Domain.Replace("https://", "https://" + bucketName + ".");
                                    }
                                    else
                                    {
                                        //start with http
                                        newUrl = S3Domain.Replace("http://", "http://" + bucketName + ".");
                                    }
                                    newUrl = string.Format("{0}/{1}", newUrl.RemoveEndSlash(),
                                        tmp.Replace(bucketName, "").RemoveStartSlash());
                                    objectNode.Attributes["data"].Value = newUrl;
                                }

                            }
                        }
                      
                    }
                }
                return this.GetXmlContent();
               
            }
            catch
            {
                return this.GetXmlContent();
            }
        }

        public string AddNoshuffleAttrForPassage(List<int> dataFileUploadPassageIds, List<int> qti3pPassageIds, List<int> qtiRefObjectIds, List<string> passageUrls)
        {
            //add attr 'noshuffle=true' to object
            var objectNodes = _xDoc.GetElementsByTagName("object");

            for (int i = 0; i < objectNodes.Count; i++)
            {
                var node = objectNodes[i];

                var dataFileUploadPassageId = 0;
                var qti3pPassageId = 0;
                var qtiRefObjectId = 0;
                var data = string.Empty;
                if (node.Attributes["dataFileUploadPassageID"] != null)
                {
                    //Progressive DataFile Upload
                    dataFileUploadPassageId = Int32.Parse(node.Attributes["dataFileUploadPassageID"].Value);
                }

                if (node.Attributes["Qti3pPassageID"] != null)
                {
                    //Progressive DataFile Upload in ThirdPartyItemBank
                    qti3pPassageId = Int32.Parse(node.Attributes["Qti3pPassageID"].Value);
                }

                if (node.Attributes["refObjectID"] != null)
                {
                    qtiRefObjectId = Int32.Parse(node.Attributes["refObjectID"].Value); //only linkit passage has refObjectID
                }

                if (node.Attributes["data"] != null)
                {
                    data = node.Attributes["data"].Value;
                }

                if ((dataFileUploadPassageId > 0 && dataFileUploadPassageIds.Contains(dataFileUploadPassageId)) 
                    || (qti3pPassageId > 0 && qti3pPassageIds.Contains(qti3pPassageId)) 
                    || (qtiRefObjectId > 0 && qtiRefObjectIds.Contains(qtiRefObjectId))
                    || (passageUrls.Contains(data)))
                {
                    XmlUtils.SetOrUpdateNodeAttribute(ref node, "noshuffle", "true");
                }
            }

            return this.GetXmlContent();
        }

        public string UpdateS3LinkForItemMediaQuestionGroup(string S3Domain, string bucketName, string folderName)
        {
            try
            {                 
                var result = this.GetXmlContent();                  
               
                //update audio
                //First: audio of question,then audioref will be stored in itembody, such as <itembody audioref="/ItemSet_16485/English Sentences with Audio Using the Word _Ashamed_-201502270922512097.mp3">
                //then change to S3 such as  to S3 such as audioref="https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_16485/English Sentences with Audio Using the Word _Ashamed_-201502270922512097.mp3"
                var itembodyNodes = _xDoc.GetElementsByTagName("div");
                 
                if (itembodyNodes != null)
                {
                    foreach (XmlNode itembodyNode in itembodyNodes)
                    {
                        //Get attribute src
                        if (itembodyNode.Attributes["audioRef"] != null)
                        {
                            itembodyNode.Attributes["audioRef"].Value = XmlContentProcessing.ReplaceByS3LinkForItemMediaQuestionGroup(S3Domain, bucketName, folderName, itembodyNode.Attributes["audioRef"].Value);
                        }
                    }
                }
                result = this.GetXmlContent();                
                return result;
            }
            catch
            {
                return this.GetXmlContent();
            }
        }

        public static string ReplaceByS3LinkForItemMediaQuestionGroup(string S3Domain, string bucketName, string folderName, string mediaPath)
        {
            //change source="http://portal.linkit.com/Content/FlashModules/TestMaker/proc/linkitTestMaker.php/getImage/ItemSet_8752/images.jpg"/>
            // or src="/ItemSet_16485/ad-152962277-9190-1417056016-201502270920282340.jpg"
            //to S3 direct linkt such as  https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_8752/eagle.png

            //Update for AU
            //Now https://s3.amazonaws.com/testitemmedia-au/ItemSet_51643/love-201510120324338737.jpg
            //change to  https://testitemmedia-au.s3.amazonaws.com/ItemSet_51643/love-201510120324338737.jpg

            string result = string.Empty;
            if (string.IsNullOrEmpty(mediaPath))
            {
                return string.Empty;
            }
            result = mediaPath;
            var idx = mediaPath.LastIndexOf("RO/RO_0_media");
            var qti3pIdx = mediaPath.LastIndexOf(DataFileUploadConstant.QTI3pSourceUploadSubFolder);
            if (idx >= 0 || qti3pIdx > 0)
            {
                if (idx >= 0)
                {
                    result = mediaPath.Substring(idx);
                }
                else
                {
                    result = mediaPath.Substring(qti3pIdx);
                }
                var subDomain = UrlUtil.GenerateS3Subdomain(S3Domain, bucketName);
                if (string.IsNullOrEmpty(folderName))
                {
                    result = string.Format("{0}/{1}", subDomain.RemoveEndSlash(), result.Replace(" ", "%20").RemoveStartSlash());
                }
                else
                {
                    result = string.Format("{0}/{1}/{2}", subDomain.RemoveEndSlash(), folderName.RemoveStartSlash().RemoveEndSlash(),
                        result.Replace(" ", "%20").RemoveStartSlash());
                }

            }
            return result;
        }

        #region xml         
        static string CharacterEntities(string input)
        {
            //input = "Something with &mdash; or other character entities.";
            StringBuilder output = new StringBuilder(input.Length);

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '&')
                {
                    int startOfEntity = i; // just for easier reading
                    int endOfEntity = input.IndexOf(';', startOfEntity);
                    string entity = input.Substring(startOfEntity, endOfEntity - startOfEntity);
                    int unicodeNumber = (int)(HttpUtility.HtmlDecode(entity)[0]);
                    output.Append("&#" + unicodeNumber + ";");
                    i = endOfEntity; // continue parsing after the end of the entity
                }
                else
                    output.Append(input[i]);
            }
            return output.ToString();
        }
        #endregion

        private void ReplaceByS3LinkForItemMediaDestinationObject(XmlNodeList destinationObjectNodes, string S3Domain, string bucketName, string folderName)
        {
            if (destinationObjectNodes != null)
            {
                foreach (XmlNode destinationObjectNode in destinationObjectNodes)
                {
                    //Get attribute src
                    if (destinationObjectNode.Attributes["src"] != null)
                    {
                        destinationObjectNode.Attributes["src"].Value =
                            XmlContentProcessing.ReplaceByS3LinkForItemMedia(S3Domain, bucketName, folderName,
                                destinationObjectNode.Attributes["src"].Value);
                    }
                    //sometime use source instead of src
                    if (destinationObjectNode.Attributes["source"] != null)
                    {
                        destinationObjectNode.Attributes["source"].Value =
                            XmlContentProcessing.ReplaceByS3LinkForItemMedia(S3Domain, bucketName, folderName,
                                destinationObjectNode.Attributes["source"].Value);
                    }
                }
            }
        }

    }
}
