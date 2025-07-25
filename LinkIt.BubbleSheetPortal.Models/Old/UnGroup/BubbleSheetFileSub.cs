using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BubbleSheetFileSub
    {
        private string _inputFilePath = string.Empty;
        private string _inputFileName = string.Empty;
        private string _resolution = string.Empty;
        private string _outputFileName = string.Empty;

        public int BubblSheetFileSubId { get; set; }
        public int BubbleSheetFileId { get; set; }
        public int RosterPosition { get; set; }
        public int PageNumber { get; set; }

        public int? PageType { get; set; }

        public DateTime CreateDate { get; set; }

        public string InputFilePath
        {
            get { return _inputFilePath; }
            set { _inputFilePath = value.ConvertNullToEmptyString(); }
        }

        public string InputFileName
        {
            get { return _inputFileName; }
            set { _inputFileName = value.ConvertNullToEmptyString(); }
        }

        public string Resolution
        {
            get { return _resolution; }
            set { _resolution = value.ConvertNullToEmptyString(); }
        }

        public string OutputFileName
        {
            get { return _outputFileName; }
            set { _outputFileName = value.ConvertNullToEmptyString(); }
        }

        public int StudentID { get; set; }
    }
}
