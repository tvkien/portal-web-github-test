using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica
{
    public class DragAndDropProcessing
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

            var divNode = XmlUtils.GetSingleChildNodeByName(itemBodyNode, "div");
            XmlNode matchInteractionNode = XmlUtils.GetSingleChildNodeByName(divNode, "gapMatchInteraction");
            if (matchInteractionNode == null)
            {
                resource.Error = "Can not find tag gapMatchInteraction";
                return;
            }
            
            Dictionary<string, string> identifierDesDic = new Dictionary<string, string>();

            //find all tag <gap> to build desNodeList
            var gapNodeList = itemBodyNode.OwnerDocument.GetElementsByTagName("gap");
            var gapNodeCount = gapNodeList.Count;
            for (int i = 0; i < gapNodeCount; i++)
            {
                var gapNode = gapNodeList[0];
                var desNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "destinationObject", null);
                XmlUtils.SetOrUpdateNodeAttribute(ref desNode, "partialID",
                    string.Format("Partial_{0}", i + 1));
                XmlUtils.SetOrUpdateNodeAttribute(ref desNode, "type", "text");

                var identifier = XmlUtils.GetNodeAttribute(gapNode, "identifier");
                var identifierLinkit = string.Format("DEST_{0}", i + 1);

                var itemNewNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element,
                    "destinationItem", null);
                XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "destIdentifier", identifierLinkit);
                XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "order", (i + 1).ToString());
                XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "width", "55");
                XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "height", "20");
                XmlUtils.SetOrUpdateNodeAttribute(ref itemNewNode, "numberDroppable", "1");

                desNode.AppendChild(itemNewNode);                
                identifierDesDic.Add(identifier, identifierLinkit);

                //replace gapNode to desNode
                gapNode.ParentNode.ReplaceChild(desNode, gapNode);
            }

            Dictionary<string, string> identifierSourceDic = new Dictionary<string, string>();
            //find all tag <gapText> to build destNodeList
            var gapTextNodeList = itemBodyNode.OwnerDocument.GetElementsByTagName("gapText");
            var gapTextCount = gapTextNodeList.Count;
            for (int i = 0; i < gapTextCount; i++)
            {
                var gapTextNode = gapTextNodeList[0];

                var identifier = XmlUtils.GetNodeAttribute(gapTextNode, "identifier");
                var identifierLinkit = string.Format("SRC{0}", i + 1);

                var newNode = itemBodyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "sourceObject", null);
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "srcIdentifier", identifierLinkit);
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "partialID", "Partial_1");
                if(i > 0)
                    XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "style", "width: 85px; height: 20px; margin-left:5px");//default value in Portal
                else
                {
                    XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "style", "width: 85px; height: 20px;");
                }
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "type", "text");
                XmlUtils.SetOrUpdateNodeAttribute(ref newNode, "data-limit", "unlimited");

                newNode.InnerText = gapTextNode.InnerText;
                identifierSourceDic.Add(identifier, identifierLinkit);

                //replace gapText to sourceObject
                gapTextNode.ParentNode.ReplaceChild(newNode, gapTextNode);
            }

            //crate a new div that will be used to replace matchInteraction
            XmlNode dvMatchInteractionNode = assessmentItemTag.OwnerDocument.CreateElement("div");
            XmlUtils.SetOrUpdateNodeAttribute(ref dvMatchInteractionNode, "class", "matchInteraction");
           
            dvMatchInteractionNode.InnerXml = matchInteractionNode.InnerXml;
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
                //mapKey look like gap2 choice2 -> parse gap2 and choice2 
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
