using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Network
{
    public class RosterUploadService : IRosterUploadService
    {
        public void UploadFile(HttpPostedFileBase postedFile, Request request, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("Roster file path cannot be empty");
            }

            var bytes = new byte[postedFile.ContentLength];
            postedFile.InputStream.Read(bytes, 0, postedFile.ContentLength);

            var fileExtension = Path.GetExtension(postedFile.FileName);
            string[] allowedExtensions = { ".txt", ".csv" };

            if (Array.IndexOf(allowedExtensions, fileExtension) == -1)
            {
                throw new InvalidOperationException("Unsupported file extension");
            }

            var fileName = request.Id.ToString(CultureInfo.InvariantCulture) + fileExtension;

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var filePathAndName = Path.Combine(filePath, fileName);

            try
            {
                var fileStream = new FileStream(filePathAndName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                fileStream.Position = fileStream.Length;
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (IOException)
            {
                throw new IOException("Network share not found.  Current location: " + filePath);
            }
            catch (Exception)
            {
                throw new Exception("An error occured while trying to save the roster to the file share.");
            }
        }

        public void MoveRosterFileBackToUploadedFolder(int requestId, string importedFileName, string uploadFilePath,
            string completedFilePath, Action<string> logErrorAction)
        {
            var fileExtension = Path.GetExtension(importedFileName);
            string fileName = requestId + fileExtension;
            string uploadFilePathAndName = Path.Combine(uploadFilePath, fileName);
            string completedFilePathAndName = Path.Combine(completedFilePath, fileName);

            try
            {
                File.Move(completedFilePathAndName, uploadFilePathAndName);
            }
            catch (Exception ex)
            {
                logErrorAction(
                    $"fileExtension: '{fileExtension}', fileName: '{fileName}', uploadFilePathAndName: '{uploadFilePathAndName}', completedFilePathAndName: '{completedFilePathAndName}', Exception detail: {ex.ToString()}");

                throw new Exception("An error occured while trying to move the roster to the completed folder.");
            }
        }


        public void UploadTestDataFile(HttpPostedFileBase postedFile, Request request, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("TestData file path cannot be empty");
            }

            var bytes = new byte[postedFile.ContentLength];
            postedFile.InputStream.Read(bytes, 0, postedFile.ContentLength);
            var fileName = request.Id.ToString(CultureInfo.InvariantCulture) + ".txt";
            var filePathAndName = Path.Combine(filePath, fileName);

            try
            {
                var fileStream = new FileStream(filePathAndName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                fileStream.Position = fileStream.Length;
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (IOException)
            {
                throw new IOException("Network share not found.  Current location: " + filePath);
            }
            catch (Exception)
            {
                throw new Exception("An error occured while trying to save the TestData to the file share.");
            }
        }

        public void UploadRosterFile(HttpPostedFileBase postedFile, Request request, string destinationFolder)
        {
            if (string.IsNullOrEmpty(destinationFolder))
            {
                throw new InvalidOperationException("Destination folder path cannot be null or empty.");
            }

            var bytes = new byte[postedFile.ContentLength];
            postedFile.InputStream.Read(bytes, 0, postedFile.ContentLength);

            var fileExtension = Path.GetExtension(postedFile.FileName);
            string[] allowedExtensions = { ".zip" };

            if (Array.IndexOf(allowedExtensions, fileExtension) == -1)
            {
                throw new InvalidOperationException("Unsupported file extension. We are only support extensions: '.zip'");
            }

            var fileName = request.Id.ToString(CultureInfo.InvariantCulture) + fileExtension;

            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            var filePathAndName = Path.Combine(destinationFolder, fileName);

            try
            {
                var fileStream = new FileStream(filePathAndName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                fileStream.Position = fileStream.Length;
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            catch (IOException)
            {
                throw new IOException("Network share not found.  Current location: " + destinationFolder);
            }
            catch (Exception)
            {
                throw new Exception("An error occured while trying to save the roster to the file share.");
            }
        }
    }
}
