using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingNew
{
    public class DragAndDropProcessing
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

                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(ref assessmentItemTag);

                resource.ProcessingStep.Append("->Create Main Body Tag");

                ConvertMatchInteractionAndResponseDeclaration(ref assessmentItemTag, ref resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
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
                AddPartialCredit(ref assessmentItemTag, ref resource);
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

        private static void ConvertMatchInteractionAndResponseDeclaration(ref XmlNode assessmentItemTag, ref DataFileUploaderResource resource)
        {
            resource.ProcessingStep.Append("->Convert Tag matchInteraction and responseDeclaration.");
            XmlNode itemBodyNode = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itemBody");
            if (itemBodyNode == null)
            {
                resource.Error = "Can not find tag itemBody";
                return;
            }

            XmlNode matchInteractionNode = XmlUtils.GetSingleChildNodeByName(itemBodyNode, "matchInteraction");
            if (matchInteractionNode == null)
            {
                resource.Error = "Can not find tag matchInteraction";
                return;
            }
            //XmlNode firstSimpleMatchSetNode = matchInteractionNode.FirstChild;
            //XmlNode secondSimpleMatchSetNode = matchInteractionNode.FirstChild.NextSibling; // not sure, there can be text node inside
            XmlNode firstSimpleMatchSetNode = null;
            XmlNode secondSimpleMatchSetNode = null;

            foreach (XmlNode node in matchInteractionNode.ChildNodes)
            {
                if (node.Name == "simpleMatchSet")
                {
                    if (firstSimpleMatchSetNode == null)
                    {
                        firstSimpleMatchSetNode = node;
                    }
                    else
                    {
                        secondSimpleMatchSetNode = node;
                        break;
                    }
                   
                }
            }
            if (firstSimpleMatchSetNode == null || secondSimpleMatchSetNode == null)
            {
                resource.Error = "Can not find enough tag firstSimpleMatchSetNode";
                return;
            }
            //use the first simpleMatchSet as source and the second simpleMatchSet as destination 
            List<XmlNode> sourceNodeList = new List<XmlNode>();
            List<XmlNode> sourceSimpleAssociableChoiceNodeList = XmlUtils.GetChildNodeListByName(firstSimpleMatchSetNode, "simpleAssociableChoice");
            Dictionary<string,string> identifierSourceDic = new Dictionary<string, string>();
            for (int i = 0; i < sourceSimpleAssociableChoiceNodeList.Count; i++)
            {
                XmlNode sourceNode = assessmentItemTag.OwnerDocument.CreateElement("sourceObject");
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceNode, "partialID", "Partial_1");
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceNode, "style", "width: 85px; height: 20px;");//default value in Portal
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceNode, "srcIdentifier", string.Format("SRC_{0}",i+1));
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceNode, "type", "text");
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceNode, "data-limit", "unlimited");

                sourceNode.InnerXml = sourceSimpleAssociableChoiceNodeList[i].InnerXml;
                identifierSourceDic.Add(XmlUtils.GetNodeAttribute(sourceSimpleAssociableChoiceNodeList[i], "identifier"), string.Format("SRC_{0}", i + 1));

                sourceNodeList.Add(sourceNode);
            }

            List<XmlNode> desNodeList = new List<XmlNode>();
            List<XmlNode> desSimpleAssociableChoiceNodeList = XmlUtils.GetChildNodeListByName(secondSimpleMatchSetNode, "simpleAssociableChoice");
            Dictionary<string, string> identifierDesDic = new Dictionary<string, string>();
            for (int i = 0; i < desSimpleAssociableChoiceNodeList.Count; i++)
            {
                XmlNode desNode = assessmentItemTag.OwnerDocument.CreateElement("destinationObject");
                XmlUtils.SetOrUpdateNodeAttribute(ref desNode, "partialID", "Partial_1");
                XmlUtils.SetOrUpdateNodeAttribute(ref desNode, "type", "text");

                XmlNode destinationItem = assessmentItemTag.OwnerDocument.CreateElement("destinationItem");
                XmlUtils.SetOrUpdateNodeAttribute(ref destinationItem, "partialID", "Partial_1");
                XmlUtils.SetOrUpdateNodeAttribute(ref destinationItem, "destIdentifier", string.Format("DEST_{0}", i + 1));
                XmlUtils.SetOrUpdateNodeAttribute(ref destinationItem, "order", (i+1).ToString());
                XmlUtils.SetOrUpdateNodeAttribute(ref destinationItem, "width", "55");
                XmlUtils.SetOrUpdateNodeAttribute(ref destinationItem, "height", "20");
                XmlUtils.SetOrUpdateNodeAttribute(ref destinationItem, "numberDroppable", "1");

                destinationItem.InnerXml = desSimpleAssociableChoiceNodeList[i].InnerXml;

                desNode.AppendChild(destinationItem);
                identifierDesDic.Add(XmlUtils.GetNodeAttribute(desSimpleAssociableChoiceNodeList[i], "identifier"), string.Format("DEST_{0}", i + 1));

                desNodeList.Add(desNode);
            }

            //crate a new div that will be used to replace matchInteraction
            XmlNode dvMatchInteractionNode = assessmentItemTag.OwnerDocument.CreateElement("div");
            XmlUtils.SetOrUpdateNodeAttribute(ref dvMatchInteractionNode, "class", "matchInteraction");

            for (int i = 0; i < sourceNodeList.Count; i++)
            {
                dvMatchInteractionNode.AppendChild(sourceNodeList[i]);
                dvMatchInteractionNode.InnerXml += "&nbsp;&nbsp;";
            }
            XmlNode newLineNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "br", null);
            dvMatchInteractionNode.AppendChild(newLineNode);

            for (int i = 0; i < desNodeList.Count; i++)
            {
                dvMatchInteractionNode.AppendChild(desNodeList[i]);
                dvMatchInteractionNode.InnerXml += "&nbsp;&nbsp;";
            }
            itemBodyNode.ReplaceChild(dvMatchInteractionNode, matchInteractionNode);
            
            //update responseDeclaration
            resource.ProcessingStep.Append("-> Update responseDeclaration");

            XmlNode responseDeclaration = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "responseDeclaration");
            if (responseDeclaration == null)
            {
                resource.Error = "Can not find tag responseDeclaration";
                return;
            }
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "identifier", "RESPONSE_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "absoluteGrading", "0");//no use absolute
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "absoluteGradingPoints", "1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "partialGradingThreshold", "0.5");//no use threshold
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "relativeGrading", "0");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "relativeGradingPoints", "1");
            XmlUtils.SetOrUpdateNodeAttribute(ref responseDeclaration, "pointsValue", "1");

            //get correctResponse
            resource.ProcessingStep.Append("-> Get correctResponse");
            XmlNode correctResponseNode = XmlUtils.GetSingleChildNodeByName(responseDeclaration, "correctResponse");
            //remove this old correctResponse Node
            responseDeclaration.RemoveChild(correctResponseNode);

            //get mapping node
            resource.ProcessingStep.Append("-> Get mapping node");
            XmlNode mappingNode = XmlUtils.GetSingleChildNodeByName(responseDeclaration, "mapping");
            if (mappingNode == null)
            {
                resource.Error = "Can not find tag mapping";
                return;
            }

            List<XmlNode> mapEntryNodeList = XmlUtils.GetChildNodeListByName(mappingNode, "mapEntry");
            List<string> destIdentifierList = new List<string>();
            List<string> srcIdentifier = new List<string>();

            for (int i = 0; i < mapEntryNodeList.Count; i++)
            {
                //get the mapKey
                var mapKey = XmlUtils.GetNodeAttribute(mapEntryNodeList[i], "mapKey");
                //mapKey look like SB RA -> parse SB and RA 
                destIdentifierList.Add(mapKey.Split(' ')[1].ToString());
                srcIdentifier.Add(mapKey.Split(' ')[0].ToString());
            }
            responseDeclaration.RemoveChild(mappingNode);

            //create new correctResponse according to linkit format
            resource.ProcessingStep.Append("-> Create new  correctResponse");
            for (int i = 0; i < destIdentifierList.Count; i++)
            {
                XmlNode newCorrectResponseNode = itemBodyNode.OwnerDocument.CreateElement("correctResponse");

                string destIdentifierNew = identifierDesDic[destIdentifierList[i]];//map des from customer to linkit format
                string srcIdentifierNew = identifierSourceDic[srcIdentifier[i]];

                XmlUtils.SetOrUpdateNodeAttribute(ref newCorrectResponseNode, "order",(i+1).ToString());
                XmlUtils.SetOrUpdateNodeAttribute(ref newCorrectResponseNode, "destIdentifier", destIdentifierNew);
                XmlUtils.SetOrUpdateNodeAttribute(ref newCorrectResponseNode, "srcIdentifier", srcIdentifierNew);

                responseDeclaration.AppendChild(newCorrectResponseNode);
            }

        }

        private static void AddPartialCredit(ref XmlNode assessmentItemTag,
            ref DataFileUploaderResource resource)
        {
            resource.ProcessingStep.Append("-> Add partialcredit");
            XmlNode itemBody = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itemBody");
            var mainBody = itemBody.FirstChild;
            if (XmlUtils.GetNodeAttribute(mainBody, "class") == null ||
                XmlUtils.GetNodeAttribute(mainBody, "class") != "mainBody"
                )
            {
                resource.Error = "Can not find div mainBody";
                return;
            }

            //add <partialcredit id="RESPONSE_1" partialid="Partial_1" responseidentifier="RESPONSE_1"/> following Linkit format
            XmlNode partialcredit = assessmentItemTag.OwnerDocument.CreateElement("partialcredit");
            XmlUtils.SetOrUpdateNodeAttribute(ref partialcredit, "id", "RESPONSE_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref partialcredit, "partialid", "Partial_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref partialcredit, "responseidentifier", "RESPONSE_1");

            XmlUtils.InsertFirstChild(ref mainBody, partialcredit);
        }


    }
}
