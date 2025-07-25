using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTI3pItem
    {
        private string xmlContent = string.Empty;

        public int QTI3pItemID { get; set; }
        public string XmlContent
        {
            get { return xmlContent; }
            set { xmlContent = value.ConvertNullToEmptyString(); }
        }

        public string Title { get; set; }
        public int QTISchemaID { get; set; }
        public string CorrectAnswer { get; set; }
        public string OriginPath { get; set; }
        public string FilePath { get; set; }
        public string UrlPath { get; set; }
        public string Difficulty { get; set; }
        public decimal? Pvalue { get; set; }
        public string Subject { get; set; }
        public string GradeLevel { get; set; }
        public int? GradeID { get; set; }
        public string BloomsTaxonomy { get; set; }
        public string ABStandardGUIDs { get; set; }
        public string Identifier { get; set; }
        public string ItemRubric { get; set; }
        public string MathOriginPath { get; set; }
        public int? BloomsID { get; set; }
        public int? ContentFocusID { get; set; }
        public int? ItemDifficultyID { get; set; }
        public int? QTI3pSourceID { get; set; }
        public string XmlSource { get; set; }
        public string ContentFocus { get; set; }
        public int? SubjectID { get; set; }
        public int TotalRow { get; set; }
        //Information for building tooltip
        public string GradeName { get; set; }
        public string Standard { get; set; }
        public string PassageNumber { get; set; }
        public string PassageGrade { get; set; }
        public string PassageSubject { get; set; }
        public string PassageWordCount { get; set; }
        public string PassageTextType { get; set; }
        public string PassageTextSubType { get; set; }
        public string PassageFleschKinkaid { get; set; }
        public string Qti3pItemDOK { get; set; }
        public bool From3pUpload { get; set; }
    }
}
