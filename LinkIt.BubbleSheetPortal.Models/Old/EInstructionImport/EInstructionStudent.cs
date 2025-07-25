using System;
using Envoc.Core.Shared.Extensions;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.EInstructionImport
{
    public class EInstructionStudent
    {
        private string inputName = string.Empty;
        private string localCode = string.Empty;
        private string className = string.Empty;
        private string sectionName = string.Empty;
        public string score = string.Empty;

        public int ID { get; set; }

        public string InputName
        {
            get { return inputName; }
            set { inputName = value.ConvertNullToEmptyString(); }
        }

        public string LocalCode
        {
            get { return localCode; }
            set { localCode = value.ConvertNullToEmptyString(); }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value.ConvertNullToEmptyString(); }
        }

        public string SectionName
        {
            get { return sectionName; }
            set { sectionName = value.ConvertNullToEmptyString(); }
        }

        public string Score
        {
            get { return score; }
            set { score = value.ConvertNullToEmptyString(); }
        }

        public string IsSelected {get;set;}        
    }
}
