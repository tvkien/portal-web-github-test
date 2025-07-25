using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services.CodeGen
{
    public class AuthenticationCodeGenerator
    {
        private const string _allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        private readonly Random _rd = new Random();
        private readonly IClassRepository _classRepository;
        private readonly IReadOnlyRepository<School> _schoolRepository;
        private readonly IReadOnlyRepository<District> _districtRepository;
        private readonly IReadOnlyRepository<State> _stateRepository;

        public AuthenticationCodeGenerator(
            IClassRepository classRepository,
            IReadOnlyRepository<School> schoolRepository,
            IReadOnlyRepository<District> districtRepository,
            IReadOnlyRepository<State> stateRepository)
        {
            _classRepository = classRepository;
            _schoolRepository = schoolRepository;
            _districtRepository = districtRepository;
            _stateRepository = stateRepository;
        }

        public string GenerateAuthenticationCode(int length = 4)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = _allowedChars[_rd.Next(0, _allowedChars.Length)];
            }
            return new string(chars);
        }

        public DateTime GetExpirationDate(int classId)
        {
            var classEntity = _classRepository.GetClassByID(classId);
            var school = _schoolRepository.Select().FirstOrDefault(x => x.Id == classEntity.SchoolId);

            int stateId;
            if (school.StateId.HasValue)
            {
                stateId = school.StateId.Value;
            }
            else
            {
                var district = _districtRepository.Select().FirstOrDefault(x => x.Id == school.DistrictId);
                stateId = district.StateId;
            }

            var state = _stateRepository.Select().FirstOrDefault(x => x.Id == stateId);
            return GetExpirationDate(state.TimeZoneId);
        }

        public DateTime GetExpirationDateByDistrictID(int districtId)
        {
            int stateId = _districtRepository.Select().FirstOrDefault(x => x.Id == districtId).StateId;
            var state = _stateRepository.Select().FirstOrDefault(x => x.Id == stateId);
            return GetExpirationDate(state.TimeZoneId);
        }

        private DateTime GetExpirationDate(string timeZoneId)
        {
            var zn = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zn);
            var tomorrow = new DateTimeOffset(new DateTime(now.Year, now.Month, now.Day).AddDays(1), zn.BaseUtcOffset);
            var expirationDate = tomorrow.ToUniversalTime();

            return expirationDate.DateTime;
        }
    }
}
