using System.Collections.Generic;
using LinkIt.BubbleService.Models.Reading;
using LinkIt.BubbleService.Shared.Data;
using Lokad.Cloud.Storage;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryTestResultRepository<T> : ITableContext<T> where T : ReadResult, new()
    {
        private List<CloudEntity<T>> table;

        public InMemoryTestResultRepository()
        {
            table = AddReadResults();
        }

        private List<CloudEntity<T>> AddReadResults()
        {
            return new List<CloudEntity<T>>
                       {
                           new CloudEntity<T>
                               {
                                   PartitionKey = "partition1",
                                   RowKey = "row1",
                                   Value = new T { OutputFile = "outputfile", InputPath = "inputpath", Barcode1 = "barcode1" }
                               }
                       };
        }

        public IEnumerable<CloudEntity<T>> SelectAll()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CloudEntity<T>> Select(string partitionKey)
        {
            throw new System.NotImplementedException();
        }

        public CloudEntity<T> Select(string partitionKey, string rowKey)
        {
            return table.Find(x => x.PartitionKey.Equals(partitionKey) && x.RowKey.Equals(rowKey));
        }

        public void Create(CloudEntity<T> entity)
        {
            throw new System.NotImplementedException();
        }

        public void Update(CloudEntity<T> entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(CloudEntity<T> entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
