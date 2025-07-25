using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetFile
    {
        private string barcode2 = string.Empty;
        private string barcode1 = string.Empty;
        private string resolution = string.Empty;
        private string inputFilePath = string.Empty;
        private string inputFileName = string.Empty;
        private string fileDisposition = string.Empty;
        private string outputFileName = string.Empty;
        private string resultString = string.Empty;

        public int BubbleSheetFileId { get; set; }
        public int BubbleSheetId { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsUnmappedGeneric { get; set; }
        public DateTime? Date { get; set; }
        public int? PageNumber { get; set; }
        public decimal? ProcessingTime { get; set; }
        public int? RosterPosition { get; set; }
        public int? ResultCount { get; set; }
        public int? UserId { get; set; }
        public int? DistrictId { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string Ticket { get; set; }

        public int? ClassId { get; set; }
        public bool IsGenericSheet { get; set; }

        public string UrlSafeOutputFileName 
        { 
            get { return OutputFileName.Replace('/', '-'); }
        }

        public string Barcode1
        {
            get { return barcode1; }
            set { barcode1 = value.ConvertNullToEmptyString(); }
        }

        public string Barcode2
        {
            get { return barcode2; }
            set { barcode2 = value.ConvertNullToEmptyString(); }
        }

        public string InputFilePath
        {
            get { return inputFilePath; }
            set { inputFilePath = value.ConvertNullToEmptyString(); }
        }

        public string InputFileName
        {
            get { return inputFileName; }
            set { inputFileName = value.ConvertNullToEmptyString(); }
        }

        public string Resolution
        {
            get { return resolution; }
            set { resolution = value.ConvertNullToEmptyString(); }
        }

        public string FileDisposition
        {
            get { return fileDisposition; }
            set { fileDisposition = value.ConvertNullToEmptyString(); }
        }

        public string OutputFileName
        {
            get { return outputFileName; }
            set { outputFileName = value.ConvertNullToEmptyString(); }
        }

        public string ResultString
        {
            get { return resultString; }
            set { resultString = value.ConvertNullToEmptyString(); }
        }

        public int GenericTestIndex { get; set; }
    }
}
