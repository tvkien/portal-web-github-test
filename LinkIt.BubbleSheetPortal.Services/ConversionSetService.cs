using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestUtilitiesDefineTemplates;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ConversionSetService : IConversionSetService
    {
        private readonly IRepository<ConversionSet> _repository;
        private readonly IRepository<ConversionSetDetail> _conversionSetDetailRepository;

        public ConversionSetService(IRepository<ConversionSet> repository
            , IRepository<ConversionSetDetail> conversionSetDetailRepository)
        {
            _repository = repository;
            _conversionSetDetailRepository = conversionSetDetailRepository;
        }

        public List<ConversionSet> GetAllConversionSet()
        {
            return _repository.Select().ToList();
        }

        public int SaveConversionSet(int? conversionSetId, string fileName, List<ConversionSetDetailDto> details)
        {
            var conversionSet = _repository.Select().FirstOrDefault(x => x.ConverstionSetID == conversionSetId);
            if (conversionSet == null)
            {
                conversionSet = new ConversionSet { Name = fileName };
                _repository.Save(conversionSet);
            }
            else if (conversionSet.Name != fileName)
            {
                conversionSet.Name = fileName;
                _repository.Save(conversionSet);
            }

            var conversionSetDetails = _conversionSetDetailRepository.Select()
                .Where(x => x.ConversionSetID == conversionSet.ConverstionSetID).ToList();
            if (details.Count == conversionSetDetails.Count)
            {
                for(var i = 0; i < details.Count; i++)
                {
                    conversionSetDetails[i].Input1 = details[i].Input1;
                    conversionSetDetails[i].ConvertedScore = details[i].ConvertedScore;
                    conversionSetDetails[i].ConvertedScore_A = details[i].ConvertedScore_A;

                    _conversionSetDetailRepository.Save(conversionSetDetails[i]);
                }
            }
            else
            {
                conversionSetDetails.ForEach(x => _conversionSetDetailRepository.Delete(x));
                details.ForEach(x => {
                    _conversionSetDetailRepository.Save(new ConversionSetDetail
                    {
                        ConversionSetID = conversionSet.ConverstionSetID,
                        Input1 = x.Input1,
                        ConvertedScore = x.ConvertedScore,
                        ConvertedScore_A = x.ConvertedScore_A
                    });
                });
            }

            return conversionSet.ConverstionSetID;
        }
    }
}
