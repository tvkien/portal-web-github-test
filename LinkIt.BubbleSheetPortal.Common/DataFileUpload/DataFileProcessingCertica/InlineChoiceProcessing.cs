using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;
namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica
{
    internal class InlineChoiceProcessing
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

                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(assessmentItemTag);

                //update responseDeclaration tag
                resource.ProcessingStep.Append("->Update Response Declaration.");
                SetResponseDeclarationNode(assessmentItemTag);
                
                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove responseProcessing.");
                DataFileProcessing.RemoveResponseProcessingTag(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Rubric Block.");
                resource.Error = DataFileProcessing.RemoveRubricBlockTag(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Update inlineChoice Interaction.");
                resource.Error = SetInlineChoiceInteractionTag(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

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
                //Move tag object under itemBody
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
            return resource;

        }

        private static void SetResponseDeclarationNode(XmlNode assessmentItemTag)
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
        }        
        private static string SetInlineChoiceInteractionTag(XmlNode assessmentItemTag)
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

            var inlineChoiceInteractionNodeList = itemBodyNode.OwnerDocument.GetElementsByTagName("inlineChoiceInteraction");
            if (inlineChoiceInteractionNodeList != null)
            {
                for (int i = 0; i < inlineChoiceInteractionNodeList.Count; i++)
                {
                    var inlineChoiceInteractionNode = inlineChoiceInteractionNodeList[i];

                    XmlUtils.SetOrUpdateNodeAttribute(ref inlineChoiceInteractionNode, "responseIdentifier", "RESPONSE_1");
                    var inlineChoiceNodeList = inlineChoiceInteractionNode.ChildNodes;
                    var index = 0;
                    foreach (XmlNode inlineChoiceNode in inlineChoiceNodeList)
                    {
                        if (inlineChoiceNode.NodeType == XmlNodeType.Element && inlineChoiceNode.Name == "inlineChoice")
                        {
                            var updateInlineChoice = inlineChoiceNodeList[index];
                            XmlUtils.SetOrUpdateNodeAttribute(ref updateInlineChoice, "pointsValue", "0");
                            XmlUtils.SetOrUpdateNodeAttribute(ref updateInlineChoice, "fixed", "true");

                            var spanNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "span",
                                itemBodyNode.OwnerDocument.DocumentElement.NamespaceURI);
                            XmlUtils.SetOrUpdateNodeAttribute(ref spanNode, "class", "inlineChoiceAnswer");
                            XmlUtils.MoveChildNodes(ref updateInlineChoice, ref spanNode);
                            updateInlineChoice.AppendChild(spanNode);
                        }
                        index++;
                    }

                }
            }
           
            return string.Empty;
        }                     
    }
}
