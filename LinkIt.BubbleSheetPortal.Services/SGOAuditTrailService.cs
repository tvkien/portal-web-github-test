using System;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Services.SGOAuditTrailParser;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOAuditTrailService
    {
        private readonly ISGOAuditTrailRepository _repository;
        private readonly IRepository<Student> _studentRepository;
        private readonly IRepository<SGOGroup> _sgoGroupRepository;
        private readonly IRepository<SGODataPoint> _sgoDataPointRepository;

        public SGOAuditTrailService(ISGOAuditTrailRepository repository, IRepository<Student> studentRepository,
            IRepository<SGOGroup> sgoGroupRepository, IRepository<SGODataPoint> sgoDataPointRepository)
        {
            _repository = repository;
            _studentRepository = studentRepository;
            _sgoGroupRepository = sgoGroupRepository;
            _sgoDataPointRepository = sgoDataPointRepository;
        }

        public SGOAuditTrailData Save(SGOAuditTrailData obj)
        {
            if (obj != null)
            {
                _repository.Save(obj);
            }
            return obj;
        }

        public IQueryable<SGOAuditTrailData> GetBySGOID(int sgoID)
        {
            var query = _repository.Select().Where(o => o.SGOID == sgoID);
            return query;
        }

        public SGOAuditTrailSearchResult GetAuditTrailBySGOID(int sgoID)
        {
            var result = new SGOAuditTrailSearchResult();

            result.SGOAuditTrailSearchItems = _repository.GetAuditTrailBySGOID(sgoID).ToList();

            var sgoAuditTrailParser = new MainSGOAuditTrailParser();
            sgoAuditTrailParser.Parse(result.SGOAuditTrailSearchItems);

            result.Students =
                 _studentRepository.Select().Where(o => sgoAuditTrailParser.StudentIDs.Contains(o.Id)).ToList();
            result.Groups =
                 _sgoGroupRepository.Select().Where(o => sgoAuditTrailParser.GroupIDs.Contains(o.SGOGroupID)).ToList();
            result.DataPoints =
                 _sgoDataPointRepository.Select()
                     .Where(o => sgoAuditTrailParser.SGODataPointIDs.Contains(o.SGODataPointID)).ToList();

            foreach (var item in result.SGOAuditTrailSearchItems)
            {
                var infoBuilder = new StringBuilder();
                infoBuilder.Append("<root>");

                if (item.StudentIDs != null && result.Students != null)
                {
                    foreach (var studentID in item.StudentIDs)
                    {
                        var student = result.Students.FirstOrDefault(o => o.Id == studentID);
                        if (student == null) continue;
                        infoBuilder.AppendFormat("<student id=\"{0}\" firstname=\"{1}\" lastname=\"{2}\"></student>",
                            studentID, student.FirstName, student.LastName);
                    }
                }

                if (item.GroupIDs != null && result.Groups != null)
                {
                    foreach (var groupID in item.GroupIDs)
                    {
                        var group = result.Groups.FirstOrDefault(o => o.SGOGroupID == groupID);
                        if (group == null) continue;
                        infoBuilder.AppendFormat("<group id=\"{0}\" name=\"{1}\"></group>",
                            groupID, group.Name);
                    }
                }

                if (item.SGODataPointIDs != null && result.DataPoints != null)
                {
                    foreach (var dataPointID in item.SGODataPointIDs)
                    {
                        var dataPoint = result.DataPoints.FirstOrDefault(o => o.SGODataPointID == dataPointID);
                        if (dataPoint == null) continue;
                        infoBuilder.AppendFormat("<datapoint id=\"{0}\" name=\"{1}\"></datapoint>",
                            dataPointID, dataPoint.Name);
                    }
                }

                infoBuilder.Append("</root>");
                item.ReferenceData = infoBuilder.ToString();
            }

            return result;
        }

        public void AddSGOAuditTrail(AddSGOAuditTrailsDTO dto)
        {
            if (dto == null || !dto.SGOActionTypeID.HasValue || !dto.SGOID.HasValue || !dto.ChangedByUserID.HasValue)
                return;
            var entity = new SGOAuditTrailData
            {
                SGOActionTypeID = dto.SGOActionTypeID.Value,
                SGOID = dto.SGOID.Value,
                ActionDetail = dto.ActionDetail,
                ChangedOn = DateTime.UtcNow,
                ChagedByUserID = dto.ChangedByUserID.Value
            };

            _repository.Save(entity);
        }
    }
}
