using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryRequestParameterRepository : IInsertSelect<RequestParameter>
    {
        public List<RequestParameter> Items { get; set; } 

        public InMemoryRequestParameterRepository()
        {
            Items = new List<RequestParameter>();
        }

        public IQueryable<RequestParameter> Select()
        {
            return Items.AsQueryable();
        }

        public void Save(RequestParameter item)
        {
            Items.Add(item);
        }
    }
}