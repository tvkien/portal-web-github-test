using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class MasterStandardResourceRepository : IReadOnlyRepository<MasterStandardResource>
    {
        private readonly Table<MasterStandardView> view;
        private readonly LearningLibraryDataContext _learningLibraryDataContext;

        public MasterStandardResourceRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            view = UserDataContext.Get(connectionString).GetTable<MasterStandardView>();
            _learningLibraryDataContext = LearningLibraryDataContext.Get(connectionString);
        }

        public IQueryable<MasterStandardResource> Select()
        {
            return view.Select(x => new MasterStandardResource
            {
                MasterStandardID = x.MasterStandardID,
                State = x.State,
                Subject = x.Subject,
                Year = x.Year,
                Grade = x.Grade,
                Level = x.Level,
                Label = x.Label,
                Number = x.Number,
                Description = x.Description,
                GUID = x.GUID,
                ParentGUID = x.ParentGUID,
                StateId = x.StateId.HasValue?x.StateId.Value:0,
                LoGrade = x.LoGrade,
                HiGrade = x.HiGrade,
                LowGradeID = x.LowGradeID.HasValue?x.LowGradeID.Value:0,
                HighGradeID = x.HighGradeID.HasValue?x.HighGradeID.Value:0,
                Children = x.Children,
                Archived = x.Archived,
                CountChildren = x.CountChildren.HasValue?x.CountChildren.Value:0

            });
        }
    }
}
