using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Interfaces
{
    public interface IInsertDeleteRepository<T> where T : class
    {
        void Save(T item);
        void Delete(T item);
        void InsertMultipleRecord(List<T> items);
    }
}
