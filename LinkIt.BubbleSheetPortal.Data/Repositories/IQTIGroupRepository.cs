using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public  interface IQTIGroupRepository : IRepository<QtiGroup>
    {
        List<QtiGroup> GetlistQtiGroups(int qtiBankId, int userId, int roleId, int districtId);

        List<QtiGroup> GetOwnerListQtiGroupByQtiBankId(int qtiBankId);

        string DeleteItemSetAndItems(int qtiItemSetId, int userId);
        void ReassignQuestionOrder(int qtiGroupId);
    }
}
