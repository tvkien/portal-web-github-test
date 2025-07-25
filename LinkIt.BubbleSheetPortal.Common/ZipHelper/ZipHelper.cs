using Ionic.Zip;
using LinkIt.BubbleSheetPortal.Common.ZipHelper.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace LinkIt.BubbleSheetPortal.Common.ZipHelper
{
    public static class ZipHelper
    {
        public static byte[] Zip(string temporaryPath, List<ZipModel> filesTobeZip)
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

                using (ZipFile zipEntries = new ZipFile())
                {
                    List<string> filePaths = new List<string>();
                    foreach (var item in filesTobeZip)
                    {
                        //write all file to Disk
                        string _filePath = Path.Combine(_processFolder, item.FileRelativeName);
                        File.WriteAllBytes(_filePath, item.FileData);
                        filePaths.Add(_filePath);
                    }
                    zipEntries.AddFiles(filePaths, string.Empty);
                    zipEntries.Save(_finalFilePath);
                }
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
                        Directory.Delete(_processFolder, true);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static string RosterZip(string tempPath, List<ZipModel> filesTobeZip, int requestId)
        {
            string destinationFolder = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).Replace(".", ""));

            try
            {
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }
                string fileName = $"{requestId}.zip";
                string finalZipFilePath = Path.Combine(destinationFolder, fileName);

                using (ZipFile zipEntries = new ZipFile())
                {
                    List<string> filePaths = new List<string>();
                    foreach (var item in filesTobeZip)
                    {
                        string childFilePath = Path.Combine(destinationFolder, item.FileRelativeName);
                        File.WriteAllBytes(childFilePath, item.FileData);
                        filePaths.Add(childFilePath);
                    }
                    zipEntries.AddFiles(filePaths, string.Empty);
                    zipEntries.Save(finalZipFilePath);

                    return finalZipFilePath;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;
        }
    }
}
