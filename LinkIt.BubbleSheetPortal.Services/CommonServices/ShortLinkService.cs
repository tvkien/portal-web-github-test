using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories.MasterData;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Services.CodeGen;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services.CommonServices
{
    public class ShortLinkService : IShortLinkService
    {
        private IShortLinkRepository _shortLinkRepository;
        private readonly IReadOnlyRepository<Configuration> _repositoryConfiguration;
        private readonly ConfigurationService _configurationService;
        private readonly TestCodeGenerator _testCodeGenerator;
        private readonly string ShortLinkStringLength = "ShortLinkStringLength";

        public ShortLinkService(IShortLinkRepository shortLinkRepository, ConfigurationService configurationService, TestCodeGenerator testCodeGenerator, IReadOnlyRepository<Configuration> repositoryConfiguration)
        {
            _shortLinkRepository = shortLinkRepository;
            _configurationService = configurationService;
            _testCodeGenerator = testCodeGenerator;
            _repositoryConfiguration = repositoryConfiguration;
        }

        public string GenerateShortLink(string url)
        {
            var codeLength = 6;
            var config = _repositoryConfiguration.Select().FirstOrDefault(m => m.Name == ShortLinkStringLength);

            if (config != null)
            {
                int.TryParse(config.Value, out codeLength);
            }

            var code = _testCodeGenerator.GenerateTestCode(codeLength, string.Empty).ToLower();

            var isExisted = true;
            while(isExisted)
            {
                var link = GetFullLink(code);
                if(string.IsNullOrEmpty(link))
                {
                    isExisted = false;
                }
                else
                {
                    code = _testCodeGenerator.GenerateTestCode(codeLength, string.Empty).ToLower();
                }
            }

            var model = new ShortLinkDto
            {
                Code = code,
                FullLink = url
            };

            _shortLinkRepository.Add(model);
            var portalUrl = _configurationService.GetConfigurationByKey(Constanst.DEFAULTSURVEYURL);

            return $"{portalUrl.Value}?{model.Code}";
        }

        public string GetFullLink(string code)
        {
            var fullLink = _shortLinkRepository.GetFullLinkByCode(code);

            return fullLink;
        }
        public ShortLinkDto GenerateSurveyKeyAndFullLink(int surveyId, int schoolId, string code)
        {
            var shortLink = new ShortLinkDto();
            if (surveyId > 0)
            {
                string strSurveyUrl = System.Configuration.ConfigurationManager.AppSettings["SurveyUrl"] ?? "http://survey.linkit.devblock.net";
                shortLink.FullLink = string.Format("{0}?schoolId={1}&surveyId={2}&code={3}&returnurl=", strSurveyUrl, schoolId, surveyId, code);
                shortLink.Code = code.ToLower().Substring(3);
            }

            return shortLink;
        }
        public string GenerateSurveyTestTakerURL(int qTITestClassAssignmentId, int surveyId, int schoolId, string code)
        {
            string strURL = string.Empty;
            if (surveyId > 0)
            {
                string strSurveyUrl = System.Configuration.ConfigurationManager.AppSettings["SurveyUrl"] ?? "http://survey.linkit.devblock.net";
                strURL = string.Format("{0}?schoolId={1}&surveyId={2}&code={3}&returnurl=", strSurveyUrl, schoolId, surveyId, code);
                strURL = GenerateSurveyShortLink(strURL, qTITestClassAssignmentId);
            }
            return strURL;
        }
        public string GenerateSurveyShortLink(string url, int qTITestClassAssignmentId)
        {
            string code = GenerateSurveyKey();
            var model = new ShortLinkDto
            {
                Code = code,
                FullLink = url,
                QTITestClassAssignmentId = qTITestClassAssignmentId
            };

            _shortLinkRepository.Add(model);
            var portalUrl = _configurationService.GetConfigurationByKey(Constanst.DEFAULTSURVEYURL);

            return $"{portalUrl.Value}?{model.Code}";
        }
        private string GenerateSurveyKey()
        {
            var codeLength = 6;
            var config = _repositoryConfiguration.Select().FirstOrDefault(m => m.Name == ShortLinkStringLength);

            if (config != null)
            {
                int.TryParse(config.Value, out codeLength);
            }

            var code = _testCodeGenerator.GenerateTestCode(codeLength, string.Empty).ToLower();

            var isExisted = true;
            while (isExisted)
            {
                var link = GetFullLink(code);
                if (string.IsNullOrEmpty(link))
                {
                    isExisted = false;
                }
                else
                {
                    code = _testCodeGenerator.GenerateTestCode(codeLength, string.Empty).ToLower();
                }
            }

            return code;
        }
    }
}
