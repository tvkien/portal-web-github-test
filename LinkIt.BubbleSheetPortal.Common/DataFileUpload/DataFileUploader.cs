using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload
{
    public class DataFileUploader
    {
        private readonly  DataFileUploaderParameter _parameter;
        public DataFileUploader(DataFileUploaderParameter parameter)
        {
            _parameter = parameter;
        }
        public  DataFileUploaderResult ProcessDataUploadFiles()
        {
            IDataFileProcessing dataFileProcessing;
            if (_parameter.UploadTo3pItem)
            {
                switch(_parameter.QTI3pSourceId)
                {
                    case (int)QTI3pSourceEnum.Progress:
                        dataFileProcessing = new DataFileProcessingNew.DataFileProcessing();
                        break;
                    case (int)QTI3pSourceEnum.Mastery:
                        dataFileProcessing = new DataFileProcessingCertica.DataFileProcessing();
                        break;
                    default:
                        dataFileProcessing = new DataFileProcessingNew.DataFileProcessing();
                        break;
                }
            }
            else
            {
                //check if there's any file assessment.xml in the extractedFoler folder or not
                if (!System.IO.File.Exists(string.Format("{0}/{1}", _parameter.ExtractedFoler, "assessment.xml")))
                {
                    dataFileProcessing = new DataFileProcessing.DataFileProcessing();
                }
                else
                {
                    dataFileProcessing = new DataFileProcessingNew.DataFileProcessing();
                }
            }
            var result = dataFileProcessing.Process(_parameter);

            if (result.Resources.Any(x => !string.IsNullOrEmpty(x.Error) && !string.IsNullOrEmpty(x.ErrorDetail)))
            {
                result.Result =
                    result.Resources.FirstOrDefault(
                        x => !string.IsNullOrEmpty(x.Error) && !string.IsNullOrEmpty(x.ErrorDetail)).Error;
            }
            else if(result.Resources.Any(x => !string.IsNullOrEmpty(x.Error)))
            {
                var resourceErrors = result.Resources.Where(x => !string.IsNullOrEmpty(x.Error));
                foreach (var resource in resourceErrors)
                {
                    result.Error += string.Format("{0} : {1}; ", resource.ResourceFileName, resource.Error);                    
                }

                result.Result = "Have some formatting issues. Please make it correct.";
            }
            return result;

        }

        /// <summary>
        /// Create a relative sub folder for storing uploaded item files ( on S3 or local )
        /// </summary>
        /// <param name="qti3pSourceId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CreateSubFolderStoring(int qti3pSourceId, string fileName)
        {
            var subFolderPath = string.Empty;
            switch (qti3pSourceId)
            {
                case (int)QTI3pSourceEnum.Progress:
                    subFolderPath += string.Format("{0}/{1}", DataFileUploadConstant.QTI3pSourceUploadSubFolder, QTI3pSourceEnum.Progress.ToString());
                    break;
            }
            subFolderPath = string.Format("{0}/{1}", subFolderPath, fileName);
            return subFolderPath;

        }
    }
}
