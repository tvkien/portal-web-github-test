using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica
{
    public class Image_DragAndDropProcessing
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
                //if (DataFileProcessing.CheckComplexType(assessmentItemTag,  resource))
                //{
                //    return ComplexProcessing.Convert(parameter, resource);
                //}

                //Linkit use this style sheet
                resource.ProcessingStep.Append("->Add Linkit StyleSheetNode.");
                DataFileProcessing.AddStyleSheetNode(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Outcome Declaration.");
                DataFileProcessing.RemoveOutcomeDeclarationTag(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Response Processing.");
                DataFileProcessing.RemoveResponseProcessingTag(assessmentItemTag);

                resource.ProcessingStep.Append("->Remove Rubric Block.");
                resource.Error = DataFileProcessing.RemoveRubricBlockTag(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                resource.ProcessingStep.Append("->Convert GapMatchInteraction And ResponseDeclaration tag");
                ConvertMatchInteractionAndResponseDeclaration(ref assessmentItemTag, ref resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
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
                AddPartialCredit(ref assessmentItemTag, ref resource);
                if (!string.IsNullOrEmpty(resource.Error))
                {
                    return resource;
                }

                DataFileProcessing.UpdateMediaFilePathInXmlContent(assessmentItemTag, parameter, resource);
                if (!string.IsNullOrWhiteSpace(resource.Error))
                {
                    return resource;
                }
                //update file path of image of destination object
                DataFileProcessing.UpdateMediaFilePathDestinationInXmlContent(assessmentItemTag, parameter, resource);
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

        private static void ConvertMatchInteractionAndResponseDeclaration(ref XmlNode assessmentItemTag, ref DataFileUploaderResource resource)
        {
            resource.ProcessingStep.Append("->Convert Tag gapMatchInteraction and responseDeclaration.");
            XmlNode itemBodyNode = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itemBody");
            if (itemBodyNode == null)
            {
                resource.Error = "Can not find tag itemBody";
                return;
            }

            var direction = string.Empty;
            var divNode = XmlUtils.GetSingleChildNodeByName(itemBodyNode, "div");
            var directionNode = XmlUtils.GetSingleChildNodeByName(divNode, "div");
            if (directionNode != null)
            {
                var id = XmlUtils.GetNodeAttribute(directionNode, "id");
                if (id == "directions")
                {
                    direction = directionNode.InnerText;
                }
            }
            XmlNode matchInteractionNode = XmlUtils.GetSingleChildNodeByName(divNode, "graphicGapMatchInteraction");
            if (matchInteractionNode == null)
            {
                resource.Error = "Can not find tag graphicGapMatchInteraction";
                return;
            }

            List<XmlNode> sourceNodeList = new List<XmlNode>();
            Dictionary<string, string> identifierSourceDic = new Dictionary<string, string>();
            //find all tag <gapImg> to build sourceNodeList
            var gapImgNodeList = itemBodyNode.OwnerDocument.GetElementsByTagName("gapImg");

            for (int i = 0; i < gapImgNodeList.Count; i ++)
            {
                var gapNode = gapImgNodeList[i];
                var identifier = XmlUtils.GetNodeAttribute(gapNode, "identifier");
                var identifierLinkit = string.Format("SRC_{0}", i + 1);

                var objectNode = XmlUtils.GetSingleChildNodeByName(gapNode, "object");
                if (objectNode != null)
                {
                    var height = XmlUtils.GetNodeAttribute(objectNode, "height");
                    var width = XmlUtils.GetNodeAttribute(objectNode, "width");
                    var src = XmlUtils.GetNodeAttribute(objectNode, "data");

                    var newNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "sourceObject", null);
                    XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "srcIdentifier", identifierLinkit);
                    XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "partialID", "Partial_1");
                    XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "style", "width: " + width + "px; height: " + height + "px;");//default value in Portal
                    XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "type", "image");
                    XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "data-limit", "unlimited");                    

                    var imgNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "img", null);
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "srcIdentifier", identifierLinkit);
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "percent", "10");
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "style", "");
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "height", height);
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "width", width);
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "class", "imageupload");
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "drawable", "false");
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "src", src);

                    newNode.AppendChild(imgNode);

                    sourceNodeList.Add(newNode);
                    identifierSourceDic.Add(identifier, identifierLinkit);
                }
            }

            List<XmlNode> desNodeList = new List<XmlNode>();
            Dictionary<string, string> identifierDesDic = new Dictionary<string, string>();
            //find all tag <associableHotspot> to build destNodeList
            var objectDestNode = XmlUtils.GetSingleChildNodeByName(matchInteractionNode, "object");
            var heightDest = XmlUtils.GetNodeAttribute(objectDestNode, "height");
            var widthDest = XmlUtils.GetNodeAttribute(objectDestNode, "width");
            var srcDest = XmlUtils.GetNodeAttribute(objectDestNode, "data");

            var destNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "destinationObject", null);
            XmlUtils.SetOrUpdateNodeAttribute(ref destNode, "partialID", "Partial_1");
            XmlUtils.SetOrUpdateNodeAttribute(ref destNode, "percent", "10");
            XmlUtils.SetOrUpdateNodeAttribute(ref destNode, "height", heightDest);
            XmlUtils.SetOrUpdateNodeAttribute(ref destNode, "width", widthDest);
            XmlUtils.SetOrUpdateNodeAttribute(ref destNode, "type", "image");
            XmlUtils.SetOrUpdateNodeAttribute(ref destNode, "src", srcDest);
            XmlUtils.SetOrUpdateNodeAttribute(ref destNode, "imgorgw", "1024");
            XmlUtils.SetOrUpdateNodeAttribute(ref destNode, "imgorgh", "768");

            var associableHotspotNodeList = itemBodyNode.OwnerDocument.GetElementsByTagName("associableHotspot");
            for (int i = 0; i < associableHotspotNodeList.Count; i++)
            {
                var identifier = XmlUtils.GetNodeAttribute(associableHotspotNodeList[i], "identifier");
                var identifierLinkit = string.Format("DEST_{0}", i + 1);

                var itemNewNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "destinationItem", null);
                //get coords
                var coords = XmlUtils.GetNodeAttribute(associableHotspotNodeList[i], "coords");
                if (!string.IsNullOrWhiteSpace(coords))
                {
                    var listCoord = coords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "left", listCoord[0]);
                    XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "top", listCoord[1]);
                    XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "height", listCoord[2]);
                    XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "width", listCoord[3]);
                }
                
                XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "destIdentifier", identifierLinkit);
                XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "order", (i + 1).ToString());
                XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "numberDroppable", "1");

                destNode.AppendChild(itemNewNode);                
                identifierDesDic.Add(identifier, identifierLinkit);
            }
            desNodeList.Add(destNode);

            //crate a new div that will be used to replace matchInteraction
            XmlNode dvMatchInteractionNode = assessmentItemTag.OwnerDocument.CreateElement("div");
            XmlUtils.SetOrUpdateNodeAttribute(ref dvMatchInteractionNode, "class", "matchInteraction");
            if (!string.IsNullOrEmpty(direction))
            {
                dvMatchInteractionNode.InnerText = direction;
                XmlNode brNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "br", null);
                dvMatchInteractionNode.AppendChild(brNode);
            }

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
            itemBodyNode.ReplaceChild(dvMatchInteractionNode, divNode);
            
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
                //mapKey look like can1 choice2 -> parse can1 and choice2 
                destIdentifierList.Add(mapKey.Split(' ')[0]);
                srcIdentifier.Add(mapKey.Split(' ')[1]);
            }
            responseDeclaration.RemoveChild(mappingNode);

            //create new correctResponse according to linkit format
            resource.ProcessingStep.Append("-> Create new  correctResponse");
            for (int i = 0; i < destIdentifierList.Count; i++)
            {
                XmlNode newCorrectResponseNode = assessmentItemTag.OwnerDocument.CreateElement("correctResponse");

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
