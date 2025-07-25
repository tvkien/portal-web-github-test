using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTI3pPassage
    {
        private string passageName = string.Empty;
        private string number = string.Empty;
        private string fullpath = string.Empty;
        private string subject = string.Empty;

        public int QTI3pPassageID { get; set; }
        public string PassageName
        {
            get { return passageName; }
            set { passageName = value.ConvertNullToEmptyString(); }
        }

        public string Number
        {
            get { return number; }
            set { number = value.ConvertNullToEmptyString(); }
        }

        public string Fullpath
        {
            get { return fullpath; }
            set { fullpath = value.ConvertNullToEmptyString(); }
        }

        public string Subject
        {
            get { return subject; }
            set { subject = value.ConvertNullToEmptyString(); }
        }
        public string PassageTitle
        {
            get ;set;
        }
        public string Identifier { get; set; }

        public int? Qti3pSourceID { get; set; }

        public string GradeLevel { get; set; }
        public string PassageStimulus { get; set; }
        public string ContentArea { get; set; }
        public string TextType { get; set; }
        public string TextSubType { get; set; }
        public string PassageSource { get; set; }
        public string WordCount { get; set; }
        public string Ethnicity { get; set; }
        public string CommissionedStatus { get; set; }
        public string FleschKincaid { get; set; }
        public string Gender { get; set; }
        public string MultiCultural { get; set; }
        public string CopyrightYear { get; set; }
        public string CopyrightOwner { get; set; }
        public string PassageSourceTitle { get; set; }
        public string Author { get; set; }
        public int? GradeID { get; set; }
        public int? ContentAreaID { get; set; }
        public int? TextTypeID { get; set; }
        public int? TextSubTypeID { get; set; }
        public int? WordCountID { get; set; }
        public int? FleschKincaidID { get; set; }
    }
}
