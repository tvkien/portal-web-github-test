using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data
{
    public interface IUnitOfWorkRepository<T>
    {
        IQueryable<T> Select();
        void SaveOnSubmit(T item);
        void DeleteOnSubmit(T item);
        void SaveChanges();
    }
}