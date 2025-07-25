using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;
namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingNew
{
    internal class TextEntryProcessing
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

                //check complex type
                if (DataFileProcessing.CheckComplexType(assessmentItemTag, ref resource))
                {
                    return ComplexProcessing.Convert(parameter, resource);
                }

                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", ((int)QtiSchemaEnum.TextEntry).ToString());
                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(ref assessmentItemTag);
                //update responseDeclaration tag
                resource.ProcessingStep.Append("->Update Response Declaration.");
                SetResponseDeclarationNode(ref assessmentItemTag);

                //update value tag of Correct Response
                resource.ProcessingStep.Append("->Update value tag of Correct Response.");
                resource.Error = UpdateCorrectResponse(ref assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Remove mapping tag.");
                DataFileProcessing.RemoveMappingTag(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Update ExtendedText Interaction.");
                resource.Error = SetTextEntryInteractionTag(ref assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }
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

        private static void SetResponseDeclarationNode(ref XmlNode assessmentItemTag)
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
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "range", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "spelling", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "spellingDeduction", "1");
        }

        private static string UpdateCorrectResponse(ref XmlNode assessmentItemTag)
        {
            var correctResponseNode = assessmentItemTag.OwnerDocument.GetElementsByTagName("correctResponse");
            if (correctResponseNode == null)
            {
                return "Can not find correctResponse";
            }
            XmlNode mappingNode = assessmentItemTag.OwnerDocument.GetElementsByTagName("mapping")[0];
            XmlNode mapEntryNode = XmlUtils.GetSingleChildNodeByName(mappingNode, "mapEntry");
            if (mapEntryNode == null)
            {
                return "Can not find mapEntry";
            }
            var mappedValue = XmlUtils.GetNodeAttribute(mapEntryNode, "mappedValue");
            if (string.IsNullOrWhiteSpace(mappedValue))
            {
                mappedValue = "0";
            }

            XmlNode valueNode = XmlUtils.GetSingleChildNodeByName(correctResponseNode[0], "value");
            XmlUtils.SetOrUpdateNodeAttribute(ref valueNode, "identifier", "A");
            XmlUtils.SetOrUpdateNodeAttribute(ref valueNode, "pointsValue", mappedValue);            

            return string.Empty;
        }
        private static string SetTextEntryInteractionTag(ref XmlNode assessmentItemTag)
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

            //remove old textEntryInteractionNode with parrent div tag 
            //add new textEntryInteractionNode
            var divNodeList = itemBodyNode.OwnerDocument.GetElementsByTagName("div");
            foreach (XmlNode divNode in divNodeList)
            {
                var textEntryInteractionNode = XmlUtils.GetSingleChildNodeByName(divNode, "textEntryInteraction");
                if (textEntryInteractionNode != null)
                {
                    itemBodyNode.RemoveChild(divNode);
                    XmlUtils.SetOrUpdateNodeAttribute(ref textEntryInteractionNode, "responseIdentifier", "RESPONSE_1");
                    itemBodyNode.AppendChild(textEntryInteractionNode);
                    return string.Empty;
                }
            }
            //if textEntryInteractionNode is null, create new node
            XmlNode newNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "textEntryInteraction",
                   itemBodyNode.OwnerDocument.DocumentElement.NamespaceURI);
            XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "responseIdentifier", "RESPONSE_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "expectedLength", "1");
            itemBodyNode.AppendChild(newNode);
            return string.Empty;
        }              
    }
}
