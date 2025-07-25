using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GenericBubbleSheetACTSAT
    {
        private string inputFileName = string.Empty;
        private string outputFileName = string.Empty;
        private string ticket = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;

        public int BubbleSheetId { get; set; }
        public int BubbleSheetFileId { get; set; }
        public int? ClassID { get; set; }
        public int StudentID { get; set; }
        public string ClassIds { get; set; }

        public string InputFileName
        {
            get { return inputFileName; }
            set { inputFileName = value.ConvertNullToEmptyString(); }
        }

        public string OutputFileName
        {
            get { return outputFileName; }
            set { outputFileName = value.ConvertNullToEmptyString(); }
        }

        public string Ticket
        {
            get { return ticket; }
            set { ticket = value.ConvertNullToEmptyString(); }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value.ConvertNullToEmptyString(); }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value.ConvertNullToEmptyString(); }
        }

        public int VirtualTestSubTypeId { get; set; }
    }
}
