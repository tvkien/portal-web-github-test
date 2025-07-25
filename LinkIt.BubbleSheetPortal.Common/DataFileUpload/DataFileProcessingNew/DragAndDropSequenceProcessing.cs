using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;
namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingNew
{
    internal class DragAndDropSequenceProcessing
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

                //XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", ((int)QtiSchemaEnum.DragAndDropSequence).ToString());
                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Update Response Declaration and correct response.");
                resource.Error = UpdateResponseDeclarationAndCorrectResponse(ref assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                    return resource;             

                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(ref assessmentItemTag);
              
                resource.ProcessingStep.Append("->Update orderInteraction to sourceItem.");
                resource.Error = UpdateOrderInteraction(ref assessmentItemTag);
                 if (!string.IsNullOrEmpty(resource.Error))
                    return resource;    

                //update itemBody tag
                resource.ProcessingStep.Append("->Update MainBody.");
                resource.Error = DataFileProcessing.CreateMainBodyTag(ref assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(resource.Error))
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
        private static string UpdateResponseDeclarationAndCorrectResponse(ref XmlNode assessmentItemTag)
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
            if (valueNodes == null)
            {
                return "Can not find tag value";
            }

            int valueNodeCount = valueNodes.Count;
            
            //add Linkit correctResponse node
            var simpleChoices = assessmentItemTag.OwnerDocument.GetElementsByTagName("simpleChoice");
            if (simpleChoices == null)
                return "Can not find tag simpleChoice";

            //create new correctResponse node
            XmlNode newcorrectResponse = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "correctResponse",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);            
            XmlNode newValue = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "value",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);            

            var identifier = "";
            for (int i = 0; i < valueNodeCount; i++)
            {               
                var index = 0;
                foreach(XmlNode spc in simpleChoices)
                {
                    index++;
                    var oldIdentifier = XmlUtils.GetNodeAttribute(spc, "identifier");
                    var value = valueNodes[i].InnerText;

                    if (oldIdentifier == value)
                    {
                        if(string.IsNullOrEmpty(identifier))
                            identifier += "SRC_" + index;
                        else
                            identifier += ",SRC_" + index;
                        break;
                    }
                }                                                                                              
            }

            //add new correctResponse
            newValue.InnerText = identifier;
            newcorrectResponse.AppendChild(newValue);
            responseDeclaration.AppendChild(newcorrectResponse);

            //remove original correctResponse Node                   
            responseDeclaration.RemoveChild(oldCorrectResponse);

            //update attribute responseDeclaration
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "identifier", "RESPONSE_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", "1");
                        
            return string.Empty;
        }
        private static string UpdateOrderInteraction(ref XmlNode assessmentItemTag)
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

            //add new partialSequence Node
            var orderInteraction = XmlUtils.GetSingleChildNodeByName(itemBodyNode, "orderInteraction");
            if (orderInteraction == null)
                return "Can not find tag orderInteraction";           

            var partialSequence = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "partialSequence",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
            XmlUtils.SetOrUpdateNodeAttribute(ref partialSequence, "orientation", "vertical");
            XmlUtils.SetOrUpdateNodeAttribute(ref partialSequence, "responseIdentifier", "RESPONSE_1");
            itemBodyNode.AppendChild(partialSequence);

            //convert simpleChoice to sourceItem
            var simpleChoices = itemBodyNode.OwnerDocument.GetElementsByTagName("simpleChoice");
            if (simpleChoices == null)
                return "Can not find tag simpleChoice";

            //create new sourceItem node, then appendchild to partialSequence
            var count = simpleChoices.Count;
            for (int i = 0; i < count; i++)
            {
                var identifier = "SRC_" + (i+1);
                var node = simpleChoices[i];           
                var sourceItem = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "sourceItem",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "identifier", identifier);
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "width", "150");
                var divChild = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "div",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                XmlUtils.SetOrUpdateNodeAttribute(ref divChild, "styleName", "value");
                divChild.InnerXml = node.InnerXml;
                sourceItem.AppendChild(divChild);

                partialSequence.AppendChild(sourceItem);
            }

            //remove orderInteraction
            itemBodyNode.RemoveChild(orderInteraction);
            return string.Empty;
        }           
    }
}
