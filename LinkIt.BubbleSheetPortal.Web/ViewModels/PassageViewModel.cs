namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PassageViewModel
    {
        public PassageViewModel()
        {
            QtiRefObjectID = 0;
            Data = string.Empty;
            RefNumber = 0;
            DataFileUploadPassageID = 0;
            Qti3pPassageID = 0;
            Qti3pSourceID = 0;
            DataFileUploadTypeID = 0;
        }

        #region Linkit Passage

        public int QtiRefObjectID { get; set; } //Linkit Passage
        public string Name { get; set; }

        #endregion Linkit Passage

        #region 3p Passage

        public int Qti3pPassageID { get; set; }
        public int Qti3pSourceID { get; set; }
        public string Qti3pSource { get; set; }

        #endregion 3p Passage

        //Upload DataFile, Progress,.. on Item Set

        #region DataFileUPloadPassage

        public int DataFileUploadPassageID { get; set; }
        public int DataFileUploadTypeID { get; set; }

        #endregion DataFileUPloadPassage

        public string Data { get; set; }//A hyperlink
        public int RefNumber { get; set; }// Sometime a qtiItem has <object data ="http://www.linkit.com/NWEA13-2/01QTI 2.0/01FullItemBank/05 96 DPI JPG and MathML/LanguageArtsGrade 01-0/passages/3035.htm" then RefNumber will be 3035 ( in 3035.html), this is not actual RefObjectID be cause 3035.html is assigned from passage of qti3p item
        public string FileName { get; set; }
    }
}
