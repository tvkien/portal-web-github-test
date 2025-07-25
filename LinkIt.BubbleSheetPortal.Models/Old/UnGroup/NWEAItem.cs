using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class NWEAItem
    {
        private string xmlContent = string.Empty;
        private string difficulty = string.Empty;
        private string bloomsTaxonomy = string.Empty;
        private string subject = string.Empty;
        private string urlpath= string.Empty;
        private string number= string.Empty;
        public string fleschKincaid = string.Empty;

        public int QTI3pItemID { get; set; }
        public int BloomsID { get; set; }
        public int SubjectID { get; set; }
        public int GradeID { get; set; }
        public int ItemDifficultyID { get; set; }
        
        public int WordCountID { get; set; }
        public int TextSubTypeID { get; set; }
        public int TextTypeID { get; set; }
        public int QTI3pItemPassageID { get; set; }

        public string FleschKincaid
        {
            get { return fleschKincaid; }
            set { fleschKincaid = value.ConvertNullToEmptyString(); }
        }
        public string Number
        {
            get { return number; }
            set { number = value.ConvertNullToEmptyString(); }
        }

        public string Urlpath
        {
            get { return urlpath; }
            set { urlpath = value.ConvertNullToEmptyString(); }
        }

         public string Subject
        {
            get { return subject; }
            set { subject = value.ConvertNullToEmptyString(); }
        }
        public string XMLContent
        {
            get { return xmlContent; }
            set { xmlContent = value.ConvertNullToEmptyString(); }
        }

        public string Difficulty
        {
            get { return difficulty; }
            set { difficulty = value.ConvertNullToEmptyString(); }
        }

         public string BloomsTaxonomy
        {
            get { return bloomsTaxonomy; }
            set { bloomsTaxonomy = value.ConvertNullToEmptyString(); }
        }
    }
}
