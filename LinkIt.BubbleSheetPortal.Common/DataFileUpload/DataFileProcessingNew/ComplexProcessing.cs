using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;
namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingNew
{
    internal class ComplexProcessing
    {
        public static DataFileUploaderResource Convert(DataFileUploaderParameter parameter,
            DataFileUploaderResource resource)
        {
            try
            {
                resource.ProcessingStep.Append("Start Converting.");
                var doc = DataFileProcessing.LoadXml(ref resource, parameter.ExtractedFoler);
                if (doc == null)
                {
                    return resource;
                }

                var assessmentItemTag = DataFileProcessing.GetAssessmentItemNode(ref doc, ref resource);
                if (assessmentItemTag == null)
                {
                    return resource;
                }
                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", ((int)QtiSchemaEnum.Complex).ToString());
                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(ref assessmentItemTag);

                //update responseDeclaration tag
                resource.ProcessingStep.Append("->Update Response Declaration.");
                resource.Error = SetResponseDeclarationNodeAndItemBody(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Remove Rubric Block fo ExtendedText.");
                resource.Error = DataFileProcessing.RemoveRubricBlockTag(ref assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Remove mapping tag for TextEntry.");
                DataFileProcessing.RemoveMappingTag(ref assessmentItemTag);

                //update itemBody tag
                resource.ProcessingStep.Append("->Update MainBody.");
                resource.Error = DataFileProcessing.CreateMainBodyTag(ref assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                DataFileProcessing.UpdateMediaFilePathInXmlContent(ref assessmentItemTag, parameter, ref resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
                {
                    return resource;
                }

                //Update passage
                //Move tag object under itemBody
                DataFileProcessing.MoveTagObject(assessmentItemTag, resource, parameter);
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
    }
}
