using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.Interfaces
{
    public interface IInsertSelect<T> where T : class
    {
        IQueryable<T> Select();
        void Save(T item);
    }
}
