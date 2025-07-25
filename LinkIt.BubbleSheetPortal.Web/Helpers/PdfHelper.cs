using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Drawing;
using Envoc.BubbleService.Imaging;
using System.Drawing.Imaging;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public static class PdfHelper
    {

        private static readonly int DEFAULT_RESOLUTION = 96;
        private static readonly int DEFAULT_WIDTH = 794;
        private static readonly int DEFAULT_HEIGHT = 1123;

        public static void ExtractPage(string sourcePdfPath, string outputPdfPath, int pageNumber)
        {
            PdfReader reader = null;
            Document document = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                reader = new PdfReader(sourcePdfPath);

                // Capture the correct size and orientation for the page:
                document = new Document(reader.GetPageSizeWithRotation(pageNumber));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(document,
                    new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

                document.Open();

                // Extract the desired page number:
                importedPage = pdfCopyProvider.GetImportedPage(reader, pageNumber);
                pdfCopyProvider.AddPage(importedPage);
                document.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExtractPages(string sourcePdfPath, string outputPdfPath, int startPage, int endPage)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                reader = new PdfReader(sourcePdfPath);

                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                sourceDocument = new Document(reader.GetPageSizeWithRotation(startPage));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(sourceDocument,
                    new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

                sourceDocument.Open();

                // Walk the specified range and add the page copies to the output file:
                for (int i = startPage; i <= endPage; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExtractPages(string sourcePdfPath, string outputPdfPath, int[] extractThesePages)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            try
            {
                // Intialize a new PdfReader instance with the 
                // contents of the source Pdf file:
                reader = new PdfReader(sourcePdfPath);

                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                sourceDocument = new Document(reader.GetPageSizeWithRotation(extractThesePages[0]));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(sourceDocument,
                    new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

                sourceDocument.Open();

                // Walk the array and add the page copies to the output file:
                foreach (int pageNumber in extractThesePages)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, pageNumber);
                    pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte[] MergeFiles(List<byte[]> listFilesInBytes)
        {
            using(var outputStream = new MemoryStream())
            {
                var readerList = new List<PdfReader>();
                foreach (var file in listFilesInBytes)
                {
                    var reader = new PdfReader(file);
                    readerList.Add(reader);
                }

                var document = new Document(PageSize.A4);

                var pdfCopyProvider = new PdfCopy(document, outputStream);
                document.Open();

                foreach (var pdfReader in readerList)
                {
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        var importedPage = pdfCopyProvider.GetImportedPage(pdfReader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }
                    pdfReader.Close();
                }

                document.Close();
                pdfCopyProvider.Close();
                var completeFile = outputStream.ToArray();
                return completeFile;
            }
        }

        public static byte[] MergeFilesAddBlankToOddFile(List<byte[]> listFilesInBytes)
        {
            using (var outputStream = new MemoryStream())
            {
                var readerList = new List<PdfReader>();
                foreach (var file in listFilesInBytes)
                {
                    var reader = new PdfReader(file);
                    readerList.Add(reader);
                }

                var document = new Document(PageSize.A4);

                var pdfCopyProvider = new PdfCopy(document, outputStream);
                document.Open();

                foreach (var pdfReader in readerList)
                {
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        var importedPage = pdfCopyProvider.GetImportedPage(pdfReader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }

                    if (pdfReader.NumberOfPages%2 != 0 && readerList.Count > 1)
                    {
                        var rectangle = pdfReader.GetPageSize(1);
                        pdfCopyProvider.AddPage(rectangle, 0);
                    }

                    pdfReader.Close();
                }

                document.Close();
                pdfCopyProvider.Close();
                var completeFile = outputStream.ToArray();
                return completeFile;
            }
        }

        public static List<string> ConvertImage(MemoryStream stream, int? resolution = null, bool resizeImage = true)
        {
            List<string> listTestImages = new List<string>();
            var listOfPages = ConvertToPng(stream, resolution, resizeImage);

            foreach (var page in listOfPages)
            {
                var imageBase64 = string.Format("{0},{1}", "data:image/png;base64", Convert.ToBase64String(page));
                listTestImages.Add(imageBase64);
            }

            return listTestImages;
        }
        public static List<byte[]> ConvertToPng(MemoryStream stream, int? resolution = null, bool resizeImage = true)
        {
            // Init license Atalasoft - please do not remove it
            var temp = ImagingConfiguration.temp;

            stream.Position = 0;
            EnvocPdfDocument pdfDocument;
            try
            {
                pdfDocument = new EnvocPdfDocument(stream);
            }
            catch (Exception)
            {
                return new List<byte[]>();
            }

            List<byte[]> listTestImages = new List<byte[]>();
            Bitmap newBitmap;

            foreach (var pdfPage in pdfDocument.Pages)
            {
                var pdfReader = new EnvocPdfDecoder();
                pdfReader.Resolution = !resolution.HasValue ? DEFAULT_RESOLUTION : resolution.Value;
                var image = pdfReader.Read(pdfPage.Stream, pdfPage.Frame, null);
                if (resizeImage)
                    newBitmap = ResizeImage(image.ToBitmap(), DEFAULT_WIDTH, DEFAULT_HEIGHT);
                else
                    newBitmap = image.ToBitmap();

                var imageStream = new MemoryStream();
                newBitmap.Save(imageStream, ImageFormat.Png);
                imageStream.Position = 0;
                listTestImages.Add(imageStream.ToArray());
            }

            return listTestImages;
        }

        private static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new System.Drawing.Rectangle(0, 0, width, height),
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        }

        public static bool ValidatePDF(Stream stream)
        {
            // Init license Atalasoft - please do not remove it
            var temp = ImagingConfiguration.temp;

            EnvocPdfDocument pdfDocument;
            pdfDocument = new EnvocPdfDocument(stream);
            foreach (var pdfPage in pdfDocument.Pages)
            {
                var pdfReader = new EnvocPdfDecoder();
                pdfReader.Resolution = DEFAULT_RESOLUTION;
                var image = pdfReader.Read(pdfPage.Stream, pdfPage.Frame, null);
                try
                {
                    var bitmap = image.ToBitmap();
                    if (bitmap != null)
                        return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
