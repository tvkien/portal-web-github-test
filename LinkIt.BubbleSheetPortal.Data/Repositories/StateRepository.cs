using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly Table<StateEntity> table;
        private readonly UserDataContext _context;
        public StateRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = UserDataContext.Get(connectionString);
            table = _context.GetTable<StateEntity>();
        }

        public IQueryable<State> Select()
        {
            return table.Select(state => new State
                {
                    Id = state.StateID,
                    Name = state.Name,
                    Code = state.Code,
                    ZipRange = state.ZipRange,
                    International = state.International,
                    TimeZoneId = state.TimeZoneID
                });
        }
        public List<State> GetStateForUser(int userId, int districtId, bool useStateOfDistrictSchool = false, bool useStateSchool = false)
        {
            var state = _context.fnGetStateForUser(userId, districtId, useStateOfDistrictSchool, useStateSchool)
                .Select(x => new State
                {
                    Id = x.StateId.GetValueOrDefault(),
                    Name = x.Name,
                    Code = x.Code,
                    ZipRange = x.ZipRange,
                    International = x.International
                }).ToList();

            return state;
        }

        public string GetTimeZoneId(int stateId)
        {
            var state = Select().FirstOrDefault(x => x.Id == stateId);
            return state != null ? state?.TimeZoneId : string.Empty;
        }
    }
}
