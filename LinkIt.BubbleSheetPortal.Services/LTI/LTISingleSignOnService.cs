using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories.LTI;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.LTI;
using LinkIt.BubbleSheetPortal.Models.DTOs.SSO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LTISingleSignOnService
    {
        private readonly IReadOnlyRepository<LTIInformation> _ltiInformationRepository;
        private readonly IRepository<LTIRequestHistory> _ltiRequestHistoryRepository;
        private readonly ILTIRequestHistoryRepository _ltiHistoryRepository;
        private readonly IRepository<SSORedirectUrl> _ssoRedirectUrlRepository;

        public LTISingleSignOnService(IReadOnlyRepository<LTIInformation> ltiInformationRepository,
                                        IRepository<LTIRequestHistory> ltiRequestHistoryRepository,
                                        ILTIRequestHistoryRepository ltiRequestHistory,
                                        IRepository<SSORedirectUrl> ssoRedirectUrlRepository)
        {
            this._ltiInformationRepository = ltiInformationRepository;
            this._ltiRequestHistoryRepository = ltiRequestHistoryRepository;
            this._ltiHistoryRepository = ltiRequestHistory;
            this._ssoRedirectUrlRepository = ssoRedirectUrlRepository;
        }

        public LTIInformation GetLTIInformation(string clientId)
        {
            var ltiInformation = _ltiInformationRepository.Select().FirstOrDefault(x => x.ClientID == clientId);
            if (ltiInformation == null)
                return null;

            return ltiInformation;
        }

        public LTIInformation GetLTIInformationByDeploymentId(string deploymentID)
        {
            var ltiInformation = _ltiInformationRepository.Select().FirstOrDefault(x => x.DeploymentID == deploymentID);
            if (ltiInformation == null)
                return null;

            return ltiInformation;
        }

        public bool LtiParamIsValid(LtiAuthorizeDto ltiParams, LTIInformation ltiInformation)
        {
            System.ComponentModel.DataAnnotations.ValidationContext valContext = new System.ComponentModel.DataAnnotations.ValidationContext(ltiParams, null, null);
            var result = new List<ValidationResult>();
            if (!Validator.TryValidateObject(ltiParams, valContext, result, true))
                return false;

            if (ltiParams.PlatformID == ltiInformation.PlatformID
                && ltiParams.ClientId == ltiInformation.ClientID
                && ltiParams.DeploymentId == ltiInformation.DeploymentID)
                return true;

            return false;
        }

        public void SaveLTIRequestHistory(LtiAuthorizeDto ltiAuthorizeDto)
        {
            var ltiRequestHistory = new LTIRequestHistory
            {
                PlatformID = ltiAuthorizeDto.PlatformID,
                ClientID = ltiAuthorizeDto.ClientId,
                DeploymentID = ltiAuthorizeDto.DeploymentId,
                State = ltiAuthorizeDto.State,
                Nonce = ltiAuthorizeDto.Nonce,
                IsCompleted = false
            };
            _ltiRequestHistoryRepository.Save(ltiRequestHistory);
        }

        public void UpdateStatus(string nonce, bool isCompleted)
        {
            _ltiHistoryRepository.UpdateStatus(nonce, isCompleted);
        }

        public bool ValidationAuthorize(IdTokenDto idTokenDto)
        {
            var ltiRequestHistory = _ltiRequestHistoryRepository.Select().FirstOrDefault(x => x.Nonce.Equals(idTokenDto.Nonce));
            if (ltiRequestHistory == null)
                return false;

            System.ComponentModel.DataAnnotations.ValidationContext valContext = new System.ComponentModel.DataAnnotations.ValidationContext(idTokenDto, null, null);
            var result = new List<ValidationResult>();
            if (!Validator.TryValidateObject(idTokenDto, valContext, result, true))
                return false;

            if (string.Compare(ltiRequestHistory.State, idTokenDto.State) != 0)
                return false;

            return true;
        }

        public string GetRedirectUrl(int ssoInformationId, int roleId, string type)
        {
            var ssoRedirectInfo = _ssoRedirectUrlRepository.Select().FirstOrDefault(x => x.SSOInformationId == ssoInformationId
                                                                                    && x.Type == type && x.RoleId == roleId);
            if (ssoRedirectInfo == null)
                return string.Empty;

            return ssoRedirectInfo.RedirectUrl;
        }
        public SSORedirectUrl GetObjectRedirectUrl(int ssoInformationId, int roleId, string type)
        {
            var ssoRedirectInfo = _ssoRedirectUrlRepository.Select().FirstOrDefault(x => x.SSOInformationId == ssoInformationId
                                                                                    && x.Type == type && x.RoleId == roleId);
            if (ssoRedirectInfo == null)
                return null;

            return new SSORedirectUrl()
            {
                RedirectUrl= ssoRedirectInfo.RedirectUrl,
                RoleId= ssoRedirectInfo.RoleId,
                Type= ssoRedirectInfo.Type,
                SSORedirectUrlId = ssoRedirectInfo.SSORedirectUrlId,
                SSOInformationId= ssoRedirectInfo.SSOInformationId,
                XLIModuleCode = ssoRedirectInfo.XLIModuleCode
                
            }; 
        }
    }
}
