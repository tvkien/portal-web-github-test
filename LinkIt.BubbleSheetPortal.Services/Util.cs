using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ServiceUtil
    {

        public static IEnumerable<string> RandomGuidIds(int length, int elementsCount)
        {
            Random randomer = new Random();
            var guids = new List<string>();
            var randomGuids = RandomGuidIds(length, elementsCount, ref guids, randomer);
            return randomGuids;
        }
        public static IEnumerable<string> RandomGuidIds(int length, int elementsCount, ref List<string> lastGuids, Random randomer)
        {
            if (lastGuids.Count == elementsCount)
                return lastGuids;

            var requestNewGuidCount = elementsCount - lastGuids.Count;

            var charactersRange = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var maxValue = charactersRange.Length - 1;
            var randomGuids = Enumerable.Range(0, requestNewGuidCount * length)
                   .Select(c =>
                   {
                       var randomIndex = randomer.Next(maxValue);
                       return randomIndex;
                   })
                   .Select((value, index) => new { value, index })
                   .GroupBy(c => c.index / length)
                   .Select(c =>
                   {
                       var randomChars = c.Select(randomIndex => charactersRange[randomIndex.value]).ToArray();
                       return new string(randomChars);
                   })
                   .ToList();


             lastGuids = randomGuids
                .Concat(lastGuids)
                .Distinct()
                .ToList();

            return RandomGuidIds(length, elementsCount, ref
                lastGuids, randomer);
        }
        public static  List<int> GetIdListFromIdString(string idString)
        {
            //idString looks like 1,2,3,4,5
            List<int> idList = new List<int>();
            if (!String.IsNullOrEmpty(idString))
            {
                string[] idArray = idString.Split(',');
                foreach (var id in idArray)
                {
                    if (id.Length > 0)
                    {
                        idList.Add(Int32.Parse(id));
                    }
                }

            }
            return idList;
        }
        
        public static XmlContentDocument LoadXmlDocument(string xmlContent)
        {

            var doc = new XmlContentDocument();

            try
            {
                doc.LoadXml(xmlContent);//in general, LoadXml need the content must be encoded html 
            }
            catch (Exception)
            {
                //ItemEditor encode some characters,they are needed to be replace to enable LoadXml
                foreach (var itemEditorChar in GetItemEditorSpecialCharacters())
                {
                    xmlContent = xmlContent.Replace(itemEditorChar.Key, itemEditorChar.Value);
                }

                try
                {
                    doc.LoadXml(xmlContent);
                }
                catch
                {
                    //throw new Exception("Can not load content of item!");
                }

            }

            return doc;

        }
        private static List<KeyValuePair<string, string>> GetItemEditorSpecialCharacters()
        {
            var results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("&nbsp;", "&#160"));//&nbsp; is now saved as &#160
            return results;
        }
        public static string EncodeXmlContent(string xmlContent)
        {
            xmlContent = xmlContent
                .Replace("&lt;", "##lt;")
                .Replace("&gt;", "##gt;")
                .Replace("&quot;", "##quot;")
                .Replace("&apos;", "##apos;")
                .Replace("&amp;", "##amp;");

            xmlContent = WebUtility.HtmlDecode(xmlContent); // to encode &nbsp; and other characters with the same kind

            xmlContent = xmlContent
                .Replace("##lt;", "&lt;")
                .Replace("##gt;", "&gt;")
                .Replace("##quot;", "&quot;")
                .Replace("##apos;", "&apos;")
                .Replace("##amp;", "&amp;");

            return xmlContent;
        }
        private const string SpaceReplacementForQtiItemCorrectResponse = "[![CDATA[1e49d267-8bb6-434a-8cb5-64786b465874]]]";
        //Use for QtiItem.CorrectAnswer, QtiItem.ResponseProcessing, QtiItemAnswerScore.Answer
        public static string ReplaceSpace(string xmlContent)
        {
            return xmlContent.Replace("&#160;", ServiceUtil.SpaceReplacementForQtiItemCorrectResponse);
        }
        public static string RollbackSpace(string xmlContent)
        {
            if (string.IsNullOrEmpty(xmlContent))
            {
                return xmlContent;
            }
            return xmlContent.Replace(ServiceUtil.SpaceReplacementForQtiItemCorrectResponse," ");
        }
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs,bool overwrite)
        {
            // Get the subdirectories for the specified directory.
            if (!Directory.Exists(sourceDirName))
            {
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (File.Exists(temppath))
                {
                    if (overwrite)
                    {
                        File.Delete(temppath);
                        file.CopyTo(temppath, false);
                    }
                }
                else
                {
                    file.CopyTo(temppath, false);
                }
               
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs,overwrite);
                }
            }
        }

        
        
        public static string RemoveS3LinkFromMediaContent(string content, string AUVirtualTestROFolder, string AUVirtualTestFolder, bool isPassage)
        {
            //image
            XmlContentProcessing doc = new XmlContentProcessing(content);
            XmlNode passage = doc.GetElementsByTagName("passage")[0];
            // audio
            var audioRef = XmlUtils.GetNodeAttribute(passage, "audioRef");
            if (!string.IsNullOrEmpty(audioRef))
            {
                XmlUtils.SetOrUpdateNodeAttribute(ref passage, "audioRef", RemoveS3LinkFromMediaLink(audioRef, AUVirtualTestROFolder, AUVirtualTestFolder, isPassage));
            }
            //img
            XmlNodeList imgNodeList = doc.GetElementsByTagName("img");
            for (int i = 0; i < imgNodeList.Count; i++)
            {
                XmlNode imgNode = imgNodeList[i];
                var src = XmlUtils.GetNodeAttribute(imgNode, "src");
                if (!string.IsNullOrEmpty(src))
                {
                    XmlUtils.SetOrUpdateNodeAttribute(ref imgNode, "src", RemoveS3LinkFromMediaLink(src, AUVirtualTestROFolder, AUVirtualTestFolder, isPassage));
                }
            }
            //video
            XmlNodeList sourceNodeList = doc.GetElementsByTagName("source");
            for (int i = 0; i < sourceNodeList.Count; i++)
            {
                XmlNode sourceNode = sourceNodeList[i];
                var src = XmlUtils.GetNodeAttribute(sourceNode, "src");
                if (!string.IsNullOrEmpty(src))
                {
                    XmlUtils.SetOrUpdateNodeAttribute(ref sourceNode, "src", RemoveS3LinkFromMediaLink(src, AUVirtualTestROFolder, AUVirtualTestFolder, isPassage));
                }
            }
            return doc.GetXmlContent();
        }

        public static string RemoveS3LinkFromMediaLink(string mediaLink, string AUVirtualTestROFolder, string AUVirtualTestFolder, bool isPassage)
        {
            if (string.IsNullOrEmpty(mediaLink))
            {
                return mediaLink;
            }
            if (mediaLink.ToLower().StartsWith("http"))
            {
                return mediaLink;
            }
            //media linki such as https://testitemmedia.s3.amazonaws.com/Vina/RO/RO_6146_media/Circle-201510220725297960.mp3
            //remove https://testitemmedia.s3.amazonaws.com/Vina to keep RO/RO_6146_media/Circle-201510220725297960.mp3 only
            var result = string.Empty;
            var folder = string.Empty;

            if (isPassage)
            {
                folder = AUVirtualTestROFolder;
            }
            else
            {
                folder = AUVirtualTestFolder;
            }
            if (!string.IsNullOrEmpty(folder))
            {
                folder = folder.RemoveEndSlash().RemoveStartSlash();
                int idx = mediaLink.IndexOf(string.Format("/{0}/", folder));
                if (idx > 0)
                {
                    result = mediaLink.Substring(idx, mediaLink.Length - idx);
                    //remove folder on S3 
                    result = result.Replace("/" + folder, "");
                }
            }
           
            return result;
        }
    }
}
