using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica
{
    internal class DataFileProcessing : IDataFileProcessing
    {
        protected DataFileUploaderParameter _parameter;

        internal DataFileUploaderResult ParseManifestFile()
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
                }
                result.Resources = new List<DataFileUploaderResource>(resourceNodes.Count);
                for (int i = 0; i < resourceNodes.Count; i++)
                {
                    var fileName = resourceNodes[i].Attributes["href"].Value;
                    //Get identifier
                    var identifier = XmlUtils.GetNodeAttribute(resourceNodes[i], "identifier");
                    if (identifier != null)
                    {
                        var resource = new DataFileUploaderResource();
                        resource.Identifier = identifier;

                        //For passage
                        if (identifier.ToLower().StartsWith("p-"))
                        {
                            //get list of image files of passage
                            var fileNodes = XmlUtils.GetChildNodeListByName(resourceNodes[i], "file");
                            foreach (var fileNode in fileNodes)
                            {
                                var href = XmlUtils.GetNodeAttribute(fileNode, "href");
                                if (!string.IsNullOrEmpty(href))
                                {
                                    if (!href.ToLower().Equals(resourceNodes[i].Attributes["href"].Value.ToLower()))
                                    {
                                        resource.MediaFileList.Add(href);
                                        //example <file href="images/pg_111014_02.jpg" />
                                    }
                                }
                            }
                        }
                        //get passageList for item
                        if (identifier.ToLower().StartsWith("i-"))//for item
                        {
                            var dependencyList = XmlUtils.GetChildNodeListByName(resourceNodes[i], "dependency");
                            foreach (var dependency in dependencyList)
                            {
                                var identifierref = XmlUtils.GetNodeAttribute(dependency, "identifierref");
                                if (identifierref != null && identifierref.StartsWith("p-"))
                                    resource.PassageIdentifierRefList.Add(identifierref);
                            }
                        }
                        try
                        {
                            GetMetadata(identifier, resourceNodes[i], resource);
                        }
                        catch (Exception ex)
                        {
                            result.Result = "Error happened when parsing meta data";
                            result.Error = ex.GetFullExceptionMessage();
                        }

                        //for testing
                        //if (fileName == "qti_042506.xml" || fileName == "passages/3467.htm")
                        //if (Path.GetExtension(fileName) != null && Path.GetExtension(fileName).ToLower() == ".xml")
                        {
                            //read file to detect interactionType
                            var resourceFilePath = string.Format("{0}/{1}", _parameter.ExtractedFoler, fileName);
                            if (System.IO.File.Exists(resourceFilePath))
                            {
                                resource.ResourceFileName = fileName;
                                result.Resources.Add(resource);
                            }
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

        private void GetMetadata(string identifier, XmlNode resourceNode, DataFileUploaderResource resource)
        {
            var metadataNode = XmlUtils.GetSingleChildNodeByName(resourceNode, "metadata");
            if (metadataNode != null)
            {
                //get imsmd:lom node
                var qtiLom = XmlUtils.GetSingleChildNodeByName(metadataNode, "imsmd:lom");
                if (qtiLom != null)
                {
                    if (identifier.ToLower().StartsWith("i-")) //question, not passage
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
                                        if (valueNode.InnerText.Trim().ToLower().Equals("p-value"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                            var pValueNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (pValueNode != null)
                                            {
                                                var pText = pValueNode.InnerText;
                                                if (string.IsNullOrEmpty(pText))
                                                    pText = pValueNode.InnerXml;

                                                decimal value;
                                                if (!string.IsNullOrEmpty(pText) &&
                                                    decimal.TryParse(pText.Trim(), out value))
                                                    resource.PValue = value;
                                            }
                                        }
                                        else if (valueNode.InnerText.Trim().ToLower().Equals("grade level"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                            var grNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (grNode != null)
                                            {
                                                var pText = grNode.InnerText;
                                                if (string.IsNullOrEmpty(pText))
                                                    pText = grNode.InnerXml;

                                                resource.GradeLevel = pText.Trim();
                                            }
                                        }
                                        else if (valueNode.InnerText.Trim().ToLower().Equals("subject"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                            var sbNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (sbNode != null)
                                            {
                                                var pText = sbNode.InnerText;
                                                if (string.IsNullOrEmpty(pText))
                                                    pText = sbNode.InnerXml;

                                                resource.Subject = pText.Trim();
                                            }
                                        }
                                        else if (valueNode.InnerText.Trim().ToLower().Equals("difficulty"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                            var dfNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (dfNode != null)
                                            {
                                                var pText = dfNode.InnerText;
                                                if (string.IsNullOrEmpty(pText))
                                                    pText = dfNode.InnerXml;

                                                resource.Difficulty = pText.Trim();
                                            }
                                        }
                                        else if (valueNode.InnerText.Trim().ToLower().Equals("blooms taxonomy"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                            var btNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (btNode != null)
                                            {
                                                var pText = btNode.InnerText;
                                                if (string.IsNullOrEmpty(pText))
                                                    pText = btNode.InnerXml;

                                                resource.BloomsTaxonomy = pText.Trim();
                                            }
                                        }
                                        else if (valueNode.InnerText.Trim().ToLower().Equals("content focus"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                            var cfNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (cfNode != null)
                                            {
                                                var pText = cfNode.InnerText;
                                                if (string.IsNullOrEmpty(pText))
                                                    pText = cfNode.InnerXml;

                                                resource.ContentFocus = pText.Trim();
                                            }
                                        }

                                        else if (valueNode.InnerText.Trim().ToLower().Equals("abstandardguids"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                            var guidValueNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (guidValueNode != null)
                                            {
                                                var guidStr = guidValueNode.InnerText;
                                                if (string.IsNullOrWhiteSpace(guidStr))
                                                    guidStr = guidValueNode.InnerXml;
                                                if (!string.IsNullOrWhiteSpace(guidStr))
                                                {
                                                    if (guidStr.IndexOf(",", StringComparison.CurrentCulture) > -1)
                                                    {
                                                        var guidList = guidStr.Split(new char[] { ',' },
                                                            StringSplitOptions.RemoveEmptyEntries);
                                                        foreach (var guid in guidList)
                                                        {
                                                            resource.GUIDList.Add(guid);
                                                        }
                                                    }

                                                    resource.ABStandardGUIDs = guidStr;
                                                }
                                            }

                                        }
                                        else if (valueNode.InnerText.Trim().ToLower().Equals("depthofknowledge"))
                                        {
                                            var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                            var dokValueNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                "imsmd:string");
                                            if (dokValueNode != null)
                                            {
                                                var dokText = dokValueNode.InnerText;
                                                if (string.IsNullOrEmpty(dokText))
                                                    dokText = dokValueNode.InnerXml;

                                                resource.DOKCode = dokText;
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
                                        if (!string.IsNullOrEmpty(identifier) && identifier.ToLower().StartsWith("p-"))
                                        {
                                            //passge item -> Get passage title
                                            if (valueNode.InnerText.Trim().ToLower().Equals("passage title"))
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
                                                                resource.PassageTitle = resource.PassageTitle.Remove(0, 1);//remove the first "
                                                            }
                                                            if (resource.PassageTitle.EndsWith("\""))
                                                            {
                                                                resource.PassageTitle = resource.PassageTitle.Substring(0, resource.PassageTitle.Length - 1);//remove the end "
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            //if (valueNode.InnerText.Trim().ToLower().Equals("passagetype"))
                                            //{
                                            //    var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                            //        "imsmd:entry");
                                            //    if (entryNodes != null && entryNodes.Count > 0)
                                            //    {
                                            //        var entryStringNode =
                                            //            XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                            //        if (entryStringNode != null)
                                            //        {
                                            //            resource.PassageType = entryStringNode.InnerText.DecodeHtml();
                                            //        }
                                            //    }
                                            //}
                                            //if (valueNode.InnerText.Trim().ToLower().Equals("genre"))
                                            //{
                                            //    var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                            //        "imsmd:entry");
                                            //    if (entryNodes != null && entryNodes.Count > 0)
                                            //    {
                                            //        var entryStringNode =
                                            //            XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                            //        if (entryStringNode != null)
                                            //        {
                                            //            resource.Genre = entryStringNode.InnerText.DecodeHtml();
                                            //        }
                                            //    }

                                            //}
                                            if (valueNode.InnerText.Trim().ToLower().Equals("passage lexile"))
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

                                            //if (valueNode.InnerText.Trim().ToLower().Equals("spache"))
                                            //{
                                            //    var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                            //        "imsmd:entry");
                                            //    if (entryNodes != null && entryNodes.Count > 0)
                                            //    {
                                            //        var entryStringNode =
                                            //            XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                            //        if (entryStringNode != null)
                                            //        {
                                            //            resource.Spache = entryStringNode.InnerText.DecodeHtml();
                                            //        }
                                            //    }

                                            //}

                                            //if (valueNode.InnerText.Trim().ToLower().Equals("dalechall"))
                                            //{
                                            //    var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                            //        "imsmd:entry");
                                            //    if (entryNodes != null && entryNodes.Count > 0)
                                            //    {
                                            //        var entryStringNode =
                                            //            XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                            //        if (entryStringNode != null)
                                            //        {
                                            //            resource.DaleChall = entryStringNode.InnerText.DecodeHtml();
                                            //        }
                                            //    }

                                            //}
                                            //if (valueNode.InnerText.Trim().ToLower().Equals("rmm"))
                                            //{
                                            //    var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                            //        "imsmd:entry");
                                            //    if (entryNodes != null && entryNodes.Count > 0)
                                            //    {
                                            //        var entryStringNode =
                                            //            XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                            //        if (entryStringNode != null)
                                            //        {
                                            //            resource.RMM = entryStringNode.InnerText.DecodeHtml();
                                            //        }
                                            //    }

                                            //}
                                            if (valueNode.InnerText.Trim().ToLower().Equals("grade level"))
                                            {
                                                var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                                var grNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                    "imsmd:string");
                                                if (grNode != null)
                                                {
                                                    var pText = grNode.InnerText;
                                                    if (string.IsNullOrEmpty(pText))
                                                        pText = grNode.InnerXml;

                                                    resource.PGradeLevel = pText.Trim();
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("subject"))
                                            {
                                                var entryNode = XmlUtils.GetSingleChildNodeByName(taxonNode, "imsmd:entry");

                                                var sbNode = XmlUtils.GetSingleChildNodeByName(entryNode,
                                                    "imsmd:string");
                                                if (sbNode != null)
                                                {
                                                    var pText = sbNode.InnerText;
                                                    if (string.IsNullOrEmpty(pText))
                                                        pText = sbNode.InnerXml;

                                                    resource.PSubject = pText.Trim();
                                                }
                                            }

                                            if (valueNode.InnerText.Trim().ToLower().Equals("passage stimulus"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PStimulus = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("content area"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PContentArea = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("text type"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PTextType = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("text sub type"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PTextSubType = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("passage source"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PassageSource = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("word count"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PWordCount = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("ethnicity"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PEthnicity = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("commissioned status"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PCommissionedStatus = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("flesch kincaid"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PFleschKincaid = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("gender"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PGender = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("multi cultural"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PMultiCultural = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("copyright year"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PCopyrightYear = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("copyright owner"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PCopyrightOwner = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("passage source title"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PSourceTitle = entryStringNode.InnerText.DecodeHtml();
                                                    }
                                                }
                                            }
                                            if (valueNode.InnerText.Trim().ToLower().Equals("author"))
                                            {
                                                var entryNodes = XmlUtils.GetChildNodeListByName(taxonNode,
                                                    "imsmd:entry");
                                                if (entryNodes != null && entryNodes.Count > 0)
                                                {
                                                    var entryStringNode =
                                                        XmlUtils.GetSingleChildNodeByName(entryNodes[0], "imsmd:string");
                                                    if (entryStringNode != null)
                                                    {
                                                        resource.PAuthor = entryStringNode.InnerText.DecodeHtml();
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
        }
        public DataFileUploaderResult Process(DataFileUploaderParameter parameter)
        {
            this._parameter = parameter;
            var result = ParseManifestFile();
            result.DataFileUploadTypeId = (int)DataFileUploadTypeEnum.DataFileUploadCertica;
            if (!string.IsNullOrEmpty(result.Result))
            {
                return result;
            }

            for (int i = 0; i < result.Resources.Count; i++)
            {
                //convert passage identifierref to passage name -- exp: p-3426 ---> passages/3426.htm
                foreach (var identifierref in result.Resources[i].PassageIdentifierRefList)
                {
                    var resource = result.Resources.FirstOrDefault(x => x.Identifier == identifierref);
                    if (resource != null)
                        result.Resources[i].PassageList.Add(resource.ResourceFileName);
                }

                var comp = 0;
                var fileName = result.Resources[i].ResourceFileName;
                if (Path.GetExtension(fileName) != null && Path.GetExtension(fileName).ToLower() == ".xml")
                {
                    var ext = 0;
                    var inl = 0;
                    var tex = 0;
                    var choice = 0;
                    var hottext = 0;
                    var match = 0;
                    var imgmatch = 0;
                    var imghs = 0;
                    var ddsequence = 0;
                    //read file to detect interactionType
                    var doc = LoadXml(result.Resources[i], parameter.ExtractedFoler);
                    if (doc == null)
                    {
                        break;
                    }

                    var assessmentItemTag = doc.GetElementsByTagName("assessmentItem");
                    if (assessmentItemTag != null)
                    {
                        //get title
                        var title = XmlUtils.GetNodeAttribute(assessmentItemTag[0], "title");
                        result.Resources[i].Title = title;
                    }

                    var extendedTextInteraction = doc.GetElementsByTagName("extendedTextInteraction");
                    if (extendedTextInteraction != null && extendedTextInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.ExtendedText;

                        ext++;
                        comp += extendedTextInteraction.Count;
                    }
                    var inlineChoiceInteraction = doc.GetElementsByTagName("inlineChoiceInteraction");
                    if (inlineChoiceInteraction != null && inlineChoiceInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.InlineChoice;

                        inl++;
                        comp += inlineChoiceInteraction.Count;
                    }
                    var textEntryInteraction = doc.GetElementsByTagName("textEntryInteraction");
                    if (textEntryInteraction != null && textEntryInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.TextEntry;

                        tex++;
                        comp += textEntryInteraction.Count;
                    }
                    var choiceInteraction = doc.GetElementsByTagName("choiceInteraction");
                    if (choiceInteraction != null && choiceInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.MultipleChoice;

                        choice++;
                        comp += choiceInteraction.Count;
                    }
                    var orderInteraction = doc.GetElementsByTagName("orderInteraction");
                    if (orderInteraction != null && orderInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.DragAndDropSequence;

                        ddsequence++;
                        comp += orderInteraction.Count;
                    }
                    var hottextInteraction = doc.GetElementsByTagName("hottextInteraction");
                    if (hottextInteraction != null && hottextInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.TextHotSpot;

                        hottext++;
                        comp += hottextInteraction.Count;
                    }
                    var matchInteraction = doc.GetElementsByTagName("gapMatchInteraction");
                    if (matchInteraction != null && matchInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.DragAndDrop;

                        match++;
                        comp += matchInteraction.Count;
                    }
                    var imageMatchInteraction = doc.GetElementsByTagName("graphicGapMatchInteraction");
                    if (imageMatchInteraction != null && imageMatchInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.DragAndDrop;

                        imgmatch++;
                        comp += imageMatchInteraction.Count;
                    }
                    var hotspotInteraction = doc.GetElementsByTagName("hotspotInteraction");
                    if (hotspotInteraction != null && hotspotInteraction.Count > 0)
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.ImageHotSpot;

                        imghs++;
                        comp += hotspotInteraction.Count;
                    }

                    if (comp > 1) //complex question
                    {
                        result.Resources[i].QtiSchemaID = (int)QtiSchemaEnum.Complex;
                        result.Resources[i] = ComplexProcessing.Convert(_parameter, result.Resources[i], doc);
                        continue;
                    }

                    switch (result.Resources[i].QtiSchemaID)
                    {
                        case (int)QtiSchemaEnum.ExtendedText:
                            result.Resources[i] = ExtendedTextProcessing.Convert(_parameter, result.Resources[i], doc);
                            break;
                        case (int)QtiSchemaEnum.InlineChoice:
                            result.Resources[i] = InlineChoiceProcessing.Convert(_parameter, result.Resources[i], doc);
                            break;
                        case (int)QtiSchemaEnum.TextEntry:
                            result.Resources[i] = TextEntryProcessing.Convert(_parameter, result.Resources[i], doc);
                            break;
                        case (int)QtiSchemaEnum.MultipleChoice:
                            result.Resources[i] = MultipleChoiceProcessing.Convert(_parameter, result.Resources[i], doc);
                            break;
                        case (int)QtiSchemaEnum.DragAndDrop:
                            if (match > 0)
                                result.Resources[i] = DragAndDropProcessing.Convert(_parameter, result.Resources[i], doc);
                            else if (imgmatch > 0)
                                result.Resources[i] = Image_DragAndDropProcessing.Convert(_parameter, result.Resources[i], doc);
                            break;
                        case (int)QtiSchemaEnum.TextHotSpot:
                            result.Resources[i] = TextHotSpotProcessing.Convert(_parameter, result.Resources[i], doc);
                            break;
                        case (int)QtiSchemaEnum.DragAndDropSequence:
                            result.Resources[i] = OrderInteractionProcessing.Convert(_parameter, result.Resources[i]);
                            break;
                        case (int)QtiSchemaEnum.ImageHotSpot:
                            result.Resources[i] = ImageHotSpotProcessing.Convert(_parameter, result.Resources[i], doc);
                            break;

                    }
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
        internal static XmlContentProcessing LoadXml(DataFileUploaderResource resource, string extractedFolder)
        {
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
        internal static XmlNode GetAssessmentItemNode(XmlContentProcessing doc, DataFileUploaderResource resource)
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
        internal static bool CheckComplexType(XmlNode assessmentItemTag, DataFileUploaderResource resource)
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
        internal static bool CheckMultiSelect(XmlNode assessmentItemTag, DataFileUploaderResource resource)
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
        internal static void AddStyleSheetNode(XmlNode assessmentItemTag)
        {
            XmlUtils.RemoveNodeByName(ref assessmentItemTag, "stylesheet");
            XmlNode styleSheetNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "stylesheet",
                   assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
            XmlUtils.AddAttribute(styleSheetNode, "href", "stylesheet/linkitStyleSheet.css");
            XmlUtils.AddAttribute(styleSheetNode, "type", "text/css");
            XmlUtils.InsertFirstChild(ref assessmentItemTag, styleSheetNode);
        }
        public static void UpdateMediaFilePathInXmlContent(XmlNode assessmentItemTag, DataFileUploaderParameter parameter, DataFileUploaderResource resource)
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

        public static void UpdateMediaFilePathDestinationInXmlContent(XmlNode assessmentItemTag, DataFileUploaderParameter parameter, DataFileUploaderResource resource)
        {
            try
            {
                resource.ProcessingStep.Append("->Update MediaFilePathDestinationInXmlContent");
                XmlNodeList destinationNodeList = assessmentItemTag.OwnerDocument.GetElementsByTagName("destinationObject");
                if (destinationNodeList == null || destinationNodeList.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < destinationNodeList.Count; i++)
                {
                    XmlNode imgNode = destinationNodeList[i];
                    var src = XmlUtils.GetNodeAttributeCaseInSensitive(imgNode, "src");
                    try
                    {
                        //get value of src
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

                                resource.MediaFileList.Add(src);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        resource.Error = string.Format("Can not update image file of destination in xml content: {0} ", src);
                        resource.ErrorDetail = ex.GetFullExceptionMessage();
                    }

                }
            }
            catch (Exception ex)
            {
                resource.Error = string.Format("Can not update image file of destination in xml content.");
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }

        }
        public static string CreateMainBodyTag(XmlNode assessmentItemTag)
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
        public static void RemoveOutcomeDeclarationTag(XmlNode assessmentItemTag)
        {
            XmlUtils.RemoveNodeByName(ref assessmentItemTag, "outcomeDeclaration");
        }
        internal static void InsertTagObjectForPassage(XmlNode assessmentItemTag, DataFileUploaderResource resource)
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

                var firstChild = itemBodyNode.FirstChild;
                foreach (var passage in resource.PassageList)
                {
                    var objectNode = assessmentItemTag.OwnerDocument.CreateNode(XmlNodeType.Element, "object", assessmentItemTag.OwnerDocument.DocumentElement.NamespaceURI);
                    XmlUtils.SetOrUpdateNodeAttribute(ref objectNode, "data", passage);
                    itemBodyNode.InsertBefore(objectNode, firstChild);
                }
            }
            catch (Exception ex)
            {
                resource.Error = string.Format("Can not create object tag under itemBody in xml content.");
                resource.ErrorDetail = ex.GetFullExceptionMessage();
            }
        }

        public static void RemoveResponseProcessingTag(XmlNode assessmentItemTag)
        {
            XmlUtils.RemoveNodeByName(ref assessmentItemTag, "responseProcessing");
        }

        public static string RemoveRubricBlockTag(XmlNode assessmentItemTag)
        {
            var removedNodes = assessmentItemTag.OwnerDocument.GetElementsByTagName("rubricBlock");
            var nodeCount = removedNodes.Count;
            for (int i = 0; i < nodeCount; i++)
            {
                var node = removedNodes[0];
                var parentNode = node.ParentNode;
                if (parentNode != null)
                {
                    parentNode.RemoveChild(node);
                }
            }
            return string.Empty;
        }
    }
}
