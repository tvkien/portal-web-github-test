using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingNew
{
    internal class DataFileProcessing : IDataFileProcessing
    {
        protected DataFileUploaderParameter _parameter;

        internal Dictionary<string, int> ParseAssessmentFile(DataFileUploaderResult result)
        {
            var questionOrderDic = new Dictionary<string, int>();
            XmlDocument doc = new XmlDocument();
            try
            {
                var assessmentFilePath = string.Format("{0}/{1}", _parameter.ExtractedFoler, "assessment.xml");
                if (!System.IO.File.Exists(assessmentFilePath))
                {
                    result.Result =
                        "The uploading file does not contain assessment.xml, please double check and try again.";
                }
                else
                {
                    var assessmentContent = System.IO.File.ReadAllText(assessmentFilePath);
                    doc.LoadXml(assessmentContent);

                    //get assessmentItemRef
                    var assessmentItemRefs = doc.GetElementsByTagName("assessmentItemRef");
                    var index = 0;
                    foreach (XmlNode assessmentItemRef in assessmentItemRefs)
                    {
                        var fileName = XmlUtils.GetNodeAttribute(assessmentItemRef, "href");
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            index ++;
                            questionOrderDic.Add(fileName, index);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = "Can not load assessment.xml";
                result.Error = ex.GetFullExceptionMessage();

            }
            return questionOrderDic;
        }
        internal DataFileUploaderResult ParseManifestFile(Dictionary<string,int> questionOrderDic)
        {
            XmlDocument doc = new XmlDocument();
            var result = new DataFileUploaderResult();
            
            // Try to Open imsmanifest.xml
            try
            {
                var imsmanifestPath = string.Format("{0}/{1}", _parameter.ExtractedFoler, "imsmanifest.xml");
                if (!System.IO.File.Exists(imsmanifestPath))
                {
                    result.Result =
                        "The uploading file does not contain imsmanifest.xml, please double check and try again.";
                }
                else
                {
                    var imsmanifestContent = System.IO.File.ReadAllText(imsmanifestPath);
                    doc.LoadXml(imsmanifestContent);                   
                }


            }
            catch (Exception ex)
            {
                result.Result = "Can not load imsmanifest.xml";
                result.Error = ex.GetFullExceptionMessage();
            }
            if (!string.IsNullOrEmpty(result.Result))
            {
                return result;
            }
            try
            {

                // Get all resource node
                var resourceNodes = doc.GetElementsByTagName("resource");
                if (resourceNodes.Count == 0)
                {
                    result.Result = "Can not find tag resource in imsmanifest.xml";
                    return result;
                }
                result.Resources = new List<DataFileUploaderResource>(resourceNodes.Count);
                for (int i = 0; i < resourceNodes.Count; i++)
                {
                    var type = XmlUtils.GetNodeAttribute(resourceNodes[i], "type");
                    if (type != null //&& type.ToLower() == "imsqti_item_xmlv2p1"
                        )
                    {
                        var resource = new DataFileUploaderResource();
                        //For passage
                        if (type.ToLower().Equals("webcontent"))
                        {
                            //Get identifier
                            resource.Identifier = XmlUtils.GetNodeAttribute(resourceNodes[i], "identifier");
                            //get list of image files of passage
                            var fileNodes = XmlUtils.GetChildNodeListByName(resourceNodes[i], "file");
                            foreach (var fileNode in fileNodes)
                            {
                                var href = XmlUtils.GetNodeAttribute(fileNode, "href");
                                if (!string.IsNullOrEmpty(href))
                                {
                                    if (!href.ToLower().Equals(resourceNodes[i].Attributes["href"].Value.ToLower()))
                                    {
                                        resource.MediaFileList.Add(href);//example <file href="images/pg_111014_02.jpg" />
                                    }
                                }
                            }
                        }
                        resource.ResourceFileName = resourceNodes[i].Attributes["href"].Value;
                        if (questionOrderDic.ContainsKey(resource.ResourceFileName))
                        {
                            resource.QuestionOrder = questionOrderDic[resource.ResourceFileName];
                        }
                        try
                        {
                            if (resource.ResourceFileName != "assessment.xml")
                            {
                                GetMetadata(type, resourceNodes[i], ref resource);
                            }
                        }
                        catch (Exception ex)
                        {
                            result.Result = "Error happened when parsing meta data";
                            result.Error = ex.GetFullExceptionMessage();
                        }
                        if (!string.IsNullOrEmpty(result.Result))
                        {
                            return result;//stop if there's any error
                        }

                        if (!string.IsNullOrEmpty(resource.InteractionType))
                        {
                            switch (resource.InteractionType.ToLower())
                            {
                                case "choiceinteraction":
                                    resource.QtiSchemaID = (int) QtiSchemaEnum.MultipleChoice;
                                    break;

                                case "extendedtextinteraction":
                                    resource.QtiSchemaID = (int) QtiSchemaEnum.ExtendedText;
                                    break;

                                case "hottextinteraction":
                                    resource.QtiSchemaID = (int) QtiSchemaEnum.TextHotSpot;
                                    break;

                                case "inlinechoiceinteraction":
                                    resource.QtiSchemaID = (int) QtiSchemaEnum.InlineChoice;
                                    break;
                                case "matchinteraction":
                                    resource.QtiSchemaID = (int) QtiSchemaEnum.DragAndDrop;
                                    break;
                                case "orderinteraction":
                                    resource.QtiSchemaID = (int) QtiSchemaEnum.DragAndDropSequence;
                                    break;
                                case "textentryinteraction":
                                    resource.QtiSchemaID = (int) QtiSchemaEnum.TextEntry;
                                    break;
                                default:
                                    resource.QtiSchemaID = 0;
                                    break;
                            }
                        }
                        else
                        {
                            //resource.Error = "Can not find tag imsqti:qtiMetadata or imsqti:interactionType to detect interactionType";
                        }
                        //for testing
                        //if (resource.ResourceFileName == "128727.xml")
                        {
                            result.Resources.Add(resource);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = "Can not read one or more resource tag.";
                result.Error = ex.GetFullExceptionMessage();
            }
            if (result.Resources.Count == 0)
            {
                result.Result = "Unable to find resource to import from file manifest.";
            }
            return result;
        }

        private void GetMetadata(string type, XmlNode resourceNode, ref DataFileUploaderResource resource)
        {
            var metadataNode = XmlUtils.GetSingleChildNodeByName(resourceNode, "metadata");
            if (metadataNode != null)
            {
                if (!type.ToLower().Equals("webcontent")) //question, not passage
                {
                    var qtiMetadata = XmlUtils.GetSingleChildNodeByName(metadataNode, "imsqti:qtiMetadata");
                    if (qtiMetadata != null)
                    {
                        var interactionTypeNode = XmlUtils.GetSingleChildNodeByName(qtiMetadata,
                            "imsqti:interactionType");
                        if (interactionTypeNode != null)
                        {
                            resource.InteractionType = interactionTypeNode.InnerText;
                        }
                        else
                        {
                            resource.Error = "Can not find tag imsqti:interactionType in imsmanifest.xml";
                            return;
                        }
                    }
                    else
                    {
                        resource.Error = "Can not find tag imsqti:qtiMetadata in imsmanifest.xml";
                        return;
                    }
                }

                //get imsmd:lom node
                var qtiLom = XmlUtils.GetSingleChildNodeByName(metadataNode, "imsmd:lom");
                if (qtiLom != null)
                {
                    if (!type.ToLower().Equals("webcontent")) //not passage
                    {
                        var qtiClassification = XmlUtils.GetSingleChildNodeByName(qtiLom, "imsmd:classification");
                        if (qtiClassification != null)
                        {
                            var qtiTaxonPaths = XmlUtils.GetChildNodeListByName(qtiClassification, "imsmd:taxonPath");
                            foreach (var taxonPathNode in qtiTaxonPaths)
                            {
                                var sourceNode = XmlUtils.GetSingleChildNodeByName(taxonPathNode, "imsmd:source");
                                var taxonNode = XmlUtils.GetSingleChildNodeByName(taxonPathNode, "imsmd:taxon");
                                if (sourceNode != null && taxonNode != null)
                                {
                                    var valueNode = XmlUtils.GetSingleChildNodeByName(sourceNode, "imsmd:string");
                                    if (valueNode != null)
                                    {

                                        if (valueNode.InnerXml.Contains("AB GUID"))
                                        {
                                            var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode, "imsmd:entry");
                                            foreach (var entryNode in entryNodes)
                                            {
                                                var guidValueNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                    "imsmd:string");
                                                if (guidValueNode != null)
                                                {
                                                    var guid = guidValueNode.InnerText;
                                                    if (string.IsNullOrEmpty(guid))
                                                        guid = guidValueNode.InnerXml;
                                                    if (!string.IsNullOrEmpty(guid))
                                                        resource.GUIDList.Add(guid.Trim());
                                                }
                                            }
                                        }
                                        else if (valueNode.InnerXml.Contains("Depth of Knowledge/DOK"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");


                                            var dokValueNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (dokValueNode != null)
                                            {
                                                var dokText = dokValueNode.InnerText;
                                                if (string.IsNullOrEmpty(dokText))
                                                    dokText = dokValueNode.InnerXml;

                                                int dok;
                                                if (!string.IsNullOrEmpty(dokText) &&
                                                    int.TryParse(dokText.Trim(), out dok))
                                                    resource.DOK = dok;
                                            }

                                        }

                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        //passage
                        List<XmlNode> qtiClassificationList = XmlUtils.GetChildNodeListByName(qtiLom,
                            "imsmd:classification");
                        for (int i = 0; i < qtiClassificationList.Count; i++)
                        {
                            XmlNode qtiClassification = qtiClassificationList[i];

                            var qtiTaxonPaths = XmlUtils.GetChildNodeListByName(qtiClassification, "imsmd:taxonPath");
                            foreach (var taxonPathNode in qtiTaxonPaths)
                            {
                                var sourceNode = XmlUtils.GetSingleChildNodeByName(taxonPathNode, "imsmd:source");
                                var taxonNode = XmlUtils.GetSingleChildNodeByName(taxonPathNode, "imsmd:taxon");
                                if (sourceNode != null && taxonNode != null)
                                {
                                    var valueNode = XmlUtils.GetSingleChildNodeByName(sourceNode, "imsmd:string");
                                    if (valueNode != null)
                                    {
                                        if (type.ToLower().Equals("webcontent") &&
                                            !string.IsNullOrEmpty(resource.Identifier) &&
                                            resource.Identifier.ToLower().StartsWith("passage"))
                                        {
                                            //passge item -> Get passage title
                                            if (valueNode.InnerText.Trim().ToLower().Equals("title"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PassageTitle = entryStringNode.InnerText.DecodeHtml();
                                                        if (!string.IsNullOrEmpty(resource.PassageTitle))
                                                        {
                                                            if (resource.PassageTitle.StartsWith("\""))
                                                            {
                                                                resource.PassageTitle = resource.PassageTitle.Remove(0,
                                                                    1); //remove the first "
                                                            }
                                                            if (resource.PassageTitle.EndsWith("\""))
                                                            {
                                                                resource.PassageTitle =
                                                                    resource.PassageTitle.Substring(0,
                                                                        resource.PassageTitle.Length - 1);
                                                                    //remove the end "
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("passagetype"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PassageType = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("genre"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.Genre = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }

                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("lexile"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.Lexile = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }

                                            }

                                            if (valueNode.InnerText.Trim().ToLower().Equals("spache"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.Spache = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }

                                            }

                                            if (valueNode.InnerText.Trim().ToLower().Equals("dalechall"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.DaleChall = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }

                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("rmm"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.RMM = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }

                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (!type.ToLower().Equals("webcontent")) //question, not passage                
                    resource.Error = "Can not find tag metadata in imsmanifest.xml";                
            }
        }

        public DataFileUploaderResult Process(DataFileUploaderParameter parameter)
        {
            this._parameter = parameter;
            var result = new DataFileUploaderResult();
            var questionOrderDic = ParseAssessmentFile(result);
            if (!string.IsNullOrEmpty(result.Result))
            {
                return result;
            }

            result = ParseManifestFile(questionOrderDic);

            if (parameter.UploadTo3pItem)
                result.QTI3pSourceId = parameter.QTI3pSourceId;
            else
                result.DataFileUploadTypeId = (int)DataFileUploadTypeEnum.DataFileUploadProgressive;

            if (!string.IsNullOrEmpty(result.Result))
            {
                return result;
            }
            for (int i = 0; i < result.Resources.Count; i++)
            {

                switch (result.Resources[i].QtiSchemaID)
                {
                    case (int)QtiSchemaEnum.MultipleChoice:
                        result.Resources[i] = MultipleChoiceProcessing.Convert(_parameter, result.Resources[i]);
                        break;
                    case (int)QtiSchemaEnum.ExtendedText:
                        result.Resources[i] = ExtendedTextProcessing.Convert(_parameter, result.Resources[i]);
                        break;
                    case (int)QtiSchemaEnum.TextEntry:
                        result.Resources[i] = TextEntryProcessing.Convert(_parameter, result.Resources[i]);
                        break;
                    case (int)QtiSchemaEnum.InlineChoice:
                        result.Resources[i] = InlineChoiceProcessing.Convert(_parameter, result.Resources[i]);
                        break;
                    case (int)QtiSchemaEnum.TextHotSpot:
                        result.Resources[i] = TextHotSpotProcessing.Convert(_parameter, result.Resources[i]);
                        break;
                    case (int)QtiSchemaEnum.DragAndDrop:
                        result.Resources[i] = DragAndDropProcessing.Convert(_parameter, result.Resources[i]);
                        break;
                    case (int)QtiSchemaEnum.DragAndDropSequence:
                        result.Resources[i] = DragAndDropSequenceProcessing.Convert(_parameter, result.Resources[i]);
                        break;
                    default:
                        break;

                }
            }

            return result;
        }

        internal static string RemoveLineBreakBetweenAssessmentAttributes(string xml)
        {
            //Sometime a tag contains line break between its attributes, like this
            //<assessmentItem xmlns="http://www.imsglobal.org/xsd/imsqti_v2p1"
            //xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //xmlns:m="http://www.w3.org/1998/Math/MathML"
            //xsi:schemaLocation="http://www.imsglobal.org/xsd/imsqti_v2p1  http://www.imsglobal.org/xsd/qti/qtiv2p1/imsqti_v2p1p1.xsd"
            //identifier="item-128702" title="Sample Export - Reading Advanced - 128702" adaptive="false" timeDependent="false">
            //<responseDeclaration

            //-> Need to remove line break between attributes
            //<assessmentItem xmlns="http://www.imsglobal.org/xsd/imsqti_v2p1"  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:m="http://www.w3.org/1998/Math/MathML" xsi:schemaLocation="http://www.imsglobal.org/xsd/imsqti_v2p1  http://www.imsglobal.org/xsd/qti/qtiv2p1/imsqti_v2p1p1.xsd" identifier="item-128702" title="Sample Export - Reading Advanced - 128702" adaptive="false" timeDependent="false"><responseDeclaration
            try
            {
                int assessmentItemStartPos = xml.IndexOf("<assessmentItem");
                int responseDeclarationStartPos = xml.IndexOf("<responseDeclaration");
                if (assessmentItemStartPos >= 0 && responseDeclarationStartPos > 0)
                {
                    //get string between assessmentItemStartPos and responseDeclarationStartPos
                    var assessmentItem = xml.Substring(assessmentItemStartPos,
                        responseDeclarationStartPos - assessmentItemStartPos);
                    assessmentItem = assessmentItem.Replace("\r\n", "\n");
                    assessmentItem = assessmentItem.Replace("\r", "\n");
                    assessmentItem = assessmentItem.Replace("\n", "");
                    return assessmentItem +
                           xml.Substring(responseDeclarationStartPos, xml.Length - responseDeclarationStartPos);
                }
                else
                {
                    return xml;
                }
            }
            catch
            {
                return xml;
            }
        }

        internal static XmlContentProcessing LoadXml(ref DataFileUploaderResource resource, string extractedFolder)
        {
            resource.ProcessingStep.Append("->Check Extension.");
            if (Path.GetExtension(resource.ResourceFileName) != null && Path.GetExtension(resource.ResourceFileName).ToLower() == ".css")
            {
                resource.Error = string.Format("\"{0}\" is ignored,it's CSS file.", resource.ResourceFileName);
                resource.QtiSchemaID = 0;
            }

            resource.ProcessingStep.Append("->Read resource file");
            var resourceFilePath = string.Format("{0}/{1}", extractedFolder, resource.ResourceFileName);
            if (!System.IO.File.Exists(resourceFilePath))
            {
                //resource.Error = string.Format("Can not find resource file \"{0}\"", resource.ResourceFileName);
                return null;
            }
            var resourceContent = System.IO.File.ReadAllText(resourceFilePath);
            resource.OriginalContent = resourceContent;

            resourceContent = resourceContent.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "");

            resource.ProcessingStep.Append("->Remove Trouble Characters.");
            resourceContent = resourceContent.RemoveTroublesomeCharacters();

            resource.ProcessingStep.Append("->Remove LineBreak Between Assessment Attributes.");
            resourceContent = DataFileProcessing.RemoveLineBreakBetweenAssessmentAttributes(resourceContent);

            resource.ProcessingStep.Append("->Load into XmlContentProcessing to process.");
            XmlContentProcessing doc = new XmlContentProcessing(resourceContent);
            if (doc == null || !doc.IsXmlLoadedSuccess)
            {
                resource.Error = string.Format("Can not load resource file \"{0}\" in xml to process.", resource.ResourceFileName);
                resource.ErrorDetail = doc.LoadXmlContentException;
                return null;
            }
            return doc;
        }

        internal static XmlNode GetAssessmentItemNode(ref XmlContentProcessing doc, ref DataFileUploaderResource resource)
        {
            //get tag assessmentItem
            resource.ProcessingStep.Append("->Get tag assessmentItem.");
            XmlNodeList assessmentItemTags = doc.GetElementsByTagName("assessmentItem");
            if (assessmentItemTags == null || assessmentItemTags.Count == 0)
            {
                resource.Error = string.Format("Can not load assessmentItem of resource file \"{0}\" in xml to process.", resource.ResourceFileName);
                return null;
            }
            resource.ProcessingStep.Append("->Update attributes for assessmentItemTag.");
            var assessmentItemTag = assessmentItemTags[0];
            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "xsi:schemaLocation", "http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd");
            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "adaptive", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "timeDependent", "false");
            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "xmlUnicode", "true");
            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "toolName", "linkitTLF");
            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "toolVersion", "2.0");
            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "qtiSchemeID", resource.QtiSchemaID.ToString());



            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "xmlns", "http://www.imsglobal.org/xsd/imsqti_v2p0");
            XmlUtils.SetOrUpdateNodeAttribute(ref assessmentItemTag, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            XmlUtils.AddAttribute(assessmentItemTag, "Importfrom", DataFileUploadTypeEnum.DataFileUploadProgressive.ToString());
            return assessmentItemTags[0];
        }

        internal static bool CheckComplexType(XmlNode assessmentItemTag, ref DataFileUploaderResource resource)
        {
            //check complex type
            resource.ProcessingStep.Append("->Check complex type");
            var responseDeclaration = assessmentItemTag.OwnerDocument.GetElementsByTagName("responseDeclaration");
            if (responseDeclaration != null && responseDeclaration.Count >= 2)
            {
                resource.QtiSchemaID = (int)QtiSchemaEnum.Complex;
                return true;

            }
            return false;
        }
        internal static bool CheckMultiSelect(XmlNode assessmentItemTag, ref DataFileUploaderResource resource)
        {
            //check complex type
            resource.ProcessingStep.Append("->Check Multi Select type");
            XmlNode correctResponse = assessmentItemTag.OwnerDocument.GetElementsByTagName("correctResponse")[0];
            if (correctResponse != null)
            {
                int countValueNode = 0;
                foreach (XmlNode value in correctResponse.ChildNodes)
                {
                    if (value.Name == "value")
                    {
                        countValueNode++;
                    }
                }
                if (countValueNode > 1)
                {
                    resource.ProcessingStep.Append("->Found Multi Select type");
                    return true;
                }

            }
            return false;
        }
        internal static void AddStyleSheetNode(ref XmlNode assessmentItemTag)
        {
            XmlUtils.RemoveNodeByName(ref assessmentItemTag, "stylesheet");
            XmlNode styleSheetNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "stylesheet",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
            XmlUtils.AddAttribute(styleSheetNode, "href", "stylesheet/linkitStyleSheet.css");
            XmlUtils.AddAttribute(styleSheetNode, "type", "text/css");
            XmlUtils.InsertFirstChild(ref assessmentItemTag, styleSheetNode);
        }
        public static void UpdateMediaFilePathInXmlContent(ref XmlNode assessmentItemTag, DataFileUploaderParameter parameter, ref DataFileUploaderResource resource)
        {
            try
            {
                resource.ProcessingStep.Append("->Update MediaFilePathInXmlContent");
                XmlNodeList imgNodeList = assessmentItemTag.OwnerDocument.GetElementsByTagName("img");
                if (imgNodeList == null || imgNodeList.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < imgNodeList.Count; i++)
                {
                    XmlNode imgNode = imgNodeList[i];
                    var src = XmlUtils.GetNodeAttributeCaseInSensitive(imgNode, "src");
                    try
                    {
                        //get value of src
                        if (!string.IsNullOrWhiteSpace(src))
                        {
                            if (!src.ToLower().StartsWith("http"))
                            {
                                //change src value
                                if (parameter.UploadTo3pItem)
                                {
                                    if (parameter.S3TestMedia.ToLower().StartsWith("http"))
                                    {
                                        //now use absolute s3 link
                                        var s3Path = string.Format("{0}/{1}", parameter.S3TestMedia.RemoveEndSlash(),
                                            src.RemoveStartSlash());
                                        XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "src", s3Path);
                                    }
                                    else
                                    {
                                        var s3Path = parameter.S3TestMedia;
                                        if (!string.IsNullOrEmpty(parameter.AUVirtualTestFolder))
                                        {
                                            s3Path = s3Path.Replace(parameter.AUVirtualTestFolder, "");
                                        }
                                        s3Path.Replace("//", "/");
                                        XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "src", string.Format("/{0}/{1}", s3Path, src));
                                    }
                                    
                                }
                                else
                                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "src", string.Format("/ItemSet_{0}/{1}", parameter.QtiGroupId, src));

                                resource.MediaFileList.Add(src);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        resource.Error = string.Format("Can not update image file in xml content: {0} ", src);
                        resource.ErrorDetail = ex.GetFullExceptionMessage();
                    }

                }
            }
            catch (Exception ex)
            {
                resource.Error = string.Format("Can not update image file in xml content.");
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }

        }

        public static string CreateMainBodyTag(ref XmlNode assessmentItemTag)
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
            XmlNode mainBodyNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "div",
                assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
            XmlUtils.SetOrUpdateNodeAttribute(ref mainBodyNode, "class", "mainBody");
            XmlUtils.SetOrUpdateNodeAttribute(ref mainBodyNode, "styleName", "mainBody");

            //move all child nodes of itemBodyNode into mainBodyNode
            XmlUtils.MoveChildNodes(ref itemBodyNode, ref mainBodyNode);

            //then //insert mainBodyNode as the first child of itemBodyNode
            XmlUtils.InsertFirstChild(ref itemBodyNode, mainBodyNode);

            return string.Empty;
        }
        public static void RemoveOutcomeDeclarationTag(ref XmlNode assessmentItemTag)
        {
            XmlUtils.RemoveNodeByName(ref assessmentItemTag, "outcomeDeclaration");
        }
        internal static void MoveTagObject(XmlNode assessmentItemTag, DataFileUploaderResource resource, DataFileUploaderParameter parameter)
        {
            try
            {
                resource.ProcessingStep.Append("->MoveTagObject");
                //get itemBody tag
                XmlNode itemBodyNode = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itemBody");
                if (itemBodyNode == null)
                {
                    //try to get with name itembody
                    itemBodyNode = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "itembody");
                }
                if (itemBodyNode == null)
                {
                    resource.Error = "Can not find tag itemBody";
                }

                XmlNodeList objectNodeList = assessmentItemTag.OwnerDocument.GetElementsByTagName("object");
                if (objectNodeList == null || objectNodeList.Count == 0)
                {
                    return;
                }
                var firstChild = itemBodyNode.FirstChild;
                int i = 0;
                do
                {
                    var firstNode = objectNodeList[i];
                    if (firstNode != null)
                    {
                        var type = XmlUtils.GetNodeAttribute(firstNode, "type");
                        var data = XmlUtils.GetNodeAttribute(firstNode, "data");
                        if (type == "text/html") //passage
                        {
                            resource.PassageList.Add(data);

                            itemBodyNode.InsertBefore(firstNode, firstChild);
                            //sourceNode.RemoveChild(firstNode);//no need to do this, firstNode will be automatically removed from sourceNode
                        }
                        else if (type == "audio/mpeg")//audio
                        {
                            MoveTagObjectAudio(resource, parameter, data, firstNode);
                            //Remove object audio node
                            var pNode = firstNode.ParentNode;
                            if (pNode != null) pNode.RemoveChild(firstNode);
                        }
                    }
                    i++;
                } while (i < objectNodeList.Count);


            }
            catch (Exception ex)
            {
                resource.Error = string.Format("Can not move object tag under itemBody in xml content.");
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }
        }

        private static void MoveTagObjectAudio(DataFileUploaderResource resource, DataFileUploaderParameter parameter,
            string data, XmlNode firstNode)
        {
            resource.MediaFileList.Add(data);
            //detect audio of question or audio of answer
            XmlNode parentNode = firstNode;
            do
            {
                parentNode = parentNode.ParentNode;
                if (parentNode != null)
                {
                    if (parentNode.Name == "itemBody" //audio of question
                        || parentNode.Name == "simpleChoice" //audio of answer of choiceInteraction(multiplechoice)
                        )
                    {
                        {
                            if (!data.ToLower().StartsWith("http"))
                            {
                                //change src value
                                if (parameter.UploadTo3pItem)
                                {
                                    if (parameter.S3TestMedia.ToLower().StartsWith("http"))
                                    {
                                        //now use absolute s3 link
                                        var s3Path = string.Format("{0}/{1}", parameter.S3TestMedia.RemoveEndSlash(),
                                            data.RemoveStartSlash());
                                        XmlUtils.SetOrUpdateNodeAttribute(ref parentNode, "audioRef", s3Path);
                                    }
                                    else
                                    {
                                        var s3Path = parameter.S3TestMedia;
                                        if (!string.IsNullOrEmpty(parameter.AUVirtualTestFolder))
                                        {
                                            s3Path = s3Path.Replace(parameter.AUVirtualTestFolder, "");
                                        }
                                        s3Path.Replace("//", "/");
                                        XmlUtils.SetOrUpdateNodeAttribute(ref parentNode, "audioRef",
                                            string.Format("/{0}/{1}", s3Path, data));
                                    }
                                }
                                else
                                    XmlUtils.SetOrUpdateNodeAttribute(ref parentNode, "audioRef",
                                        string.Format("/ItemSet_{0}/{1}", parameter.QtiGroupId, data));
                            }
                        }

                        break;
                    }
                }
            } while (parentNode != null && parentNode.Name != "assessmentItem");           
        }

        internal static string RemoveRubricBlockTag(ref XmlNode assessmentItemTag)
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
            XmlUtils.RemoveNodeByName(ref itemBodyNode, "rubricBlock");
            return string.Empty;
        }

        internal static void RemoveMappingTag(ref XmlNode assessmentItemTag)
        {
            XmlNode responseDeclaration = XmlUtils.GetSingleChildNodeByName(assessmentItemTag, "responseDeclaration");
            XmlUtils.RemoveNodeByName(ref responseDeclaration, "mapping");
        }
    }

}
