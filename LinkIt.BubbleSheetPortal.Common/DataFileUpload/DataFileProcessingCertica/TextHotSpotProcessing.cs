using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;
namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica
{
    internal class TextHotSpotProcessing
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

                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", ((int)QtiSchemaEnum.TextHotSpot).ToString());
                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(assessmentItemTag);

                resource.ProcessingStep.Append("->Update Response Declaration and correct response.");
                resource.Error = UpdateResponseDeclarationAndCorrectResponse(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                    return resource;             

                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Rubric Block.");
                resource.Error = DataFileProcessing.RemoveRubricBlockTag(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Update hottextInteraction to textHotSpot.");
                resource.Error = UpdateHottextInteraction(assessmentItemTag);
                 if (!string.IsNullOrEmpty(resource.Error))
                    return resource;    

                //update itemBody tag
                resource.ProcessingStep.Append("->Update MainBody.");
                resource.Error = DataFileProcessing.CreateMainBodyTag(assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(resource.Error))
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
                resource.ProcessingStep.Append("->DONE. Get final converted XmlContent.");
                resource.XmlContent = doc.GetXmlContent();
            }
            catch (Exception ex)
            {
                resource.Error = string.Format("There was some error when processing \"{0}\" ", resource.ResourceFileName);
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }
            return resource;

        }
        private static string UpdateResponseDeclarationAndCorrectResponse(XmlNode assessmentItemTag)
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
            var oldCorrectResponse = XmlUtils.GetSingleChildNodeByName(responseDeclaration, "correctResponse");
            if (oldCorrectResponse == null)
                return "Can not find tag correctResponse";

            var valueNodes = assessmentItemTag.OwnerDocument.GetElementsByTagName("value");            
            if (valueNodes.Count == 0)
            {
                return "Can not find tag value";
            }

            int valueNodeCount = valueNodes.Count;
            
            //add Linkit correctResponse node
            var hottexts = assessmentItemTag.OwnerDocument.GetElementsByTagName("hottext");
            if (hottexts.Count == 0)
                return "Can not find tag hottext";

            for (int i = 0; i < valueNodeCount; i++)
            {
                var identifier = "";
                var index = 0;
                foreach(XmlNode ht in hottexts)
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
                XmlNode newNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "correctResponse",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "identifier", identifier);
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "pointValue", "1");
                responseDeclaration.AppendChild(newNode);
            }

            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "identifier", "RESPONSE_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "absoluteGrading", "1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "partialGrading", "0");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", valueNodeCount.ToString());

            //remove original correctResponse Node                   
            responseDeclaration.RemoveChild(oldCorrectResponse);
            return string.Empty;
        }
        private static string UpdateHottextInteraction(XmlNode assessmentItemTag)
        {
            try
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

                //add new textHotSpot Node
                var hotTextInteration = XmlUtils.GetSingleChildNodeByName(itemBodyNode, "hottextInteraction");
                if (hotTextInteration == null)
                    return "Can not find tag hottextInteraction";

                var maxChoices = XmlUtils.GetNodeAttribute(hotTextInteration, "maxChoices");

                var textHotSpot = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "textHotSpot",
                    assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                XmlUtils.SetOrUpdateNodeAttribute(ref textHotSpot, "id", "RESPONSE_1");
                XmlUtils.SetOrUpdateNodeAttribute(ref textHotSpot, "maxSelected", maxChoices);
                XmlUtils.SetOrUpdateNodeAttribute(ref textHotSpot, "responseIdentifier", "RESPONSE_1");
                itemBodyNode.AppendChild(textHotSpot);

                //move all child nodes of hotTextInteration into itemBody
                XmlUtils.MoveChildNodes(ref hotTextInteration, ref itemBodyNode);

                //remove hottextInteraction
                itemBodyNode.RemoveChild(hotTextInteration);

                //convert hottext to sourceText
                var hottexts = itemBodyNode.OwnerDocument.GetElementsByTagName("hottext");
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
                    var node = hottexts[0];
                    var sourceText = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "sourceText",
                        assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                    XmlUtils.SetOrUpdateNodeAttribute(ref sourceText, "identifier", identifier);
                    XmlUtils.SetOrUpdateNodeAttribute(ref sourceText, "pointValue", "1");
                    sourceText.InnerText = node.InnerText;
                    node.ParentNode.ReplaceChild(sourceText, node);
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
