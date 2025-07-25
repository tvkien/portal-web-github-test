using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessing
{
    internal class DataFileProcessing : IDataFileProcessing
    {
        protected DataFileUploaderParameter _parameter;
        public virtual DataFileUploaderResult ParseManifestFile()
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
                    result.Result = "Can not find tag resource in  imsmanifest.xml";
                }
                result.Resources = new List<DataFileUploaderResource>(resourceNodes.Count);
                var index = 0;
                for (int i = 0; i < resourceNodes.Count; i++)
                {
                    var type = XmlUtils.GetNodeAttribute(resourceNodes[i], "type");
                    if (type != null
                        && type.ToLower() == "imsqti_item_xmlv2p1"
                        )
                    {
                        var resource = new DataFileUploaderResource();
                        resource.ResourceFileName = resourceNodes[i].Attributes["href"].Value;
                        if (Path.GetExtension(resource.ResourceFileName) == ".xml")
                        {
                            index ++;
                            resource.QuestionOrder = index;
                        }
                        resource.QtiSchemaID = (int)QtiSchemaEnum.MultipleChoice;//there's only one type Multiple choice
                        result.Resources.Add(resource);
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

        public DataFileUploaderResult Process(DataFileUploaderParameter parameter)
        {
            this._parameter = parameter;
            var result = ParseManifestFile();
            result.DataFileUploadTypeId = (int)DataFileUploadTypeEnum.DataFileUpload;
            if (!string.IsNullOrEmpty(result.Result))
            {
                return result;
            }
            //the first resource is instruction, ignore it
            if (result.Resources.Count > 0)
            {
                result.Resources[0].ErrorDetail = "It's an instruction content.";
                result.Resources[0].QtiSchemaID = 0;

                for (int i = 1; i < result.Resources.Count; i++)
                {
                    //if (result.Resources[i].ResourceFileName == "ICASMA20070502_KsQ.xml")
                    {
                        switch (result.Resources[i].QtiSchemaID)
                        {
                            case (int) QtiSchemaEnum.MultipleChoice:
                                result.Resources[i] = MultipleChoiceProcessing.Convert(_parameter, result.Resources[i]);
                                break;
                            default:
                                break;

                        }
                    }
                }
            }
            

            return result;
        }
    }
}
