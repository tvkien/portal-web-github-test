using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;

namespace LinkIt.BubbleSheetPortal.Web.Models.TLDS
{
    public class TldsDigitalS3Setting
    {
        private int _profileId;
        private Guid _profileLinkId;

        public static TldsDigitalS3Setting GetTldsDigitalS3Setting(int prodileId, Guid profileLinkId)
        {
            var setting = new TldsDigitalS3Setting();
            setting._profileId = prodileId;
            setting._profileLinkId = profileLinkId;
            return setting;
        }
        public  string TLDSBucket
        {
            get
            {
                return LinkitConfigurationManager.GetS3Settings().TLDSBucket;//get from Vault
            }
        }

        private string _tldsFolder;
        public  string TLDSFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsFolder))
                {
                    _tldsFolder = ConfigurationManager.AppSettings[ContaintUtil.AppSettingTLDSFolderName];
                    if (_tldsFolder == null)
                    {
                        _tldsFolder = string.Empty;
                    }
                }
                return _tldsFolder;
            }
        }

        private string _tldsFormFolder;
        public  string TLDSFormFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsFormFolder))
                {
                    _tldsFormFolder = string.Format("{0}/{1}/{2}/Form", TLDSFolder.RemoveEndSlash(), _profileId, _profileLinkId);
                }
             
                return _tldsFormFolder;
            }
        }
        public string TLDSUploadStatementFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsFormFolder))
                {
                    _tldsFormFolder = string.Format("{0}/StatementUploaded", TLDSFolder.RemoveEndSlash());
                }

                return _tldsFormFolder;
            }
        }
        public string GetFormPath(string pdfFormFileName)
        {
            string s3FormFileFolder = string.Format("{0}/{1}", TLDSFormFolder.RemoveEndSlash(),
                pdfFormFileName);
            return s3FormFileFolder;
        }
        public string GetStatementUploadedPath(string pdfStatementUploadedFileName)
        {
            string s3FormFileFolder = string.Format("{0}/{1}", TLDSUploadStatementFolder.RemoveEndSlash(),
                pdfStatementUploadedFileName);
            return s3FormFileFolder;
        }
        private string _tldsPhotoFolder;
        public  string TLDSPhotoFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tldsPhotoFolder))
                {
                    _tldsPhotoFolder = string.Format("{0}/{1}/{2}/Photo", TLDSFolder.RemoveEndSlash(), _profileId, _profileLinkId);
                }
                
                return _tldsPhotoFolder;
            }
        }

        public string GetPhotoPath(string photoFileName)
        {
            string s3PhotoFileFolder = string.Format("{0}/{1}", TLDSPhotoFolder.RemoveEndSlash(),
                photoFileName);
            return s3PhotoFileFolder;
        }
    }
}
