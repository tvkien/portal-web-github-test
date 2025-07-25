using LinkIt.BubbleSheetPortal.Common.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica
{
    internal class MultipleChoiceProcessing
    {
        public static DataFileUploaderResource Convert(DataFileUploaderParameter parameter,
            DataFileUploaderResource resource, XmlContentProcessing doc)
        {            
            try
            {               
                resource.ProcessingStep.Append("Start Converting.");
                
                var assessmentItemTag = DataFileProcessing.GetAssessmentItemNode(doc, resource);
                if (assessmentItemTag == null)
                {
                    return resource;
                }

                //check complex type
                //if (DataFileProcessing.CheckComplexType(assessmentItemTag, resource))
                //{
                //    return ComplexProcessing.Convert(parameter, resource);
                //}

                //Check if this is a single select of multi select
                if (DataFileProcessing.CheckMultiSelect(assessmentItemTag, resource))
                {
                    resource.QtiSchemaID = (int)QtiSchemaEnum.MultiSelect;
                    XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", resource.QtiSchemaID.ToString());
                }
                else
                {
                    resource.QtiSchemaID = (int)QtiSchemaEnum.MultipleChoice;
                    XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", ((int)QtiSchemaEnum.MultipleChoice).ToString());
                }                
                
                //check if there's choiceInteraction or not
                resource.ProcessingStep.Append("->Check if there's choiceInteraction or not.");
                var choiceInteractionNode = doc.GetElementsByTagName("choiceInteraction");
                if (choiceInteractionNode == null || choiceInteractionNode.Count == 0)
                {
                    resource.Error = "Can not find tag choiceInteraction.";
                    return resource;
                }
                

                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(assessmentItemTag);

                resource.ProcessingStep.Append("->Update correct response.");
                var correctResponse = UpdateCorrectResponse(assessmentItemTag);
                if (string.IsNullOrWhiteSpace(correctResponse))
                {
                    resource.Error = "Can not get correctResponse of resource file.";
                    return resource;
                }

                resource.ProcessingStep.Append("->Set Response DeclarationNode.");
                SetResponseDeclarationNode(assessmentItemTag, correctResponse, resource.QtiSchemaID);

                resource.ProcessingStep.Append("->Remove Outcome DeclarationTag.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Response Processing.");
                DataFileProcessing.RemoveResponseProcessingTag(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Rubric Block.");
                resource.Error = DataFileProcessing.RemoveRubricBlockTag(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Update ChoiceInteraction");
                string result = UpdateChoiceInteraction(assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    resource.Error = string.Format("There was some error when processing \"{0}\": {1} ", resource.ResourceFileName, result);
                    return resource;
                }

                resource.ProcessingStep.Append("->Create Main Body Tag");
                result = DataFileProcessing.CreateMainBodyTag(assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    resource.Error = string.Format("There was some error when processing \"{0}\": {1} ", resource.ResourceFileName, result);
                    return resource;
                }
                resource.ProcessingStep.Append("->Update Simplechoice");
                result = UpdateSimplechoice(assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    resource.Error = string.Format("Can not Update Simplechoice \"{0}\": {1} ", resource.ResourceFileName, result);
                    return resource;
                }
                
                DataFileProcessing.UpdateMediaFilePathInXmlContent(assessmentItemTag, parameter, resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
                {
                    return resource;
                }
                //Update passage
                //Move tag object under itemBody
                DataFileProcessing.InsertTagObjectForPassage(assessmentItemTag, resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
                {
                    return resource;
                }

                //after all, get the xmlContent
                resource.ProcessingStep.Append("->DONE. Get final converted XmlContent.");
                resource.XmlContent = doc.GetXmlContent();
                //SupportRemoveBlankSpans(resource);
            }
            catch (Exception ex)
            {
                resource.Error = string.Format("There was some error when processing \"{0}\" ", resource.ResourceFileName);
                resource.ErrorDetail = ex.GetFullExceptionMessage();

            }
            return resource;
        }

        private static string UpdateCorrectResponse(XmlNode assessmentItemTag)
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
        private static void SetResponseDeclarationNode(XmlNode assessmentItemTag, string correctResponse, int qtiSchemaId)
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
            if (qtiSchemaId == (int) QtiSchemaEnum.MultipleChoice)
            {
                XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "cardinality", "single");
            }
            if (qtiSchemaId == (int) QtiSchemaEnum.MultiSelect)
            {
                XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "cardinality", "multiple");
            }
            
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
        
        private static string UpdateChoiceInteraction(XmlNode assessmentItemTag)
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
        private static string UpdateSimplechoice(XmlNode assessmentItemTag)
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
                XmlNode div = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "div", assessmentItemTag.OwnerDocument.NamespaceURI);



                XmlUtils.MoveChildNodes(ref simpleChoiceNode, ref div);

                //replace p to div
                //get inner xml of simpleChoiceNode
                var innerXml = div.InnerXml;
                //make sure innerXml is covered by a Root to enable XmlContentProcessing.ReplaceTag work
                innerXml = string.Format("<ROOT_SimpleChoiceNode>{0}</ROOT_SimpleChoiceNode>", innerXml);
                XmlContentProcessing xml = new XmlContentProcessing(innerXml);
                xml.ReplaceTag("p", "div");
                innerXml = xml.GetXmlContent();
                div.RemoveAll();
                div.InnerXml = innerXml.Replace("<ROOT_SimpleChoiceNode>", "").Replace("</ROOT_SimpleChoiceNode>", "");

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
        private static void UpdateMediaFilePathInXmlContent(XmlNode assessmentItemTag, int qtiGroupId, DataFileUploaderResource resource)
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
                        resource.Error = string.Format("Can not update image file in xml content: {0} ", src);
                        resource.ErrorDetail = ex.GetFullExceptionMessage();
                    }

                }
            }
            catch (Exception ex)
            {
                resource.Error = string.Format("Can not update image file in xml content.");
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }

        }
        
        private static void SupportRemoveBlankSpans(DataFileUploaderResource resource)
        {
            if (!resource.IsValidQuestionResourceFile) { return; }
            if (!string.IsNullOrEmpty(resource.Error)) { return; }
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
                    return;
                }

                XmlNodeList spanNodeList = doc.GetElementsByTagName("span");
                if (spanNodeList != null)
                {
                    for (int i = spanNodeList.Count - 1; i >= 0; i--)
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
            catch (Exception ex)
            {
                resource.Error = "Can not Support Remove Blank Paragraphs";
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }
        }

        
    }
}
