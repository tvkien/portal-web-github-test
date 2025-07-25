using System;
using System.Collections.Generic;
using System.IO;
using LinkIt.BubbleService.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStorageContext<T> : IStorageContext<T> where T : IFileBlob, new()
    {
        public void Store(T entity)
        {
        }

        public bool Delete(string name)
        {
            return true;
        }

        public T GetBlob(string name)
        {
            return new T
                       {
                           Name = "Testing",
                           ContentType = "image/png",
                           Stream = new MemoryStream()
                       };
        }

        public IEnumerable<string> ListAllNames(string prefix = null)
        {
            return new List<string>
                       {
                           "Test",
                           "Test 2"
                       };
        }

        public string GetPublicReadUrl(string name, TimeSpan length)
        {
            return "https://www.google.com/images/nav_logo107.png";
        }
    }
}