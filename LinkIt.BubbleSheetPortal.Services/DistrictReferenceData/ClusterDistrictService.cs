using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Services.DistrictReferenceData
{
    public class ClusterDistrictService
    {
        private readonly IReadOnlyRepository<ClusterDistrict> repository;

        public ClusterDistrictService(IReadOnlyRepository<ClusterDistrict> repository)
        {
            this.repository = repository;
        }

        public List<Cluster> GetClustersByDistrictID(int districtId)
        {
            var clusterDistrictsQ = repository.Select().Where(x => x.DistrictID.Equals(districtId));
            var subjectQ = (from cluster in clusterDistrictsQ
                            select cluster.SubjectName).Distinct();

            List<Cluster> clusterList = new List<Cluster>();

            foreach (string groupname in subjectQ.ToList())
            {
                Cluster cluster = new Cluster();
                var clusterNameQ = (from clus in clusterDistrictsQ
                                    where clus.SubjectName == groupname
                                    select clus.ListClusterName).Distinct();
                cluster.SubjectName = groupname;
                cluster.ListClusterName = string.Join("<br/>", clusterNameQ.ToArray());
                clusterList.Add(cluster);
            }

            return clusterList.OrderBy(c => c.SubjectName).ThenBy(c => c.ListClusterName).ToList();
        }
    }
}