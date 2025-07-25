using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessing
{
    internal class MultipleChoiceProcessing
    {
        public static DataFileUploaderResource Convert(DataFileUploaderParameter parameter, DataFileUploaderResource resource)
        {

            try
            {
                resource.ProcessingStep.Append("Start Converting.");
                resource.ProcessingStep.Append("->Check Extension.");
                if (Path.GetExtension(resource.ResourceFileName) != null && Path.GetExtension(resource.ResourceFileName).ToLower() == ".css")
                {
                    resource.ErrorDetail = string.Format("\"{0}\" is ignored,it's CSS file.", resource.ResourceFileName);
                    resource.QtiSchemaID = 0;
                    return resource;
                }
                resource.ProcessingStep.Append("->Read resource file.");
                var resourceFilePath = string.Format("{0}/{1}", parameter.ExtractedFoler, resource.ResourceFileName);
                if (!System.IO.File.Exists(resourceFilePath))
                {
                    //resource.Error = string.Format("Can not find resource file \"{0}\"", resource.ResourceFileName);
                    return resource;
                }
                var resourceContent = System.IO.File.ReadAllText(resourceFilePath);
                resource.OriginalContent = resourceContent;

                resourceContent = resourceContent.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "");

                resource.ProcessingStep.Append("->Remove Trouble Characters.");
                resourceContent = resourceContent.RemoveTroublesomeCharacters();

                resource.ProcessingStep.Append("->Load into XmlContentProcessing to process.");

                XmlContentProcessing doc = new XmlContentProcessing(resourceContent);
                if (doc == null || !doc.IsXmlLoadedSuccess)
                {
                    resource.Error = string.Format("Can not load resource file \"{0}\" in xml to process", resource.ResourceFileName);
                    resource.ErrorDetail = doc.LoadXmlContentException;
                    return resource;
                }
                //get tag assessmentItem
                resource.ProcessingStep.Append("->Get tag assessmentItem.");
                XmlNodeList assessmentItemTags = doc.GetElementsByTagName("assessmentItem");
                if (assessmentItemTags == null || assessmentItemTags.Count == 0)
                {
                    resource.Error = string.Format("Can not load assessmentItem of resource file \"{0}\" in xml to process.", resource.ResourceFileName);
                    return resource;
                }
                //check if there's choiceInteraction or not
                resource.ProcessingStep.Append("->Check if there's choiceInteraction or not.");

                var choiceInteractionNode = doc.GetElementsByTagName("choiceInteraction");
                if (choiceInteractionNode == null || choiceInteractionNode.Count == 0)
                {
                    resource.Error = "Can not find tag choiceInteraction.";
                    return resource;
                }
                resource.ProcessingStep.Append("->Update attributes for assessmentItemTag.");

                XmlNode assessmentItemTag = assessmentItemTags[0];
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "xsi:schemaLocation", "http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd");
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "adaptive", "false");
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "timeDependent", "false");
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "xmlUnicode", "true");
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "toolName", "linkitTLF");
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "toolVersion", "2.0");
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", "1");
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "xmlns", "http://www.imsglobal.org/xsd/imsqti_v2p0");
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                XmlUtils.AddAttribute(assessmentItemTag, "Importfrom", DataFileUploadTypeEnum.DataFileUpload.ToString());

                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                AddStyleSheetNode(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Update correct response.");
                var correctResponse = UpdateCorrectResponse(ref assessmentItemTag);
                if (string.IsNullOrWhiteSpace(correctResponse))
                {
                    resource.Error = "Can not get Update CorrectResponse of resource file.";
                    return resource;
                }
                resource.ProcessingStep.Append("->Set Response DeclarationNode.");
                SetResponseDeclarationNode(ref assessmentItemTag, correctResponse);

                resource.ProcessingStep.Append("->Remove  Outcome DeclarationTag.");
                RemoveOutcomeDeclarationTag(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Update ChoiceInteraction");
                string result = UpdateChoiceInteraction(ref assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    resource.Error = string.Format("Can not Update ChoiceInteraction \"{0}\": {1} ", resource.ResourceFileName, result);
                    return resource;
                }

                resource.ProcessingStep.Append("->Create Main Body Tag");
                result = CreateMainBodyTag(ref assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    resource.Error = string.Format("Can not Create MainBodyTag \"{0}\": {1} ", resource.ResourceFileName, result);
                    return resource;
                }

                resource.ProcessingStep.Append("->Update Simplechoice");
                result = UpdateSimplechoice(ref assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    resource.Error = string.Format("Can not Update Simplechoice \"{0}\": {1} ", resource.ResourceFileName, result);
                    return resource;
                }

                resource.ProcessingStep.Append("->Update MediaFilePathInXmlContent");
                UpdateMediaFilePathInXmlContent(ref assessmentItemTag, parameter.QtiGroupId, ref resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
                {
                    return resource;
                }
                //after all, get the xmlContent
                resource.ProcessingStep.Append("->DONE. Get final converted XmlContent.");
                resource.XmlContent = doc.GetXmlContent();
                //SupportRemoveBlankSpans(ref resource);
            }
            catch (Exception ex)
            {
                resource.Error = string.Format("There was some error when converting \"{0}\" ", resource.ResourceFileName);
                resource.ErrorDetail = ex.GetFullExceptionMessage();

            }
            return resource;
        }

        private static void AddStyleSheetNode(ref XmlNode assessmentItemTag)
        {
            XmlUtils.RemoveNodeByName(ref assessmentItemTag, "stylesheet");
            XmlNode styleSheetNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "stylesheet",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
            XmlUtils.AddAttribute(styleSheetNode, "href", "stylesheet/linkitStyleSheet.css");
            XmlUtils.AddAttribute(styleSheetNode, "type", "text/css");
            XmlUtils.InsertFirstChild(ref assessmentItemTag, styleSheetNode);
        }
        private static string UpdateCorrectResponse(ref XmlNode assessmentItemTag)
        {
            List<string> alphabetAnswerIdentifiers =
             new List<string>(new string[]
                                     {
                                         "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"
                                         , "Q", "R", "S", "T", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF",
                                         "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP"
                                     }); //Flash does not use 'U'
            string correctResponse = ""; // they uset correctResponse like ITBmxrNUHTbUuuNsc_R_3 
            //<correctResponse>
            //<value>ITBmxrNUHTbUuuNsc_R_3</value>
            //</correctResponse>

            //So it's necessary to convert into A,B,C,...
            if (assessmentItemTag.OwnerDocument.GetElementsByTagName("correctResponse") == null)
            {
                return string.Empty;
            }
            XmlNode correctResponseNode = assessmentItemTag.OwnerDocument.GetElementsByTagName("correctResponse")[0];
            XmlNode valudeNode = XmlUtils.GetSingleChildNodeByName(correctResponseNode, "value");
            if (valudeNode == null)
            {
                return string.Empty;
            }
            correctResponse = valudeNode.InnerText;//value node
            if (string.IsNullOrWhiteSpace(correctResponse))
            {
                return correctResponse;
            }
            XmlNodeList simpleChoiceNodeList = assessmentItemTag.OwnerDocument.GetElementsByTagName("simpleChoice");
            List<string> identifierList = new List<string>();
            foreach (XmlNode simpleChoiceNode in simpleChoiceNodeList)
            {
                identifierList.Add(XmlUtils.GetNodeAttribute(simpleChoiceNode, "identifier"));
            }
            var orderedIdentifierList = identifierList.OrderBy(x => x).ToList();
            string linkitCorrectResponse = string.Empty;
            for (int i = 0; i < orderedIdentifierList.Count; i++)
            {
                if (orderedIdentifierList[i] == correctResponse)
                {
                    linkitCorrectResponse = alphabetAnswerIdentifiers[i];
                }
            }
            //Update identifier into Linkit format
            for (int i = 0; i < simpleChoiceNodeList.Count; i++)
            {
                XmlNode simpleChoiceNode = simpleChoiceNodeList[i];
                var identifier = XmlUtils.GetNodeAttribute(simpleChoiceNode, "identifier");
                for (int j = 0; j < orderedIdentifierList.Count; j++)
                {
                    if (orderedIdentifierList[j] == identifier)
                    {
                        XmlUtils.SetOrUpdateNodeAttribute(ref simpleChoiceNode, "identifier", alphabetAnswerIdentifiers[j]);
                    }
                }
            }

            return linkitCorrectResponse;
        }
        private static void SetResponseDeclarationNode(ref XmlNode assessmentItemTag, string correctResponse)
        {
            XmlNode responseDeclaration = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "responseDeclaration");
            if (responseDeclaration == null)
            {
                XmlNode newNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "responseDeclaration",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                var stylesheet = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "stylesheet");
                assessmentItemTag.InsertBefore(newNode, stylesheet);
                responseDeclaration = newNode;
            }
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "identifier", "RESPONSE_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "baseType", "identifier");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "cardinality", "single");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "method", "default");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "caseSensitive", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "type", "string");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", "1");
            //set correctResponse
            XmlNode correctResponseNode = XmlUtils.GetSingleChildNodeByName(responseDeclaration, "correctResponse");
            if (correctResponseNode == null)
            {
                correctResponseNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "correctResponse",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                XmlNode valueNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "value",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                valueNode.InnerText = correctResponse;
                correctResponseNode.AppendChild(valueNode);
                responseDeclaration.AppendChild(correctResponseNode);
            }
            else
            {
                XmlNode valueNode = XmlUtils.GetSingleChildNodeByName(correctResponseNode, "value");
                if (valueNode != null)
                {
                    valueNode.InnerText = correctResponse;
                }
                else
                {
                    valueNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "value",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                    valueNode.InnerText = correctResponse;
                }
            }
        }

        private static void RemoveOutcomeDeclarationTag(ref XmlNode assessmentItemTag)
        {
            XmlUtils.RemoveNodeByName(ref assessmentItemTag, "outcomeDeclaration");

        }
        private static string CreateMainBodyTag(ref XmlNode assessmentItemTag)
        {
            //get itemBody tag
            XmlNode itemBodyNode = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itemBody");
            if (itemBodyNode == null)
            {
                //try to get with name itembody
                itemBodyNode = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itembody");
            }
            if (itemBodyNode == null)
            {
                return "Can not find tag itemBody";
            }
           
            XmlNode mainBodyNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "div",
                assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
            XmlUtils.SetOrUpdateNodeAttribute(ref mainBodyNode, "class", "mainBody");
            XmlUtils.SetOrUpdateNodeAttribute(ref mainBodyNode, "styleName", "mainBody");

            //move all child nodes of itemBodyNode intomainBodyNode
            XmlUtils.MoveChildNodes(ref itemBodyNode, ref mainBodyNode);

            //then //insert mainBodyNode as the first child of itemBodyNode
            XmlUtils.InsertFirstChild(ref itemBodyNode, mainBodyNode);
           
            return string.Empty;
        }

        private static string UpdateChoiceInteraction(ref XmlNode assessmentItemTag)
        {
            //get itemBody tag
            XmlNode itemBodyNode = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itemBody");
            if (itemBodyNode == null)
            {
                //try to get with name itembody
                itemBodyNode = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itembody");
            }
            if (itemBodyNode == null)
            {
                return "Can not find tag itemBody";
            }

            XmlNode choiceInteractionNode = XmlUtils.GetSingleChildNodeByName(itemBodyNode, "choiceInteraction");
            if (choiceInteractionNode == null)
            {
                return "Can not find choiceInteraction";
            }

            XmlUtils.SetOrUpdateNodeAttribute(ref choiceInteractionNode, "responseIdentifier", "RESPONSE_1");

            return string.Empty;
        }

        private static string UpdateSimplechoice(ref XmlNode assessmentItemTag)
        {
            //Need to add <div class="answer" styleName="answer"> right after simpleChoice 
            //Convert any p inside to div

            XmlNodeList simpleChoiceNodeList = assessmentItemTag.OwnerDocument.GetElementsByTagName("simpleChoice");
            if (simpleChoiceNodeList == null)
            {
                return "Can not find tag simpleChoice";
            }
            if (simpleChoiceNodeList.Count == 0)
            {
                return "Can not find tag simpleChoice";
            }
            for (int i = 0; i < simpleChoiceNodeList.Count; i++)
            {
                XmlNode simpleChoiceNode = simpleChoiceNodeList[i];
                XmlNode div= assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "div",assessmentItemTag.OwnerDocument.NamespaceURI);

              
                
                XmlUtils.MoveChildNodes(ref simpleChoiceNode,ref div);
                
                //replace p to div
                //get inner xml of simpleChoiceNode
                var innerXml = div.InnerXml;
                //make sure innerXml is covered by a Root to enable XmlContentProcessing.ReplaceTag work
                innerXml = string.Format("<ROOT_SimpleChoiceNode>{0}</ROOT_SimpleChoiceNode>", innerXml);
                XmlContentProcessing xml = new XmlContentProcessing(innerXml);
                xml.ReplaceTag("p","div");
                innerXml = xml.GetXmlContent();
                div.RemoveAll();
                div.InnerXml = innerXml.Replace("<ROOT_SimpleChoiceNode>", "").Replace("</ROOT_SimpleChoiceNode>","");

                var attClass = assessmentItemTag.OwnerDocument.CreateAttribute("class");
                attClass.Value = "answer";
                var attStyleName = assessmentItemTag.OwnerDocument.CreateAttribute("styleName");
                attStyleName.Value = "answer";
                div.Attributes.Append(attClass);
                div.Attributes.Append(attStyleName);

                simpleChoiceNode.AppendChild(div);

            }

            return string.Empty;
        }

        private static void UpdateMediaFilePathInXmlContent(ref XmlNode assessmentItemTag,int qtiGroupId, ref DataFileUploaderResource resource)
        {
            try
            {
                XmlNodeList imgNodeList = assessmentItemTag.OwnerDocument.GetElementsByTagName("img");
                if (imgNodeList == null || imgNodeList.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < imgNodeList.Count; i++)
                {
                    XmlNode imgNode = imgNodeList[i];
                    var src = XmlUtils.GetNodeAttributeCaseInSensitive(imgNode, "src");
                    try
                    {
                        //get value of src
                        if (!string.IsNullOrWhiteSpace(src))
                        {
                            if (!src.ToLower().StartsWith("http"))
                            {
                                //change src value
                                XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "src", string.Format("/ItemSet_{0}/{1}", qtiGroupId, src));

                               resource.MediaFileList.Add(src);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        resource.Error = string.Format("Can not update image file in xml content: {0}", src);
                        resource.ErrorDetail =  ex.GetFullExceptionMessage();
                    }

                }
            }
            catch (Exception ex)
            {
                resource.Error = "Can not update image file in xml content";
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }


        }

        private static void SupportRemoveBlankSpans(ref DataFileUploaderResource resource)
        {
            if (!resource.IsValidQuestionResourceFile) { return;}
            if (!string.IsNullOrEmpty(resource.Error)) { return;}
            if (string.IsNullOrEmpty(resource.XmlContent)) { return; }
            //remove blank paragraph like this 
            //<p id="DMAYB">
            //<span id="58BAB"> </span>
            //</p>
            //This function is just an addition to make linkit qti item look betters, because at this point, the customer xml content has been converted to Linkit format successfully
            string xmlContent = resource.XmlContent;
            resource.ProcessingStep.Append("->Support Remove Blank Paragraphs");

            string original = xmlContent;
            try
            {
                var doc = new XmlContentProcessing(xmlContent);
                if (!doc.IsXmlLoadedSuccess)
                {
                    resource.ProcessingStep.Append("->Support Remove Blank Paragraphs:Load into XmlContentProcessing error");
                    return ;
                }
              
                XmlNodeList spanNodeList = doc.GetElementsByTagName("span");
                if (spanNodeList != null)
                {
                    for (int i = spanNodeList.Count - 1; i >=0 ; i--)
                    {
                        var spanNode = spanNodeList[i];
                        if (spanNode.InnerText.Trim().Length == 0)
                        {
                            var parent = spanNode.ParentNode;
                            parent.RemoveChild(spanNode);
                            //i--; //because removing a spanNode will decrease spanNodeList.Count so it's necessary to decrease i to get the next item to process
                            //if (i < 0)
                            //{
                            //    i = 0;
                            //}
                        }
                    }
                }
                xmlContent = doc.GetXmlContent();
                if (string.IsNullOrEmpty(xmlContent))
                {
                    resource.ProcessingStep.Append("->Support Remove Blank Paragraphs:Failed");
                }
                else
                {
                    resource.ProcessingStep.Append("->Support Remove Blank Paragraphs:OK-Saved updated XmlContent");
                    resource.XmlContent = xmlContent;
                    resource.ErrorDetail = original;//keep the original for checking later, if necessary
                    
                }

            }
            catch(Exception ex)
            {
                resource.Error = "Can not Support Remove Blank Paragraphs";
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }
        }
    }
}
