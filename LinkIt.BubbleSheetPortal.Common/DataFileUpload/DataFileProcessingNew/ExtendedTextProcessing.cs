using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;
namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingNew
{
    internal class ExtendedTextProcessing
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

                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", ((int)QtiSchemaEnum.ExtendedText).ToString());
                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(ref assessmentItemTag);
                //update responseDeclaration tag
                resource.ProcessingStep.Append("->Update Response Declaration.");
                SetResponseDeclarationNode(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Rubric Block.");
                resource.Error = DataFileProcessing.RemoveRubricBlockTag(ref assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Update ExtendedText Interaction.");
                resource.Error = SetExtendedTextInteractionTag(ref assessmentItemTag);
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
        }

        private static string SetExtendedTextInteractionTag(ref XmlNode assessmentItemTag)
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
            XmlNode extendedTextInteraction = XmlUtils.GetSingleChildNodeByName(itemBodyNode, "extendedTextInteraction");
            if (extendedTextInteraction == null)
            {
                XmlNode newNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "extendedTextInteraction",
                   itemBodyNode.OwnerDocument.DocumentElement.NamespaceURI);
                itemBodyNode.AppendChild(newNode);
                extendedTextInteraction = newNode;
            }
            XmlUtils.SetOrUpdateNodeAttribute(ref extendedTextInteraction, "responseIdentifier", "RESPONSE_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref extendedTextInteraction, "expectedLength", "50000");

            return string.Empty;
        }                
    }
}
