using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryAuthorTestWithoutTestResultRepository : IReadOnlyRepository<AuthorTestWithoutTestResult>
    {
        private readonly List<AuthorTestWithoutTestResult> table = new List<AuthorTestWithoutTestResult>();

        public InMemoryAuthorTestWithoutTestResultRepository()
        {
            table = AddInMemoryAuthorTestWithoutTestResultRepository();
        }

        private List<AuthorTestWithoutTestResult> AddInMemoryAuthorTestWithoutTestResultRepository()
        {
            return new List<AuthorTestWithoutTestResult>
                    {                           
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju1", NameLast = "Mona0", UserId = 74820, UserName = "Monaco1"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju2", NameLast = "Mona9", UserId = 74821, UserName = "Monaco2"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju3", NameLast = "Mona8", UserId = 74822, UserName = "Monaco3"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju4", NameLast = "Mona7", UserId = 74823, UserName = "Monaco4"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju5", NameLast = "Mona6", UserId = 74824, UserName = "Monaco5"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju6", NameLast = "Mona5", UserId = 74825, UserName = "Monaco6"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju7", NameLast = "Mona4", UserId = 74826, UserName = "Monaco7"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju8", NameLast = "Mona3", UserId = 74827, UserName = "Monaco8"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju9", NameLast = "Mona2", UserId = 74828, UserName = "Monaco9"}    ,
                        new AuthorTestWithoutTestResult {DistrictId = 280, NameFirst = "Ju0", NameLast = "Mona1", UserId = 74829, UserName = "Monaco0"}    
                    };
        }

        public IQueryable<AuthorTestWithoutTestResult> Select()
        {
            return table.AsQueryable();
        }
    }
}
