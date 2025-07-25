using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models.TestMaker;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Services.Survey
{
    public class ReviewSurveyService
    {
        private readonly IManageTestRepository _manageTestRepository;

        public ReviewSurveyService(
            IManageTestRepository manageTestRepository)
        {
            _manageTestRepository = manageTestRepository;
        }

        public List<BankData> GetSurveyBanksByUserID(int userID, int roleId, int districtId, bool showArchived, bool filterByDistrict = true)
        {
            return _manageTestRepository.GetSurveyBanksByUserID(userID, roleId, districtId, showArchived, filterByDistrict).ToList();
        }
    }
}
