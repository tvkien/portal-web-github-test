using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;
namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica
{
    internal class ComplexProcessing
    {
        public static DataFileUploaderResource Convert(DataFileUploaderParameter parameter,
            DataFileUploaderResource resource, XmlContentProcessing doc)
        {
            try
            {
                resource.ProcessingStep.Append("Start Converting.");
                if (doc == null)
                {
                    return resource;
                }

                var assessmentItemTag = DataFileProcessing.GetAssessmentItemNode(doc, resource);
                if (assessmentItemTag == null)
                {
                    return resource;
                }
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", ((int)QtiSchemaEnum.Complex).ToString());
                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove responseProcessing.");
                DataFileProcessing.RemoveResponseProcessingTag(assessmentItemTag);

                //update responseDeclaration tag
                resource.ProcessingStep.Append("->Update Response Declaration.");
                resource.Error = SetResponseDeclarationNodeAndItemBody(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Remove Rubric Block for ExtendedText.");
                resource.Error = DataFileProcessing.RemoveRubricBlockTag(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Remove mapping tag for TextEntry.");
                //DataFileProcessing.RemoveMappingTag(ref assessmentItemTag);

                //update itemBody tag
                resource.ProcessingStep.Append("->Update MainBody.");
                resource.Error = DataFileProcessing.CreateMainBodyTag(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                DataFileProcessing.UpdateMediaFilePathInXmlContent(assessmentItemTag, parameter, resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
                {
                    return resource;
                }

                //Update passage
                //insert tag object under itemBody
                DataFileProcessing.InsertTagObjectForPassage(assessmentItemTag, resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
                {
                    return resource;
                }

                //after all, get the xmlContent
                resource.XmlContent = doc.GetXmlContent();
            }
            catch (Exception ex)
            {
                resource.Error = string.Format("There was some error when processing \"{0}\" ", resource.ResourceFileName);
                resource.ErrorDetail = ex.GetFullExceptionMessage();

            }

            //resource.Error = "Complex Question, we have not supported yet.";
            return resource;

        }

        private static string SetResponseDeclarationNodeAndItemBody(XmlNode assessmentItemTag)
        {
            var responseDeclarations = XmlUtils.GetChildNodeListByName(assessmentItemTag,"responseDeclaration");
            if(responseDeclarations == null && responseDeclarations.Count == 0)
                return "Can not find tag responseDeclaration";
            var responseDic = new Dictionary<string, XmlNode>();
            for (int i = 0; i < responseDeclarations.Count; i++)
            {
                var responseNode = responseDeclarations[i];
                var identifier = XmlUtils.GetNodeAttribute(responseNode, "identifier");
                var newIdentifier = "RESPONSE_" + (i + 1);
                XmlUtils.SetOrUpdateNodeAttribute(ref responseNode, "identifier", newIdentifier);
                responseDic.Add(identifier, responseNode);
            }
            //Extended text
            ProcessExtendedText(assessmentItemTag, responseDic);

            //Text Entry
            ProcessTextEntry(assessmentItemTag, responseDic);

            //Choice Interaction - MultipleChoice
            ProcessMultipleChoice(assessmentItemTag, responseDic);

            //inlineChoice
            ProcessInlineChoice(assessmentItemTag, responseDic);

            //texthotspot
            ProcessTextHotSpot(assessmentItemTag, responseDic);

            return string.Empty;
        }

        private static void ProcessInlineChoice(XmlNode assessmentItemTag, Dictionary<string, XmlNode> responseDic)
        {
            var inlineChoiceInteractions = assessmentItemTag.OwnerDocument.GetElementsByTagName("inlineChoiceInteraction");
            for (int i = 0; i < inlineChoiceInteractions.Count; i++)
            {
                var inlineChoiceInteraction = inlineChoiceInteractions[i];
                var responseIdentifier = XmlUtils.GetNodeAttribute(inlineChoiceInteraction, "responseIdentifier");
                var responseDeclaration = responseDic[responseIdentifier];
                XmlUtils.RemoveNodeByName(ref responseDeclaration, "defaultValue");

                var newIdentifier = XmlUtils.GetNodeAttribute(responseDeclaration, "identifier");
                SetResponseDeclarationInlineChoiceNode(responseDeclaration, inlineChoiceInteraction);
                SetInlineChoiceInteractionTag(inlineChoiceInteraction, newIdentifier);
            }
        }

        private static void ProcessExtendedText(XmlNode assessmentItemTag, Dictionary<string, XmlNode> responseDic)
        {
            var extendedTextInteractions = assessmentItemTag.OwnerDocument.GetElementsByTagName("extendedTextInteraction");
            for (int i = 0; i < extendedTextInteractions.Count; i++)
            {
                var indentifier = XmlUtils.GetNodeAttribute(extendedTextInteractions[i], "responseIdentifier");
                var responseDeclaration = responseDic[indentifier];                

                var newIdentifier = XmlUtils.GetNodeAttribute(responseDeclaration, "identifier");
                SetResponseDeclarationNode(responseDeclaration);
                SetExtendedTextInteractionTag(extendedTextInteractions[i], newIdentifier);
            }
        }

        private static void ProcessTextEntry(XmlNode assessmentItemTag, Dictionary<string, XmlNode> responseDic)
        {
            var textEntryInteractions = assessmentItemTag.OwnerDocument.GetElementsByTagName("textEntryInteraction");
            for (int i = 0; i < textEntryInteractions.Count; i++)
            {
                var responseIdentifier = XmlUtils.GetNodeAttribute(textEntryInteractions[i], "responseIdentifier");
                var responseDeclaration = responseDic[responseIdentifier];
                XmlUtils.RemoveNodeByName(ref responseDeclaration, "defaultValue");

                var newIdentifier = XmlUtils.GetNodeAttribute(responseDeclaration, "identifier");
                SetTextEntryResponseDeclarationNode(responseDeclaration);
                UpdateCorrectResponseTextEntry(responseDeclaration);
                SetTextEntryInteractionTag(textEntryInteractions[i], newIdentifier);
            }
        }

        private static string ProcessMultipleChoice(XmlNode assessmentItemTag, Dictionary<string, XmlNode> responseDic)
        {
            var error = string.Empty;
            var choiceInteractions = assessmentItemTag.OwnerDocument.GetElementsByTagName("choiceInteraction");
            for (int i = 0; i < choiceInteractions.Count; i++)
            {
                var choiceInteraction = choiceInteractions[i];
                var responseIdentifier = XmlUtils.GetNodeAttribute(choiceInteraction, "responseIdentifier");
                var responseDeclaration = responseDic[responseIdentifier];
                var cardinality = XmlUtils.GetNodeAttribute(responseDeclaration, "cardinality");

                var newIdentifier = XmlUtils.GetNodeAttribute(responseDeclaration, "identifier");
                error = UpdateCorrectResponse(responseDeclaration, choiceInteraction);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return "Can not update correctResponse of resource file.";
                }
                SetResponseDeclarationMultipleChoiceNode(responseDeclaration, cardinality);
                UpdateChoiceInteraction(choiceInteraction, newIdentifier);
                error = UpdateSimplechoice(choiceInteraction);
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }
            }
            return string.Empty;
        }
        private static void ProcessTextHotSpot(XmlNode assessmentItemTag, Dictionary<string, XmlNode> responseDic)
        {
            var hottextInteractions = assessmentItemTag.OwnerDocument.GetElementsByTagName("hottextInteraction");
            for (int i = 0; i < hottextInteractions.Count; i++)
            {
                var responseIdentifier = XmlUtils.GetNodeAttribute(hottextInteractions[i], "responseIdentifier");
                var responseDeclaration = responseDic[responseIdentifier];
                UpdateCorrectResponseTextHotSpot(responseDeclaration, hottextInteractions[i], responseIdentifier);
                UpdateHottextInteraction(hottextInteractions[i], responseIdentifier);
            }
        }
        private static void SetResponseDeclarationMultipleChoiceNode(XmlNode responseDeclaration, string cardinality)
        {
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "baseType", "identifier");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "cardinality", cardinality);
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "method", "default");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "caseSensitive", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "type", "string");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", "1");
        }
        private static void UpdateChoiceInteraction(XmlNode choiceInteractionNode, string responseIdentifier)
        {
            XmlUtils.SetOrUpdateNodeAttribute(ref choiceInteractionNode, "responseIdentifier", responseIdentifier);
        }
        private static string UpdateSimplechoice(XmlNode choiceInteractionNode)
        {
            //Need to add <div class="answer" styleName="answer"> right after simpleChoice 
            //Convert any p inside to div

            var simpleChoiceNodeList = choiceInteractionNode.ChildNodes;
            //if (simpleChoiceNodeList == null)
            //{
            //    return "Can not find tag simpleChoice";
            //}
            //if (simpleChoiceNodeList.Count == 0)
            //{
            //    return "Can not find tag simpleChoice";
            //}            
            for (int i = 0; i < simpleChoiceNodeList.Count; i++)
            {
                XmlNode simpleChoiceNode = simpleChoiceNodeList[i];
                if (simpleChoiceNode.NodeType == XmlNodeType.Element && simpleChoiceNode.Name == "simpleChoice")
                {
                    XmlNode div = choiceInteractionNode.OwnerDocument.CreateNode(XmlNodeType.Element, "div", choiceInteractionNode.OwnerDocument.NamespaceURI);
                    //replace p to div
                    XmlUtils.MoveChildNodes(ref simpleChoiceNode, ref div);
                    //get inner xml of simpleChoiceNode
                    var innerXml = div.InnerXml;
                    //make sure innerXml is covered by a Root to enable XmlContentProcessing.ReplaceTag work
                    innerXml = string.Format("<ROOT_SimpleChoiceNode>{0}</ROOT_SimpleChoiceNode>", innerXml);
                    XmlContentProcessing xml = new XmlContentProcessing(innerXml);
                    xml.ReplaceTag("p", "div");
                    innerXml = xml.GetXmlContent();
                    div.RemoveAll();
                    div.InnerXml = innerXml.Replace("<ROOT_SimpleChoiceNode>", "").Replace("</ROOT_SimpleChoiceNode>", "");

                    var attClass = choiceInteractionNode.OwnerDocument.CreateAttribute("class");
                    attClass.Value = "answer";
                    var attStyleName = choiceInteractionNode.OwnerDocument.CreateAttribute("styleName");
                    attStyleName.Value = "answer";
                    div.Attributes.Append(attClass);
                    div.Attributes.Append(attStyleName);

                    simpleChoiceNode.AppendChild(div);
                }
            }

            return string.Empty;
        }
        private static string UpdateCorrectResponse(XmlNode responseDeclaration, XmlNode choiceInteraction)
        {
            List<string> alphabetAnswerIdentifiers =
             new List<string>(new string[]
                                     {
                                          "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"
                                         , "Q", "R", "S", "T", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF",
                                         "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP"
                                     }); //Flash does not use 'U'

            var simpleChoiceNodeList = XmlUtils.GetChildNodeListByName(choiceInteraction, "simpleChoice");
            List<string> identifierList = new List<string>();
            foreach (XmlNode simpleChoiceNode in simpleChoiceNodeList)
            {
                identifierList.Add(XmlUtils.GetNodeAttribute(simpleChoiceNode, "identifier"));
            }
            var orderedIdentifierList = identifierList.OrderBy(x => x).ToList();

            string correctResponse = ""; // they uset correctResponse like ITBmxrNUHTbUuuNsc_R_3 

            var correctResponseNodes = XmlUtils.GetChildNodeListByName(responseDeclaration, "correctResponse");
            for (int i = 0; i < correctResponseNodes.Count; i++)
            {
                var correctResponseNode = correctResponseNodes[i];

                var valueNodes = XmlUtils.GetChildNodeListByName(correctResponseNode, "value");

                for (int j = 0; j < valueNodes.Count; j++)
                {
                    var valueNode = valueNodes[j];
                    correctResponse = valueNode.InnerText;//value node
                    //find index of correctResponse in orderedIdentifierList
                    var index = orderedIdentifierList.IndexOf(correctResponse);
                    if (index > -1)
                    {
                        //update correctResponse
                        valueNode.InnerText = alphabetAnswerIdentifiers[index];
                    }
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
            return string.Empty;
        }

        private static void SetResponseDeclarationNode(XmlNode responseDeclaration)
        {          
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "baseType", "identifier");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "cardinality", "single");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "method", "default");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "caseSensitive", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "type", "string");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", "1");
        }
        private static void SetResponseDeclarationInlineChoiceNode(XmlNode responseDeclaration, XmlNode inlineChoiceInteraction)
        {
            List<string> alphabetAnswerIdentifiers =
             new List<string>(new string[]
                                     {
                                          "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"
                                         , "Q", "R", "S", "T", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF",
                                         "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP"
                                     }); //Flash does not use 'U'

            var inlineChoiceNodeList = XmlUtils.GetChildNodeListByName(inlineChoiceInteraction, "inlineChoice");
            List<string> identifierList = new List<string>();
            foreach (XmlNode inlineChoiceNode in inlineChoiceNodeList)
            {
                identifierList.Add(XmlUtils.GetNodeAttribute(inlineChoiceNode, "identifier"));
            }
            var orderedIdentifierList = identifierList.OrderBy(x => x).ToList();
            var correctResponseNode = XmlUtils.GetSingleChildNodeByName(responseDeclaration, "correctResponse");

            var valueNode = XmlUtils.GetSingleChildNodeByName(correctResponseNode, "value");
            if (valueNode != null)
            {
                var correctResponse = valueNode.InnerText;//value node
                //find index of correctResponse in orderedIdentifierList
                var index = orderedIdentifierList.IndexOf(correctResponse);
                if (index > -1)
                {
                    //update correctResponse
                    valueNode.InnerText = alphabetAnswerIdentifiers[index];
                }
            }
            //Update identifier into Linkit format
            for (int i = 0; i < inlineChoiceNodeList.Count; i++)
            {
                XmlNode inlineChoiceNode = inlineChoiceNodeList[i];
                
                var identifier = XmlUtils.GetNodeAttribute(inlineChoiceNode, "identifier");
                for (int j = 0; j < orderedIdentifierList.Count; j++)
                {
                    if (orderedIdentifierList[j] == identifier)
                    {
                        XmlUtils.SetOrUpdateNodeAttribute(ref inlineChoiceNode, "identifier", alphabetAnswerIdentifiers[j]);
                    }
                }
            }
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "baseType", "identifier");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "cardinality", "single");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "method", "default");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "caseSensitive", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "type", "string");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", "1");
        }
        private static void SetExtendedTextInteractionTag(XmlNode extendedTextInteraction, string responseIdentifier)
        {
            XmlUtils.SetOrUpdateNodeAttribute(ref extendedTextInteraction, "responseIdentifier", responseIdentifier);
            XmlUtils.SetOrUpdateNodeAttribute(ref extendedTextInteraction, "expectedLength", "50000");
        }
        private static void SetTextEntryResponseDeclarationNode(XmlNode responseDeclaration)
        {
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "baseType", "identifier");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "cardinality", "single");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "method", "default");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "caseSensitive", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "type", "string");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", "1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "range", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "spelling", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "spellingDeduction", "1");
        }
        private static void SetTextEntryInteractionTag(XmlNode textEntryInteraction, string responseIdentifier)
        {
            XmlUtils.SetOrUpdateNodeAttribute(ref textEntryInteraction, "responseIdentifier", responseIdentifier);
        }
        private static void SetInlineChoiceInteractionTag(XmlNode inlineChoiceInteractionNode, string responseIdentifier)
        {
            XmlUtils.SetOrUpdateNodeAttribute(ref inlineChoiceInteractionNode, "responseIdentifier", responseIdentifier);
            var inlineChoiceNodeList = inlineChoiceInteractionNode.ChildNodes;

            for (int i = 0; i < inlineChoiceNodeList.Count; i++)
            {
                var updateInlineChoice = inlineChoiceNodeList[i];
                if (updateInlineChoice.NodeType == XmlNodeType.Element && updateInlineChoice.Name == "inlineChoice")
                {
                    XmlUtils.SetOrUpdateNodeAttribute(ref updateInlineChoice, "pointsValue", "0");
                    var spanNode = inlineChoiceInteractionNode.OwnerDocument.CreateNode(XmlNodeType.Element, "span", null);
                    XmlUtils.SetOrUpdateNodeAttribute(ref spanNode, "class", "inlineChoiceAnswer");
                    XmlUtils.MoveChildNodes(ref updateInlineChoice, ref spanNode);
                    updateInlineChoice.AppendChild(spanNode);
                }
            }
        }
        private static void UpdateCorrectResponseTextEntry(XmlNode correctResponseNode)
        {
            var mappedValue = "0";
            var mappingNode = XmlUtils.GetSingleChildNodeByName(correctResponseNode, "mapping");
            if (mappingNode != null)
            {
                XmlNode mapEntryNode = XmlUtils.GetSingleChildNodeByName(mappingNode, "mapEntry");
                if (mapEntryNode != null)
                {
                    mappedValue = XmlUtils.GetNodeAttribute(mapEntryNode, "mappedValue");
                }
            }
            XmlNode valueNode = XmlUtils.GetSingleChildNodeByName(correctResponseNode, "value");
            XmlUtils.SetOrUpdateNodeAttribute(ref valueNode, "identifier", "A");
            XmlUtils.SetOrUpdateNodeAttribute(ref valueNode, "pointsValue", mappedValue);
        }
        private static string UpdateCorrectResponseTextHotSpot(XmlNode responseDeclaration, XmlNode hottextInteraction, string responseIdentifier)
        {
            var oldCorrectResponse = XmlUtils.GetSingleChildNodeByName(responseDeclaration, "correctResponse");
            if (oldCorrectResponse == null)
                return "Can not find tag correctResponse";

            var valueNodes = XmlUtils.GetChildNodeListByName(oldCorrectResponse, "value");
            if (valueNodes.Count == 0)
            {
                return "Can not find tag value";
            }

            int valueNodeCount = valueNodes.Count;

            List<XmlNode> hottexts = new List<XmlNode>();
            var findChildNodeName = false;
            //add Linkit correctResponse node
            //var hottexts = XmlUtils.RecurseGetChildNodeListByName(hottextInteraction, "hottext");
            XmlUtils.RecurseGetChildNodeListByName(hottextInteraction, "hottext",false, hottexts);
            if (hottexts.Count == 0)
                return "Can not find tag hottext";

            for (int i = 0; i < valueNodeCount; i++)
            {
                var identifier = "";
                var index = 0;
                foreach (XmlNode ht in hottexts)
                {
                    index++;
                    var oldIdentifier = XmlUtils.GetNodeAttribute(ht, "identifier");
                    var value = valueNodes[i].InnerText;

                    if (oldIdentifier == value)
                    {
                        identifier = "HS_" + index;
                        break;
                    }
                }
                XmlNode newNode = responseDeclaration.OwnerDocument.CreateNode(XmlNodeType.Element, "correctResponse",
                   responseDeclaration.OwnerDocument.DocumentElement.NamespaceURI);
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "identifier", identifier);
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "pointValue", "1");
                responseDeclaration.AppendChild(newNode);
            }
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "identifier", responseIdentifier);
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "absoluteGrading", "1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "partialGrading", "0");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", valueNodeCount.ToString());

            //remove original correctResponse Node                   
            responseDeclaration.RemoveChild(oldCorrectResponse);
            return string.Empty;
        }
        private static string UpdateHottextInteraction(XmlNode hottextInteraction, string responseIdentifier)
        {
            try
            {               
                var hottexts = new List<XmlNode>();
                //convert hottext to sourceText
                XmlUtils.RecurseGetChildNodeListByName(hottextInteraction, "hottext", false, hottexts);
                if (hottexts.Count == 0)
                    return "Can not find tag hottext";

                var count = hottexts.Count;
                for (int i = 0; i < count; i++)
                {
                    var child = XmlUtils.GetSingleChildNodeByName(hottexts[i], "hottext");
                    if (child != null)
                        return "Hottext tag contain the childs is hottext tag, wrong format";
                }
                for (int i = 0; i < count; i++)
                {
                    var identifier = "HS_" + (i + 1);
                    var node = hottexts[i];
                    if (node != null)
                    {
                        var sourceText = hottextInteraction.OwnerDocument.CreateNode(XmlNodeType.Element, "sourceText",
                            hottextInteraction.OwnerDocument.DocumentElement.NamespaceURI);
                        XmlUtils.SetOrUpdateNodeAttribute(ref sourceText, "identifier", identifier);
                        XmlUtils.SetOrUpdateNodeAttribute(ref sourceText, "pointValue", "1");
                        sourceText.InnerText = node.InnerText;
                        if(node.ParentNode != null)
                            node.ParentNode.ReplaceChild(sourceText, node);
                    }
                }

                var maxChoices = XmlUtils.GetNodeAttribute(hottextInteraction, "maxChoices");

                var textHotSpot = hottextInteraction.OwnerDocument.CreateNode(XmlNodeType.Element, "textHotSpot",
                    hottextInteraction.OwnerDocument.DocumentElement.NamespaceURI);
                XmlUtils.SetOrUpdateNodeAttribute(ref textHotSpot, "id", responseIdentifier);
                XmlUtils.SetOrUpdateNodeAttribute(ref textHotSpot, "maxSelected", maxChoices);
                XmlUtils.SetOrUpdateNodeAttribute(ref textHotSpot, "responseIdentifier", responseIdentifier);

                var divTag = hottextInteraction.OwnerDocument.CreateNode(XmlNodeType.Element, "div",
                    hottextInteraction.OwnerDocument.DocumentElement.NamespaceURI);

                var parentNode = hottextInteraction.ParentNode;
                if (parentNode != null)
                {
                    divTag.AppendChild(textHotSpot);

                    //move all child nodes of hotTextInteration into itemBody
                    XmlUtils.MoveChildNodes(ref hottextInteraction, ref divTag);
                    //remove hottextInteraction
                    parentNode.ReplaceChild(divTag, hottextInteraction);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
