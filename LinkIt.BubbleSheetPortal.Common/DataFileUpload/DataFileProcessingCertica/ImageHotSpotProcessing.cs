using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;
namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica
{
    internal class ImageHotSpotProcessing
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

                XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", ((int)QtiSchemaEnum.ImageHotSpot).ToString());
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

                resource.ProcessingStep.Append("->Update Response Declaration and correct response.");
                resource.Error = UpdateResponseDeclarationAndCorrectResponse(assessmentItemTag);
                if (!string.IsNullOrEmpty(resource.Error))
                    return resource;

                resource.ProcessingStep.Append("->Update hotspotInteraction to imageHotSpot.");
                resource.Error = UpdateHotspotInteraction(assessmentItemTag, parameter, resource);
                if (!string.IsNullOrEmpty(resource.Error))
                    return resource;    

                //update itemBody tag
                resource.ProcessingStep.Append("->Update MainBody.");
                resource.Error = DataFileProcessing.CreateMainBodyTag(assessmentItemTag);
                if (!string.IsNullOrWhiteSpace(resource.Error))
                {
                    return resource;
                }
                //update mediaFilePath
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
            var hotspotChoices = assessmentItemTag.OwnerDocument.GetElementsByTagName("hotspotChoice");
            if (hotspotChoices.Count == 0)
                return "Can not find tag hottext";

            for (int i = 0; i < valueNodeCount; i++)
            {
                var identifier = "";
                var index = 0;
                foreach(XmlNode hs in hotspotChoices)
                {
                    index++;
                    var oldIdentifier = XmlUtils.GetNodeAttribute(hs, "identifier");
                    var value = valueNodes[i].InnerText;

                    if (oldIdentifier == value)
                    {
                        identifier = "IHS_" + index;
                        break;
                    }
                }                                
                XmlNode newNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "correctResponse",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "identifier", identifier);
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "pointValue", "0");
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
        private static string UpdateHotspotInteraction(XmlNode assessmentItemTag, DataFileUploaderParameter parameter,
            DataFileUploaderResource resource)
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

            //add new imageHotSpot Node
            var hotspotInteraction = XmlUtils.GetSingleChildNodeByName(itemBodyNode, "hotspotInteraction");
            if (hotspotInteraction == null)
                return "Can not find tag hotspotInteraction";

            var imageHotSpot = CreateImageHotSpotNode(hotspotInteraction, parameter, resource);
            itemBodyNode.AppendChild(imageHotSpot);

            //move all child nodes of hotspotInteraction into itemBody
            XmlUtils.MoveChildNodes(ref hotspotInteraction, ref imageHotSpot);

            //remove hotspotInteraction
            itemBodyNode.RemoveChild(hotspotInteraction);

            //convert hotspotChoice to sourceItem
            var hotspotChoices = itemBodyNode.OwnerDocument.GetElementsByTagName("hotspotChoice");
            if (hotspotChoices.Count == 0)
                return "Can not find tag hotspotChoice";

            var count = hotspotChoices.Count;
            for (int i = 0; i < count; i++)
            {
                var identifier = "IHS_" + (i+1);
                var node = hotspotChoices[0];

                var sourceItem = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "sourceItem",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);

                //get coords
                var coords = XmlUtils.GetNodeAttribute(node, "coords");
                if (!string.IsNullOrWhiteSpace(coords))
                {
                    var listCoord = coords.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "left", listCoord[0]);
                    XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "top", listCoord[1]);
                    XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "width", listCoord[2]);
                    XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "height", listCoord[3]);
                }

                //get identifier
                var innerText = XmlUtils.GetNodeAttribute(node, "identifier");
                if (!string.IsNullOrWhiteSpace(innerText))
                {
                    sourceItem.InnerText = innerText;
                }
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "identifier", identifier);
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "pointValue", "0");                
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "typeHotSpot", "letter");
                XmlUtils.SetOrUpdateNodeAttribute(ref sourceItem, "hiddenHotSpot", "false");

                node.ParentNode.ReplaceChild(sourceItem, node);
            }

            return string.Empty;
        }
        private static XmlNode CreateImageHotSpotNode(XmlNode hotspotInteraction, DataFileUploaderParameter parameter, DataFileUploaderResource resource)
        {
            var maxChoices = XmlUtils.GetNodeAttribute(hotspotInteraction, "maxChoices");
            var imgObject = XmlUtils.GetSingleChildNodeByName(hotspotInteraction, "object");

            var imageHotSpot = hotspotInteraction.OwnerDocument.CreateNode(XmlNodeType.Element, "imageHotSpot",
                   hotspotInteraction.OwnerDocument.DocumentElement.NamespaceURI);

            if (imgObject != null)
            {
                UpdateImageFilePathOfHotSpot(imgObject, imageHotSpot, parameter, resource);
                //remove imgObject
                hotspotInteraction.RemoveChild(imgObject);
            }

            XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "maxhotspot", maxChoices);
            XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "responseIdentifier", "RESPONSE_1");            

            return imageHotSpot;
        }
        private static void UpdateImageFilePathOfHotSpot(XmlNode imgObject, XmlNode imageHotSpot, DataFileUploaderParameter parameter, DataFileUploaderResource resource)
        {
            resource.ProcessingStep.Append("->Update ImageFilePathOfHotSpot");
            var src = XmlUtils.GetNodeAttribute(imgObject, "data");
            var width = XmlUtils.GetNodeAttribute(imgObject, "width");
            var height = XmlUtils.GetNodeAttribute(imgObject, "height");
            
            //change img file path to s3
            if (!string.IsNullOrWhiteSpace(src))
            {
                if (!src.ToLower().StartsWith("http"))
                {
                    //change src value
                    if (parameter.S3TestMedia.ToLower().StartsWith("http"))
                    {
                        //now use absolute s3 link
                        var s3Path = string.Format("{0}/{1}", parameter.S3TestMedia.RemoveEndSlash(),
                            src.RemoveStartSlash());
                        XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "src", s3Path);
                    }
                    else
                    {
                        var s3Path = parameter.S3TestMedia;
                        if (!string.IsNullOrEmpty(parameter.AUVirtualTestFolder))
                        {
                            s3Path = s3Path.Replace(parameter.AUVirtualTestFolder, "");
                        }
                        s3Path.Replace("//", "/");
                        XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "src",
                            string.Format("/{0}/{1}", s3Path, src));
                    }

                }
            }

            XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "percent", "10");
            XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "imgorgw", width);
            XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "imgorgh", height);
            XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "width", width);
            XmlUtils.SetOrUpdateNodeAttribute(ref imageHotSpot, "height", height);

            resource.MediaFileList.Add(src);
        }
    }
}
