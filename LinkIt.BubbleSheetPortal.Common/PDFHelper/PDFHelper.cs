using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Common.PDFHelper
{
    public static class PDFHelper
    {
        public static void ExtractPages(string sourcePdfPath, string outputPdfPath, int startPage, int endPage)
        {
            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                using (PdfReader reader = new PdfReader(sourcePdfPath))
                {
                    // For simplicity, I am assuming all the pages share the same size
                    // and rotation as the first page:
                    using (Document sourceDocument = new Document(reader.GetPageSizeWithRotation(startPage)))
                    {
                        // Initialize an instance of the PdfCopyClass with the source 
                        // document and an output file stream:
                        using (PdfCopy pdfCopyProvider = new PdfCopy(sourceDocument,
                              new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create)))
                        {
                            sourceDocument.Open();
                            // Walk the specified range and add the page copies to the output file:
                            for (int i = startPage; i <= endPage; i++)
                            {
                                PdfImportedPage importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                                pdfCopyProvider.AddPage(importedPage);
                            }
                            sourceDocument.Close();
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public static void CombineMultiplePDFs(IEnumerable<string> fileNames, string outFile)
        {
            // step 1: creation of a document-object
            using (Document document = new Document())
            {
                //create newFileStream object which will be disposed at the end
                using (FileStream newFileStream = new FileStream(outFile, FileMode.Create))
                {
                    // step 2: we create a writer that listens to the document
                    using (PdfCopy writer = new PdfCopy(document, newFileStream))
                    {
                        if (writer == null)
                        {
                            return;
                        }

                        // step 3: we open the document
                        document.Open();
                        foreach (string fileName in fileNames)
                        {
                            // we create a reader for a certain document
                            using (PdfReader reader = new PdfReader(fileName))
                            {
                                reader.ConsolidateNamedDestinations();
                                writer.AddDocument(reader);
                                reader.Close();
                            }
                        }
                        // step 5: we close the document and writer
                        writer.Close();
                        document.Close();
                    }
                }//disposes the newFileStream object
            }
        }
        public static Dictionary<int, string> ReadTextPerPage(string fileFullPath)
        {
            using (PdfReader reader = new PdfReader(fileFullPath))
            {
                Dictionary<int, string> res = new Dictionary<int, string>();
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    var text = PdfTextExtractor.GetTextFromPage(reader, i);
                    res.Add(i, text);
                }
                return res;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slpitData">list of: from index, to index</param>
        /// <returns></returns>
        public static List<byte[]> SplitFileReturnFileBinary(string sourcePath, List<int[]> list)
        {
            var fileList = list.Select((c, i) =>
            {
                string desFilePath = sourcePath.Replace(".pdf", $"_{i + 1}.pdf");
                ExtractPages(sourcePath, desFilePath, list[i][0], list[i][1]);
                if (File.Exists(desFilePath))
                {
                    var res = File.ReadAllBytes(desFilePath);
                    try
                    {
                        File.Delete(desFilePath);
                    }
                    catch (Exception) { }
                    return res;
                }
                else
                {
                    return null;
                }
            }).ToList();
            return fileList;
        }

        public static byte[] MergePDFFilesData(string temporaryPath, List<byte[]> files)
        {
            string _processFolder = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).Replace(".", "");
            _processFolder = Path.Combine(temporaryPath, _processFolder);

            try
            {
                if (!Directory.Exists(_processFolder))
                {
                    Directory.CreateDirectory(_processFolder);
                }

                // random zip file name
                string _randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).Replace(".", "") + ".zip";
                string _finalFilePath = Path.Combine(_processFolder, _randomFileName);

                List<string> filePaths = new List<string>();
                foreach (var item in files)
                {
                    //write all file to Disk
                    string _randomSubFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).Replace(".", "") + ".pdf";
                    string _filePath = Path.Combine(_processFolder, _randomSubFileName);
                    File.WriteAllBytes(_filePath, item);
                    filePaths.Add(_filePath);
                }
                CombineMultiplePDFs(filePaths, _finalFilePath);
                return File.ReadAllBytes(_finalFilePath);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                try
                {
                    if (Directory.Exists(_processFolder))
                    {
                        Directory.Delete(_processFolder,true);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
